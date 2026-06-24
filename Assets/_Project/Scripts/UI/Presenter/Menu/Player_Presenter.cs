using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 角色选择面板 Presenter。
/// 负责读取玩家数据、维护角色条目列表，并处理选择或购买角色的业务结果。
/// </summary>
[RequireComponent(typeof(Player_View))]
public class Player_Presenter : MonoBehaviour
{
    // 已创建的角色条目 Presenter 缓存，避免每次打开面板都重复实例化。
    private readonly List<CharacterItem_Presenter> characterItems = new List<CharacterItem_Presenter>();

    // 角色面板 View 引用。
    private Player_View view;
    // 当前玩家数据缓存，用于判断角色解锁和选中状态。
    private PlayerData currentPlayerData;

    private void Awake()
    {
        EnsureView();
    }

    private void OnEnable()
    {
        EnsureView();
        AddButtonListener(view.buttonBack, HidePlayerPanel);
        EventCenter.Instance.AddEventListener<PlayerData>(E_EventType.PlayerDataChanged, OnPlayerDataChanged);

        PlayerDataManager.Instance.Load();
        currentPlayerData = PlayerDataManager.Instance.Data;
        RefreshCharacterItems();
    }

    private void OnDisable()
    {
        EnsureView();
        RemoveButtonListener(view.buttonBack, HidePlayerPanel);
        HideCharacterItems();
        EventCenter.Instance.RemoveEventListener<PlayerData>(E_EventType.PlayerDataChanged, OnPlayerDataChanged);
    }

    /// <summary>
    /// 玩家数据变化时刷新角色列表。
    /// </summary>
    /// <param name="playerData">最新玩家数据。</param>
    private void OnPlayerDataChanged(PlayerData playerData)
    {
        currentPlayerData = playerData;
        RefreshCharacterItems();
    }

    /// <summary>
    /// 响应角色条目按钮点击。
    /// 已解锁角色执行选择，未解锁角色尝试购买。
    /// </summary>
    /// <param name="config">角色配置。</param>
    private void OnCharacterItemRequested(PlayerConfig config)
    {
        if (config == null)
        {
            return;
        }

        bool isUnlocked = currentPlayerData != null && currentPlayerData.IsCharacterUnlocked(config.ResourceId);
        bool changed = isUnlocked
            ? PlayerDataManager.Instance.SelectCharacter(config.ResourceId)
            : PlayerDataManager.Instance.TryPurchaseCharacter(config);

        if (changed)
        {
            currentPlayerData = PlayerDataManager.Instance.Data;
            RefreshCharacterItems();
        }
    }

    /// <summary>
    /// 根据 GameResources 中的玩家配置刷新所有角色条目。
    /// </summary>
    private void RefreshCharacterItems()
    {
        if (view == null || view.contentRoot == null || GameResources.Instance == null || GameResources.Instance.PlayerConfigs == null)
        {
            return;
        }

        IReadOnlyList<PlayerConfig> configs = GameResources.Instance.PlayerConfigs;
        EnsureItemCount(configs.Count);

        for (int i = 0; i < characterItems.Count; i++)
        {
            CharacterItem_Presenter item = characterItems[i];
            if (i >= configs.Count)
            {
                item.Hide();
                continue;
            }

            PlayerConfig config = configs[i];
            if (config == null)
            {
                item.Hide();
                continue;
            }

            bool isUnlocked = currentPlayerData != null && currentPlayerData.IsCharacterUnlocked(config.ResourceId);
            bool isSelected = currentPlayerData != null && currentPlayerData.IsCharacterSelected(config.ResourceId);
            bool canAfford = currentPlayerData != null && currentPlayerData.gold >= config.UnlockPrice;

            item.Initialize(config, isUnlocked, isSelected, canAfford, OnCharacterItemRequested);
        }
    }

    /// <summary>
    /// 隐藏所有角色条目，并让条目 Presenter 自行解绑按钮回调。
    /// </summary>
    private void HideCharacterItems()
    {
        for (int i = 0; i < characterItems.Count; i++)
        {
            if (characterItems[i] != null)
            {
                characterItems[i].Hide();
            }
        }
    }

    /// <summary>
    /// 确保角色条目数量足够显示全部配置。
    /// </summary>
    /// <param name="count">需要的条目数量。</param>
    private void EnsureItemCount(int count)
    {
        while (characterItems.Count < count)
        {
            CharacterItem_Presenter item = CreateCharacterItem();
            if (item == null)
            {
                return;
            }

            characterItems.Add(item);
        }
    }

    /// <summary>
    /// 创建单个角色条目实例，并确保其拥有条目 Presenter。
    /// </summary>
    /// <returns>角色条目 Presenter。</returns>
    private CharacterItem_Presenter CreateCharacterItem()
    {
        if (view.characterItemPrefab == null || view.contentRoot == null)
        {
            return null;
        }

        GameObject itemObject = Instantiate(view.characterItemPrefab, view.contentRoot);
        CharacterItem_Presenter item = itemObject.GetComponent<CharacterItem_Presenter>();
        if (item == null)
        {
            item = itemObject.AddComponent<CharacterItem_Presenter>();
        }

        return item;
    }

    /// <summary>
    /// 隐藏角色选择面板。
    /// </summary>
    private void HidePlayerPanel()
    {
        UIManager.Instance.HidePanel(UIPanelId.Player);
    }

    /// <summary>
    /// 确保 View 引用存在。
    /// </summary>
    private void EnsureView()
    {
        if (view == null)
        {
            view = GetComponent<Player_View>();
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
