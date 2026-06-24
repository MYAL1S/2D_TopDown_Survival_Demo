using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 宝箱结算面板 Presenter。
/// 负责显示本局金币奖励，并处理领取按钮逻辑。
/// </summary>
[RequireComponent(typeof(Chest_View))]
public class Chest_Presenter : MonoBehaviour
{
    // 领取奖励后返回的主菜单场景名。
    [SerializeField]
    private string mainMenuSceneName = "MainMenu";

    // 宝箱结算 View 引用。
    private Chest_View view;

    private void Awake()
    {
        EnsureView();
    }

    private void OnEnable()
    {
        EnsureView();
        SetCoinsAmount(LevelManager.Instance.Gold);
        AddButtonListener(view.takeButton, OnTakeClicked);
    }

    private void OnDisable()
    {
        EnsureView();
        RemoveButtonListener(view.takeButton, OnTakeClicked);
    }

    /// <summary>
    /// 点击领取按钮后结算金币并返回主菜单。
    /// </summary>
    private void OnTakeClicked()
    {
        LevelManager.Instance.ClaimChestRewardAndReturnToMainMenu(mainMenuSceneName);
    }

    /// <summary>
    /// 刷新宝箱面板中的金币数量文本。
    /// </summary>
    private void SetCoinsAmount(int amount)
    {
        SetText(view.coinsAmountLabel, amount.ToString());
    }

    private void EnsureView()
    {
        if (view == null)
        {
            view = GetComponent<Chest_View>();
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
