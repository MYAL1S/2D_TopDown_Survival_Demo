using UnityEngine;

#region REQUIRE COMPONENTS
[RequireComponent(typeof(DeathEvent))]
#endregion
[DisallowMultipleComponent]
public class EnemyDropHandler : MonoBehaviour
{
    [SerializeField]
    private ItemDropGenerator itemDropGenerator;

    private DeathEvent deathEvent;
    private Enemy enemy;

    private void Awake()
    {
        deathEvent = GetComponent<DeathEvent>();
        enemy = GetComponent<Enemy>();
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
        if (enemy == null)
        {
            enemy = GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.CacheComponents();
            }
        }

        ItemDropGenerator dropGenerator = ResolveItemDropGenerator();
        if (dropGenerator == null || enemy == null)
        {
            return;
        }

        dropGenerator.GenerateDrop(transform.position, enemy.Config);
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
