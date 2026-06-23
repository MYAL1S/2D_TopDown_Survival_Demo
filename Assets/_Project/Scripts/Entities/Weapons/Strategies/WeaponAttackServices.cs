using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 武器攻击服务类 
/// 提供了武器攻击相关的服务方法
/// </summary>
public sealed class WeaponAttackServices
{
    // 玩家武器系统引用
    // 用于处理武器发射逻辑
    private readonly PlayerWeaponSystem weaponSystem;
    // 缓存的AOE区域目标列表 避免频繁分配内存
    private readonly List<Enemy> areaTargets = new List<Enemy>(32);

    public WeaponAttackServices(PlayerWeaponSystem weaponSystem)
    {
        this.weaponSystem = weaponSystem;
    }

    /// <summary>
    /// 尝试获取攻击上下文中的目标敌人
    /// </summary>
    /// <param name="context">攻击上下文</param>
    /// <param name="target">目标敌人</param>
    /// <returns></returns>
    public bool TryGetTarget(AttackContext context, out Enemy target)
    {
        target = context.GetTargetComponent<Enemy>();
        return target != null && target.IsAlive;
    }

    /// <summary>
    /// 发射武器的投射物
    /// </summary>
    /// <param name="context">攻击上下文</param>
    /// <param name="target">目标敌人</param>
    public void FireProjectile(AttackContext context, Enemy target)
    {
        weaponSystem.FireProjectile(context, target);
    }

    /// <summary>
    /// 应用配置的伤害到目标敌人
    /// </summary>
    /// <param name="target">目标敌人</param>
    /// <param name="impactPosition">击中位置</param>
    /// <param name="damage">伤害值</param>
    /// <param name="weaponConfig">武器配置</param>
    /// <returns></returns>
    public bool ApplyConfiguredDamage(
        AttackContext context,
        Enemy target,
        Vector3 impactPosition,
        float damage,
        WeaponConfig weaponConfig)
    {
        StatusEffectSourceContext sourceContext = new StatusEffectSourceContext(
            context.AttackerObject,
            context.AttackerObject,
            context.AttackerObject,
            weaponConfig);

        return WeaponDamageApplier.ApplyConfiguredDamage(
            target,
            impactPosition,
            damage,
            weaponConfig,
            areaTargets,
            sourceContext);
    }

    /// <summary>
    /// 生成AOE区域
    /// </summary>
    /// <param name="context">攻击上下文</param>
    /// <param name="aoeConfig">AOE配置</param>
    /// <param name="position">生成位置</param>
    /// <returns></returns>
    public AOEZoneController SpawnAOE(AttackContext context, AOEConfig aoeConfig, Vector3 position)
    {
        return AOESystem.SpawnAOE(aoeConfig, position, context.AttackerObject, context.WeaponConfig);
    }
}
