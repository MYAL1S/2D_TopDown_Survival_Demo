using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 游戏面板 View
/// </summary>
public class Game_View : MonoBehaviour
{
    // 本局金币文本
    public TextMeshProUGUI goldText;
    // 本局击杀数文本
    public TextMeshProUGUI enemiesKilledText;
    // 关卡剩余时间文本
    public TextMeshProUGUI timerText;
    // 暂停按钮
    public Button pauseButton;
}
