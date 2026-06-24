using UnityEngine;

[RequireComponent(typeof(Health))]
[RequireComponent(typeof(InjuredEvent))]
[DisallowMultipleComponent]
/// <summary>
/// 玩家血条显示组件
/// 受伤后显示血条，并通过移动 Healthbar Mask 的 X 轴位置表现血量变化
/// </summary>
public class PlayerHealthBar : MonoBehaviour
{
    // 血条整体根节点，用于控制血条显隐
    [SerializeField]
    private GameObject healthBarRoot;

    // 血条遮罩节点，通过移动该节点的本地 X 坐标实现扣血显示
    [SerializeField]
    private Transform healthbarMaskTransform;

    // 玩家生命组件，用于读取当前血量和最大血量
    private Health health;
    // 受伤事件组件，用于在第一次受伤时显示血条
    private InjuredEvent injuredEvent;
    // 满血时遮罩的原始缩放，用于刷新时恢复宽度
    private Vector3 fullMaskLocalScale;
    // 满血时遮罩的原始位置，作为偏移计算基准
    private Vector3 fullMaskLocalPosition;
    // 满血遮罩在 X 轴上的长度，扣血偏移量会按该长度计算
    private float fullMaskXLength;
    // 记录血条是否已经因为受伤显示过
    private bool hasShownHealthBar;
    // 避免反复覆盖满血基准数据
    private bool hasCachedFullMaskTransform;

    private void Awake()
    {
        health = GetComponent<Health>();

        CacheFullMaskTransform();
    }

    private void OnEnable()
    {
        if (health == null)
        {
            health = GetComponent<Health>();
        }

        injuredEvent = GetComponent<InjuredEvent>();

        if (health != null)
        {
            health.OnHealthChanged += OnHealthChanged;
        }

        if (injuredEvent != null)
        {
            injuredEvent.OnInjured += OnInjured;
        }

        hasShownHealthBar = false;
        SetHealthBarVisible(false);
        RefreshHealthBar();
    }

    private void OnDisable()
    {
        if (health != null)
        {
            health.OnHealthChanged -= OnHealthChanged;
        }

        if (injuredEvent != null)
        {
            injuredEvent.OnInjured -= OnInjured;
        }
    }

    private void OnInjured(InjuredEvent eventSource, int damage)
    {
        hasShownHealthBar = true;
        SetHealthBarVisible(true);
        RefreshHealthBar();
    }

    private void OnHealthChanged(Health changedHealth)
    {
        if (hasShownHealthBar)
        {
            RefreshHealthBar();
        }
    }

    private void RefreshHealthBar()
    {
        if (health == null || healthbarMaskTransform == null || health.MaxHealth <= 0f)
        {
            return;
        }

        CacheFullMaskTransform();

        float percent = Mathf.Clamp01(health.CurrentHealth / health.MaxHealth);
        float missingPercent = 1f - percent;
        float xOffset = fullMaskXLength * missingPercent;

        healthbarMaskTransform.localScale = fullMaskLocalScale;

        Vector3 position = fullMaskLocalPosition;
        position.x = fullMaskLocalPosition.x - xOffset;
        healthbarMaskTransform.localPosition = position;
    }

    /// <summary>
    /// 设置血条根节点显隐
    /// </summary>
    /// <param name="isVisible">是否显示血条</param>
    private void SetHealthBarVisible(bool isVisible)
    {
        if (healthBarRoot != null)
        {
            healthBarRoot.SetActive(isVisible);
        }
    }

    /// <summary>
    /// 缓存满血状态下遮罩的位置和长度，后续扣血时只移动位置不改变缩放
    /// </summary>
    private void CacheFullMaskTransform()
    {
        if (hasCachedFullMaskTransform || healthbarMaskTransform == null)
        {
            return;
        }

        fullMaskLocalScale = healthbarMaskTransform.localScale;
        fullMaskLocalPosition = healthbarMaskTransform.localPosition;
        fullMaskXLength = Mathf.Abs(fullMaskLocalScale.x);
        hasCachedFullMaskTransform = true;
    }
}
