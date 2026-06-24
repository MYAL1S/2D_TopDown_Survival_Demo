using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 暂停面板Presenter
/// 负责暂停界面的音频设置、恢复游戏和退出关卡
/// </summary>
[RequireComponent(typeof(Pause_View))]
public class Pause_Presenter : MonoBehaviour
{
    // 点击退出时返回的主菜单场景名
    [SerializeField]
    private string mainMenuSceneName = "MainMenu";

    // 暂停面板View引用
    private Pause_View view;
    // 回填UI值时屏蔽控件回调 防止初始化时触发设置修改
    private bool suppressViewCallbacks;

    // 当前设置数据
    private static SettingData Settings => SettingDataManager.Instance.Data;

    private void Awake()
    {
        EnsureView();
    }

    private void OnEnable()
    {
        EnsureView();
        ApplySettingsToView();
        ApplySettingsToAudio();
        AddViewListeners();
    }

    private void OnDisable()
    {
        RemoveViewListeners();
    }

    private void OnApplicationQuit()
    {
        SaveSettings();
    }

    private void OnSoundToggleChanged(bool isOn)
    {
        if (suppressViewCallbacks)
        {
            return;
        }

        Settings.soundEnabled = isOn;
        ApplySettingsToAudio();
    }

    private void OnMusicToggleChanged(bool isOn)
    {
        if (suppressViewCallbacks)
        {
            return;
        }

        Settings.musicEnabled = isOn;
        ApplySettingsToAudio();
        AudioManager.Instance.PlayDefaultMusic();
    }

    private void OnSoundVolumeChanged(float volume)
    {
        if (suppressViewCallbacks)
        {
            return;
        }

        Settings.soundVolume = Mathf.Clamp01(volume);
        ApplySettingsToAudio();
    }

    private void OnMusicVolumeChanged(float volume)
    {
        if (suppressViewCallbacks)
        {
            return;
        }

        Settings.musicVolume = Mathf.Clamp01(volume);
        ApplySettingsToAudio();
    }

    /// <summary>
    /// 点击返回时保存音频设置并恢复游戏
    /// </summary>
    private void OnBackClicked()
    {
        SaveSettings();
        LevelManager.Instance.ResumeGame();
        UIManager.Instance.HidePanel(UIPanelId.Pause);
    }

    /// <summary>
    /// 点击退出时保存音频设置并离开当前关卡
    /// </summary>
    private void OnExitClicked()
    {
        SaveSettings();
        LevelManager.Instance.ExitLevel(mainMenuSceneName);
    }

    private void EnsureView()
    {
        if (view == null)
        {
            view = GetComponent<Pause_View>();
        }
    }

    private void AddViewListeners()
    {
        RemoveViewListeners();

        AddToggleListener(view.toggleSound, OnSoundToggleChanged);
        AddToggleListener(view.toggleMusic, OnMusicToggleChanged);
        AddSliderListener(view.sliderSound, OnSoundVolumeChanged);
        AddSliderListener(view.sliderMusic, OnMusicVolumeChanged);
        AddButtonListener(view.buttonBack, OnBackClicked);
        AddButtonListener(view.buttonExit, OnExitClicked);
    }

    private void RemoveViewListeners()
    {
        if (view == null)
        {
            return;
        }

        RemoveToggleListener(view.toggleSound, OnSoundToggleChanged);
        RemoveToggleListener(view.toggleMusic, OnMusicToggleChanged);
        RemoveSliderListener(view.sliderSound, OnSoundVolumeChanged);
        RemoveSliderListener(view.sliderMusic, OnMusicVolumeChanged);
        RemoveButtonListener(view.buttonBack, OnBackClicked);
        RemoveButtonListener(view.buttonExit, OnExitClicked);
    }

    /// <summary>
    /// 将设置数据回填到暂停面板控件
    /// </summary>
    private void ApplySettingsToView()
    {
        if (view == null)
        {
            return;
        }

        suppressViewCallbacks = true;
        SetToggle(view.toggleSound, Settings.soundEnabled);
        SetToggle(view.toggleMusic, Settings.musicEnabled);
        SetSlider(view.sliderSound, Settings.soundVolume);
        SetSlider(view.sliderMusic, Settings.musicVolume);
        suppressViewCallbacks = false;
    }

    private static void SaveSettings()
    {
        SettingDataManager.Instance.Save();
    }

    private static void ApplySettingsToAudio()
    {
        AudioManager.Instance.ApplySettings(Settings);
    }

    private static void SetToggle(Toggle toggle, bool value)
    {
        if (toggle != null)
        {
            toggle.isOn = value;
        }
    }

    private static void SetSlider(Slider slider, float value)
    {
        if (slider != null)
        {
            slider.value = Mathf.Clamp01(value);
        }
    }

    private static void AddToggleListener(Toggle toggle, UnityEngine.Events.UnityAction<bool> action)
    {
        if (toggle != null)
        {
            toggle.onValueChanged.AddListener(action);
        }
    }

    private static void RemoveToggleListener(Toggle toggle, UnityEngine.Events.UnityAction<bool> action)
    {
        if (toggle != null)
        {
            toggle.onValueChanged.RemoveListener(action);
        }
    }

    private static void AddSliderListener(Slider slider, UnityEngine.Events.UnityAction<float> action)
    {
        if (slider != null)
        {
            slider.onValueChanged.AddListener(action);
        }
    }

    private static void RemoveSliderListener(Slider slider, UnityEngine.Events.UnityAction<float> action)
    {
        if (slider != null)
        {
            slider.onValueChanged.RemoveListener(action);
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
