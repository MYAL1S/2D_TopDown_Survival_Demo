using UnityEngine;

public readonly struct AttackContext
{
    public readonly GameObject AttackerObject;
    public readonly GameObject TargetObject;
    public readonly Vector3 Origin;
    public readonly Vector3 Direction;
    public readonly float Damage;
    public readonly float Range;
    public readonly GameObject ProjectilePrefab;
    public readonly float ProjectileSpeed;
    public readonly float ProjectileHitRadius;
    public readonly WeaponConfig WeaponConfig;

    public AttackContext(
        GameObject attackerObject,
        GameObject targetObject,
        Vector3 origin,
        Vector3 direction,
        float damage,
        float range,
        GameObject projectilePrefab = null,
        float projectileSpeed = 0f,
        float projectileHitRadius = 0f,
        WeaponConfig weaponConfig = null)
    {
        AttackerObject = attackerObject;
        TargetObject = targetObject;
        Origin = origin;
        Direction = direction.sqrMagnitude > 0f ? direction.normalized : Vector3.right;
        Damage = Mathf.Max(0f, damage);
        Range = Mathf.Max(0f, range);
        ProjectilePrefab = projectilePrefab;
        ProjectileSpeed = Mathf.Max(0f, projectileSpeed);
        ProjectileHitRadius = Mathf.Max(0f, projectileHitRadius);
        WeaponConfig = weaponConfig;
    }

    public T GetAttackerComponent<T>() where T : Component
    {
        return AttackerObject != null ? AttackerObject.GetComponent<T>() : null;
    }

    public T GetTargetComponent<T>() where T : Component
    {
        return TargetObject != null ? TargetObject.GetComponent<T>() : null;
    }
}
