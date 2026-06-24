using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 敌人刷怪系统
/// 根据 SpawnConfig 中的曲线随关卡时间动态生成敌人，并维护当前活动敌人列表
/// </summary>
[RequireComponent(typeof(EnemySpawnManager))]
[DisallowMultipleComponent]
public class SpawnSystem : MonoBehaviour
{
    /// <summary>
    /// 刷怪系统状态
    /// </summary>
    private enum SpawnState
    {
        // 停止刷怪，时间和预算会被重置
        Stopped,
        // 正常刷怪，按时间累计刷怪预算
        Running,
        // 暂停刷怪，保留当前时间与预算
        Paused
    }

    // 当前使用的刷怪配置
    [SerializeField]
    private SpawnConfig spawnConfig;

    // 是否在 Start 时自动开始刷怪
    [SerializeField]
    private bool startOnPlay = true;

    // 当前活动敌人字典，键为敌人的 InstanceID
    private readonly Dictionary<int, Enemy> activeEnemies = new Dictionary<int, Enemy>();
    // 已死亡敌人 id 列表，统一在 Update 中从活动字典移除
    private readonly List<int> deadEnemyIds = new List<int>();

    // 敌人生成管理器，负责实例化和对象池复用
    private EnemySpawnManager enemySpawnManager;

    // 当前刷怪系统已运行时间
    private float elapsedTime;
    // 刷怪预算累计值，允许按小数速率平滑生成敌人
    private float spawnBudget;
    private SpawnState spawnState = SpawnState.Stopped;

    public int ActiveEnemyCount => activeEnemies.Count;
    public bool IsSpawning => spawnState == SpawnState.Running;
    public bool IsPaused => spawnState == SpawnState.Paused;
    public SpawnConfig CurrentSpawnConfig => spawnConfig;

    private void Awake()
    {
        enemySpawnManager = GetComponent<EnemySpawnManager>();
    }

    private void Start()
    {
        if (GameResources.Instance == null)
        {
            Debug.LogError($"{nameof(SpawnSystem)} needs {nameof(GameResources)} in the scene.", this);
            enabled = false;
            return;
        }

        if (enemySpawnManager == null)
        {
            Debug.LogError($"{nameof(SpawnSystem)} needs {nameof(EnemySpawnManager)} in the scene.", this);
            enabled = false;
            return;
        }

        if (startOnPlay)
        {
            StartSpawning();
        }
    }

    /// <summary>
    /// 开始刷怪
    /// </summary>
    public void StartSpawning()
    {
        if (!CanSpawn())
        {
            return;
        }

        spawnState = SpawnState.Running;
    }

    /// <summary>
    /// 切换当前关卡使用的刷怪配置
    /// </summary>
    /// <param name="config">新的刷怪配置</param>
    /// <param name="startImmediately">设置完成后是否立即开始刷怪</param>
    public void SetSpawnConfig(SpawnConfig config, bool startImmediately)
    {
        StopSpawning();
        ClearActiveEnemies();
        spawnConfig = config;

        if (startImmediately)
        {
            StartSpawning();
        }
    }

    /// <summary>
    /// 暂停刷怪，但保留已经累计的时间和预算
    /// </summary>
    public void PauseSpawning()
    {
        if (spawnState == SpawnState.Running)
        {
            spawnState = SpawnState.Paused;
        }
    }

    /// <summary>
    /// 停止刷怪并重置时间与预算
    /// </summary>
    public void StopSpawning()
    {
        spawnState = SpawnState.Stopped;
        elapsedTime = 0f;
        spawnBudget = 0f;
    }

    private void Update()
    {
        CleanupDeadEnemies();

        if (spawnState != SpawnState.Running)
        {
            return;
        }

        Camera mainCamera = Camera.main;
        Transform playerTarget = FindPlayerTarget();

        if (mainCamera == null || playerTarget == null)
        {
            return;
        }

        elapsedTime += Time.deltaTime;

        int maxAlive = spawnConfig.GetMaxAlive(elapsedTime);
        if (activeEnemies.Count >= maxAlive)
        {
            return;
        }

        spawnBudget += spawnConfig.GetSpawnRate(elapsedTime) * Time.deltaTime;

        int spawnCapacity = maxAlive - activeEnemies.Count;
        int spawnCount = Mathf.Min(
            Mathf.FloorToInt(spawnBudget),
            spawnConfig.GetBatchCount(elapsedTime),
            spawnConfig.MaxSpawnPerFrame,
            spawnCapacity);

        if (spawnCount <= 0)
        {
            return;
        }

        for (int i = 0; i < spawnCount; i++)
        {
            SpawnEnemy(mainCamera, playerTarget);
        }

        spawnBudget -= spawnCount;
    }

    /// <summary>
    /// 查找当前玩家位置，作为敌人追踪目标
    /// </summary>
    /// <returns>玩家 Transform，找不到时返回 null</returns>
    private Transform FindPlayerTarget()
    {
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        return playerObject != null ? playerObject.transform : null;
    }

    /// <summary>
    /// 从资源配置中随机选择敌人，并在屏幕外生成
    /// </summary>
    private void SpawnEnemy(Camera mainCamera, Transform playerTarget)
    {
        EnemyConfig selectedConfig = GameResources.Instance.GetRandomEnemyConfig(spawnConfig.GetEnemyResourceIds(elapsedTime));

        if (selectedConfig == null)
        {
            Debug.LogWarning($"{nameof(SpawnSystem)} could not find an enemy config in {nameof(GameResources)}.", this);
            return;
        }

        Vector3 spawnPosition = GetRandomOffScreenPosition(mainCamera);
        Enemy enemy = enemySpawnManager.SpawnEnemy(
            selectedConfig,
            spawnPosition,
            Quaternion.identity,
            transform,
            playerTarget);

        if (enemy == null)
        {
            return;
        }

        activeEnemies[enemy.GetInstanceID()] = enemy;
    }

    /// <summary>
    /// 根据相机视野计算一个屏幕外随机出生点
    /// </summary>
    /// <param name="mainCamera">主相机</param>
    /// <returns>屏幕外出生位置</returns>
    private Vector3 GetRandomOffScreenPosition(Camera mainCamera)
    {
        float halfHeight = mainCamera.orthographicSize;
        float halfWidth = halfHeight * mainCamera.aspect;
        Vector3 cameraPosition = mainCamera.transform.position;

        float minX = cameraPosition.x - halfWidth;
        float maxX = cameraPosition.x + halfWidth;
        float minY = cameraPosition.y - halfHeight;
        float maxY = cameraPosition.y + halfHeight;
        float margin = spawnConfig.SpawnMargin;

        switch (Random.Range(0, 4))
        {
            case 0:
                return new Vector3(Random.Range(minX - margin, maxX + margin), maxY + margin, spawnConfig.SpawnZ);

            case 1:
                return new Vector3(Random.Range(minX - margin, maxX + margin), minY - margin, spawnConfig.SpawnZ);

            case 2:
                return new Vector3(minX - margin, Random.Range(minY - margin, maxY + margin), spawnConfig.SpawnZ);

            default:
                return new Vector3(maxX + margin, Random.Range(minY - margin, maxY + margin), spawnConfig.SpawnZ);
        }
    }

    /// <summary>
    /// 记录已死亡敌人，稍后统一从活动敌人字典中移除
    /// </summary>
    /// <param name="instanceID">敌人实例 id</param>
    public void AddToDeadEnemies(int instanceID)
    {
        if (!deadEnemyIds.Contains(instanceID))
        {
            deadEnemyIds.Add(instanceID);
        }
    }

    /// <summary>
    /// 批量清理已死亡敌人，避免遍历活动敌人字典时修改字典结构
    /// </summary>
    private void CleanupDeadEnemies()
    {
        for (int i = 0; i < deadEnemyIds.Count; i++)
        {
            activeEnemies.Remove(deadEnemyIds[i]);
        }

        deadEnemyIds.Clear();
    }

    /// <summary>
    /// 清理当前活动敌人
    /// 切换关卡配置或退出关卡时调用
    /// </summary>
    public void ClearActiveEnemies()
    {
        foreach (Enemy enemy in activeEnemies.Values)
        {
            if (enemy != null)
            {
                Destroy(enemy.gameObject);
            }
        }

        activeEnemies.Clear();
        deadEnemyIds.Clear();
    }

    /// <summary>
    /// 检查当前是否满足开始刷怪的条件
    /// </summary>
    /// <returns>可以开始刷怪返回 true</returns>
    private bool CanSpawn()
    {
        if (spawnConfig == null)
        {
            Debug.LogError($"{nameof(SpawnSystem)} needs a {nameof(SpawnConfig)} reference.", this);
            return false;
        }

        if (GameResources.Instance == null)
        {
            Debug.LogError($"{nameof(SpawnSystem)} needs {nameof(GameResources)} in the scene.", this);
            return false;
        }

        if (enemySpawnManager == null)
        {
            Debug.LogError($"{nameof(SpawnSystem)} needs {nameof(EnemySpawnManager)} in the scene.", this);
            return false;
        }

        return true;
    }
}
