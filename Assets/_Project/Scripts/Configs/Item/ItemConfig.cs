using UnityEngine;

/// <summary>
/// 物品配置
/// </summary>
[CreateAssetMenu(fileName = "ItemConfig_", menuName = "ScriptableObjects/ItemConfig")]
public class ItemConfig : ScriptableObject
{
    [SerializeField]
    [Tooltip("Stable id used by drop systems and runtime lookup.")]
    private string resourceId;

    [Header("ITEM DETAILS")]
    [SerializeField]
    [Tooltip("The name of the item.")]
    private string itemName;

    [SerializeField]
    [Tooltip("The effect type of the item.")]
    private ItemEffectType effectType;

    [SerializeField]
    [Min(0f)]
    [Tooltip("The amount of the effect applied by the item.")]
    private float effectAmount = 1f;

    [SerializeField]
    [Tooltip("The prefab used for the item pickup.")]
    private GameObject pickupPrefab;

    [SerializeField]
    [Tooltip("The prefab used for the item pickup effect.")]
    private GameObject pickupEffectPrefab;

    public string ResourceId => string.IsNullOrWhiteSpace(resourceId) ? itemName : resourceId;
    public string ItemName => itemName;
    public ItemEffectType EffectType => effectType;
    public float EffectAmount => effectAmount;
    public GameObject PickupPrefab => pickupPrefab;
    public GameObject PickupEffectPrefab => pickupEffectPrefab;
}
