using UnityEngine;

#region REQUIREMENTS
[RequireComponent(typeof(InjuredEvent))]
[RequireComponent(typeof(DeathEvent))]
#endregion
[DisallowMultipleComponent]
public class Health : MonoBehaviour
{
    [Min(1f)]
    private float maxHealth = 1f;

    [Min(0f)]
    private float defense;

    private InjuredEvent injuredEvent;
    private DeathEvent deathEvent;

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
    /// 初始化生命值组件 设置最大生命值和防御力 并重置当前生命值为最大生命值
    /// </summary>
    /// <param name="maxHealth">最大生命值</param>
    /// <param name="defense">防御力</param>
    public void Initialize(float maxHealth, float defense = 0f)
    {
        this.maxHealth = Mathf.Max(1f, maxHealth);
        this.defense = Mathf.Max(0f, defense);
        ResetHealth();
    }

    /// <summary>
    /// 重置当前生命值为最大生命值 并将存活状态设置为 true
    /// </summary>
    public void ResetHealth()
    {
        CurrentHealth = maxHealth;
        IsAlive = true;
    }

    /// <summary>
    /// 对生命值组件造成伤害 
    /// 根据防御力计算最终伤害值 
    /// 并更新当前生命值 如果当前生命值降至 0 或以下 
    /// 则触发死亡事件
    /// </summary>
    /// <param name="amount"></param>
    public void TakeDamage(float amount)
    {
        if (!IsAlive)
        {
            return;
        }

        int finalDamage = Mathf.Max(1, Mathf.RoundToInt(amount - defense));
        CurrentHealth = Mathf.Max(0f, CurrentHealth - finalDamage);
        injuredEvent.CallInjuredEvent(finalDamage);

        if (CurrentHealth <= 0f)
        {
            Die();
        }
    }

    /// <summary>
    /// 治疗方法
    /// </summary>
    /// <param name="amount">治疗的血量</param>
    /// <returns></returns>
    public float Heal(float amount)
    {
        // 只有当生命值组件处于存活状态时才允许治疗
        if (!IsAlive)
        {
            return 0f;
        }

        float healAmount = Mathf.Max(0f, amount);
        // 如果治疗量小于等于 0 或者当前生命值已经满了 就不进行治疗
        if (healAmount <= 0f || CurrentHealth >= maxHealth)
        {
            return 0f;
        }

        // 计算实际治疗量 
        float previousHealth = CurrentHealth;
        // 将当前生命值增加治疗量但不超过最大生命值 
        CurrentHealth = Mathf.Min(maxHealth, CurrentHealth + healAmount);
        // 返回实际治疗的血量
        return CurrentHealth - previousHealth;
    }

    /// <summary>
    /// 提供一个直接杀死生命值组件的方法 
    /// 将当前生命值设置为 0 并触发死亡事件
    /// </summary>
    public void Kill()
    {
        if (!IsAlive)
        {
            return;
        }

        CurrentHealth = 0f;
        Die();
    }

    /// <summary>
    /// 死亡逻辑 
    /// 将存活状态设置为 false 并触发死亡事件
    /// </summary>
    private void Die()
    {
        IsAlive = false;
        deathEvent.CallDeathEvent();
    }
}
