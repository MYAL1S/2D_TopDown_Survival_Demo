using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 单体伤害策略
/// </summary>
[CreateAssetMenu(fileName = "WeaponDamageStrategy_SingleTarget", menuName = "ScriptableObjects/Weapons/Strategies/Damage/Single Target")]
public class SingleTargetDamageStrategy : WeaponDamageStrategy
{
    public override bool ApplyDamage(
        Enemy target,
        Vector3 impactPosition,
        float damage,
        WeaponConfig weaponConfig,
        List<Enemy> areaTargets)
    {
        return WeaponDamageApplier.ApplySingleTargetDamage(target, damage);
    }
}
