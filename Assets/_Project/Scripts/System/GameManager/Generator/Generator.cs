using UnityEngine;

/// <summary>
/// 关卡生成系统总入口
/// 缓存敌人生成、掉落生成和玩家生成相关子系统，方便其他管理器统一访问
/// </summary>
public class Generator : MonoBehaviour
{
    private static Generator _instance;
    public static Generator Instance => _instance;

    // 敌人对象池与实例化管理器
    [SerializeField]
    private EnemySpawnManager enemySpawnManager;

    // 敌人刷怪流程控制系统
    [SerializeField]
    private SpawnSystem spawnSystem;

    // 道具掉落生成器
    [SerializeField]
    private ItemDropGenerator itemDropGenerator;

    // 玩家角色选择与生成系统
    [SerializeField]
    private CharacterSelectionSystem characterSelectionSystem;

    public EnemySpawnManager EnemySpawnManager => enemySpawnManager;
    public SpawnSystem SpawnSystem => spawnSystem;
    public ItemDropGenerator ItemDropGenerator => itemDropGenerator;
    public CharacterSelectionSystem CharacterSelectionSystem => characterSelectionSystem;

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }

        _instance = this;
        ResolveChildGenerators();
        DontDestroyOnLoad(gameObject);
    }

    private void OnDestroy()
    {
        if (_instance == this)
        {
            _instance = null;
        }
    }

    private void OnValidate()
    {
        ResolveChildGenerators();
    }

    /// <summary>
    /// 自动从子节点查找生成相关组件，减少预制体手动漏绑导致的空引用
    /// </summary>
    private void ResolveChildGenerators()
    {
        if (enemySpawnManager == null)
        {
            enemySpawnManager = GetComponentInChildren<EnemySpawnManager>(true);
        }

        if (spawnSystem == null)
        {
            spawnSystem = GetComponentInChildren<SpawnSystem>(true);
        }

        if (itemDropGenerator == null)
        {
            itemDropGenerator = GetComponentInChildren<ItemDropGenerator>(true);
        }

        if (characterSelectionSystem == null)
        {
            characterSelectionSystem = GetComponentInChildren<CharacterSelectionSystem>(true);
        }
    }
}
