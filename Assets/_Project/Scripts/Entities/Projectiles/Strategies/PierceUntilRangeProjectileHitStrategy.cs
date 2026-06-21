using UnityEngine;

[CreateAssetMenu(fileName = "ProjectileHitStrategy_PierceUntilRange", menuName = "ScriptableObjects/Weapons/Strategies/Projectile Hit/Pierce Until Range")]
public class PierceUntilRangeProjectileHitStrategy : ProjectileHitStrategy
{
    
    public override void HandleHit(ProjectileController projectile, Enemy enemy)
    {
        projectile.TryApplyConfiguredDamage(enemy, projectile.transform.position);
    }
}
