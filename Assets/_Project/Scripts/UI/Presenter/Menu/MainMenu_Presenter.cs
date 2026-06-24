using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 用于处理主菜单界面的逻辑
/// 包括显示关卡信息、玩家数据和按钮交互
/// </summary>
[RequireComponent(typeof(MainMenu_View))]
public class MainMenu_Presenter : MonoBehaviour
{
    [SerializeField]
    private string gameSceneName = "Game";

    // 主菜单View的引用
    private MainMenu_View view;
    // 当前显示的关卡索引
    private int currentStageIndex;
    private PlayerData currentPlayerData;

    // 获取当前显示的关卡配置
    public StageConfig CurrentStageConfig => GetCurrentStageConfig();

    private void Awake()
    {
        // 缓存对MainMenu_View组件的引用
        view = GetComponent<MainMenu_View>();
    }

    private void OnEnable()
    {
        EnsureView();
        RefreshCurrentData();
        // 绑定事件
        AddButtonListener(view.buttonLeft, ShowPreviousStage);
        AddButtonListener(view.buttonRight, ShowNextStage);
        AddButtonListener(view.buttonPlay, PlayCurrentStage);
        AddButtonListener(view.buttonSettings, ShowSettingPanel);
        AddButtonListener(view.buttonCharacters, ShowPlayerPanel);
        EventCenter.Instance.AddEventListener<PlayerData>(E_EventType.PlayerDataChanged, OnPlayerDataChanged);
    }

    private void Start()
    {
        RefreshCurrentData();
    }

    private void OnDisable()
    {
        EnsureView();
        // 解绑事件
        RemoveButtonListener(view.buttonLeft, ShowPreviousStage);
        RemoveButtonListener(view.buttonRight, ShowNextStage);
        RemoveButtonListener(view.buttonPlay, PlayCurrentStage);
        RemoveButtonListener(view.buttonSettings, ShowSettingPanel);
        RemoveButtonListener(view.buttonCharacters, ShowPlayerPanel);
        EventCenter.Instance.RemoveEventListener<PlayerData>(E_EventType.PlayerDataChanged, OnPlayerDataChanged);
    }

    private void OnPlayerDataChanged(PlayerData playerData)
    {
        currentPlayerData = playerData;
        RefreshPlayerDataView(playerData);
        RefreshStageView();
    }

    private void EnsureView()
    {
        if (view == null)
        {
            view = GetComponent<MainMenu_View>();
        }
    }

    private void RefreshCurrentData()
    {
        currentPlayerData = PlayerDataManager.Instance.Data;
        RefreshPlayerDataView(currentPlayerData);
        RefreshStageView();
    }

    /// <summary>
    /// 显示上一个关卡
    /// </summary>
    private void ShowPreviousStage()
    {
        // 获取关卡总数
        int stageCount = GetStageCount();
        if (stageCount == 0)
        {
            return;
        }

        // 使用循环索引来显示上一个关卡
        currentStageIndex = (currentStageIndex - 1 + stageCount) % stageCount;
        RefreshStageView();
    }

    /// <summary>
    /// 显示下一个关卡
    /// </summary>
    private void ShowNextStage()
    {
        int stageCount = GetStageCount();
        if (stageCount == 0)
        {
            return;
        }

        currentStageIndex = (currentStageIndex + 1) % stageCount;
        RefreshStageView();
    }

    /// <summary>
    /// 刷新关卡视图
    /// </summary>
    private void RefreshStageView()
    {
        // 获取当前关卡配置
        StageConfig stageConfig = GetCurrentStageConfig();
        // 如果没有关卡配置
        if (stageConfig == null)
        {
            // 则清空视图并禁用按钮
            SetText(view.textStageName, string.Empty);
            SetText(view.textStageNumber, string.Empty);
            SetImage(view.imageStage, null);
            SetVisible(view.imageLock, false);
            SetButtonInteractable(view.buttonPlay, false);
            return;
        }

        // 根据关卡配置更新视图
        SetText(view.textStageName, stageConfig.StageName);
        SetText(view.textStageNumber, $"Stage {stageConfig.StageLevel}");
        SetImage(view.imageStage, stageConfig.StageImage);

        // 检查关卡是否解锁
        bool isUnlocked = currentPlayerData != null && currentPlayerData.IsStageUnlocked(stageConfig.ResourceId);
        SetVisible(view.imageLock, !isUnlocked);
        SetButtonInteractable(view.buttonPlay, isUnlocked);
    }

    /// <summary>
    /// 刷新玩家数据视图
    /// </summary>
    /// <param name="playerData">玩家数据</param>
    private void RefreshPlayerDataView(PlayerData playerData)
    {
        int gold = playerData != null ? playerData.gold : 0;
        SetText(view.textGold, gold.ToString());
    }

    /// <summary>
    /// 显示设置面板
    /// </summary>
    private void ShowSettingPanel()
    {
        UIManager.Instance.ShowPanel(UIPanelId.Setting);
    }

    /// <summary>
    /// 显示玩家面板
    /// </summary>
    private void ShowPlayerPanel()
    {
        UIManager.Instance.ShowPanel(UIPanelId.Player);
    }

    private void PlayCurrentStage()
    {
        StageConfig stageConfig = GetCurrentStageConfig();
        if (stageConfig == null || currentPlayerData == null || !currentPlayerData.IsStageUnlocked(stageConfig.ResourceId))
        {
            return;
        }

        if (!GameLaunchContext.StartGame(stageConfig, currentPlayerData.selectedCharacterId, gameSceneName))
        {
            RefreshStageView();
            return;
        }

        SetButtonInteractable(view.buttonPlay, false);
        UIManager.Instance.HideAllPanels();
    }

    /// <summary>
    /// 获取当前显示的关卡配置
    /// </summary>
    /// <returns>当前关卡配置</returns>
    private StageConfig GetCurrentStageConfig()
    {
        if (GameResources.Instance == null || GameResources.Instance.StageConfigs == null || GameResources.Instance.StageConfigs.Count == 0)
        {
            return null;
        }

        currentStageIndex = Mathf.Clamp(currentStageIndex, 0, GameResources.Instance.StageConfigs.Count - 1);
        return GameResources.Instance.StageConfigs[currentStageIndex];
    }

    /// <summary>
    /// 获取关卡总数
    /// </summary>
    /// <returns>关卡总数</returns>
    private int GetStageCount()
    {
        if (GameResources.Instance == null || GameResources.Instance.StageConfigs == null)
        {
            return 0;
        }

        return GameResources.Instance.StageConfigs.Count;
    }

    /// <summary>
    /// 设置TextMeshProUGUI的文本内容
    /// </summary>
    /// <param name="label">TextMeshProUGUI组件</param>
    /// <param name="text">要设置的文本</param>
    private static void SetText(TMPro.TextMeshProUGUI label, string text)
    {
        if (label != null)
        {
            label.text = text;
        }
    }

    /// <summary>
    /// 设置Image的Sprite
    /// </summary>
    /// <param name="image">Image组件</param>
    /// <param name="sprite">Sprite资源</param>
    private static void SetImage(Image image, Sprite sprite)
    {
        if (image == null)
        {
            return;
        }

        image.sprite = sprite;
        image.enabled = sprite != null;
    }

    /// <summary>
    /// 设置UI元素的可见性
    /// </summary>
    /// <param name="graphic">需要设置的UI元素</param>
    /// <param name="visible">是否可见</param>
    private static void SetVisible(Graphic graphic, bool visible)
    {
        if (graphic != null)
        {
            graphic.enabled = visible;
        }
    }

    /// <summary>
    /// 设置按钮的可交互状态
    /// </summary>
    /// <param name="button">需要设置的按钮</param>
    /// <param name="interactable">是否可交互</param>
    private static void SetButtonInteractable(Button button, bool interactable)
    {
        if (button != null)
        {
            button.interactable = interactable;
        }
    }

    /// <summary>
    /// 为按钮添加点击事件监听器
    /// </summary>
    /// <param name="button">需要添加监听器的按钮</param>
    /// <param name="action">点击事件触发时调用的函数</param>
    private static void AddButtonListener(Button button, UnityEngine.Events.UnityAction action)
    {
        if (button != null)
        {
            button.onClick.AddListener(action);
        }
    }

    /// <summary>
    /// 为按钮移除点击事件监听器
    /// </summary>
    /// <param name="button">需要移除监听器的按钮</param>
    /// <param name="action">点击事件触发时调用的函数</param>
    private static void RemoveButtonListener(Button button, UnityEngine.Events.UnityAction action)
    {
        if (button != null)
        {
            button.onClick.RemoveListener(action);
        }
    }
}
