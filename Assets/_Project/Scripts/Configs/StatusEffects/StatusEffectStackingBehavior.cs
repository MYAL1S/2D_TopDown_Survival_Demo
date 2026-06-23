/// <summary>
/// 状态效果叠加行为
/// </summary>
public enum StatusEffectStackingBehavior
{
    /// <summary>
    /// 当应用相同的状态效果时
    /// 刷新持续时间并重置堆叠计数
    /// </summary>
    Refresh,
    /// <summary>
    /// 当应用相同的状态效果时
    /// 叠加效果并增加堆叠计数
    /// </summary>
    Stack,
    /// <summary>
    /// 当应用相同的状态效果时
    /// 不会影响当前的状态效果
    /// </summary>
    Independent
}
