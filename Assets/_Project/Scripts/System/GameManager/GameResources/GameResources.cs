using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 游戏资源管理器
/// 负责维护玩家与敌人配置
/// 并提供按资源 ID 的查询能力
/// </summary>
[DisallowMultipleComponent]
public class GameResources : MonoBehaviour
{
    private static GameResources _instance;
    public static GameResources Instance => _instance;

    // 玩家信息配置列表
    [Header("Player Resources")]
    [SerializeField]
    private PlayerConfig[] playerConfigs;

    // 敌人信息配置列表
    [Header("Enemy Resources")]
    [SerializeField]
    private EnemyConfig[] enemyConfigs;

    // 运行时缓存
    // 用 ResourceId 作为键 配置对象作为值 以便快速查找
    private readonly Dictionary<string, PlayerConfig> playerConfigLookup = new Dictionary<string, PlayerConfig>();
    private readonly Dictionary<string, EnemyConfig> enemyConfigLookup = new Dictionary<string, EnemyConfig>();

    public IReadOnlyList<PlayerConfig> PlayerConfigs => playerConfigs;
    public IReadOnlyList<EnemyConfig> EnemyConfigs => enemyConfigs;

    /// <summary>
    /// 初始化单例并构建查找缓存。
    /// </summary>
    protected virtual void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }

        _instance = this;
        DontDestroyOnLoad(gameObject);
        BuildLookupCaches();
    }

    /// <summary>
    /// 获取第一个可用的玩家配置
    /// 作为默认配置
    /// </summary>
    public PlayerConfig GetDefaultPlayerConfig()
    {
        return GetFirstValid(playerConfigs);
    }

    /// <summary>
    /// 获取第一个可用的敌人配置
    /// 作为默认配置。
    /// </summary>
    public EnemyConfig GetDefaultEnemyConfig()
    {
        return GetFirstValid(enemyConfigs);
    }

    /// <summary>
    /// 根据资源 ID 获取玩家配置
    /// 如果 ID 为空则返回默认配置
    /// </summary>
    public PlayerConfig GetPlayerConfig(string resourceId)
    {
        if (string.IsNullOrWhiteSpace(resourceId))
        {
            return GetDefaultPlayerConfig();
        }

        return playerConfigLookup.TryGetValue(resourceId, out PlayerConfig config) ? config : null;
    }

    /// <summary>
    /// 根据资源 ID 获取敌人配置
    /// 如果 ID 为空则返回默认配置
    /// </summary>
    public EnemyConfig GetEnemyConfig(string resourceId)
    {
        if (string.IsNullOrWhiteSpace(resourceId))
        {
            return GetDefaultEnemyConfig();
        }

        return enemyConfigLookup.TryGetValue(resourceId, out EnemyConfig config) ? config : null;
    }

    /// <summary>
    /// 在指定候选 ID 中随机获取一个可用的敌人配置
    /// 如果未提供候选，则从全部敌人配置中随机选择
    /// </summary>
    public EnemyConfig GetRandomEnemyConfig(IReadOnlyList<string> resourceIds)
    {
        if (resourceIds == null || resourceIds.Count == 0)
        {
            return GetRandomValid(enemyConfigs);
        }

        List<EnemyConfig> availableConfigs = new List<EnemyConfig>();
        for (int i = 0; i < resourceIds.Count; i++)
        {
            EnemyConfig config = GetEnemyConfig(resourceIds[i]);
            if (config != null)
            {
                availableConfigs.Add(config);
            }
        }

        return GetRandomValid(availableConfigs);
    }

    /// <summary>
    /// 重新构建运行时查找缓存
    /// </summary>
    private void BuildLookupCaches()
    {
        playerConfigLookup.Clear();
        enemyConfigLookup.Clear();

        AddPlayerConfigsToCache(playerConfigs);
        AddEnemyConfigsToCache(enemyConfigs);
    }

    /// <summary>
    /// 将所有玩家配置写入缓存
    /// 便于按 ResourceId 快速查找
    /// </summary>
    private void AddPlayerConfigsToCache(PlayerConfig[] configs)
    {
        if (configs == null)
        {
            return;
        }

        for (int i = 0; i < configs.Length; i++)
        {
            PlayerConfig config = configs[i];
            if (config == null || string.IsNullOrWhiteSpace(config.ResourceId))
            {
                continue;
            }

            playerConfigLookup[config.ResourceId] = config;
        }
    }

    /// <summary>
    /// 将所有敌人配置写入缓存
    /// 便于按 ResourceId 快速查找
    /// </summary>
    private void AddEnemyConfigsToCache(EnemyConfig[] configs)
    {
        if (configs == null)
        {
            return;
        }

        for (int i = 0; i < configs.Length; i++)
        {
            EnemyConfig config = configs[i];
            if (config == null || string.IsNullOrWhiteSpace(config.ResourceId))
            {
                continue;
            }

            enemyConfigLookup[config.ResourceId] = config;
        }
    }

    /// <summary>
    /// 从列表中返回第一个非空配置
    /// </summary>
    private static T GetFirstValid<T>(IReadOnlyList<T> configs) where T : Object
    {
        if (configs == null)
        {
            return null;
        }

        for (int i = 0; i < configs.Count; i++)
        {
            if (configs[i] != null)
            {
                return configs[i];
            }
        }

        return null;
    }

    /// <summary>
    /// 从列表中随机返回一个非空配置
    /// </summary>
    private static T GetRandomValid<T>(IReadOnlyList<T> configs) where T : Object
    {
        if (configs == null || configs.Count == 0)
        {
            return null;
        }

        List<T> validConfigs = new List<T>();
        for (int i = 0; i < configs.Count; i++)
        {
            if (configs[i] != null)
            {
                validConfigs.Add(configs[i]);
            }
        }

        if (validConfigs.Count == 0)
        {
            return null;
        }

        return validConfigs[Random.Range(0, validConfigs.Count)];
    }
}
