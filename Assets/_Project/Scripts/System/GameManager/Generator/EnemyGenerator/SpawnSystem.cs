using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 敌人刷怪系统
/// 使用动画曲线控制随游戏进度动态变化的刷怪压力 维护活跃敌人列表
/// 支持预算系统 平滑分配每帧的生成数量 并支持暂停/停止控制
/// </summary>
[RequireComponent(typeof(EnemySpawnManager))]
[DisallowMultipleComponent]
public class SpawnSystem : MonoBehaviour
{
    /// <summary>
    /// 刷怪状态
    /// </summary>
    private enum SpawnState
    {
        // 停止刷怪
        // 重置时间与预算 清理活跃敌人列表
        Stopped,
        // 正常刷怪
        // 累积时间与预算 按曲线计算生成数量 执行生成
        Running,
        // 暂停刷怪
        // 保持状态与预算 不重置已累积时间
        Paused
    }

    // 生成配置 ScriptableObject
    [SerializeField]
    private SpawnConfig spawnConfig;

    [SerializeField]
    private bool startOnPlay = true;

    // 活跃敌人字典
    // 键为 InstanceID
    private readonly Dictionary<int, EnemyController> activeEnemies = new Dictionary<int, EnemyController>();
    // 待清理的死亡敌人 ID 列表
    // 用于避免在迭代字典时修改其结构
    private readonly List<int> deadEnemyIds = new List<int>();

    // 敌人生成管理器
    // 负责实例化与对象池管理
    private EnemySpawnManager enemySpawnManager;

    // 游戏已运行的总时间
    // 用于从曲线中查询动态参数
    private float elapsedTime;
    // 生成预算累积值
    // 表示当前可生成的敌人份数 可能为小数
    private float spawnBudget;
    private SpawnState spawnState = SpawnState.Stopped;

    public int ActiveEnemyCount => activeEnemies.Count;
    public bool IsSpawning => spawnState == SpawnState.Running;
    public bool IsPaused => spawnState == SpawnState.Paused;

    /// <summary>
    /// 缓存敌人生成管理器引用
    /// </summary>
    private void Awake()
    {
        enemySpawnManager = GetComponent<EnemySpawnManager>();
    }

    /// <summary>
    /// 初始化资源引用并根据配置决定是否自动开始刷
    /// </summary>
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
    /// 暂停当前刷怪
    /// 保持状态与预算 不重置已累积时间
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

    /// <summary>
    /// 每帧更新刷怪逻辑
    /// 清理死亡敌人、累积时间、更新生成预算、计算生成数量并执行生成
    /// </summary>
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

        // 获取当前允许的最大活跃敌人数（随时间推移增加）
        int maxAlive = spawnConfig.GetMaxAlive(elapsedTime);
        if (activeEnemies.Count >= maxAlive)
        {
            return;
        }

        // 根据当前刷怪速率累积生成预算（每秒的生成份数）
        spawnBudget += spawnConfig.GetSpawnRate(elapsedTime) * Time.deltaTime;

        // 计算当前剩余的生成容量并综合约束得到本帧生成数量
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

        // 本帧内按计算数量逐个生成敌人
        for (int i = 0; i < spawnCount; i++)
        {
            SpawnEnemy(mainCamera, playerTarget);
        }

        // 扣除已使用的预算
        spawnBudget -= spawnCount;
    }

    /// <summary>
    /// 查找并返回标记为 "Player" 的玩家对象的 Transform（作为敌人追踪目标）
    /// </summary>
    private Transform FindPlayerTarget()
    {
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        return playerObject != null ? playerObject.transform : null;
    }

    /// <summary>
    /// 从资源池中随机选择敌人配置
    /// 计算屏幕外生成点
    /// 并请求生成管理器创建敌人
    /// 生成成功后将敌人加入活跃字典以便管理
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
        EnemyController enemyController = enemySpawnManager.SpawnEnemy(
            selectedConfig,
            spawnPosition,
            Quaternion.identity,
            transform,
            playerTarget);

        if (enemyController == null)
        {
            return;
        }

        // 将新生成的敌人加入活跃字典
        activeEnemies[enemyController.GetInstanceID()] = enemyController;
    }

    /// <summary>
    /// 根据相机视野计算一个在屏幕外的随机位置
    /// 用作敌人出生点（上、下、左、右四个方向随机）
    /// </summary>
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
            // 上方
            case 0:
                return new Vector3(Random.Range(minX - margin, maxX + margin), maxY + margin, spawnConfig.SpawnZ);

            // 下方
            case 1:
                return new Vector3(Random.Range(minX - margin, maxX + margin), minY - margin, spawnConfig.SpawnZ);

            // 左方
            case 2:
                return new Vector3(minX - margin, Random.Range(minY - margin, maxY + margin), spawnConfig.SpawnZ);

            // 右方
            default:
                return new Vector3(maxX + margin, Random.Range(minY - margin, maxY + margin), spawnConfig.SpawnZ);
        }
    }

    /// <summary>
    /// 将敌人实例 ID 添加到待删除列表中 由 Update 循环统一清理
    /// </summary>
    /// <param name="instanceID"></param>
    public void AddToDeadEnemies(int instanceID)
    {
        if (!deadEnemyIds.Contains(instanceID))
        {
            deadEnemyIds.Add(instanceID);
        }
    }

    /// <summary>
    /// 清理已死亡或销毁的敌人
    /// 先记录 ID 再批量清理 以避免在迭代时修改字典
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
    /// 检查是否具备开始刷怪的条件
    /// 配置、资源和管理器都存在
    /// </summary>
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
