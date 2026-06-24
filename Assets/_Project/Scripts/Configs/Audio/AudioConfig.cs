using UnityEngine;

/// <summary>
/// 音频配置类型
/// </summary>
public enum AudioConfigType
{
    // 背景音乐
    Music,
    // 一次性音效
    SoundEffect,
}

/// <summary>
/// 音频配置文件
/// 通过 ResourceId 缓存在 GameResources 中，外部只需要传入 id 即可播放对应音乐或音效
/// </summary>
[CreateAssetMenu(fileName = "AudioConfig_", menuName = "ScriptableObjects/AudioConfig")]
public class AudioConfig : ScriptableObject
{
    // 稳定资源 id，用于 AudioManager 和其他系统按 id 查找音频配置
    [SerializeField]
    [Tooltip("Stable id used by runtime systems to request this audio.")]
    private string resourceId;

    // 音频类型，决定该配置由音乐通道还是音效对象池播放
    [SerializeField]
    private AudioConfigType audioType = AudioConfigType.SoundEffect;

    // 实际播放的音频片段
    [SerializeField]
    private AudioClip clip;

    // 当前配置自身的音量缩放，会与全局音量设置相乘
    [SerializeField]
    [Range(0f, 1f)]
    private float volumeScale = 1f;

    // 是否循环播放，音乐类型会强制循环
    [SerializeField]
    private bool loop;

    // 是否忽略 Time.timeScale，适合暂停界面仍需正常释放的 UI 音效
    [SerializeField]
    [Tooltip("Use realtime for release timing. Enable this for UI sounds that should work while the game is paused.")]
    private bool ignoreTimeScale;

    // 资源 id 对外只读，避免运行时修改配置表键值
    public string ResourceId => resourceId;
    // 音频类型对外只读
    public AudioConfigType AudioType => audioType;
    // 音频片段对外只读
    public AudioClip Clip => clip;
    // 配置音量缩放对外只读
    public float VolumeScale => volumeScale;
    // 音乐始终循环，音效按配置决定
    public bool Loop => loop || audioType == AudioConfigType.Music;
    // 是否忽略时间缩放对外只读
    public bool IgnoreTimeScale => ignoreTimeScale;
}
