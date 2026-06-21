using UnityEngine;

/// <summary>
/// 玩家伤害接收组件
/// </summary>
[RequireComponent(typeof(Health))]
[DisallowMultipleComponent]
public class PlayerDamageReceiver : MonoBehaviour
{
    // 无敌持续时间
    // 受到伤害后玩家将进入无敌状态 在此期间无法再次受到伤害
    [SerializeField]
    [Min(0f)]
    private float invincibilityDuration = 0.75f;

    // 生命值组件 用于管理玩家的生命值状态
    private Health health;
    // 无敌状态结束时间 用于判断玩家是否处于无敌状态
    private float invincibleUntilTime;

    // 玩家是否处于无敌状态
    public bool IsInvincible => Time.time < invincibleUntilTime;

    private void Awake()
    {
        // 获取生命值组件 以便后续管理玩家的生命值状态
        health = GetComponent<Health>();
    }

    /// <summary>
    /// 初始化玩家伤害接收组件 设置无敌持续时间
    /// </summary>
    /// <param name="invincibilityDuration">无敌持续时间</param>
    public void Initialize(float invincibilityDuration)
    {
        this.invincibilityDuration = Mathf.Max(0f, invincibilityDuration);
        invincibleUntilTime = 0f;
    }

    /// <summary>
    /// 尝试对玩家造成伤害 如果玩家处于无敌状态或已死亡 则不会造成伤害
    /// </summary>
    /// <param name="damageAmount">受到的伤害</param>
    /// <returns></returns>
    public bool TryTakeDamage(float damageAmount)
    {
        if (health == null || !health.IsAlive || IsInvincible)
        {
            return false;
        }

        // 调用战斗系统的 ApplyDamage 方法来处理伤害应用逻辑
        bool damageApplied = CombatSystem.ApplyDamage(health, damageAmount);
        // 如果成功应用了伤害并且玩家仍然存活 则进入无敌状态
        if (damageApplied && health.IsAlive)
        {
            invincibleUntilTime = Time.time + invincibilityDuration;
        }

        return damageApplied;
    }
}
