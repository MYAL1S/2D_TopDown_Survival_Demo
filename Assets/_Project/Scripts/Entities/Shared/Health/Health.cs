using System;
using UnityEngine;

#region REQUIREMENTS
[RequireComponent(typeof(InjuredEvent))]
[RequireComponent(typeof(DeathEvent))]
#endregion
[DisallowMultipleComponent]
/// <summary>
/// 通用生命值组件
/// 负责管理当前生命、最大生命、防御、受伤事件、死亡事件以及生命变化通知
/// </summary>
public class Health : MonoBehaviour
{
    // 最大生命值，至少为 1
    [Min(1f)]
    private float maxHealth = 1f;

    // 防御值，会从受到的伤害中扣除
    [Min(0f)]
    private float defense;

    private InjuredEvent injuredEvent;
    private DeathEvent deathEvent;

    // 生命值发生变化时触发，玩家血条通过该事件刷新显示
    public event Action<Health> OnHealthChanged;

    public float MaxHealth => maxHealth;
    public float CurrentHealth { get; private set; }
    public float Defense => defense;
    public bool IsAlive { get; private set; }

    private void Awake()
    {
        injuredEvent = GetComponent<InjuredEvent>();
        deathEvent = GetComponent<DeathEvent>();

        if (CurrentHealth <= 0f)
        {
            ResetHealth();
        }
    }

    /// <summary>
    /// 初始化生命组件，并重置当前生命到最大值
    /// </summary>
    /// <param name="maxHealth">最大生命值</param>
    /// <param name="defense">防御值</param>
    public void Initialize(float maxHealth, float defense = 0f)
    {
        this.maxHealth = Mathf.Max(1f, maxHealth);
        this.defense = Mathf.Max(0f, defense);
        ResetHealth();
    }

    /// <summary>
    /// 将当前生命恢复到最大值，并标记为存活
    /// </summary>
    public void ResetHealth()
    {
        CurrentHealth = maxHealth;
        IsAlive = true;
        NotifyHealthChanged();
    }

    /// <summary>
    /// 对该生命组件造成伤害
    /// 伤害会先扣除防御值，至少造成 1 点伤害；生命归零时触发死亡事件
    /// </summary>
    /// <param name="amount">原始伤害值。</param>
    public void TakeDamage(float amount)
    {
        if (!IsAlive)
        {
            return;
        }

        int finalDamage = Mathf.Max(1, Mathf.RoundToInt(amount - defense));
        CurrentHealth = Mathf.Max(0f, CurrentHealth - finalDamage);
        injuredEvent.CallInjuredEvent(finalDamage);
        NotifyHealthChanged();

        if (CurrentHealth <= 0f)
        {
            Die();
        }
    }

    /// <summary>
    /// 治疗当前对象
    /// </summary>
    /// <param name="amount">尝试恢复的生命值</param>
    /// <returns>实际恢复的生命值。</returns>
    public float Heal(float amount)
    {
        if (!IsAlive)
        {
            return 0f;
        }

        float healAmount = Mathf.Max(0f, amount);
        if (healAmount <= 0f || CurrentHealth >= maxHealth)
        {
            return 0f;
        }

        float previousHealth = CurrentHealth;
        CurrentHealth = Mathf.Min(maxHealth, CurrentHealth + healAmount);
        NotifyHealthChanged();
        return CurrentHealth - previousHealth;
    }

    /// <summary>
    /// 直接击杀当前对象
    /// </summary>
    public void Kill()
    {
        if (!IsAlive)
        {
            return;
        }

        CurrentHealth = 0f;
        NotifyHealthChanged();
        Die();
    }

    /// <summary>
    /// 通知外部生命值发生变化
    /// </summary>
    private void NotifyHealthChanged()
    {
        OnHealthChanged?.Invoke(this);
    }

    /// <summary>
    /// 执行死亡逻辑，并触发 DeathEvent
    /// </summary>
    private void Die()
    {
        IsAlive = false;
        deathEvent.CallDeathEvent();
    }
}
