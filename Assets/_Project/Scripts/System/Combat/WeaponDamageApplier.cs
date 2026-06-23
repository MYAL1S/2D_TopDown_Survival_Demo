using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 武器伤害应用器
/// </summary>
public static class WeaponDamageApplier
{
    /// <summary>
    /// 应用配置的伤害策略
    /// </summary>
    /// <param name="target">目标敌人</param>
    /// <param name="impactPosition">击中位置</param>
    /// <param name="damage">伤害值</param>
    /// <param name="weaponConfig">武器配置</param>
    /// <param name="areaTargets">范围内的敌人列表</param>
    /// <returns></returns>
    public static bool ApplyConfiguredDamage(
        Enemy target,
        Vector3 impactPosition,
        float damage,
        WeaponConfig weaponConfig,
        List<Enemy> areaTargets)
    {
        return ApplyConfiguredDamage(
            target,
            impactPosition,
            damage,
            weaponConfig,
            areaTargets,
            StatusEffectSourceContext.None);
    }

    /// <summary>
    /// 应用配置的伤害策略
    /// </summary>
    /// <param name="target">目标敌人</param>
    /// <param name="impactPosition">击中位置</param>
    /// <param name="damage">伤害值</param>
    /// <param name="weaponConfig">武器配置</param>
    /// <param name="areaTargets">范围内的敌人列表</param>
    /// <param name="sourceContext">状态效果源上下文</param>
    /// <returns></returns>
    public static bool ApplyConfiguredDamage(
        Enemy target,
        Vector3 impactPosition,
        float damage,
        WeaponConfig weaponConfig,
        List<Enemy> areaTargets,
        StatusEffectSourceContext sourceContext)
    {
        if (weaponConfig == null || weaponConfig.DamageStrategy == null)
        {
            return false;
        }

        // 解析状态效果源上下文 确保使用正确的武器配置
        StatusEffectSourceContext resolvedSource = ResolveWeaponSource(sourceContext, weaponConfig);
        return weaponConfig.DamageStrategy.ApplyDamage(
            target,
            impactPosition,
            damage,
            weaponConfig,
            areaTargets,
            resolvedSource);
    }

    /// <summary>
    /// 应用单体伤害
    /// </summary>
    /// <param name="target">目标敌人</param>
    /// <param name="damage">伤害值</param>
    /// <returns></returns>
    public static bool ApplySingleTargetDamage(Enemy target, float damage)
    {
        return ApplySingleTargetDamage(target, damage, null);
    }

    /// <summary>
    /// 应用单体伤害
    /// 并根据武器配置应用状态效果
    /// </summary>
    /// <param name="target">目标敌人</param>
    /// <param name="damage">伤害值</param>
    /// <param name="weaponConfig">武器配置</param>
    /// <returns></returns>
    public static bool ApplySingleTargetDamage(Enemy target, float damage, WeaponConfig weaponConfig)
    {
        return ApplySingleTargetDamage(target, damage, weaponConfig, StatusEffectSourceContext.None);
    }

    /// <summary>
    /// 应用单体伤害
    /// </summary>
    /// <param name="target">目标敌人</param>
    /// <param name="damage">伤害值</param>
    /// <param name="weaponConfig">武器配置</param>
    /// <param name="sourceContext">状态效果源上下文</param>
    /// <returns></returns>
    public static bool ApplySingleTargetDamage(
        Enemy target,
        float damage,
        WeaponConfig weaponConfig,
        StatusEffectSourceContext sourceContext)
    {
        if (target == null || !target.IsAlive)
        {
            return false;
        }

        bool appliedHit = damage <= 0f || CombatSystem.ApplyDamage(target.Health, damage);
        if (appliedHit && target.IsAlive)
        {
            ApplyWeaponStatusEffects(target, weaponConfig, sourceContext);
        }

        return appliedHit;
    }

    /// <summary>
    /// 应用范围伤害
    /// </summary>
    /// <param name="origin">中心点</param>
    /// <param name="radius">伤害半径</param>
    /// <param name="damage">伤害值</param>
    /// <param name="areaTargets">范围内的敌人列表</param>
    /// <returns></returns>
    public static bool ApplyAreaDamage(Vector3 origin, float radius, float damage, List<Enemy> areaTargets)
    {
        return ApplyAreaDamage(origin, radius, damage, areaTargets, null);
    }

    /// <summary>
    /// 应用范围伤害
    /// </summary>
    /// <param name="origin">中心点</param>
    /// <param name="radius">伤害半径</param>
    /// <param name="damage">伤害值</param>
    /// <param name="areaTargets">范围内的敌人列表</param>
    /// <param name="weaponConfig">武器配置</param>
    /// <returns></returns>
    public static bool ApplyAreaDamage(
        Vector3 origin,
        float radius,
        float damage,
        List<Enemy> areaTargets,
        WeaponConfig weaponConfig)
    {
        return ApplyAreaDamage(
            origin,
            radius,
            damage,
            areaTargets,
            weaponConfig,
            StatusEffectSourceContext.None);
    }

    /// <summary>
    /// 应用范围伤害
    /// </summary>
    /// <param name="origin">中心点</param>
    /// <param name="radius">伤害半径</param>
    /// <param name="damage">伤害值</param>
    /// <param name="areaTargets">范围内的敌人列表</param>
    /// <param name="weaponConfig">武器配置</param>
    /// <param name="sourceContext">状态效果源上下文</param>
    /// <returns></returns>
    public static bool ApplyAreaDamage(
        Vector3 origin,
        float radius,
        float damage,
        List<Enemy> areaTargets,
        WeaponConfig weaponConfig,
        StatusEffectSourceContext sourceContext)
    {
        if (areaTargets == null)
        {
            return false;
        }

        TargetingSystem.CollectAliveEnemiesInRange(origin, radius, areaTargets);
        bool appliedDamage = false;
        for (int i = 0; i < areaTargets.Count; i++)
        {
            appliedDamage |= ApplySingleTargetDamage(areaTargets[i], damage, weaponConfig, sourceContext);
        }

        return appliedDamage;
    }

    /// <summary>
    /// 应用武器的状态效果
    /// </summary>
    /// <param name="target">目标敌人</param>
    /// <param name="weaponConfig">武器配置</param>
    /// <returns></returns>
    public static bool ApplyWeaponStatusEffects(Enemy target, WeaponConfig weaponConfig)
    {
        return ApplyWeaponStatusEffects(target, weaponConfig, StatusEffectSourceContext.None);
    }

    /// <summary>
    /// 应用武器的状态效果
    /// </summary>
    /// <param name="target">目标敌人</param>
    /// <param name="weaponConfig">武器配置</param>
    /// <param name="sourceContext">状态效果源上下文</param>
    /// <returns></returns>
    public static bool ApplyWeaponStatusEffects(
        Enemy target,
        WeaponConfig weaponConfig,
        StatusEffectSourceContext sourceContext)
    {
        if (target == null || target.StatusEffectManager == null || weaponConfig == null)
        {
            return false;
        }

        IReadOnlyList<StatusEffectConfig> statusEffects = weaponConfig.OnHitStatusEffects;
        if (statusEffects == null || statusEffects.Count == 0)
        {
            return false;
        }

        if (Random.value > weaponConfig.OnHitStatusApplyChance)
        {
            return false;
        }

        StatusEffectSourceContext resolvedSource = ResolveWeaponSource(sourceContext, weaponConfig);
        bool appliedAny = false;
        for (int i = 0; i < statusEffects.Count; i++)
        {
            StatusEffectConfig statusEffect = statusEffects[i];
            if (statusEffect == null)
            {
                continue;
            }

            target.StatusEffectManager.ApplyEffect(statusEffect, resolvedSource);
            appliedAny = true;
        }

        return appliedAny;
    }

    /// <summary>
    /// 解析状态效果源上下文 确保使用正确的武器配置
    /// </summary>
    /// <param name="sourceContext">状态效果源上下文</param>
    /// <param name="weaponConfig">武器配置</param>
    /// <returns></returns>
    private static StatusEffectSourceContext ResolveWeaponSource(
        StatusEffectSourceContext sourceContext,
        WeaponConfig weaponConfig)
    {
        if (sourceContext.WeaponConfig == weaponConfig)
        {
            return sourceContext;
        }

        return sourceContext.WithWeaponConfig(weaponConfig);
    }
}
