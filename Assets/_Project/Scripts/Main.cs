using UnityEngine;

/// <summary>
/// 主菜单场景入口
/// 负责在游戏启动时初始化玩家数据、打开主菜单面板并启动默认背景音乐
/// </summary>
public class Main : MonoBehaviour
{
    private void Awake()
    {
        // 提前触发玩家数据管理器初始化，保证 UI 打开时可以直接读取本地数据
        _ = PlayerDataManager.Instance;
    }

    private void Start()
    {
        // UIManager 首次调用时会创建并缓存 Canvas 与 EventSystem
        UIManager.Instance.ShowPanel(UIPanelId.MainMenu);
        // 读取本地音频设置并应用到 AudioManager
        AudioManager.Instance.ApplySettings(SettingDataManager.Instance.Data);
        // 主菜单显示后开始播放默认背景音乐
        AudioManager.Instance.PlayDefaultMusic();
    }
}
