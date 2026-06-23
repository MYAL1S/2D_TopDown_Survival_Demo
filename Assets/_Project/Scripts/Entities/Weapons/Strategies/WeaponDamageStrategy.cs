using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 武器伤害策略 抽象基类 定义了应用伤害的接口
/// </summary>
public abstract class WeaponDamageStrategy : ScriptableObject
{
    public abstract bool ApplyDamage(
        Enemy target,
        Vector3 impactPosition,
        float damage,
        WeaponConfig weaponConfig,
        List<Enemy> areaTargets,
        StatusEffectSourceContext sourceContext);
}
