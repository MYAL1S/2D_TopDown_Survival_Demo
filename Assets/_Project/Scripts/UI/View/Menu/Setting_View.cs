using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 设置面板View
/// 保存音效、音乐相关控件的手动绑定引用
/// </summary>
public class Setting_View : MonoBehaviour
{
    // 音效开关
    public Toggle toggleSound;
    // 音乐开关
    public Toggle toggleMusic;
    // 音效音量滑条
    public Slider sliderSound;
    // 音乐音量滑条
    public Slider sliderMusic;
    // 返回按钮
    public Button buttonBack;
    // 退出按钮 目前同样用于关闭设置面板
    public Button buttonExit;
}
