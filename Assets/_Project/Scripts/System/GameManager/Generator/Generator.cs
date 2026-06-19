using UnityEngine;

public class Generator : MonoBehaviour
{
    private static Generator _instance;
    public static Generator Instance => _instance;

    [SerializeField]
    private EnemySpawnManager enemySpawnManager;

    [SerializeField]
    private SpawnSystem spawnSystem;

    [SerializeField]
    private ItemDropGenerator itemDropGenerator;

    [SerializeField]
    private CharacterSelectionSystem characterSelectionSystem;

    public EnemySpawnManager EnemySpawnManager => enemySpawnManager;
    public SpawnSystem SpawnSystem => spawnSystem;
    public ItemDropGenerator ItemDropGenerator => itemDropGenerator;
    public CharacterSelectionSystem CharacterSelectionSystem => characterSelectionSystem;

    private void Awake()
    {
        if(_instance == null)
            _instance = this;
        ResolveChildGenerators();
        DontDestroyOnLoad(gameObject);
    }


    private void OnValidate()
    {
        ResolveChildGenerators();
    }

    private void ResolveChildGenerators()
    {
        if (enemySpawnManager == null)
            enemySpawnManager = GetComponentInChildren<EnemySpawnManager>(true);

        if (spawnSystem == null)
            spawnSystem = GetComponentInChildren<SpawnSystem>(true);

        if (itemDropGenerator == null)
            itemDropGenerator = GetComponentInChildren<ItemDropGenerator>(true);

        if (characterSelectionSystem == null)
            characterSelectionSystem = GetComponentInChildren<CharacterSelectionSystem>(true);
    }
}
