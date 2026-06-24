using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 角色选择条目 Presenter。
/// 负责渲染单个角色条目，并把点击事件回传给 Player_Presenter。
/// </summary>
[RequireComponent(typeof(CharacterItem_View))]
public class CharacterItem_Presenter : MonoBehaviour
{
    // 当前条目绑定的 View。
    private CharacterItem_View view;
    // 当前条目对应的角色配置。
    private PlayerConfig config;
    // 点击解锁或选择按钮时回调给父级 Presenter。
    private Action<PlayerConfig> actionRequested;

    private void Awake()
    {
        EnsureView();
    }

    private void OnDestroy()
    {
        RemoveListeners();
    }

    /// <summary>
    /// 初始化并刷新角色条目。
    /// </summary>
    /// <param name="playerConfig">角色配置。</param>
    /// <param name="isUnlocked">是否已解锁。</param>
    /// <param name="isSelected">是否为当前选中角色。</param>
    /// <param name="canAfford">金币是否足够购买。</param>
    /// <param name="onActionRequested">按钮点击回调。</param>
    public void Initialize(PlayerConfig playerConfig, bool isUnlocked, bool isSelected, bool canAfford, Action<PlayerConfig> onActionRequested)
    {
        RemoveListeners();

        config = playerConfig;
        actionRequested = onActionRequested;

        Refresh(isUnlocked, isSelected, canAfford);
        AddListeners();
    }

    /// <summary>
    /// 隐藏当前条目并解绑按钮回调。
    /// </summary>
    public void Hide()
    {
        RemoveListeners();
        gameObject.SetActive(false);
        config = null;
        actionRequested = null;
    }

    /// <summary>
    /// 根据解锁、选中和金币状态刷新条目显示。
    /// </summary>
    /// <param name="isUnlocked">是否已解锁。</param>
    /// <param name="isSelected">是否为当前选中角色。</param>
    /// <param name="canAfford">金币是否足够购买。</param>
    private void Refresh(bool isUnlocked, bool isSelected, bool canAfford)
    {
        EnsureView();

        if (config == null)
        {
            Hide();
            return;
        }

        gameObject.SetActive(true);
        SetText(view.titleLabel, config.playerCharacterName);
        SetText(view.costLabel, config.UnlockPrice.ToString());
        SetText(view.selectedLabel, isSelected ? "Selected" : "Available");
        SetText(view.hpLabel, Mathf.RoundToInt(config.MaxHealth).ToString());
        SetText(view.speedLabel, config.MoveSpeed.ToString("0.#"));
        SetImage(view.iconImage, config.CharacterIcon);
        SetImage(view.abilityImage, config.DefaultWeaponIcon);
        SetVisible(view.costRoot, !isUnlocked);
        SetVisible(view.selectedLabel != null ? view.selectedLabel.gameObject : null, isUnlocked);

        if (view.unlockButton != null)
        {
            view.unlockButton.gameObject.SetActive(true);
            view.unlockButton.interactable = isUnlocked ? !isSelected : canAfford;
        }
    }

    /// <summary>
    /// 点击按钮后把当前角色配置交给父级 Presenter 处理。
    /// </summary>
    private void OnActionButtonClicked()
    {
        actionRequested?.Invoke(config);
    }

    private void AddListeners()
    {
        EnsureView();

        if (view.unlockButton != null)
        {
            view.unlockButton.onClick.AddListener(OnActionButtonClicked);
        }
    }

    private void RemoveListeners()
    {
        EnsureView();

        if (view.unlockButton != null)
        {
            view.unlockButton.onClick.RemoveListener(OnActionButtonClicked);
        }
    }

    private void EnsureView()
    {
        if (view == null)
        {
            view = GetComponent<CharacterItem_View>();
        }
    }

    private static void SetText(TextMeshProUGUI label, string text)
    {
        if (label != null)
        {
            label.text = text;
        }
    }

    private static void SetImage(Image image, Sprite sprite)
    {
        if (image == null)
        {
            return;
        }

        image.sprite = sprite;
        image.enabled = sprite != null;
    }

    private static void SetVisible(GameObject target, bool visible)
    {
        if (target != null)
        {
            target.SetActive(visible);
        }
    }
}
