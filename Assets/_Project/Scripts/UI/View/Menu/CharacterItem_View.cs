using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 角色选择条目 View。
/// 严格 Passive View：只保存单个角色条目的手动绑定控件引用。
/// </summary>
public class CharacterItem_View : MonoBehaviour
{
    // 角色名称文本。
    public TextMeshProUGUI titleLabel;
    // 角色头像。
    public Image iconImage;
    // 未解锁时显示的价格文本。
    public TextMeshProUGUI costLabel;
    // 价格显示区域根节点。
    public GameObject costRoot;
    // 生命值文本。
    public TextMeshProUGUI hpLabel;
    // 移动速度文本。
    public TextMeshProUGUI speedLabel;
    // 默认武器或能力图标。
    public Image abilityImage;
    // 已解锁时显示的选择状态文本。
    public TextMeshProUGUI selectedLabel;
    // 解锁或选择按钮。
    public Button unlockButton;
}
