using UnityEngine;

/// <summary>
/// AOE武器攻击策略
/// </summary>
[CreateAssetMenu(fileName = "WeaponAttackStrategy_AOE", menuName = "ScriptableObjects/Weapons/Strategies/AOE Attack")]
public class AOEWeaponAttackStrategy : WeaponAttackStrategy
{
    public override bool TryExecute(AttackContext context, WeaponAttackServices services)
    {
        WeaponConfig weaponConfig = context.WeaponConfig;
        if (weaponConfig == null || weaponConfig.AOEConfig == null)
        {
            return false;
        }

        // 解析生成位置
        Vector3 spawnPosition = ResolveSpawnPosition(context, services, weaponConfig.AOESpawnPositionMode);
        return services.SpawnAOE(context, weaponConfig.AOEConfig, spawnPosition) != null;
    }

    private static Vector3 ResolveSpawnPosition(
        AttackContext context,
        WeaponAttackServices services,
        AOESpawnPositionMode spawnPositionMode)
    {
        // 根据配置的生成位置模式，确定AOE的生成位置
        switch (spawnPositionMode)
        {
            // 如果配置为攻击者位置，则使用攻击者对象的位置
            // 如果攻击者对象为空，则使用攻击的起点位置
            case AOESpawnPositionMode.Attacker:
                return context.AttackerObject != null ? context.AttackerObject.transform.position : context.Origin;
            case AOESpawnPositionMode.Origin:
                return context.Origin;
            case AOESpawnPositionMode.Target:
            default:
                return services.TryGetTarget(context, out Enemy target) ? target.transform.position : context.Origin;
        }
    }
}
