using System.Collections.Generic;
using UnityEngine;
using System;

/// <summary>
/// 状态效果管理器
/// </summary>
[DisallowMultipleComponent]
public class StatusEffectManager : MonoBehaviour
{
    // 活跃状态效果字典
    private readonly Dictionary<string, List<IStatusEffect>> activeEffectsById =
        new Dictionary<string, List<IStatusEffect>>();
    // 临时列表 用于在Tick过程中存储没有效果的ID 以便之后清理
    private readonly List<string> emptyEffectIds = new List<string>();

    // 状态效果上下文 由当前游戏对象构建 提供给状态效果实例以访问必要的信息和功能
    private StatusEffectContext context;
    // 死亡事件组件 引用以便在实体死亡时清理状态效果
    private DeathEvent deathEvent;

    // 效果应用和移除事件 允许外部系统订阅以响应状态效果的变化
    public event Action<StatusEffectManager, IStatusEffect> OnEffectApplied;
    public event Action<StatusEffectManager, IStatusEffect> OnEffectRemoved;

    // 当前活跃的状态效果组数量 
    public int ActiveEffectGroupCount => activeEffectsById.Count;

    /// <summary>
    /// 应用状态效果配置 创建一个状态效果实例并应用到实体上
    /// </summary>
    /// <param name="config">状态效果配置</param>
    public void ApplyEffect(StatusEffectConfig config)
    {
        ApplyEffect(config, StatusEffectSourceContext.None);
    }

    /// <summary>
    /// 应用状态效果配置 创建一个状态效果实例并应用到实体上
    /// </summary>
    /// <param name="config">状态效果配置</param>
    /// <param name="sourceContext">状态效果来源上下文</param>
    public void ApplyEffect(StatusEffectConfig config, StatusEffectSourceContext sourceContext)
    {
        if (config == null)
        {
            return;
        }

        if (!config.CanApplyTo(gameObject))
        {
            return;
        }

        ApplyEffect(new StatusEffectInstance(config, sourceContext));
    }

    private void Awake()
    {
        // 缓存死亡事件组件引用 以便在实体死亡时清理状态效果
        deathEvent = GetComponent<DeathEvent>();
        // 构建状态效果上下文 提供给状态效果实例以访问必要的信息和功能
        RebuildContext();
    }

    private void OnEnable()
    {
        // 构建状态效果上下文 以确保在启用时状态效果实例访问到最新的信息和功能
        RebuildContext();
        // 监听全局状态效果Tick事件
        // 以便在每个Tick周期更新状态效果的持续时间和触发效果逻辑
        StatusEffectTickSystem.OnGlobalTick += StatusEffectTickSystem_OnGlobalTick;
        if (deathEvent != null)
        {
            deathEvent.OnDeath += DeathEvent_OnDeath;
        }
    }

    private void OnDisable()
    {
        // 取消监听全局状态效果Tick事件 以避免在禁用时继续更新状态效果
        StatusEffectTickSystem.OnGlobalTick -= StatusEffectTickSystem_OnGlobalTick;
        if (deathEvent != null)
        {
            deathEvent.OnDeath -= DeathEvent_OnDeath;
        }
    }

    private void OnDestroy()
    {
        // 在销毁时清理所有状态效果 以确保状态效果不会在实体销毁后继续影响实体或占用资源
        ClearAll();
    }

    /// <summary>
    /// 应用状态效果实例 根据效果的叠加行为处理效果的应用逻辑
    /// </summary>
    /// <param name="effect">状态效果</param>
    public void ApplyEffect(IStatusEffect effect)
    {
        // 确保效果实例和ID有效
        if (effect == null || string.IsNullOrWhiteSpace(effect.EffectId))
        {
            return;
        }

        // 根据效果的叠加行为处理应用逻辑
        // 如果没有相同ID的效果 则直接添加新效果
        if (!activeEffectsById.TryGetValue(effect.EffectId, out List<IStatusEffect> effects) || effects.Count == 0)
        {
            AddNewEffect(effect);
            return;
        }

        // 根据叠加行为刷新或叠加现有效果 或添加独立效果
        switch (effect.StackingBehavior)
        {
            case StatusEffectStackingBehavior.Refresh:
                effects[0].UpdateSourceContext(effect.SourceContext);
                effects[0].Refresh(context);
                break;
            case StatusEffectStackingBehavior.Stack:
                effects[0].UpdateSourceContext(effect.SourceContext);
                effects[0].AddStack(context);
                effects[0].Refresh(context);
                break;
            case StatusEffectStackingBehavior.Independent:
                AddNewEffect(effect);
                break;
        }
    }

    /// <summary>
    /// 检查实体是否具有特定ID的状态效果 
    /// 通过检查活跃效果字典中是否存在该ID的效果列表以及列表是否非空来确定
    /// </summary>
    /// <param name="effectId">效果ID</param>
    /// <returns></returns>
    public bool HasEffect(string effectId)
    {
        // 如果效果ID为空或仅包含空白字符 则返回false
        if (string.IsNullOrWhiteSpace(effectId))
        {
            return false;
        }

        // 通过检查活跃效果字典中是否存在该ID的效果列表
        // 以及列表是否非空来确定实体是否具有特定ID的状态效果
        return activeEffectsById.TryGetValue(effectId, out List<IStatusEffect> effects) && effects.Count > 0;
    }

    /// <summary>
    /// 得到指定ID的状态效果的总叠加层数
    /// </summary>
    /// <param name="effectId">状态效果id</param>
    /// <returns></returns>
    public int GetStackCount(string effectId)
    {
        // 如果效果ID为空或仅包含空白字符 则返回0
        if (string.IsNullOrWhiteSpace(effectId))
        {
            return 0;
        }

        // 通过检查活跃效果字典中是否存在该ID的效果列表
        // 以及列表是否非空来确定实体是否具有特定ID的状态效果 如果没有则返回0
        if (!activeEffectsById.TryGetValue(effectId, out List<IStatusEffect> effects) || effects.Count == 0)
        {
            return 0;
        }

        // 计算总叠加层数 
        int stackCount = 0;
        // 通过遍历效果列表 累加每个效果的层数  
        for (int i = 0; i < effects.Count; i++)
        {
            // 如果效果是StatusEffectInstance 则累加其StackCount
            if (effects[i] is StatusEffectInstance instance)
            {
                stackCount += instance.StackCount;
            }
            // 否则每个效果算作1层
            else if (effects[i] != null)
            {
                stackCount++;
            }
        }

        return stackCount;
    }

    /// <summary>
    /// 移除指定ID的所有状态效果 
    /// 通过从活跃效果字典中找到该ID的效果列表 并调用每个效果的移除逻辑来实现
    /// </summary>
    /// <param name="effectId">状态效果ID</param>
    /// <returns></returns>
    public bool RemoveEffect(string effectId)
    {
        // 如果效果ID为空或仅包含空白字符 则返回false
        if (string.IsNullOrWhiteSpace(effectId))
        {
            return false;
        }

        // 如果活跃效果字典中不存在该ID的效果列表 则返回false
        if (!activeEffectsById.TryGetValue(effectId, out List<IStatusEffect> effects))
        {
            return false;
        }

        // 否则 遍历效果列表 从后向前移除每个效果 并调用其移除逻辑
        for (int i = effects.Count - 1; i >= 0; i--)
        {
            RemoveEffectAt(effects, i);
        }

        // 清空字典
        activeEffectsById.Remove(effectId);
        return true;
    }

    /// <summary>
    /// 清理所有状态效果 
    /// 在实体死亡或重置时调用 以确保状态效果不会在实体死亡后继续影响实体或占用资源
    /// </summary>
    public void ClearAll()
    {
        foreach (List<IStatusEffect> effects in activeEffectsById.Values)
        {
            for (int i = effects.Count - 1; i >= 0; i--)
            {
                RemoveEffectAt(effects, i);
            }
        }

        activeEffectsById.Clear();
    }

    /// <summary>
    /// 处理实体死亡事件 
    /// 在实体死亡时清理所有状态效果 以确保状态效果不会在实体死亡后继续影响实体或占用资源
    /// </summary>
    /// <param name="eventSource">死亡事件源</param>
    private void DeathEvent_OnDeath(DeathEvent eventSource)
    {
        ClearAll();
    }

    /// <summary>
    /// 处理全局状态效果Tick事件
    /// </summary>
    /// <param name="deltaTime">时间增量</param>
    private void StatusEffectTickSystem_OnGlobalTick(float deltaTime)
    {
        TickEffects(deltaTime);
    }

    /// <summary>
    /// 在每个Tick周期更新状态效果的持续时间和触发效果逻辑
    /// </summary>
    /// <param name="deltaTime">时间增量</param>
    private void TickEffects(float deltaTime)
    {
        // 清空临时列表 用于在Tick过程中存储没有效果的ID 以便之后清理
        emptyEffectIds.Clear();

        // 遍历活跃效果字典中的每个效果列表 
        foreach (KeyValuePair<string, List<IStatusEffect>> pair in activeEffectsById)
        {
            List<IStatusEffect> effects = pair.Value;
            for (int i = effects.Count - 1; i >= 0; i--)
            {
                IStatusEffect effect = effects[i];
                if (effect == null)
                {
                    effects.RemoveAt(i);
                    continue;
                }

                // 调用每个效果的Tick逻辑 并检查是否过期 如果过期则移除效果
                effect.OnTick(context, deltaTime);
                if (effect.IsExpired)
                {
                    RemoveEffectAt(effects, i);
                }
            }

            // 如果效果列表在Tick后变为空
            // 则将该ID添加到临时列表中 以便之后从字典中清理
            if (effects.Count == 0)
            {
                emptyEffectIds.Add(pair.Key);
            }
        }

        // 从字典中清理没有效果的ID 以保持字典的整洁和性能
        for (int i = 0; i < emptyEffectIds.Count; i++)
        {
            activeEffectsById.Remove(emptyEffectIds[i]);
        }
    }

    /// <summary>
    /// 添加新效果到管理器中 并调用效果的应用逻辑 以及触发应用事件
    /// </summary>
    /// <param name="effect">状态效果</param>
    private void AddNewEffect(IStatusEffect effect)
    {
        // 如果字典中不存在该效果ID的列表 
        if (!activeEffectsById.TryGetValue(effect.EffectId, out List<IStatusEffect> effects))
        {
            // 则创建一个新的列表并添加到字典中
            effects = new List<IStatusEffect>(1);
            activeEffectsById.Add(effect.EffectId, effects);
        }

        // 将效果添加到列表中  
        effects.Add(effect);
        // 调用效果的应用逻辑
        effect.OnApply(context);
        // 以及触发应用事件
        OnEffectApplied?.Invoke(this, effect);
    }

    /// <summary>
    /// 重建状态效果上下文 
    /// 在实体的相关组件发生变化时调用 以确保状态效果实例访问到最新的信息和功能
    /// </summary>
    private void RebuildContext()
    {
        context = new StatusEffectContext(gameObject);
    }

    /// <summary>
    /// 从效果列表中移除指定索引的效果 并调用效果的移除逻辑 以及触发移除事件
    /// </summary>
    /// <param name="effects">效果列表</param>
    /// <param name="index">要移除的效果索引</param>
    private void RemoveEffectAt(List<IStatusEffect> effects, int index)
    {
        // 确保效果列表和索引有效
        if (effects == null || index < 0 || index >= effects.Count)
        {
            return;
        }

        // 获取要移除的效果 
        IStatusEffect effect = effects[index];
        // 调用效果的移除逻辑 
        effect?.OnRemove(context);
        effects.RemoveAt(index);
        if (effect != null)
        {
            // 以及触发移除事件
            OnEffectRemoved?.Invoke(this, effect);
        }
    }
}
