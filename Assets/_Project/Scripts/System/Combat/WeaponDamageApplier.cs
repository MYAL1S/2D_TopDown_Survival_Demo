using System.Collections.Generic;
using UnityEngine;

public static class WeaponDamageApplier
{
    public static bool ApplyConfiguredDamage(
        Enemy target,
        Vector3 impactPosition,
        float damage,
        WeaponConfig weaponConfig,
        List<Enemy> areaTargets)
    {
        if (weaponConfig == null || weaponConfig.DamageStrategy == null)
        {
            return false;
        }

        return weaponConfig.DamageStrategy.ApplyDamage(target, impactPosition, damage, weaponConfig, areaTargets);
    }

    public static bool ApplySingleTargetDamage(Enemy target, float damage)
    {
        if (target == null || !target.IsAlive)
        {
            return false;
        }

        CombatSystem.ApplyDamage(target.Health, damage);
        return true;
    }

    public static bool ApplyAreaDamage(Vector3 origin, float radius, float damage, List<Enemy> areaTargets)
    {
        if (areaTargets == null)
        {
            return false;
        }

        TargetingSystem.CollectAliveEnemiesInRange(origin, radius, areaTargets);
        bool appliedDamage = false;
        for (int i = 0; i < areaTargets.Count; i++)
        {
            appliedDamage |= ApplySingleTargetDamage(areaTargets[i], damage);
        }

        return appliedDamage;
    }
}
