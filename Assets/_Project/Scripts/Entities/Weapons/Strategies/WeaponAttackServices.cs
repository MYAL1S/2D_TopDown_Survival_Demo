using System.Collections.Generic;
using UnityEngine;

public sealed class WeaponAttackServices
{
    private readonly PlayerWeaponSystem weaponSystem;
    private readonly List<Enemy> areaTargets = new List<Enemy>(32);

    public WeaponAttackServices(PlayerWeaponSystem weaponSystem)
    {
        this.weaponSystem = weaponSystem;
    }

    public bool TryGetTarget(AttackContext context, out Enemy target)
    {
        target = context.GetTargetComponent<Enemy>();
        return target != null && target.IsAlive;
    }

    public void FireProjectile(AttackContext context, Enemy target)
    {
        weaponSystem.FireProjectile(context, target);
    }

    public bool ApplyConfiguredDamage(Enemy target, Vector3 impactPosition, float damage, WeaponConfig weaponConfig)
    {
        return WeaponDamageApplier.ApplyConfiguredDamage(target, impactPosition, damage, weaponConfig, areaTargets);
    }
}
