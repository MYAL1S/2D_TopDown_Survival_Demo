using UnityEngine;

[DisallowMultipleComponent]
public class NearestEnemyDamageTest : MonoBehaviour
{
    [SerializeField]
    private KeyCode damageKey = KeyCode.J;

    [SerializeField]
    [Min(0f)]
    private float damageAmount = 10f;

    [SerializeField]
    private bool logResult = true;

    private void Update()
    {
        if (Input.GetKeyDown(damageKey))
        {
            DamageNearestEnemy();
        }
    }

    private void DamageNearestEnemy()
    {
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject == null)
        {
            Log("Could not damage nearest enemy. No object with Player tag was found.");
            return;
        }

        EnemyController nearestEnemy = FindNearestAliveEnemy(playerObject.transform.position);
        if (nearestEnemy == null)
        {
            Log("Could not damage nearest enemy. No alive enemy was found.");
            return;
        }

        if (nearestEnemy.Health == null)
        {
            Log($"Could not damage {nearestEnemy.name}. Missing Health component.");
            return;
        }

        nearestEnemy.Health.TakeDamage(damageAmount);
        Log($"Damaged nearest enemy '{nearestEnemy.name}' by {damageAmount}. Current health: {nearestEnemy.Health.CurrentHealth}.");
    }

    private EnemyController FindNearestAliveEnemy(Vector3 playerPosition)
    {
        EnemyController[] enemies = FindObjectsOfType<EnemyController>();
        EnemyController nearestEnemy = null;
        float nearestSqrDistance = float.MaxValue;

        for (int i = 0; i < enemies.Length; i++)
        {
            EnemyController enemy = enemies[i];
            if (enemy == null || !enemy.IsAlive)
            {
                continue;
            }

            float sqrDistance = (enemy.transform.position - playerPosition).sqrMagnitude;
            if (sqrDistance < nearestSqrDistance)
            {
                nearestSqrDistance = sqrDistance;
                nearestEnemy = enemy;
            }
        }

        return nearestEnemy;
    }

    private void Log(string message)
    {
        if (logResult)
        {
            Debug.Log(message, this);
        }
    }
}
