using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 关卡运行时信息快照
/// 用于通知UI刷新计时器和通关状态
/// </summary>
[Serializable]
public class StageRuntimeInfo
{
    // 当前关卡配置
    public StageConfig stageConfig;
    // 已经过的关卡时间
    public float elapsedSeconds;
    // 剩余关卡时间
    public float remainingSeconds;
    // 关卡总时长
    public float durationSeconds;
    // 是否已经通关
    public bool isCleared;

    // 关卡进度 0表示刚开始 1表示已到达时限
    public float Progress => durationSeconds <= 0f ? 1f : Mathf.Clamp01(elapsedSeconds / durationSeconds);
}

/// <summary>
/// 关卡运行管理器
/// 负责根据关卡配置计时 在时间结束时判定通关
/// </summary>
public class StageRuntimeManager : MonoBehaviour
{
    private static StageRuntimeManager instance;

    // 当前运行中的关卡配置
    private StageConfig currentStageConfig;
    // 当前关卡使用的刷怪系统
    private SpawnSystem spawnSystem;
    // 当前关卡已运行时间
    private float elapsedSeconds;
    // 是否正在计时
    private bool isRunning;
    // 是否已经完成通关流程
    private bool isCleared;

    public static StageRuntimeManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<StageRuntimeManager>();
            }

            if (instance == null)
            {
                GameObject managerObject = new GameObject(nameof(StageRuntimeManager));
                instance = managerObject.AddComponent<StageRuntimeManager>();
            }

            return instance;
        }
    }

    public StageConfig CurrentStageConfig => currentStageConfig;
    public float ElapsedSeconds => elapsedSeconds;
    public float RemainingSeconds => currentStageConfig == null ? 0f : Mathf.Max(0f, currentStageConfig.DurationSeconds - elapsedSeconds);
    public bool IsRunning => isRunning;
    public bool IsCleared => isCleared;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void OnDestroy()
    {
        if (instance == this)
        {
            instance = null;
        }
    }

    private void Update()
    {
        if (!isRunning || currentStageConfig == null)
        {
            return;
        }

        elapsedSeconds += Time.deltaTime;
        NotifyStageTimeChanged();

        if (elapsedSeconds >= currentStageConfig.DurationSeconds)
        {
            CompleteStage();
        }
    }

    /// <summary>
    /// 开始运行指定关卡 并绑定本关的刷怪系统
    /// </summary>
    /// <param name="stageConfig">关卡配置</param>
    /// <param name="stageSpawnSystem">刷怪系统</param>
    public void StartStage(StageConfig stageConfig, SpawnSystem stageSpawnSystem)
    {
        currentStageConfig = stageConfig;
        spawnSystem = stageSpawnSystem;
        elapsedSeconds = 0f;
        isCleared = false;
        isRunning = stageConfig != null;
        NotifyStageTimeChanged();
    }

    /// <summary>
    /// 停止当前关卡运行并清理运行时状态
    /// </summary>
    public void StopStage()
    {
        isRunning = false;
        currentStageConfig = null;
        spawnSystem = null;
        elapsedSeconds = 0f;
        isCleared = false;
    }

    /// <summary>
    /// 完成关卡流程 停止刷怪、清理敌人、解锁下一关并通知结算
    /// </summary>
    private void CompleteStage()
    {
        if (isCleared || currentStageConfig == null)
        {
            return;
        }

        isRunning = false;
        isCleared = true;
        elapsedSeconds = currentStageConfig.DurationSeconds;

        if (spawnSystem != null)
        {
            spawnSystem.StopSpawning();
            spawnSystem.ClearActiveEnemies();
        }

        UnlockNextStage();
        NotifyStageTimeChanged();
        EventCenter.Instance.EventTrigger(E_EventType.StageCleared, currentStageConfig);
    }

    /// <summary>
    /// 通知关卡时间发生变化
    /// </summary>
    private void NotifyStageTimeChanged()
    {
        EventCenter.Instance.EventTrigger(E_EventType.StageTimeChanged, CreateRuntimeInfo());
    }

    /// <summary>
    /// 创建当前关卡运行信息快照
    /// </summary>
    /// <returns>关卡运行时信息</returns>
    private StageRuntimeInfo CreateRuntimeInfo()
    {
        float duration = currentStageConfig != null ? currentStageConfig.DurationSeconds : 0f;
        return new StageRuntimeInfo
        {
            stageConfig = currentStageConfig,
            elapsedSeconds = elapsedSeconds,
            remainingSeconds = Mathf.Max(0f, duration - elapsedSeconds),
            durationSeconds = duration,
            isCleared = isCleared
        };
    }

    /// <summary>
    /// 当前关卡通关后 根据资源顺序解锁下一关
    /// </summary>
    private void UnlockNextStage()
    {
        if (currentStageConfig == null || GameResources.Instance == null || GameResources.Instance.StageConfigs == null)
        {
            return;
        }

        IReadOnlyList<StageConfig> stageConfigs = GameResources.Instance.StageConfigs;
        for (int i = 0; i < stageConfigs.Count - 1; i++)
        {
            StageConfig stageConfig = stageConfigs[i];
            if (stageConfig != null && stageConfig.ResourceId == currentStageConfig.ResourceId)
            {
                StageConfig nextStageConfig = stageConfigs[i + 1];
                if (nextStageConfig != null)
                {
                    PlayerDataManager.Instance.UnlockStage(nextStageConfig.ResourceId);
                }

                return;
            }
        }
    }
}
