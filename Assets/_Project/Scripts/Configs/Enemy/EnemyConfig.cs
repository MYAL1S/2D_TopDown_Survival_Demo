using UnityEngine;

[CreateAssetMenu(fileName = "EnemyConfig_", menuName = "ScriptableObjects/EnemyConfig")]
public class EnemyConfig : ScriptableObject
{
    [SerializeField]
    [Tooltip("Stable id used by GameResources and runtime spawners.")]
    private string resourceId;

    [Header("ENEMY BASE DETAILS")]
    [Tooltip("Enemy character name.")]
    public string enemyName;

    [Tooltip("Enemy health")]
    public int health;

    [Tooltip("Enemy speed")]
    public float speed;

    [Tooltip("Enemy atk")]
    public int atk;

    [Tooltip("Enemy defense")]
    public int def; 

    [Tooltip("Enemy hit interval in milliseconds")]
    public int hitIntervalMs;

    [Tooltip("Enemy melee attack range")]
    [Min(0.01f)]
    public float attackRange = 0.75f;

    [Tooltip("The prefab gameobject for the enemy")]
    public GameObject enemyPrefab;

    [Tooltip("Enemy runtime animator controller")]
    public RuntimeAnimatorController animatorController;

    public string ResourceId => string.IsNullOrWhiteSpace(resourceId) ? enemyName : resourceId;
    public float AttackCooldownSeconds => hitIntervalMs > 0 ? hitIntervalMs / 1000f : 1f;
    public float AttackRange => Mathf.Max(0.01f, attackRange);
}
