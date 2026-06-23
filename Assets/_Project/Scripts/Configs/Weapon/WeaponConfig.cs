using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "WeaponConfig_", menuName = "ScriptableObjects/WeaponConfig")]
public class WeaponConfig : ScriptableObject
{
    [SerializeField]
    [Tooltip("Stable id used by runtime weapon systems.")]
    private string resourceId;

    [Header("WEAPON BASE DETAILS")]
    [SerializeField]
    [Tooltip("name of weapon")]
    private string weaponName;

    [SerializeField]
    [Min(0f)]
    [Tooltip("atk of weapon")]
    private float damage = 8f;

    [SerializeField]
    [Min(0.01f)]
    [Tooltip("cooldown time of weapon")]
    private float cooldown = 0.75f;

    [SerializeField]
    [Min(0f)]
    [Tooltip("attack range of weapon")]
    private float range = 6f;

    [Header("WEAPON STRATEGIES")]
    [SerializeField]
    [Tooltip("strategy used to execute this weapon attack")]
    private WeaponAttackStrategy attackStrategy;

    [SerializeField]
    [Tooltip("strategy used to apply this weapon damage")]
    private WeaponDamageStrategy damageStrategy;

    [SerializeField]
    [Tooltip("strategy used when this weapon projectile hits an enemy")]
    private ProjectileHitStrategy projectileHitStrategy;

    [SerializeField]
    [Min(0.01f)]
    [Tooltip("radius used by area damage strategies")]
    private float damageRadius = 1f;

    [Header("AOE DETAILS")]
    [SerializeField]
    [Tooltip("aoe config spawned by AOE weapon attack strategies")]
    private AOEConfig aoeConfig;

    [SerializeField]
    [Tooltip("where the AOE zone is spawned when this weapon attacks")]
    private AOESpawnPositionMode aoeSpawnPositionMode = AOESpawnPositionMode.Target;

    [Header("ON HIT STATUS EFFECTS")]
    [SerializeField]
    [Tooltip("status effects applied when this weapon hits an enemy")]
    private StatusEffectConfig[] onHitStatusEffects;

    [SerializeField]
    [Range(0f, 1f)]
    [Tooltip("chance to apply this weapon's on hit status effects")]
    private float onHitStatusApplyChance = 1f;

    [Header("PROJECTILE DETAILS")]
    [SerializeField]
    [Min(0.01f)]
    [Tooltip("projectile speed of weapon")]
    private float projectileSpeed = 8f;

    [SerializeField]
    [Min(0.01f)]
    [Tooltip("distance from target center that counts as a hit")]
    private float projectileHitRadius = 0.2f;

    [SerializeField]
    [Tooltip("the prefab gameobject for the weapon projectile")]
    private GameObject projectilePrefab;

    [SerializeField]
    [Min(0f)]
    [Tooltip("the interval between repeated damage ticks")]
    private float piercingDamageInterval = 0.2f;

    public string ResourceId => string.IsNullOrWhiteSpace(resourceId) ? weaponName : resourceId;
    public string WeaponName => weaponName;
    public float Damage => damage;
    public float Cooldown => cooldown;
    public float Range => range;
    public WeaponAttackStrategy AttackStrategy => attackStrategy;
    public WeaponDamageStrategy DamageStrategy => damageStrategy;
    public ProjectileHitStrategy ProjectileHitStrategy => projectileHitStrategy;
    public float DamageRadius => damageRadius;
    public AOEConfig AOEConfig => aoeConfig;
    public AOESpawnPositionMode AOESpawnPositionMode => aoeSpawnPositionMode;
    public IReadOnlyList<StatusEffectConfig> OnHitStatusEffects => onHitStatusEffects;
    public float OnHitStatusApplyChance => Mathf.Clamp01(onHitStatusApplyChance);
    public float ProjectileSpeed => projectileSpeed;
    public float ProjectileHitRadius => projectileHitRadius;
    public GameObject ProjectilePrefab => projectilePrefab;
    public float PiercingDamageInterval => piercingDamageInterval;
}
