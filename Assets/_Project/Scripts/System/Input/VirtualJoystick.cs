using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// 将UI拖拽输入转换为标准化的移动向量
/// </summary>
[DisallowMultipleComponent]
public class VirtualJoystick : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    // 当前场景中可用的摇杆实例，供外部快速访问
    // 主要是提供给InputSystemAdapter读取输入使用
    public static VirtualJoystick ActiveJoystick { get; private set; }

    // UI 绑定引用，由 Inspector 配置或自动回退获取
    [SerializeField] private Canvas canvas;
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private Image backgroundImage;
    [SerializeField] private Image handleImage;
    // 手柄移动距离的缩放系数，控制视觉反馈范围
    [SerializeField] private float handleRange = 0.6f;
    // 死区阈值，小于该值的输入会被视为无效
    [SerializeField] private float deadZone = 0.2f;

    // 背景图像的RectTransform
    private RectTransform backgroundRectTransform;

    // 手柄图像的RectTransform
    private RectTransform handleRectTransform;

    // 提供给外部使用的移动输入
    public Vector2 MoveInput { get; private set; }

    private void Awake()
    {
        // 缓存组件
        if (canvas == null)
        {
            canvas = GetComponentInParent<Canvas>();
        }

        if (canvasGroup == null)
        {
            canvasGroup = GetComponent<CanvasGroup>();
        }

        if (backgroundImage != null)
        {
            backgroundRectTransform = backgroundImage.rectTransform;
        }

        if (handleImage != null)
        {
            handleRectTransform = handleImage.rectTransform;
        }

        // 初始化时把手柄复位到中心
        ResetHandle();
    }

    private void OnEnable()
    {
        // 当前启用的摇杆注册为活动实例，供输入系统读取
        ActiveJoystick = this;
        ResetHandle();
    }

    private void OnDisable()
    {
        // 关闭时清空活动实例，避免外部继续读到失效对象
        if (ActiveJoystick == this)
        {
            ActiveJoystick = null;
        }

        // 关闭时清空输入，防止残留移动向量
        MoveInput = Vector2.zero;
    }

    /// <summary>
    /// 指针按下事件 
    /// </summary>
    /// <param name="eventData"></param>
    public void OnPointerDown(PointerEventData eventData)
    {
        // 按下时直接按拖拽逻辑处理，保证第一次触摸就生效
        OnDrag(eventData);
    }

    /// <summary>
    /// 指针拖动事件
    /// 处理输入转换和手柄位置更新
    /// </summary>
    /// <param name="eventData"></param>
    public void OnDrag(PointerEventData eventData)
    {
        // 背景未绑定时不处理输入
        if (backgroundRectTransform == null)
        {
            return;
        }

        // 根据当前 Canvas 渲染模式选择正确的事件相机
        Camera eventCamera = canvas != null && canvas.renderMode != RenderMode.ScreenSpaceOverlay
            ? canvas.worldCamera
            : null;

        // 把屏幕坐标转换为背景区域内的局部坐标
        if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(
                backgroundRectTransform,
                eventData.position,
                eventCamera,
                out Vector2 localPoint))
        {
            return;
        }

        // 以背景半径为基准，把局部坐标归一化到 -1 ~ 1
        Vector2 radius = backgroundRectTransform.rect.size * 0.5f;
        Vector2 rawInput = new Vector2(
            radius.x > 0f ? localPoint.x / radius.x : 0f,
            radius.y > 0f ? localPoint.y / radius.y : 0f
        );

        // 限制最大长度，避免超出圆形摇杆范围
        rawInput = Vector2.ClampMagnitude(rawInput, 1f);
        // 小于死区的输入忽略，避免轻微抖动导致角色误动
        MoveInput = rawInput.magnitude >= deadZone ? rawInput : Vector2.zero;

        if (handleRectTransform != null)
        {
            // 手柄位置按缩放后的输入值移动，只负责视觉反馈，不影响实际输入值
            handleRectTransform.anchoredPosition = rawInput * radius * handleRange;
        }
    }

    /// <summary>
    /// 指针抬起事件
    /// 复位手柄
    /// </summary>
    /// <param name="eventData"></param>
    public void OnPointerUp(PointerEventData eventData)
    {
        // 松手后清空输入并把手柄归位
        MoveInput = Vector2.zero;
        ResetHandle();
    }

    /// <summary>
    /// 重置手柄位置到背景中心
    /// </summary>
    private void ResetHandle()
    {
        // 如果手柄对象存在，则将其重置到背景中心
        if (handleRectTransform != null)
        {
            handleRectTransform.anchoredPosition = Vector2.zero;
        }
    }
}
