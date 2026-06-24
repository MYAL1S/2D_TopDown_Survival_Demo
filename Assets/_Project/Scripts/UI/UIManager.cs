using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public enum UIPanelId
{
    // 主菜单面板
    MainMenu,
    // 设置面板
    Setting,
    // 角色选择面板
    Player,
    // 游戏内信息面板
    Game,
    // 暂停面板
    Pause,
    // 结算宝箱面板
    Chest,
}

/// <summary>
/// UI管理器
/// 负责创建并缓存Canvas、EventSystem和各个面板实例
/// </summary>
public class UIManager : MonoBehaviour
{
    /// <summary>
    /// 面板ID与面板预制体之间的绑定关系
    /// </summary>
    [Serializable]
    private class PanelBinding
    {
        // 面板唯一标识
        public UIPanelId panelId = default;
        // 面板预制体
        public GameObject panelPrefab = default;
        // 面板创建后是否默认显示
        public bool showOnCreate = default;
    }

    private static UIManager instance;

    [Header("UI Root Prefabs")]
    [SerializeField]
    private Canvas canvasPrefab;

    [SerializeField]
    private EventSystem eventSystemPrefab;

    [Header("Panel Prefabs")]
    [SerializeField]
    private PanelBinding[] panelBindings;

    // 面板预制体缓存 用于根据面板ID快速找到预制体
    private readonly Dictionary<UIPanelId, GameObject> panelPrefabLookup = new Dictionary<UIPanelId, GameObject>();
    // 面板实例缓存 避免重复创建相同面板
    private readonly Dictionary<UIPanelId, GameObject> panelInstanceLookup = new Dictionary<UIPanelId, GameObject>();

    // 所有面板的根Canvas
    private Canvas canvas;
    // UI输入事件系统
    private EventSystem eventSystem;

    public static UIManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<UIManager>();
            }

            if (instance == null)
            {
                GameObject managerObject = new GameObject(nameof(UIManager));
                instance = managerObject.AddComponent<UIManager>();
            }

            instance.EnsureInitialized();
            return instance;
        }
    }

    public Canvas CanvasRoot
    {
        get
        {
            EnsureInitialized();
            return canvas;
        }
    }

    public EventSystem EventSystem
    {
        get
        {
            EnsureInitialized();
            return eventSystem;
        }
    }

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
        EnsureInitialized();
    }

    private void OnDestroy()
    {
        if (instance == this)
        {
            instance = null;
        }
    }

    /// <summary>
    /// 显示指定面板 若面板尚未创建则自动创建
    /// </summary>
    /// <param name="panelId">面板ID</param>
    public void ShowPanel(UIPanelId panelId)
    {
        GameObject panel = GetPanel(panelId);
        if (panel != null)
        {
            panel.SetActive(true);
        }
    }

    /// <summary>
    /// 隐藏指定面板
    /// </summary>
    /// <param name="panelId">面板ID</param>
    public void HidePanel(UIPanelId panelId)
    {
        if (panelInstanceLookup.TryGetValue(panelId, out GameObject panel) && panel != null)
        {
            panel.SetActive(false);
        }
    }

    /// <summary>
    /// 切换指定面板的显隐状态
    /// </summary>
    /// <param name="panelId">面板ID</param>
    public void TogglePanel(UIPanelId panelId)
    {
        GameObject panel = GetPanel(panelId);
        if (panel != null)
        {
            panel.SetActive(!panel.activeSelf);
        }
    }

    /// <summary>
    /// 隐藏所有已经由UIManager创建和管理的面板
    /// </summary>
    public void HideAllPanels()
    {
        foreach (GameObject panel in panelInstanceLookup.Values)
        {
            if (panel != null)
            {
                panel.SetActive(false);
            }
        }
    }

    /// <summary>
    /// 获取指定面板实例 不存在时自动创建
    /// </summary>
    /// <param name="panelId">面板ID</param>
    /// <returns>面板实例</returns>
    public GameObject GetPanel(UIPanelId panelId)
    {
        EnsureInitialized();

        if (panelInstanceLookup.TryGetValue(panelId, out GameObject panel) && panel != null)
        {
            return panel;
        }

        return CreatePanel(panelId);
    }

    /// <summary>
    /// 确保UI根节点、事件系统和面板预制体缓存均已初始化
    /// </summary>
    private void EnsureInitialized()
    {
        DontDestroyOnLoad(gameObject);
        BuildPanelPrefabLookup();
        EnsureCanvas();
        EnsureEventSystem();
        DestroyLegacyScenePanelInstances();
    }

    /// <summary>
    /// 根据Inspector中配置的绑定关系建立面板预制体查找表
    /// </summary>
    private void BuildPanelPrefabLookup()
    {
        panelPrefabLookup.Clear();

        if (panelBindings == null)
        {
            return;
        }

        for (int i = 0; i < panelBindings.Length; i++)
        {
            PanelBinding binding = panelBindings[i];
            if (binding == null || binding.panelPrefab == null)
            {
                continue;
            }

            panelPrefabLookup[binding.panelId] = binding.panelPrefab;
        }
    }

    /// <summary>
    /// 确保Canvas存在 不存在时使用预制体或默认创建
    /// </summary>
    private void EnsureCanvas()
    {
        if (canvas != null)
        {
            return;
        }

        canvas = canvasPrefab != null ? Instantiate(canvasPrefab) : CreateDefaultCanvas();
        canvas.name = "UI Canvas";
        DontDestroyOnLoad(canvas.gameObject);
    }

    /// <summary>
    /// 确保EventSystem存在 不存在时使用预制体或默认创建
    /// </summary>
    private void EnsureEventSystem()
    {
        if (eventSystem != null)
        {
            return;
        }

        eventSystem = FindObjectOfType<EventSystem>();
        if (eventSystem == null)
        {
            eventSystem = eventSystemPrefab != null ? Instantiate(eventSystemPrefab) : CreateDefaultEventSystem();
        }

        eventSystem.name = "EventSystem";
        DontDestroyOnLoad(eventSystem.gameObject);
    }

    /// <summary>
    /// 创建指定面板实例并挂到Canvas下
    /// </summary>
    /// <param name="panelId">面板ID</param>
    /// <returns>创建出的面板实例</returns>
    private GameObject CreatePanel(UIPanelId panelId)
    {
        if (!panelPrefabLookup.TryGetValue(panelId, out GameObject panelPrefab) || panelPrefab == null)
        {
            return null;
        }

        GameObject panel = Instantiate(panelPrefab, canvas.transform);
        panel.name = panelPrefab.name;
        StretchToCanvas(panel);

        bool showOnCreate = ShouldShowOnCreate(panelId);
        panel.SetActive(showOnCreate);
        panelInstanceLookup[panelId] = panel;
        return panel;
    }

    /// <summary>
    /// 删除场景中旧的同名面板实例 避免和UIManager创建的面板重复
    /// </summary>
    private void DestroyLegacyScenePanelInstances()
    {
        foreach (GameObject panelPrefab in panelPrefabLookup.Values)
        {
            if (panelPrefab == null)
            {
                continue;
            }

            DestroyLoadedSceneObjectsByName(panelPrefab.name);
        }
    }

    /// <summary>
    /// 根据对象名删除当前已加载场景中的旧UI对象
    /// </summary>
    /// <param name="objectName">目标对象名</param>
    private void DestroyLoadedSceneObjectsByName(string objectName)
    {
        RectTransform[] transforms = Resources.FindObjectsOfTypeAll<RectTransform>();
        for (int i = 0; i < transforms.Length; i++)
        {
            RectTransform rectTransform = transforms[i];
            if (rectTransform == null || rectTransform.name != objectName)
            {
                continue;
            }

            GameObject target = rectTransform.gameObject;
            if (!target.scene.IsValid() || !target.scene.isLoaded || IsManagedPanel(target))
            {
                continue;
            }

            Destroy(target);
        }
    }

    /// <summary>
    /// 判断目标面板是否已经由当前UIManager管理
    /// </summary>
    /// <param name="target">待判断对象</param>
    /// <returns>是否属于管理中的面板</returns>
    private bool IsManagedPanel(GameObject target)
    {
        if (canvas != null && target.transform.IsChildOf(canvas.transform))
        {
            return true;
        }

        return panelInstanceLookup.ContainsValue(target);
    }

    /// <summary>
    /// 判断指定面板创建后是否应立即显示
    /// </summary>
    /// <param name="panelId">面板ID</param>
    /// <returns>是否默认显示</returns>
    private bool ShouldShowOnCreate(UIPanelId panelId)
    {
        if (panelBindings == null)
        {
            return false;
        }

        for (int i = 0; i < panelBindings.Length; i++)
        {
            PanelBinding binding = panelBindings[i];
            if (binding != null && binding.panelId == panelId)
            {
                return binding.showOnCreate;
            }
        }

        return false;
    }

    /// <summary>
    /// 创建默认Canvas 作为没有绑定Canvas预制体时的兜底方案
    /// </summary>
    /// <returns>新建的Canvas</returns>
    private static Canvas CreateDefaultCanvas()
    {
        GameObject canvasObject = new GameObject("UI Canvas");
        Canvas newCanvas = canvasObject.AddComponent<Canvas>();
        newCanvas.renderMode = RenderMode.ScreenSpaceOverlay;

        CanvasScaler scaler = canvasObject.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920f, 1080f);
        scaler.matchWidthOrHeight = 0.5f;

        canvasObject.AddComponent<GraphicRaycaster>();
        return newCanvas;
    }

    /// <summary>
    /// 创建默认EventSystem 作为没有绑定EventSystem预制体时的兜底方案
    /// </summary>
    /// <returns>新建的EventSystem</returns>
    private static EventSystem CreateDefaultEventSystem()
    {
        GameObject eventSystemObject = new GameObject("EventSystem");
        EventSystem newEventSystem = eventSystemObject.AddComponent<EventSystem>();
        eventSystemObject.AddComponent<StandaloneInputModule>();
        return newEventSystem;
    }

    /// <summary>
    /// 将面板RectTransform拉伸到填满Canvas
    /// </summary>
    /// <param name="panel">面板对象</param>
    private static void StretchToCanvas(GameObject panel)
    {
        RectTransform rectTransform = panel.GetComponent<RectTransform>();
        if (rectTransform == null)
        {
            return;
        }

        rectTransform.anchorMin = Vector2.zero;
        rectTransform.anchorMax = Vector2.one;
        rectTransform.anchoredPosition = Vector2.zero;
        rectTransform.sizeDelta = Vector2.zero;
        rectTransform.localScale = Vector3.one;
    }
}
