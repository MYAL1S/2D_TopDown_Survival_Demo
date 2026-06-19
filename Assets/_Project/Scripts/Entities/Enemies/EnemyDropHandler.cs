using UnityEngine;

#region REQUIRE COMPONENTS
[RequireComponent(typeof(DeathEvent))]
[RequireComponent(typeof(EnemyController))]
#endregion
[DisallowMultipleComponent]
public class EnemyDropHandler : MonoBehaviour
{
    [SerializeField]
    private ItemDropGenerator itemDropGenerator;

    private DeathEvent deathEvent;
    private EnemyController enemyController;

    private void Awake()
    {
        deathEvent = GetComponent<DeathEvent>();
        enemyController = GetComponent<EnemyController>();
    }

    private void OnEnable()
    {
        deathEvent.OnDeath += DeathEvent_OnDeath;
    }

    private void OnDisable()
    {
        deathEvent.OnDeath -= DeathEvent_OnDeath;
    }

    private void DeathEvent_OnDeath(DeathEvent eventSource)
    {
        if (enemyController == null)
        {
            enemyController = GetComponent<EnemyController>();
        }

        ItemDropGenerator dropGenerator = ResolveItemDropGenerator();
        if (dropGenerator == null || enemyController == null)
        {
            return;
        }

        dropGenerator.GenerateDrop(transform.position, enemyController.Config);
    }

    private ItemDropGenerator ResolveItemDropGenerator()
    {
        if (itemDropGenerator != null)
        {
            return itemDropGenerator;
        }

        Generator generator = Generator.Instance;
        if (generator != null)
        {
            itemDropGenerator = generator.ItemDropGenerator;
        }

        return itemDropGenerator;
    }
}
