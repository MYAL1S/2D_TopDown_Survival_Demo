/// <summary>
/// 武器运行时实例
/// </summary>
public sealed class WeaponRuntimeInstance
{
    public WeaponRuntimeInstance(WeaponConfig config, float cooldownTimer = 0f)
    {
        Config = config;
        CooldownTimer = cooldownTimer;
        Level = 1;
        IsEnabled = true;
    }
    // 武器配置数据 包含武器的属性和攻击方式等信息
    public WeaponConfig Config { get; }
    // 武器冷却时间 当前武器在攻击后需要等待的时间才能再次攻击
    public float CooldownTimer { get; private set; }
    // 武器等级 影响武器的属性和攻击效果
    public int Level { get; private set; }
    // 武器是否启用 如果武器被禁用 则无法进行攻击
    public bool IsEnabled { get; set; }
    // 武器是否正在冷却 如果冷却时间大于0 则表示武器正在冷却中 不能进行攻击
    public bool IsCoolingDown => CooldownTimer > 0f;

    /// <summary>
    /// 更新武器的冷却时间 在每帧更新中调用 
    /// 根据传入的时间增量减少冷却时间 直到冷却时间降到0或以下 表示武器可以再次攻击
    /// </summary>
    /// <param name="deltaTime">帧间隔时间</param>
    public void TickCooldown(float deltaTime)
    {
        if (CooldownTimer > 0f)
        {
            CooldownTimer -= deltaTime;
        }
    }

    /// <summary>
    /// 重置武器的冷却时间 在武器成功攻击后调用
    /// </summary>
    public void ResetCooldown()
    {
        CooldownTimer = Config != null ? Config.Cooldown : 0f;
    }

    /// <summary>
    /// 设置武器等级 根据传入的等级值更新武器的等级属性
    /// </summary>
    /// <param name="level"></param>
    public void SetLevel(int level)
    {
        Level = level < 1 ? 1 : level;
    }
}
