/// <summary>
/// 状态效果修饰器运行时接口
/// </summary>
public interface IStatusEffectModifierRuntime
{
    /// <summary>
    /// 状态效果应用时调用
    /// </summary>
    /// <param name="context">状态效果上下文</param>
    /// <param name="effect">状态效果实例</param>
    void OnApply(StatusEffectContext context, StatusEffectInstance effect);
    /// <summary>
    /// 状态效果刷新时调用
    /// </summary>
    /// <param name="context">状态效果上下文</param>
    /// <param name="effect">状态效果实例</param>
    void OnRefresh(StatusEffectContext context, StatusEffectInstance effect);
    /// <summary>
    /// 状态效果堆叠变化时调用
    /// </summary>
    /// <param name="context">状态效果上下文</param>
    /// <param name="effect">状态效果实例</param>
    void OnStackChanged(StatusEffectContext context, StatusEffectInstance effect);
    /// <summary>
    /// 状态效果每帧调用
    /// </summary>
    /// <param name="context">状态效果上下文</param>
    /// <param name="effect">状态效果实例</param>
    /// <param name="deltaTime">时间增量</param>
    void OnTick(StatusEffectContext context, StatusEffectInstance effect, float deltaTime);
    /// <summary>
    /// 状态效果移除时调用
    /// </summary>
    /// <param name="context">状态效果上下文</param>
    /// <param name="effect">状态效果实例</param>
    void OnRemove(StatusEffectContext context, StatusEffectInstance effect);
}
