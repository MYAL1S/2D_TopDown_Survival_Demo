using UnityEngine;

/// <summary>
/// 状态效果来源上下文
/// </summary>
public readonly struct StatusEffectSourceContext
{
    public static StatusEffectSourceContext None => default;

    public StatusEffectSourceContext(
        GameObject sourceObject,
        GameObject applierObject,
        GameObject damageOwnerObject,
        WeaponConfig weaponConfig = null,
        AOEConfig aoeConfig = null,
        float powerMultiplier = 1f)
    {
        SourceObject = sourceObject;
        ApplierObject = applierObject;
        DamageOwnerObject = damageOwnerObject != null ? damageOwnerObject : applierObject;
        WeaponConfig = weaponConfig;
        AOEConfig = aoeConfig;
        this.powerMultiplier = powerMultiplier > 0f ? powerMultiplier : 1f;
    }

    /// <summary>
    /// 状态效果的来源对象
    /// 通常是造成状态效果的实体
    /// </summary>
    public GameObject SourceObject { get; }
    /// <summary>
    /// 状态效果的施加者对象
    /// </summary>
    public GameObject ApplierObject { get; }
    /// <summary>
    /// 状态效果的伤害拥有者对象
    /// </summary>
    public GameObject DamageOwnerObject { get; }
    /// <summary>
    /// 状态效果的武器配置
    /// </summary>
    public WeaponConfig WeaponConfig { get; }
    /// <summary>
    /// 状态效果的AOE配置
    /// </summary>
    public AOEConfig AOEConfig { get; }

    /// <summary>
    /// 状态效果的倍率
    /// </summary>
    private readonly float powerMultiplier;
    /// <summary>
    /// 状态效果的倍率
    /// </summary>
    public float PowerMultiplier => powerMultiplier > 0f ? powerMultiplier : 1f;

    public bool HasSource =>
        SourceObject != null ||
        ApplierObject != null ||
        DamageOwnerObject != null ||
        WeaponConfig != null ||
        AOEConfig != null;

    /// <summary>
    /// 返回一个新的 StatusEffectSourceContext 实例
    /// 其中 SourceObject 被替换为指定的 sourceObject
    /// </summary>
    /// <param name="sourceObject">状态效果的来源对象</param>
    /// <returns></returns>
    public StatusEffectSourceContext WithSourceObject(GameObject sourceObject)
    {
        return new StatusEffectSourceContext(
            sourceObject,
            ApplierObject,
            DamageOwnerObject,
            WeaponConfig,
            AOEConfig,
            PowerMultiplier);
    }

    /// <summary>
    /// 返回一个新的 StatusEffectSourceContext 实例
    /// 其中 WeaponConfig 被替换为指定的 weaponConfig
    /// </summary>
    /// <param name="weaponConfig">武器配置</param>
    /// <returns></returns>
    public StatusEffectSourceContext WithWeaponConfig(WeaponConfig weaponConfig)
    {
        return new StatusEffectSourceContext(
            SourceObject,
            ApplierObject,
            DamageOwnerObject,
            weaponConfig,
            AOEConfig,
            PowerMultiplier);
    }

    /// <summary>
    /// 返回一个新的 StatusEffectSourceContext 实例
    /// </summary>
    /// <param name="aoeConfig">AOE配置</param>
    /// <returns></returns>
    public StatusEffectSourceContext WithAOEConfig(AOEConfig aoeConfig)
    {
        return new StatusEffectSourceContext(
            SourceObject,
            ApplierObject,
            DamageOwnerObject,
            WeaponConfig,
            aoeConfig,
            PowerMultiplier);
    }
}
