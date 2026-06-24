using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 游戏资源管理器
/// 维护玩家、敌人、状态效果、AOE、关卡和音频配置，并提供按资源 id 查询的能力
/// </summary>
[DisallowMultipleComponent]
public class GameResources : MonoBehaviour
{
    private static GameResources _instance;
    public static GameResources Instance => _instance;

    // 玩家角色配置列表
    [Header("Player Resources")]
    [SerializeField]
    private PlayerConfig[] playerConfigs;

    // 敌人配置列表
    [Header("Enemy Resources")]
    [SerializeField]
    private EnemyConfig[] enemyConfigs;

    // 状态效果配置列表
    [Header("Status Effect Resources")]
    [SerializeField]
    private StatusEffectConfig[] statusEffectConfigs;

    // AOE 配置列表
    [Header("AOE Resources")]
    [SerializeField]
    private AOEConfig[] aoeConfigs;

    // 关卡配置列表。
    [Header("Stage Resources")]
    [SerializeField]
    private StageConfig[] stageConfigs;

    // 音频配置列表，音乐和音效都通过这里缓存
    [Header("Audio Resources")]
    [SerializeField]
    private AudioConfig[] audioConfigs;

    // 以下字典使用 ResourceId 作为键，便于运行时快速查找配置
    private readonly Dictionary<string, PlayerConfig> playerConfigLookup = new Dictionary<string, PlayerConfig>();
    private readonly Dictionary<string, EnemyConfig> enemyConfigLookup = new Dictionary<string, EnemyConfig>();
    private readonly Dictionary<string, StatusEffectConfig> statusEffectConfigLookup = new Dictionary<string, StatusEffectConfig>();
    private readonly Dictionary<string, AOEConfig> aoeConfigLookup = new Dictionary<string, AOEConfig>();
    private readonly Dictionary<string, StageConfig> stageConfigLookup = new Dictionary<string, StageConfig>();
    private readonly Dictionary<string, AudioConfig> audioConfigLookup = new Dictionary<string, AudioConfig>();

    public IReadOnlyList<PlayerConfig> PlayerConfigs => playerConfigs;
    public IReadOnlyList<EnemyConfig> EnemyConfigs => enemyConfigs;
    public IReadOnlyList<StatusEffectConfig> StatusEffectConfigs => statusEffectConfigs;
    public IReadOnlyList<AOEConfig> AOEConfigs => aoeConfigs;
    public IReadOnlyList<StageConfig> StageConfigs => stageConfigs;
    public IReadOnlyList<AudioConfig> AudioConfigs => audioConfigs;

    /// <summary>
    /// 初始化资源单例并构建运行时查找缓存
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
    /// 获取第一个可用玩家配置，作为默认角色配置
    /// </summary>
    public PlayerConfig GetDefaultPlayerConfig()
    {
        return GetFirstValid(playerConfigs);
    }

    /// <summary>
    /// 获取第一个可用敌人配置，作为默认敌人配置
    /// </summary>
    public EnemyConfig GetDefaultEnemyConfig()
    {
        return GetFirstValid(enemyConfigs);
    }

    /// <summary>
    /// 获取第一个可用关卡配置，作为默认关卡配置
    /// </summary>
    public StageConfig GetDefaultStageConfig()
    {
        return GetFirstValid(stageConfigs);
    }

    /// <summary>
    /// 根据资源 id 获取玩家配置
    /// </summary>
    /// <param name="resourceId">玩家资源 id，空值时返回默认配置</param>
    /// <returns>玩家配置</returns>
    public PlayerConfig GetPlayerConfig(string resourceId)
    {
        if (string.IsNullOrWhiteSpace(resourceId))
        {
            return GetDefaultPlayerConfig();
        }

        return playerConfigLookup.TryGetValue(resourceId, out PlayerConfig config) ? config : null;
    }

    /// <summary>
    /// 根据资源 id 获取敌人配置
    /// </summary>
    /// <param name="resourceId">敌人资源 id，空值时返回默认配置</param>
    /// <returns>敌人配置</returns>
    public EnemyConfig GetEnemyConfig(string resourceId)
    {
        if (string.IsNullOrWhiteSpace(resourceId))
        {
            return GetDefaultEnemyConfig();
        }

        return enemyConfigLookup.TryGetValue(resourceId, out EnemyConfig config) ? config : null;
    }

    /// <summary>
    /// 根据效果 id 获取状态效果配置
    /// </summary>
    /// <param name="effectId">状态效果 id</param>
    /// <returns>状态效果配置</returns>
    public StatusEffectConfig GetStatusEffectConfig(string effectId)
    {
        if (string.IsNullOrWhiteSpace(effectId))
        {
            return null;
        }

        return statusEffectConfigLookup.TryGetValue(effectId, out StatusEffectConfig config) ? config : null;
    }

    /// <summary>
    /// 根据资源 id 获取 AOE 配置
    /// </summary>
    /// <param name="resourceId">AOE 资源 id</param>
    /// <returns>AOE 配置</returns>
    public AOEConfig GetAOEConfig(string resourceId)
    {
        if (string.IsNullOrWhiteSpace(resourceId))
        {
            return null;
        }

        return aoeConfigLookup.TryGetValue(resourceId, out AOEConfig config) ? config : null;
    }

    /// <summary>
    /// 根据资源 id 获取关卡配置
    /// </summary>
    /// <param name="resourceId">关卡资源 id，空值时返回默认配置</param>
    /// <returns>关卡配置</returns>
    public StageConfig GetStageConfig(string resourceId)
    {
        if (string.IsNullOrWhiteSpace(resourceId))
        {
            return GetDefaultStageConfig();
        }

        return stageConfigLookup.TryGetValue(resourceId, out StageConfig config) ? config : null;
    }

    /// <summary>
    /// 根据资源 id 获取音频配置
    /// </summary>
    /// <param name="resourceId">音频资源 id</param>
    /// <returns>音频配置</returns>
    public AudioConfig GetAudioConfig(string resourceId)
    {
        if (string.IsNullOrWhiteSpace(resourceId))
        {
            return null;
        }

        return audioConfigLookup.TryGetValue(resourceId, out AudioConfig config) ? config : null;
    }

    /// <summary>
    /// 根据资源 id 获取音频片段
    /// </summary>
    /// <param name="resourceId">音频资源 id</param>
    /// <returns>音频片段</returns>
    public AudioClip GetAudioClip(string resourceId)
    {
        return GetAudioConfig(resourceId)?.Clip;
    }

    /// <summary>
    /// 获取默认背景音乐配置
    /// </summary>
    /// <returns>第一个音乐类型的音频配置</returns>
    public AudioConfig GetDefaultMusicConfig()
    {
        if (audioConfigs == null)
        {
            return null;
        }

        for (int i = 0; i < audioConfigs.Length; i++)
        {
            AudioConfig config = audioConfigs[i];
            if (config != null && config.AudioType == AudioConfigType.Music)
            {
                return config;
            }
        }

        return null;
    }

    /// <summary>
    /// 从候选敌人 id 中随机获取一个可用敌人配置
    /// 候选列表为空时，从全部敌人配置中随机选择
    /// </summary>
    /// <param name="resourceIds">候选敌人资源 id</param>
    /// <returns>随机敌人配置</returns>
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
    /// 重建所有运行时查找缓存
    /// </summary>
    private void BuildLookupCaches()
    {
        playerConfigLookup.Clear();
        enemyConfigLookup.Clear();
        statusEffectConfigLookup.Clear();
        aoeConfigLookup.Clear();
        stageConfigLookup.Clear();
        audioConfigLookup.Clear();

        AddPlayerConfigsToCache(playerConfigs);
        AddEnemyConfigsToCache(enemyConfigs);
        AddStatusEffectConfigsToCache(statusEffectConfigs);
        AddAOEConfigsToCache(aoeConfigs);
        AddStageConfigsToCache(stageConfigs);
        AddAudioConfigsToCache(audioConfigs);
    }

    /// <summary>
    /// 将玩家配置写入查找缓存
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
    /// 将敌人配置写入查找缓存
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
    /// 将状态效果配置写入查找缓存
    /// </summary>
    private void AddStatusEffectConfigsToCache(StatusEffectConfig[] configs)
    {
        if (configs == null)
        {
            return;
        }

        for (int i = 0; i < configs.Length; i++)
        {
            StatusEffectConfig config = configs[i];
            if (config == null || string.IsNullOrWhiteSpace(config.EffectId))
            {
                continue;
            }

            statusEffectConfigLookup[config.EffectId] = config;
        }
    }

    /// <summary>
    /// 将 AOE 配置写入查找缓存
    /// </summary>
    private void AddAOEConfigsToCache(AOEConfig[] configs)
    {
        if (configs == null)
        {
            return;
        }

        for (int i = 0; i < configs.Length; i++)
        {
            AOEConfig config = configs[i];
            if (config == null || string.IsNullOrWhiteSpace(config.ResourceId))
            {
                continue;
            }

            aoeConfigLookup[config.ResourceId] = config;
        }
    }

    /// <summary>
    /// 将关卡配置写入查找缓存
    /// </summary>
    private void AddStageConfigsToCache(StageConfig[] configs)
    {
        if (configs == null)
        {
            return;
        }

        for (int i = 0; i < configs.Length; i++)
        {
            StageConfig config = configs[i];
            if (config == null || string.IsNullOrWhiteSpace(config.ResourceId))
            {
                continue;
            }

            stageConfigLookup[config.ResourceId] = config;
        }
    }

    /// <summary>
    /// 将音频配置写入查找缓存
    /// </summary>
    private void AddAudioConfigsToCache(AudioConfig[] configs)
    {
        if (configs == null)
        {
            return;
        }

        for (int i = 0; i < configs.Length; i++)
        {
            AudioConfig config = configs[i];
            if (config == null || string.IsNullOrWhiteSpace(config.ResourceId))
            {
                continue;
            }

            audioConfigLookup[config.ResourceId] = config;
        }
    }

    /// <summary>
    /// 返回列表中的第一个非空配置
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
