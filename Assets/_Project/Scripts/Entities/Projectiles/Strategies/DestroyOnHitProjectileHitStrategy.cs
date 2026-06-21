using UnityEngine;

[CreateAssetMenu(fileName = "ProjectileHitStrategy_DestroyOnHit", menuName = "ScriptableObjects/Weapons/Strategies/Projectile Hit/Destroy On Hit")]
public class DestroyOnHitProjectileHitStrategy : ProjectileHitStrategy
{
    public override bool RequiresAliveTarget => true;
    public override bool UsesHomingTarget => true;
    public override bool AcceptsOnlyInitialTarget => true;

    public override void HandleHit(ProjectileController projectile, Enemy enemy)
    {
        // 如果敌人被成功击中并且造成了伤害，则释放（销毁）这个子弹
        if (projectile.TryApplyConfiguredDamage(enemy, projectile.transform.position))
        {
            projectile.ReleaseProjectile();
        }
    }
}
