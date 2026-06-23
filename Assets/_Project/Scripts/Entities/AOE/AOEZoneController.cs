using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// AOE区域控制器
/// </summary>
[DisallowMultipleComponent]
public class AOEZoneController : MonoBehaviour, IPoolable
{
    // 缓存目标列表 避免频繁分配内存
    private readonly List<Enemy> targets = new List<Enemy>(64);

    // AOE配置
    private AOEConfig config;
    // 状态效果来源上下文
    private StatusEffectSourceContext statusEffectSourceContext;
    // AOE拥有者
    private GameObject owner;
    // 释放回调
    private Action<AOEZoneController> releaseCallback;
    // 已运行时间
    private float elapsedTime;
    // 计时器 用于计算AOE伤害间隔
    private float tickTimer;
    // AOE区域的有效活跃时间
    private float activeLifetime;
    // 是否应该运行AOE区域的每 tick 逻辑
    private bool shouldRunGameplayTicks;
    // 是否已初始化
    private bool hasInitialized;

    public AOEConfig Config => config;
    public GameObject Owner => owner;
    public bool IsRunning => hasInitialized;

    /// <summary>
    /// 初始化AOE区域控制器
    /// </summary>
    /// <param name="config">AOE配置</param>
    /// <param name="owner">拥有者</param>
    /// <param name="statusEffectSourceContext">状态效果来源上下文</param>
    /// <param name="releaseCallback">释放回调</param>
    public void Initialize(
        AOEConfig config,
        GameObject owner,
        StatusEffectSourceContext statusEffectSourceContext,
        Action<AOEZoneController> releaseCallback)
    {
        this.config = config;
        this.statusEffectSourceContext = statusEffectSourceContext;
        this.owner = owner;
        this.releaseCallback = releaseCallback;
        elapsedTime = 0f;
        tickTimer = 0f;
        activeLifetime = 0f;
        shouldRunGameplayTicks = false;
        hasInitialized = config != null;

        // 如果没有初始化成功
        // 则立即释放该AOE区域控制器
        if (!hasInitialized)
        {
            Release();
            return;
        }

        // 设置AOE区域的视觉缩放
        ApplyVisualScale();
        ReplaySpawnVisualEffects();

        if (config.TickOnStart)
        {
            ApplyAOETick();
        }

        if (config.Duration <= 0f)
        {
            activeLifetime = HasVisualEffects() ? config.VisualLifetime : 0f;
            if (activeLifetime <= 0f)
            {
                Release();
            }

            return;
        }

        activeLifetime = config.Duration;
        shouldRunGameplayTicks = true;
    }

    public void OnSpawnedFromPool()
    {
        elapsedTime = 0f;
        tickTimer = 0f;
        activeLifetime = 0f;
        shouldRunGameplayTicks = false;
        hasInitialized = false;
    }

    public void OnReturnedToPool()
    {
        StopVisualEffects();
        config = null;
        statusEffectSourceContext = StatusEffectSourceContext.None;
        owner = null;
        releaseCallback = null;
        targets.Clear();
        activeLifetime = 0f;
        shouldRunGameplayTicks = false;
        hasInitialized = false;
    }

    /// <summary>
    /// 每帧更新AOE区域控制器
    /// </summary>
    /// <param name="deltaTime">时间增量</param>
    public void Tick(float deltaTime)
    {
        // 如果没有初始化 或者 配置为空 则不进行任何逻辑
        if (!hasInitialized || config == null)
        {
            return;
        }

        // 更新已运行时间
        elapsedTime += deltaTime;

        // 如果应该运行AOE区域的每 tick 逻辑
        // 则进行计时和应用AOE逻辑
        if (shouldRunGameplayTicks)
        {
            tickTimer += deltaTime;

            while (tickTimer >= config.TickInterval)
            {
                tickTimer -= config.TickInterval;
                ApplyAOETick();
            }
        }

        // 如果已运行时间超过了AOE区域的有效活跃时间
        // 则释放该AOE区域控制器
        if (elapsedTime >= activeLifetime)
        {
            Release();
        }
    }

    /// <summary>
    /// 应用AOE区域的每 tick 逻辑
    /// </summary>
    private void ApplyAOETick()
    {
        // 如果没有配置 则不进行AOE逻辑
        if (config == null)
        {
            return;
        }

        // 收集AOE区域内的所有存活敌人
        TargetingSystem.CollectAliveEnemiesInRange(
            transform.position,
            config.Radius,
            targets,
            config.MaxTargetsPerTick);

        // 遍历所有目标敌人 
        for (int i = 0; i < targets.Count; i++)
        {
            Enemy target = targets[i];
            if (target == null || !target.IsAlive)
            {
                continue;
            }

            // 如果有源武器配置 则使用武器配置的伤害计算方式
            if (statusEffectSourceContext.WeaponConfig != null)
            {
                WeaponDamageApplier.ApplySingleTargetDamage(
                    target,
                    config.DamagePerTick,
                    statusEffectSourceContext.WeaponConfig,
                    statusEffectSourceContext);
            }
            // 否则使用AOE配置的伤害计算方式
            else if (config.DamagePerTick > 0f)
            {
                CombatSystem.ApplyDamage(target.Health, config.DamagePerTick);
            }

            // 如果有状态效果配置 则应用状态效果
            ApplyStatusEffects(target);
        }
    }

    /// <summary>
    /// 应用状态效果到目标敌人
    /// </summary>
    /// <param name="target">敌人目标</param>
    private void ApplyStatusEffects(Enemy target)
    {
        if (target == null || !target.IsAlive || target.StatusEffectManager == null || config.StatusEffects == null)
        {
            return;
        }

        IReadOnlyList<StatusEffectConfig> statusEffects = config.StatusEffects;
        for (int i = 0; i < statusEffects.Count; i++)
        {
            StatusEffectConfig statusEffect = statusEffects[i];
            if (statusEffect != null)
            {
                target.StatusEffectManager.ApplyEffect(statusEffect, statusEffectSourceContext);
            }
        }
    }

    /// <summary>
    /// 应用AOE区域的视觉缩放
    /// </summary>
    private void ApplyVisualScale()
    {
        // 如果没有配置 则不进行缩放
        if (config == null)
        {
            return;
        }

        // 设置AOE区域的视觉缩放
        float diameter = config.Radius * 2f;
        transform.localScale = new Vector3(diameter, diameter, 1f);
    }

    /// <summary>
    /// 重新播放AOE区域的生成视觉效果
    /// </summary>
    private void ReplaySpawnVisualEffects()
    {
        // 获取该gameobject以及子对象上的粒子系统
        ParticleSystem[] particleSystems = GetComponentsInChildren<ParticleSystem>(true);
        // 遍历所有粒子系统 
        for (int i = 0; i < particleSystems.Length; i++)
        {
            ParticleSystem particleSystem = particleSystems[i];
            if (particleSystem == null)
            {
                continue;
            }

            ParticleSystem.MainModule mainModule = particleSystem.main;
            if (!mainModule.playOnAwake)
            {
                continue;
            }

            // 如果启用了PlayOnAwake 则停止并重新播放
            particleSystem.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            particleSystem.Clear(true);
            particleSystem.Play(true);
        }

        // 获取该gameobject以及子对象上的Animator组件
        Animator[] animators = GetComponentsInChildren<Animator>(true);
        // 遍历所有Animator组件
        for (int i = 0; i < animators.Length; i++)
        {
            Animator animator = animators[i];
            // 如果Animator组件为空 或者未启用 或者没有绑定动画控制器 则跳过
            if (animator == null || !animator.enabled || animator.runtimeAnimatorController == null)
            {
                continue;
            }

            // 如果启用了Animator组件 则重新绑定并更新
            animator.Rebind();
            animator.Update(0f);
        }
    }

    /// <summary>
    /// 停止AOE区域的视觉效果
    /// </summary>
    private void StopVisualEffects()
    {
        // 获取该gameobject以及子对象上的粒子系统
        ParticleSystem[] particleSystems = GetComponentsInChildren<ParticleSystem>(true);
        for (int i = 0; i < particleSystems.Length; i++)
        {
            ParticleSystem particleSystem = particleSystems[i];
            if (particleSystem != null)
            {
                particleSystem.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
                particleSystem.Clear(true);
            }
        }
    }

    /// <summary>
    /// 检查AOE区域是否有视觉效果组件
    /// </summary>
    /// <returns></returns>
    private bool HasVisualEffects()
    {
        // 检查是否有任何子对象包含ParticleSystem或Animator组件 并且这些组件是启用的
        return GetComponentsInChildren<ParticleSystem>(true).Length > 0 ||
               GetComponentsInChildren<Animator>(true).Length > 0;
    }

    /// <summary>
    /// 释放AOE区域控制器
    /// </summary>
    private void Release()
    {
        // 重置状态
        hasInitialized = false;
        Action<AOEZoneController> callback = releaseCallback;
        if (callback != null)
        {
            callback(this);
            return;
        }

        Destroy(gameObject);
    }
}
