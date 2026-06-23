using UnityEngine;

[DisallowMultipleComponent]
public class AOEDebugSpawner : MonoBehaviour
{
    [SerializeField]
    private AOEConfig aoeConfig;

    [SerializeField]
    private KeyCode spawnKey = KeyCode.O;

    [SerializeField]
    private bool spawnAtPlayer = true;

    [SerializeField]
    private bool logResult = true;

    private void Update()
    {
        if (Input.GetKeyDown(spawnKey))
        {
            SpawnAOE();
        }
    }

    private void SpawnAOE()
    {
        if (aoeConfig == null)
        {
            Log("Could not spawn AOE. Missing AOEConfig.");
            return;
        }

        Player player = GetActivePlayer();
        Vector3 position = spawnAtPlayer && player != null ? player.transform.position : transform.position;
        GameObject owner = player != null ? player.gameObject : gameObject;
        AOESystem.SpawnAOE(aoeConfig, position, owner);
        Log($"Spawned AOE '{aoeConfig.ResourceId}' at {position}.");
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
