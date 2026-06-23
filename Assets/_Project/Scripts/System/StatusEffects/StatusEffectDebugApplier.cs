using UnityEngine;

[DisallowMultipleComponent]
public class StatusEffectDebugApplier : MonoBehaviour
{
    [Header("Player Effects")]
    [SerializeField]
    private StatusEffectConfig playerEffect;

    [SerializeField]
    private KeyCode applyPlayerEffectKey = KeyCode.H;

    [Header("Enemy Effects")]
    [SerializeField]
    private StatusEffectConfig enemyEffect;

    [SerializeField]
    private KeyCode applyNearestEnemyEffectKey = KeyCode.P;

    [SerializeField]
    [Min(0f)]
    private float enemySearchRange = 0f;

    [Header("Debug")]
    [SerializeField]
    private bool logResult = true;

    private void Update()
    {
        if (Input.GetKeyDown(applyPlayerEffectKey))
        {
            ApplyToPlayer(playerEffect);
        }

        if (Input.GetKeyDown(applyNearestEnemyEffectKey))
        {
            ApplyToNearestEnemy(enemyEffect);
        }
    }

    private void ApplyToPlayer(StatusEffectConfig config)
    {
        if (config == null)
        {
            Log("Could not apply player status effect. Missing config.");
            return;
        }

        Player player = GetActivePlayer();
        if (player == null || player.StatusEffectManager == null)
        {
            Log("Could not apply player status effect. Missing active player or StatusEffectManager.");
            return;
        }

        player.StatusEffectManager.ApplyEffect(config);
        Log($"Applied status effect '{config.EffectId}' to player '{player.name}'.");
    }

    private void ApplyToNearestEnemy(StatusEffectConfig config)
    {
        if (config == null)
        {
            Log("Could not apply enemy status effect. Missing config.");
            return;
        }

        Player player = GetActivePlayer();
        if (player == null)
        {
            Log("Could not apply enemy status effect. Missing active player.");
            return;
        }

        Enemy enemy = TargetingSystem.FindNearestAliveEnemy(player.transform.position, enemySearchRange);
        if (enemy == null || enemy.StatusEffectManager == null)
        {
            Log("Could not apply enemy status effect. Missing nearest enemy or StatusEffectManager.");
            return;
        }

        enemy.StatusEffectManager.ApplyEffect(config);
        Log($"Applied status effect '{config.EffectId}' to enemy '{enemy.name}'.");
    }

    private static Player GetActivePlayer()
    {
        if (Generator.Instance != null &&
            Generator.Instance.CharacterSelectionSystem != null &&
            Generator.Instance.CharacterSelectionSystem.ActivePlayer != null)
        {
            return Generator.Instance.CharacterSelectionSystem.ActivePlayer;
        }

        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        return playerObject != null ? playerObject.GetComponent<Player>() : null;
    }

    private void Log(string message)
    {
        if (logResult)
        {
            Debug.Log(message, this);
        }
    }
}
