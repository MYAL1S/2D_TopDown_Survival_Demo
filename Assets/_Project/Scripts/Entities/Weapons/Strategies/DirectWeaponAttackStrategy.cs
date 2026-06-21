using UnityEngine;

/// <summary>
/// 直接攻击策略 
/// 适用于近战武器或瞬发远程武器 直接对目标造成伤害
/// </summary>
[CreateAssetMenu(fileName = "WeaponAttackStrategy_Direct", menuName = "ScriptableObjects/Weapons/Strategies/Direct Attack")]
public class DirectWeaponAttackStrategy : WeaponAttackStrategy
{
    /// <summary>
    /// 尝试执行直接攻击
    /// </summary>
    /// <param name="context">攻击上下文</param>
    /// <param name="services">武器攻击服务</param>
    /// <returns></returns>
    public override bool TryExecute(AttackContext context, WeaponAttackServices services)
    {
        // 获取武器配置
        WeaponConfig weaponConfig = context.WeaponConfig;
        // 如果没有配置，无法执行攻击
        if (weaponConfig == null)
        {
            return false;
        }

        // 尝试获取目标敌人 
        services.TryGetTarget(context, out Enemy target);
        // 如果没有目标则使用攻击起点作为伤害位置
        Vector3 impactPosition = target != null ? target.transform.position : context.Origin;
        return services.ApplyConfiguredDamage(target, impactPosition, context.Damage, weaponConfig);
    }
}
