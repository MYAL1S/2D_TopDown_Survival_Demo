using System.Collections;
using UnityEngine;

[RequireComponent(typeof(DeathEvent))]
[DisallowMultipleComponent]
public class EnemyDeathHandler : MonoBehaviour
{
    [SerializeField]
    private GameObject deathEffectPrefab;

    [SerializeField]
    [Min(0f)]
    private float destroyDelay = 0.15f;

    private DeathEvent deathEvent;
    private Rigidbody2D rigidBody2D;
    private Collider2D[] colliders;
    private EnemyController enemyController;
    private bool isHandlingDeath;

    private void Awake()
    {
        deathEvent = GetComponent<DeathEvent>();
        rigidBody2D = GetComponent<Rigidbody2D>();
        enemyController = GetComponent<EnemyController>();
        colliders = GetComponentsInChildren<Collider2D>(true);
    }

    private void OnEnable()
    {
        isHandlingDeath = false;
        SetCollidersEnabled(true);
        deathEvent.OnDeath += DeathEvent_OnDeath;
    }

    private void OnDisable()
    {
        deathEvent.OnDeath -= DeathEvent_OnDeath;
    }

    private void DeathEvent_OnDeath(DeathEvent eventSource)
    {
        if (isHandlingDeath)
        {
            return;
        }

        isHandlingDeath = true;
        StartCoroutine(DeathRoutine());
        Generator generator = Generator.Instance;
        if (generator != null && generator.SpawnSystem != null && enemyController != null)
            generator.SpawnSystem.AddToDeadEnemies(enemyController.GetInstanceID());
    }

    private IEnumerator DeathRoutine()
    {
        if (rigidBody2D != null)
        {
            rigidBody2D.velocity = Vector2.zero;
        }

        SetCollidersEnabled(false);

        if (deathEffectPrefab != null)
        {
            Instantiate(deathEffectPrefab, transform.position, Quaternion.identity);
        }

        if (destroyDelay > 0f)
        {
            yield return new WaitForSeconds(destroyDelay);
        }

        ReturnToPool();
    }

    private void SetCollidersEnabled(bool isEnabled)
    {
        for (int i = 0; i < colliders.Length; i++)
        {
            if (colliders[i] != null)
            {
                colliders[i].enabled = isEnabled;
            }
        }
    }

    private void ReturnToPool()
    {
        if (enemyController == null)
        {
            enemyController = GetComponent<EnemyController>();
        }

        Generator generator = Generator.Instance;
        if (generator != null && generator.EnemySpawnManager != null && enemyController != null)
        {
            generator.EnemySpawnManager.ReleaseEnemy(enemyController);
            return;
        }

        gameObject.SetActive(false);
    }
}
