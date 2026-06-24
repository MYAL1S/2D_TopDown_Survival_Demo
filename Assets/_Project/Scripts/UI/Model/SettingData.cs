using System;
using UnityEngine;

/// <summary>
/// 音频设置本地数据模型
/// 由 SettingDataManager 持久化，AudioManager 读取后应用到音乐和音效
/// </summary>
[Serializable]
public class SettingData
{
    // 是否启用音效
    public bool soundEnabled = true;
    // 是否启用背景音乐
    public bool musicEnabled = true;
    // 音效基础音量，范围为 0 到 1
    public float soundVolume = 0.5f;
    // 背景音乐基础音量，范围为 0 到 1
    public float musicVolume = 0.5f;
    // 实际音效音量，关闭音效时强制为 0
    public float EffectiveSoundVolume => soundEnabled ? Mathf.Clamp01(soundVolume) : 0f;
    // 实际音乐音量，关闭音乐时强制为 0
    public float EffectiveMusicVolume => musicEnabled ? Mathf.Clamp01(musicVolume) : 0f;

    /// <summary>
    /// 修复设置数据，确保音量字段始终处在有效范围内
    /// </summary>
    public void Repair()
    {
        soundVolume = Mathf.Clamp01(soundVolume);
        musicVolume = Mathf.Clamp01(musicVolume);
    }
}
