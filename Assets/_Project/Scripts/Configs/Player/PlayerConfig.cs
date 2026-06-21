using UnityEngine;

[CreateAssetMenu(fileName = "PlayerConfig_", menuName = "ScriptableObjects/PlayerConfig")]
public class PlayerConfig : ScriptableObject
{
    [SerializeField]
    [Tooltip("Stable id used by GameResources and runtime spawners.")]
    private string resourceId;

    [Header("PLAYER BASE DETAILS")]
    [Tooltip("Player character name.")]
    public string playerCharacterName;

    [Tooltip("Prefab gameobject for the player")]
    public GameObject playerPrefab;

    [Tooltip("Player runtime animator controller")]
    public RuntimeAnimatorController runtimeAnimatorController;

    [Tooltip("Player base movement speed.")]
    [Min(0f)]
    [SerializeField]
    private float baseSpeed = 8f;

    [SerializeField]
    [Tooltip("Weapon equipped when the character is spawned.")]
    private WeaponConfig startingWeaponConfig;

    [Header("PLAYER COMBAT DETAILS")]
    [SerializeField]
    [Min(1f)]
    private float maxHealth = 100f;

    [SerializeField]
    [Min(0f)]
    private float defense = 0f;

    [SerializeField]
    [Min(0f)]
    private float injuredInvincibilityDuration = 0.75f;

    public string ResourceId => string.IsNullOrWhiteSpace(resourceId) ? playerCharacterName : resourceId;
    public float MoveSpeed => baseSpeed;
    public WeaponConfig StartingWeaponConfig => startingWeaponConfig;
    public float MaxHealth => maxHealth;
    public float Defense => defense;
    public float InjuredInvincibilityDuration => injuredInvincibilityDuration;
}
