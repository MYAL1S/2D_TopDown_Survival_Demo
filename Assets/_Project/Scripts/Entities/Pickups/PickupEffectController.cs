using System;
using System.Collections;
using UnityEngine;

/// <summary>
/// 拾取特效控制器
/// 负责播放拾取特效并在特效结束后通知对象池释放该对象
/// </summary>
[DisallowMultipleComponent]
public class PickupEffectController : MonoBehaviour, IPoolable
{
    // 特效的默认持续时间（秒）
    // 如果粒子系统没有正确设置持续时间
    // 则使用该值作为回退
    [SerializeField]
    [Min(0.05f)]
    private float fallbackLifetime = 2f;

    // 当前正在播放的释放协程
    private Coroutine releaseRoutine;
    // 释放回调
    // 在特效结束后调用以通知对象池释放该对象
    private Action<PickupEffectController> releaseCallback;
    // 缓存的粒子系统组件数组
    private ParticleSystem[] particleSystems;

    private void Awake()
    {
        CacheParticleSystems();
    }

    /// <summary>
    /// 播放特效并在特效结束后通过回调通知对象池释放该对象
    /// </summary>
    /// <param name="onRelease">释放资源回调函数</param>
    public void PlayAndRelease(Action<PickupEffectController> onRelease)
    {
        // 记录释放回调
        releaseCallback = onRelease;

        // 如果当前有正在播放的释放协程 先停止它
        if (releaseRoutine != null)
        {
            StopCoroutine(releaseRoutine);
        }

        // 播放粒子系统并获取特效的持续时间
        float lifetime = PlayParticleSystems();
        // 启动一个新的协程 在特效持续时间后调用释放回调
        releaseRoutine = StartCoroutine(ReleaseAfter(lifetime));
    }

    /// <summary>
    /// 当对象从对象池中获取时调用以初始化特效
    /// </summary>
    public void OnSpawnedFromPool()
    {
        CacheParticleSystems();
    }

    /// <summary>
    /// 当对象返回对象池时调用以停止特效并清理状态
    /// </summary>
    public void OnReturnedToPool()
    {
        if (releaseRoutine != null)
        {
            StopCoroutine(releaseRoutine);
            releaseRoutine = null;
        }

        StopParticleSystems();
        releaseCallback = null;
    }

    /// <summary>
    /// 缓存所有子对象中的粒子系统组件以便后续播放时使用
    /// </summary>
    private void CacheParticleSystems()
    {
        particleSystems = GetComponentsInChildren<ParticleSystem>(true);
    }

    /// <summary>
    /// 播放所有缓存的粒子系统组件 
    /// 并计算特效的持续时间
    /// </summary>
    /// <returns></returns>
    private float PlayParticleSystems()
    {
        // 使用回退持续时间作为初始值
        // 如果粒子系统没有正确设置持续时间 则至少使用该值
        float lifetime = fallbackLifetime;

        // 遍历所有粒子系统组件 播放它们并计算特效的持续时间
        for (int i = 0; i < particleSystems.Length; i++)
        {
            ParticleSystem particleSystem = particleSystems[i];
            if (particleSystem == null)
            {
                continue;
            }

            particleSystem.gameObject.SetActive(true);
            particleSystem.Clear(true);
            particleSystem.Play(true);

            ParticleSystem.MainModule main = particleSystem.main;
            lifetime = Mathf.Max(lifetime, main.duration + main.startLifetime.constantMax);
        }

        return lifetime;
    }

    /// <summary>
    /// 停止所有缓存的粒子系统组件并清理它们的状态以准备下次播放
    /// </summary>
    private void StopParticleSystems()
    {
        if (particleSystems == null)
        {
            return;
        }

        for (int i = 0; i < particleSystems.Length; i++)
        {
            ParticleSystem particleSystem = particleSystems[i];
            if (particleSystem != null)
            {
                particleSystem.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            }
        }
    }

    /// <summary>
    /// 在特效持续时间后调用释放回调以通知对象池释放该对象
    /// </summary>
    /// <param name="lifetime">持续时间</param>
    /// <returns></returns>
    private IEnumerator ReleaseAfter(float lifetime)
    {
        yield return new WaitForSeconds(lifetime);
        releaseRoutine = null;
        releaseCallback?.Invoke(this);
    }
}
