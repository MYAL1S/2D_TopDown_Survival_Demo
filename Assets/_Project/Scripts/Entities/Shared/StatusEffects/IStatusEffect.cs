/// <summary>
/// 状态效果接口
/// </summary>
public interface IStatusEffect
{
    /// <summary>
    /// 状态效果的唯一标识符
    /// </summary>
    string EffectId { get; }
    /// <summary>
    /// 状态效果的叠加行为
    /// </summary>
    StatusEffectStackingBehavior StackingBehavior { get; }
    /// <summary>
    /// 状态效果来源上下文
    /// </summary>
    StatusEffectSourceContext SourceContext { get; }
    /// <summary>
    /// 状态是否已过期 
    /// 过期的状态效果将被移除
    /// </summary>
    bool IsExpired { get; }
    /// <summary>
    /// 状态效果应用时的回调
    /// </summary>
    /// <param name="context">状态效果上下文</param>
    void OnApply(StatusEffectContext context);
    /// <summary>
    /// 状态效果每帧更新时的回调
    /// </summary>
    /// <param name="context">状态效果上下文</param>
    /// <param name="deltaTime">帧间隔时间</param>
    void OnTick(StatusEffectContext context, float deltaTime);
    /// <summary>
    /// 状态效果被移除时的回调
    /// </summary>
    /// <param name="context">状态效果上下文</param>
    void OnRemove(StatusEffectContext context);
    /// <summary>
    /// 状态效果被刷新时的回调
    /// </summary>
    /// <param name="context">状态效果上下文</param>
    void Refresh(StatusEffectContext context);
    /// <summary>
    /// 状态效果的来源上下文被更新时的回调
    /// </summary>
    /// <param name="sourceContext">状态效果来源上下文</param>
    void UpdateSourceContext(StatusEffectSourceContext sourceContext);
    /// <summary>
    /// 状态效果被叠加时的回调
    /// </summary>
    /// <param name="context">状态效果上下文</param>
    void AddStack(StatusEffectContext context);
}
