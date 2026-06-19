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

    

    public string ResourceId => string.IsNullOrWhiteSpace(resourceId) ? playerCharacterName : resourceId;
    public float MoveSpeed => baseSpeed;
}
