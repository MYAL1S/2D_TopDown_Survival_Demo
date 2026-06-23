using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// AEO配置
/// </summary>
[CreateAssetMenu(fileName = "AOEConfig_", menuName = "ScriptableObjects/AOE/AOEConfig")]
public class AOEConfig : ScriptableObject
{
    [SerializeField]
    [Tooltip("The unique id of resource")]
    private string resourceId;

    [SerializeField]
    [Min(0.01f)]
    [Tooltip("The radius of AOE")]
    private float radius = 2f;

    [SerializeField]
    [Min(0f)]
    [Tooltip("The duration time of AOE")]
    private float duration = 0f;

    [SerializeField]
    [Min(0f)]
    [Tooltip("How long instant AOE visuals stay alive after their gameplay tick")]
    private float visualLifetime = 1f;

    [SerializeField]
    [Min(0.01f)]
    [Tooltip("The interval between each tick of AOE damage")]
    private float tickInterval = 0.5f;

    [SerializeField]
    [Min(0f)]
    [Tooltip("The damage dealt by each tick of AOE damage")]
    private float damagePerTick = 0f;

    [SerializeField]
    [Tooltip("If true, AOE damage will start immediately upon activation")]
    private bool tickOnStart = true;

    [SerializeField]
    [Min(1)]
    [Tooltip("The maximum number of targets that can be affected by AOE damage in a single tick")]
    private int maxTargetsPerTick = 128;

    [SerializeField]
    [Tooltip("The status effects applied by this AOE")]
    private StatusEffectConfig[] statusEffects;

    [SerializeField]
    [Tooltip("The prefab used for the AOE zone")]
    private GameObject zonePrefab;

    public string ResourceId => string.IsNullOrWhiteSpace(resourceId) ? name : resourceId;
    public float Radius => Mathf.Max(0.01f, radius);
    public float Duration => Mathf.Max(0f, duration);
    public float VisualLifetime => Mathf.Max(0f, visualLifetime);
    public float TickInterval => Mathf.Max(0.01f, tickInterval);
    public float DamagePerTick => Mathf.Max(0f, damagePerTick);
    public bool TickOnStart => tickOnStart;
    public int MaxTargetsPerTick => Mathf.Max(1, maxTargetsPerTick);
    public IReadOnlyList<StatusEffectConfig> StatusEffects => statusEffects;
    public GameObject ZonePrefab => zonePrefab;
}
