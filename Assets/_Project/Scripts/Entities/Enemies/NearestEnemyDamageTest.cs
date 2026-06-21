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

        Enemy nearestEnemy = TargetingSystem.FindNearestAliveEnemy(playerObject.transform.position, 0f);
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

    private void Log(string message)
    {
        if (logResult)
        {
            Debug.Log(message, this);
        }
    }
}
