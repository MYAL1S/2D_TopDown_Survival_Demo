using UnityEngine;

/// <summary>
/// 投射物攻击策略
/// </summary>
[CreateAssetMenu(fileName = "WeaponAttackStrategy_Projectile", menuName = "ScriptableObjects/Weapons/Strategies/Projectile Attack")]
public class ProjectileWeaponAttackStrategy : WeaponAttackStrategy
{
    /// <summary>
    /// 执行攻击逻辑
    /// </summary>
    /// <param name="context">攻击上下文</param>
    /// <param name="services">武器攻击服务</param>
    /// <returns></returns>
    public override bool TryExecute(AttackContext context, WeaponAttackServices services)
    {
        // 获取攻击者的武器配置和目标敌人
        WeaponConfig weaponConfig = context.WeaponConfig;
        // 如果武器配置无效或无法获取目标敌人 则攻击失败
        if (weaponConfig == null || !services.TryGetTarget(context, out Enemy target))
        {
            return false;
        }

        // 如果武器没有投射物预制体 则直接应用配置的伤害
        if (context.ProjectilePrefab == null)
        {
            return services.ApplyConfiguredDamage(target, target.transform.position, context.Damage, weaponConfig);
        }

        // 发射投射物
        services.FireProjectile(context, target);
        return true;
    }
}
