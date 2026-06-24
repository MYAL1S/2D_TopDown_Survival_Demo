/// <summary>
/// 拾取物效果类型
/// PickupSystem 会根据该枚举决定拾取物作用到玩家还是通知关卡管理器
/// </summary>
public enum ItemEffectType
{
    // 增加玩家经验
    Experience,
    // 恢复玩家生命值
    Heal,
    // 增加玩家局内分数
    Score,
    // 增加本局金币收益，并通过事件通知 GamePanel 刷新显示
    Gold
}
