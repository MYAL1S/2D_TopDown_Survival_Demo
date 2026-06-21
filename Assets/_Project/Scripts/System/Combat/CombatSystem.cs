using UnityEngine;

/// <summary>
/// 战斗系统
/// </summary>
public static class CombatSystem
{
    /// <summary>
    /// 提供给外部用于对目标Health组件造成伤害的方法
    /// 该方法会检查目标是否存在且仍然存活
    /// 然后调用Health组件的TakeDamage方法来应用伤害
    /// </summary>
    /// <param name="targetHealth">目标的Health组件</param>
    /// <param name="damageAmount">造成的伤害</param>
    /// <returns></returns>
    public static bool ApplyDamage(Health targetHealth, float damageAmount)
    {
        if (targetHealth == null || !targetHealth.IsAlive)
        {
            return false;
        }

        targetHealth.TakeDamage(Mathf.Max(0f, damageAmount));
        return true;
    }
}
