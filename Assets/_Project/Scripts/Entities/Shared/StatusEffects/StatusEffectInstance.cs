using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 状态效果实例类
/// </summary>
public class StatusEffectInstance : IStatusEffect
{
    // 配置数据
    private readonly StatusEffectConfig config;
    // 修饰器列表
    private readonly List<IStatusEffectModifierRuntime> runtimeModifiers =
        new List<IStatusEffectModifierRuntime>(4);

    // 当前剩余持续时间
    private float remainingDuration;
    // 当前层数
    private int stackCount = 1;
    // 状态效果来源上下文
    private StatusEffectSourceContext sourceContext;

    public StatusEffectInstance(StatusEffectConfig config)
        : this(config, StatusEffectSourceContext.None)
    {
    }

    public StatusEffectInstance(StatusEffectConfig config, StatusEffectSourceContext sourceContext)
    {
        this.config = config;
        this.sourceContext = sourceContext;
        remainingDuration = config != null ? config.Duration : 0f;
        BuildRuntimeModifiers();
    }

    // 状态效果ID 由配置提供 如果配置为null 则返回空字符串
    public string EffectId => config != null ? config.EffectId : string.Empty;
    // 状态效果叠加行为 由配置提供 如果配置为null 则默认为刷新持续时间
    public StatusEffectStackingBehavior StackingBehavior =>
        config != null ? config.StackingBehavior : StatusEffectStackingBehavior.Refresh;
    // 状态效果是否过期 当配置为null或剩余持续时间小于等于0时 认为状态效果已过期
    public bool IsExpired => config == null || remainingDuration <= 0f;
    // 当前层数 由内部字段提供
    public int StackCount => stackCount;
    // 当前剩余持续时间 由内部字段提供
    public float RemainingDuration => remainingDuration;
    // 状态效果配置 由内部字段提供
    public StatusEffectConfig Config => config;
    // 状态效果来源上下文 由内部字段提供
    public StatusEffectSourceContext SourceContext => sourceContext;

    /// <summary>
    /// 应用状态效果时调用 触发所有修饰器的OnApply方法
    /// </summary>
    /// <param name="context">状态效果上下文</param>
    public void OnApply(StatusEffectContext context)
    {
        for (int i = 0; i < runtimeModifiers.Count; i++)
        {
            runtimeModifiers[i].OnApply(context, this);
        }
    }

    /// <summary>
    /// 每帧调用 触发所有修饰器的OnTick方法 同时更新剩余持续时间
    /// </summary>
    /// <param name="context">状态效果上下文</param>
    /// <param name="deltaTime">时间增量</param>
    public void OnTick(StatusEffectContext context, float deltaTime)
    {
        // 如果配置为null 则直接将剩余持续时间设为0并返回
        if (config == null)
        {
            remainingDuration = 0f;
            return;
        }

        // 更新剩余持续时间
        remainingDuration -= Mathf.Max(0f, deltaTime);
        // 触发所有修饰器的OnTick方法
        for (int i = 0; i < runtimeModifiers.Count; i++)
        {
            runtimeModifiers[i].OnTick(context, this, deltaTime);
        }
    }

    /// <summary>
    /// 移除状态效果时调用 触发所有修饰器的OnRemove方法 
    /// 反向遍历以确保修饰器按相反的顺序移除
    /// </summary>
    /// <param name="context">状态效果上下文</param>
    public void OnRemove(StatusEffectContext context)
    {
        // 倒序遍历移除修饰器
        for (int i = runtimeModifiers.Count - 1; i >= 0; i--)
        {
            runtimeModifiers[i].OnRemove(context, this);
        }
    }

    /// <summary>
    /// 刷新状态效果时调用 触发所有修饰器的OnRefresh方法 同时重置剩余持续时间
    /// </summary>
    /// <param name="context">状态效果上下文</param>
    public void Refresh(StatusEffectContext context)
    {
        // 如果配置不为空 
        if (config != null)
        {
            // 重置剩余持续时间为配置中的持续时间
            remainingDuration = config.Duration;
        }

        // 触发所有修饰器的OnRefresh方法
        for (int i = 0; i < runtimeModifiers.Count; i++)
        {
            runtimeModifiers[i].OnRefresh(context, this);
        }
    }

    /// <summary>
    /// 更新状态效果来源上下文
    /// </summary>
    /// <param name="sourceContext"></param>
    public void UpdateSourceContext(StatusEffectSourceContext sourceContext)
    {
        this.sourceContext = sourceContext;
    }

    /// <summary>
    /// 增加状态效果层数时调用 触发所有修饰器的OnStackChanged方法 同时更新当前层数
    /// </summary>
    /// <param name="context">状态效果上下文</param>
    public void AddStack(StatusEffectContext context)
    {
        // 如果配置为null 则直接返回 因为无法确定最大层数
        if (config == null)
        {
            return;
        }

        // 记录之前的层数
        int previousStackCount = stackCount;
        // 增加层数并确保不超过配置中的最大层数
        stackCount = Mathf.Min(stackCount + 1, config.MaxStacks);
        // 如果层数没有变化 则直接返回 不触发修饰器的OnStackChanged方法
        if (stackCount == previousStackCount)
        {
            return;
        }

        // 触发所有修饰器的OnStackChanged方法
        for (int i = 0; i < runtimeModifiers.Count; i++)
        {
            runtimeModifiers[i].OnStackChanged(context, this);
        }
    }

    /// <summary>
    /// 构建运行时修饰器列表
    /// </summary>
    private void BuildRuntimeModifiers()
    {
        // 清空现有的运行时修饰器列表
        runtimeModifiers.Clear();
        // 从配置中获取修饰器列表
        // 如果配置或修饰器列表为null 则直接返回
        if (config == null || config.Modifiers == null)
        {
            return;
        }

        IReadOnlyList<StatusEffectModifier> modifiers = config.Modifiers;
        // 遍历配置中的修饰器列表
        for (int i = 0; i < modifiers.Count; i++)
        {
            StatusEffectModifier modifier = modifiers[i];
            if (modifier == null)
            {
                continue;
            }

            // 创建对应的运行时修饰器并添加到运行时修饰器列表中
            IStatusEffectModifierRuntime runtimeModifier = modifier.CreateRuntimeModifier();
            if (runtimeModifier != null)
            {
                runtimeModifiers.Add(runtimeModifier);
            }
        }
    }
}
