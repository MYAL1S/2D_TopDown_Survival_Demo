using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "StatusEffectConfig_", menuName = "ScriptableObjects/StatusEffects/StatusEffectConfig")]
public class StatusEffectConfig : ScriptableObject
{
    [SerializeField]
    [Tooltip("The unique identifier for this status effect.")]
    private string effectId;

    [SerializeField]
    [Tooltip("The type of this status effect.")]
    private StatusEffectType effectType;

    [SerializeField]
    [Tooltip("The target type of this status effect.")]
    private StatusEffectTargetType targetType = StatusEffectTargetType.Any;

    [SerializeField]
    [Tooltip("The stacking behavior of this status effect.")]
    private StatusEffectStackingBehavior stackingBehavior = StatusEffectStackingBehavior.Refresh;

    [SerializeField]
    [Min(0.01f)]
    [Tooltip("The duration of this status effect.")]
    private float duration = 3f;

    [SerializeField]
    [Min(1)]
    [Tooltip("The maximum number of stacks for this status effect.")]
    private int maxStacks = 1;

    [SerializeField]
    [Tooltip("Composable behavior modifiers executed by this status effect.")]
    private StatusEffectModifier[] modifiers;

    public string EffectId => string.IsNullOrWhiteSpace(effectId) ? name : effectId;
    public StatusEffectType EffectType => effectType;
    public StatusEffectTargetType TargetType => targetType;
    public StatusEffectStackingBehavior StackingBehavior => stackingBehavior;
    public float Duration => duration;
    public int MaxStacks => Mathf.Max(1, maxStacks);
    public IReadOnlyList<StatusEffectModifier> Modifiers => modifiers;

    public bool CanApplyTo(GameObject target)
    {
        if (target == null)
        {
            return false;
        }

        switch (targetType)
        {
            case StatusEffectTargetType.PlayerOnly:
                return target.GetComponent<Player>() != null;
            case StatusEffectTargetType.EnemyOnly:
                return target.GetComponent<Enemy>() != null;
            default:
                return true;
        }
    }
}
