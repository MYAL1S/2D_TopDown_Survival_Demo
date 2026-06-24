using System;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// 关卡内统计信息快照
/// 用于通知GamePanel刷新金币、击杀数和暂停状态
/// </summary>
[Serializable]
public class LevelStatsInfo
{
    // 本局拾取的金币数量
    public int gold;
    // 本局击杀敌人数
    public int enemiesKilled;
    // 当前是否处于暂停状态
    public bool isPaused;
}

/// <summary>
/// 关卡管理器
/// 负责局内统计、暂停恢复、关卡结束和结算奖励
/// </summary>
public class LevelManager : MonoBehaviour
{
    private static LevelManager instance;

    // 本局统计数据
    private readonly LevelStatsInfo stats = new LevelStatsInfo();

    // 暂停前的TimeScale 用于恢复游戏
    private float previousTimeScale = 1f;
    // 防止重复进入结算流程
    private bool isEnding;
    // 防止结算金币重复入账
    private bool rewardClaimed;
    // 离开关卡后等待显示的主菜单场景名
    private string pendingMainMenuSceneName;

    public static LevelManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<LevelManager>();
            }

            if (instance == null)
            {
                GameObject managerObject = new GameObject(nameof(LevelManager));
                instance = managerObject.AddComponent<LevelManager>();
            }

            return instance;
        }
    }

    public int Gold => stats.gold;
    public int EnemiesKilled => stats.enemiesKilled;
    public bool IsPaused => stats.isPaused;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
        AddListeners();
    }

    private void OnDestroy()
    {
        RemoveListeners();

        if (instance == this)
        {
            instance = null;
        }
    }

    /// <summary>
    /// 开始一局游戏并重置局内统计
    /// </summary>
    public void StartLevel()
    {
        stats.gold = 0;
        stats.enemiesKilled = 0;
        stats.isPaused = false;
        isEnding = false;
        rewardClaimed = false;
        previousTimeScale = 1f;
        Time.timeScale = 1f;
        NotifyStatsChanged();
    }

    /// <summary>
    /// 暂停游戏并通知暂停事件
    /// </summary>
    public void PauseGame()
    {
        if (stats.isPaused)
        {
            return;
        }

        previousTimeScale = Time.timeScale <= 0f ? 1f : Time.timeScale;
        Time.timeScale = 0f;
        stats.isPaused = true;
        NotifyStatsChanged();
        EventCenter.Instance.EventTrigger(E_EventType.GamePaused);
    }

    /// <summary>
    /// 恢复游戏并通知恢复事件
    /// </summary>
    public void ResumeGame()
    {
        if (!stats.isPaused)
        {
            return;
        }

        Time.timeScale = previousTimeScale;
        stats.isPaused = false;
        NotifyStatsChanged();
        EventCenter.Instance.EventTrigger(E_EventType.GameResumed);
    }

    /// <summary>
    /// 退出当前关卡并加载主菜单场景
    /// </summary>
    /// <param name="mainMenuSceneName">主菜单场景名</param>
    public void ExitLevel(string mainMenuSceneName)
    {
        Time.timeScale = 1f;
        stats.isPaused = false;
        isEnding = false;
        StageRuntimeManager.Instance.StopStage();
        UIManager.Instance.HideAllPanels();

        pendingMainMenuSceneName = string.IsNullOrWhiteSpace(mainMenuSceneName) ? "MainMenu" : mainMenuSceneName;
        SceneManager.sceneLoaded -= OnMainMenuSceneLoaded;
        SceneManager.sceneLoaded += OnMainMenuSceneLoaded;
        SceneManager.LoadScene(pendingMainMenuSceneName);
    }

    /// <summary>
    /// 结束当前关卡并打开结算宝箱面板
    /// </summary>
    /// <param name="isVictory">是否胜利结束</param>
    public void EndLevel(bool isVictory)
    {
        if (isEnding)
        {
            return;
        }

        isEnding = true;
        Time.timeScale = 0f;
        stats.isPaused = false;
        StageRuntimeManager.Instance.StopStage();

        Generator generator = Generator.Instance;
        if (generator != null && generator.SpawnSystem != null)
        {
            generator.SpawnSystem.StopSpawning();
            generator.SpawnSystem.ClearActiveEnemies();
        }

        UIManager.Instance.HidePanel(UIPanelId.Game);
        UIManager.Instance.HidePanel(UIPanelId.Pause);
        PersistChestRewardIfNeeded();
        UIManager.Instance.ShowPanel(UIPanelId.Chest);
        NotifyStatsChanged();
    }

    /// <summary>
    /// 领取宝箱奖励并返回主菜单
    /// </summary>
    /// <param name="mainMenuSceneName">主菜单场景名</param>
    public void ClaimChestRewardAndReturnToMainMenu(string mainMenuSceneName)
    {
        PersistChestRewardIfNeeded();
        ExitLevel(mainMenuSceneName);
    }

    /// <summary>
    /// 将本局获得的金币写入玩家数据 避免重复领取
    /// </summary>
    private void PersistChestRewardIfNeeded()
    {
        if (rewardClaimed || stats.gold <= 0)
        {
            return;
        }

        rewardClaimed = true;
        PlayerDataManager.Instance.AddGold(stats.gold);
    }

    /// <summary>
    /// 监听局内掉落、击杀、通关和玩家死亡事件
    /// </summary>
    private void AddListeners()
    {
        EventCenter.Instance.AddEventListener<int>(E_EventType.GoldPickedUp, OnGoldPickedUp);
        EventCenter.Instance.AddEventListener<Enemy>(E_EventType.EnemyKilled, OnEnemyKilled);
        EventCenter.Instance.AddEventListener<StageConfig>(E_EventType.StageCleared, OnStageCleared);
        EventCenter.Instance.AddEventListener(E_EventType.PlayerDied, OnPlayerDied);
    }

    /// <summary>
    /// 移除局内事件监听
    /// </summary>
    private void RemoveListeners()
    {
        EventCenter.Instance.RemoveEventListener<int>(E_EventType.GoldPickedUp, OnGoldPickedUp);
        EventCenter.Instance.RemoveEventListener<Enemy>(E_EventType.EnemyKilled, OnEnemyKilled);
        EventCenter.Instance.RemoveEventListener<StageConfig>(E_EventType.StageCleared, OnStageCleared);
        EventCenter.Instance.RemoveEventListener(E_EventType.PlayerDied, OnPlayerDied);
    }

    /// <summary>
    /// 响应金币拾取事件并刷新统计
    /// </summary>
    /// <param name="amount">拾取金币数量</param>
    private void OnGoldPickedUp(int amount)
    {
        if (amount <= 0)
        {
            return;
        }

        stats.gold += amount;
        NotifyStatsChanged();
    }

    /// <summary>
    /// 响应敌人死亡事件并刷新击杀数
    /// </summary>
    /// <param name="enemy">被击杀的敌人</param>
    private void OnEnemyKilled(Enemy enemy)
    {
        stats.enemiesKilled++;
        NotifyStatsChanged();
    }

    /// <summary>
    /// 响应关卡通关事件
    /// </summary>
    /// <param name="stageConfig">通关的关卡配置</param>
    private void OnStageCleared(StageConfig stageConfig)
    {
        EndLevel(true);
    }

    /// <summary>
    /// 响应玩家死亡事件
    /// </summary>
    private void OnPlayerDied()
    {
        EndLevel(false);
    }

    /// <summary>
    /// 主菜单场景加载完成后显示主菜单面板
    /// </summary>
    /// <param name="scene">加载完成的场景</param>
    /// <param name="mode">加载模式</param>
    private void OnMainMenuSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name != pendingMainMenuSceneName)
        {
            return;
        }

        SceneManager.sceneLoaded -= OnMainMenuSceneLoaded;
        UIManager.Instance.ShowPanel(UIPanelId.MainMenu);
        pendingMainMenuSceneName = null;
    }

    /// <summary>
    /// 通知局内统计发生变化
    /// </summary>
    private void NotifyStatsChanged()
    {
        EventCenter.Instance.EventTrigger(E_EventType.LevelStatsChanged, CreateStatsSnapshot());
    }

    /// <summary>
    /// 创建局内统计快照 避免外部直接修改内部统计对象
    /// </summary>
    /// <returns>局内统计快照</returns>
    private LevelStatsInfo CreateStatsSnapshot()
    {
        return new LevelStatsInfo
        {
            gold = stats.gold,
            enemiesKilled = stats.enemiesKilled,
            isPaused = stats.isPaused
        };
    }
}
