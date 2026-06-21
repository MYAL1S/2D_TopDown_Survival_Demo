using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 范围伤害策略
/// 适用于爆炸物等对一定范围内的敌人造成伤害的武器
/// </summary>
[CreateAssetMenu(fileName = "WeaponDamageStrategy_Area", menuName = "ScriptableObjects/Weapons/Strategies/Damage/Area")]
public class AreaDamageStrategy : WeaponDamageStrategy
{
    public override bool ApplyDamage(
        Enemy target,
        Vector3 impactPosition,
        float damage,
        WeaponConfig weaponConfig,
        List<Enemy> areaTargets)
    {
        if (weaponConfig == null)
        {
            return false;
        }

        return WeaponDamageApplier.ApplyAreaDamage(impactPosition, weaponConfig.DamageRadius, damage, areaTargets);
    }
}
