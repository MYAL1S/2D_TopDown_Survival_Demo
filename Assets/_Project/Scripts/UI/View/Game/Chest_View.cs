using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 宝箱结算 View
/// 严格 Passive View：只保存手动绑定的显示控件引用
/// </summary>
public class Chest_View : MonoBehaviour
{
    // 本局获得金币数量文本
    public TextMeshProUGUI coinsAmountLabel;
    // 领取奖励按钮
    public Button takeButton;
}
