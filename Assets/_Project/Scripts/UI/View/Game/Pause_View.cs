using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 暂停面板View
/// 保存暂停界面上的音频设置控件和操作按钮引用
/// </summary>
public class Pause_View : MonoBehaviour
{
    // 音效开关
    public Toggle toggleSound;
    // 音乐开关
    public Toggle toggleMusic;
    // 音效音量滑条
    public Slider sliderSound;
    // 音乐音量滑条
    public Slider sliderMusic;
    // 返回游戏按钮
    public Button buttonBack;
    // 退出关卡按钮
    public Button buttonExit;
}
