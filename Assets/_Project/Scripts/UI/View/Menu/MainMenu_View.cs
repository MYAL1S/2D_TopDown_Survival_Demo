using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 主菜单View
/// 只保存手动绑定的UI控件引用 不处理业务逻辑
/// </summary>
public class MainMenu_View : MonoBehaviour
{
    // 玩家金币文本
    public TextMeshProUGUI textGold;
    // 打开设置面板按钮
    public Button buttonSettings;
    // 当前关卡展示图
    public Image imageStage;
    // 关卡未解锁时显示的锁图标
    public Image imageLock;
    // 当前关卡名称文本
    public TextMeshProUGUI textStageName;
    // 切换到上一关按钮
    public Button buttonLeft;
    // 切换到下一关按钮
    public Button buttonRight;
    // 当前关卡编号文本
    public TextMeshProUGUI textStageNumber;
    // 开始游戏按钮
    public Button buttonPlay;
    // 打开角色选择面板按钮
    public Button buttonCharacters;
}
