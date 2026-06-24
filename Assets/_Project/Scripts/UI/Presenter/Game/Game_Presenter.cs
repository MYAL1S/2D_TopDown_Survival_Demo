using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 游戏面板 Presenter。
/// 负责监听关卡数据变化，并将格式化后的显示结果写入 Game_View 控件。
/// </summary>
[RequireComponent(typeof(Game_View))]
public class Game_Presenter : MonoBehaviour
{
    // 游戏面板 View 引用。
    private Game_View view;

    private void Awake()
    {
        EnsureView();
    }

    private void OnEnable()
    {
        EnsureView();
        AddButtonListener(view.pauseButton, OnPauseClicked);
        EventCenter.Instance.AddEventListener<LevelStatsInfo>(E_EventType.LevelStatsChanged, OnLevelStatsChanged);
        EventCenter.Instance.AddEventListener<StageRuntimeInfo>(E_EventType.StageTimeChanged, OnStageTimeChanged);

        if (LevelManager.Instance != null)
        {
            SetGold(LevelManager.Instance.Gold);
            SetEnemiesKilled(LevelManager.Instance.EnemiesKilled);
        }

        if (StageRuntimeManager.Instance != null)
        {
            SetTimer(StageRuntimeManager.Instance.RemainingSeconds);
        }
    }

    private void OnDisable()
    {
        EnsureView();
        RemoveButtonListener(view.pauseButton, OnPauseClicked);
        EventCenter.Instance.RemoveEventListener<LevelStatsInfo>(E_EventType.LevelStatsChanged, OnLevelStatsChanged);
        EventCenter.Instance.RemoveEventListener<StageRuntimeInfo>(E_EventType.StageTimeChanged, OnStageTimeChanged);
    }

    /// <summary>
    /// 局内统计变化时刷新金币和击杀数。
    /// </summary>
    /// <param name="stats">局内统计快照。</param>
    private void OnLevelStatsChanged(LevelStatsInfo stats)
    {
        if (stats == null)
        {
            return;
        }

        SetGold(stats.gold);
        SetEnemiesKilled(stats.enemiesKilled);
    }

    /// <summary>
    /// 关卡时间变化时刷新计时器。
    /// </summary>
    /// <param name="info">关卡运行时信息。</param>
    private void OnStageTimeChanged(StageRuntimeInfo info)
    {
        if (info != null)
        {
            SetTimer(info.remainingSeconds);
        }
    }

    /// <summary>
    /// 点击暂停按钮时暂停游戏并显示暂停面板。
    /// </summary>
    private void OnPauseClicked()
    {
        LevelManager.Instance.PauseGame();
        UIManager.Instance.ShowPanel(UIPanelId.Pause);
    }

    /// <summary>
    /// 刷新金币文本。
    /// </summary>
    private void SetGold(int value)
    {
        SetText(view.goldText, value.ToString());
    }

    /// <summary>
    /// 刷新击杀数文本。
    /// </summary>
    private void SetEnemiesKilled(int value)
    {
        SetText(view.enemiesKilledText, value.ToString());
    }

    /// <summary>
    /// 将剩余秒数格式化为 00:00 并刷新计时器文本。
    /// </summary>
    private void SetTimer(float seconds)
    {
        int totalSeconds = Mathf.Max(0, Mathf.CeilToInt(seconds));
        int minutes = totalSeconds / 60;
        int remainingSeconds = totalSeconds % 60;
        SetText(view.timerText, $"{minutes:00}:{remainingSeconds:00}");
    }

    private void EnsureView()
    {
        if (view == null)
        {
            view = GetComponent<Game_View>();
        }
    }

    private static void SetText(TextMeshProUGUI label, string text)
    {
        if (label != null)
        {
            label.text = text;
        }
    }

    private static void AddButtonListener(Button button, UnityEngine.Events.UnityAction action)
    {
        if (button != null)
        {
            button.onClick.AddListener(action);
        }
    }

    private static void RemoveButtonListener(Button button, UnityEngine.Events.UnityAction action)
    {
        if (button != null)
        {
            button.onClick.RemoveListener(action);
        }
    }
}
