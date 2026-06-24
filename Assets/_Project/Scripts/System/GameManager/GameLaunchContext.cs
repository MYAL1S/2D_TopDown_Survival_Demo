using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// 游戏启动上下文
/// 用于在主菜单切换到游戏场景时临时保存关卡和角色选择信息
/// </summary>
public static class GameLaunchContext
{
    // 默认游戏场景名
    private const string DefaultGameSceneName = "Game";

    // 等待进入游戏场景后应用的关卡配置
    private static StageConfig pendingStageConfig;
    // 等待进入游戏场景后生成的角色ID
    private static string pendingCharacterId;
    // 等待加载的游戏场景名
    private static string pendingGameSceneName = DefaultGameSceneName;
    // 防止重复点击Play导致多次加载场景
    private static bool isLoading;

    /// <summary>
    /// 开始加载游戏场景并缓存本次启动所需的关卡和角色信息
    /// </summary>
    /// <param name="stageConfig">要进入的关卡配置</param>
    /// <param name="characterId">要使用的角色ID</param>
    /// <param name="gameSceneName">游戏场景名</param>
    /// <returns>是否成功开始加载</returns>
    public static bool StartGame(StageConfig stageConfig, string characterId, string gameSceneName = DefaultGameSceneName)
    {
        if (isLoading || stageConfig == null)
        {
            return false;
        }

        pendingStageConfig = stageConfig;
        pendingCharacterId = characterId;
        pendingGameSceneName = string.IsNullOrWhiteSpace(gameSceneName) ? DefaultGameSceneName : gameSceneName;
        isLoading = true;
        Time.timeScale = 1f;

        SceneManager.sceneLoaded -= OnSceneLoaded;
        SceneManager.sceneLoaded += OnSceneLoaded;
        SceneManager.LoadScene(pendingGameSceneName);
        return true;
    }

    /// <summary>
    /// 游戏场景加载完成后应用等待中的启动参数
    /// </summary>
    /// <param name="scene">加载完成的场景</param>
    /// <param name="mode">加载模式</param>
    private static void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name != pendingGameSceneName)
        {
            return;
        }

        SceneManager.sceneLoaded -= OnSceneLoaded;
        ConfigureGameScene();
        ClearPendingState();
    }

    /// <summary>
    /// 根据主菜单缓存的关卡和角色信息配置游戏场景
    /// </summary>
    private static void ConfigureGameScene()
    {
        Generator generator = Generator.Instance != null ? Generator.Instance : Object.FindObjectOfType<Generator>();
        if (generator == null)
        {
            Debug.LogError($"{nameof(GameLaunchContext)} could not find {nameof(Generator)} after loading the game scene.");
            return;
        }

        SpawnSystem spawnSystem = generator.SpawnSystem;
        if (spawnSystem != null)
        {
            spawnSystem.SetSpawnConfig(pendingStageConfig != null ? pendingStageConfig.SpawnConfig : null, false);
        }

        string characterId = GetPlayableCharacterId();
        CharacterSelectionSystem characterSelectionSystem = generator.CharacterSelectionSystem;
        if (characterSelectionSystem != null)
        {
            characterSelectionSystem.SelectCharacter(characterId);
        }

        if (spawnSystem != null)
        {
            spawnSystem.StartSpawning();
        }

        LevelManager.Instance.StartLevel();
        UIManager.Instance.ShowPanel(UIPanelId.Game);
        UIManager.Instance.HidePanel(UIPanelId.Pause);
        UIManager.Instance.HidePanel(UIPanelId.Chest);
        StageRuntimeManager.Instance.StartStage(pendingStageConfig, spawnSystem);
    }

    /// <summary>
    /// 获取本局真正可使用的角色ID
    /// 如果传入角色未解锁 则回退到默认角色
    /// </summary>
    /// <returns>可生成的角色ID</returns>
    private static string GetPlayableCharacterId()
    {
        PlayerData playerData = PlayerDataManager.Instance.Data;
        string characterId = !string.IsNullOrWhiteSpace(pendingCharacterId)
            ? pendingCharacterId
            : playerData.selectedCharacterId;

        if (!string.IsNullOrWhiteSpace(characterId) && playerData.IsCharacterUnlocked(characterId))
        {
            return characterId;
        }

        PlayerConfig defaultConfig = GameResources.Instance != null ? GameResources.Instance.GetDefaultPlayerConfig() : null;
        return defaultConfig != null ? defaultConfig.ResourceId : characterId;
    }

    /// <summary>
    /// 清理等待中的启动参数 避免影响下一次进入游戏
    /// </summary>
    private static void ClearPendingState()
    {
        pendingStageConfig = null;
        pendingCharacterId = null;
        pendingGameSceneName = DefaultGameSceneName;
        isLoading = false;
    }
}
