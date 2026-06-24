using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 设置面板Presenter
/// 只负责音效和音乐设置的读取、显示、应用与保存
/// </summary>
[RequireComponent(typeof(Setting_View))]
public class Setting_Presenter : MonoBehaviour
{
    // 设置面板View引用
    private Setting_View view;
    // 回填UI值时屏蔽Toggle/Slider回调 避免初始化时误触发保存逻辑
    private bool suppressViewCallbacks;

    // 当前设置数据
    private static SettingData Settings => SettingDataManager.Instance.Data;

    private void Awake()
    {
        EnsureView();
        ApplySettingsToView();
        ApplySettingsToAudio();
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

    /// <summary>
    /// 音效开关变化时应用到音频管理器
    /// </summary>
    /// <param name="isOn">是否开启音效</param>
    private void OnSoundToggleChanged(bool isOn)
    {
        if (suppressViewCallbacks)
        {
            return;
        }

        Settings.soundEnabled = isOn;
        ApplySettingsToAudio();
    }

    /// <summary>
    /// 音乐开关变化时应用到音频管理器
    /// </summary>
    /// <param name="isOn">是否开启音乐</param>
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

    /// <summary>
    /// 音效音量变化时应用到音频管理器
    /// </summary>
    /// <param name="volume">音量值</param>
    private void OnSoundVolumeChanged(float volume)
    {
        if (suppressViewCallbacks)
        {
            return;
        }

        Settings.soundVolume = Mathf.Clamp01(volume);
        ApplySettingsToAudio();
    }

    /// <summary>
    /// 音乐音量变化时应用到音频管理器
    /// </summary>
    /// <param name="volume">音量值</param>
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
    /// 关闭设置面板前保存设置
    /// </summary>
    private void OnCloseClicked()
    {
        SaveSettings();
        UIManager.Instance.HidePanel(UIPanelId.Setting);
    }

    private void EnsureView()
    {
        if (view == null)
        {
            view = GetComponent<Setting_View>();
        }
    }

    private void AddViewListeners()
    {
        RemoveViewListeners();

        AddToggleListener(view.toggleSound, OnSoundToggleChanged);
        AddToggleListener(view.toggleMusic, OnMusicToggleChanged);
        AddSliderListener(view.sliderSound, OnSoundVolumeChanged);
        AddSliderListener(view.sliderMusic, OnMusicVolumeChanged);
        AddButtonListener(view.buttonBack, OnCloseClicked);
        AddButtonListener(view.buttonExit, OnCloseClicked);
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
        RemoveButtonListener(view.buttonBack, OnCloseClicked);
        RemoveButtonListener(view.buttonExit, OnCloseClicked);
    }

    /// <summary>
    /// 将设置数据回填到View控件
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
