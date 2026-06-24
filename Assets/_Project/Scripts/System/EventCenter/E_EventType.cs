/// <summary>
/// 事件中心使用的事件类型枚举
/// 新增跨系统通知时，在这里添加枚举值并通过 EventCenter 触发或监听
/// </summary>
public enum E_EventType
{
    /// <summary>
    /// 玩家完整数据发生变化，例如金币、解锁角色、解锁关卡或选中角色改变
    /// </summary>
    PlayerDataChanged,
    /// <summary>
    /// 玩家金币数量发生变化
    /// </summary>
    PlayerGoldChanged,
    /// <summary>
    /// 有角色被解锁，事件参数为角色 id
    /// </summary>
    CharacterUnlocked,
    /// <summary>
    /// 有关卡被解锁，事件参数为关卡 id
    /// </summary>
    StageUnlocked,
    /// <summary>
    /// 关卡计时发生变化，事件参数为 StageRuntimeInfo
    /// </summary>
    StageTimeChanged,
    /// <summary>
    /// 当前关卡通关
    /// </summary>
    StageCleared,
    /// <summary>
    /// 玩家拾取金币，事件参数为拾取到的金币数量
    /// </summary>
    GoldPickedUp,
    /// <summary>
    /// 敌人被击杀，事件参数为被击杀的 Enemy
    /// </summary>
    EnemyKilled,
    /// <summary>
    /// 玩家死亡
    /// </summary>
    PlayerDied,
    /// <summary>
    /// 当前关卡统计数据变化，事件参数为 LevelStatsInfo
    /// </summary>
    LevelStatsChanged,
    /// <summary>
    /// 游戏暂停
    /// </summary>
    GamePaused,
    /// <summary>
    /// 游戏恢复
    /// </summary>
    GameResumed,
}
