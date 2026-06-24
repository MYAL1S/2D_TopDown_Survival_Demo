using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// 全局音频管理器
/// 负责背景音乐播放、音效播放、音量设置应用，以及音效对象池复用
/// </summary>
public class AudioManager : MonoBehaviour
{
    // 默认预热的音效对象数量
    private const int DefaultSoundEffectPoolSize = 16;

    private static AudioManager instance;

    // 背景音乐使用的 AudioSource
    private AudioSource musicSource;
    // 音效对象池，避免频繁创建和销毁 GameObject
    private ComponentPool<SoundEffect> soundEffectPool;
    // 当前正在播放的音效集合，用于设置变化时同步音量
    private readonly HashSet<SoundEffect> activeSoundEffects = new HashSet<SoundEffect>();
    // 正在播放的音效根节点
    private Transform activeSoundRoot;
    // 空闲音效对象根节点
    private Transform inactiveSoundRoot;
    // 当前应用中的音频设置
    private SettingData currentSettings = new SettingData();
    // 当前背景音乐配置自身的音量缩放
    private float musicVolumeScale = 1f;

    public static AudioManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<AudioManager>();
            }

            if (instance == null)
            {
                GameObject managerObject = new GameObject(nameof(AudioManager));
                instance = managerObject.AddComponent<AudioManager>();
            }

            return instance;
        }
    }

    // 当前音频设置数据
    public SettingData CurrentSettings => currentSettings;
    // 设置与静音状态共同计算后的音效音量
    public float EffectiveSoundVolume => currentSettings.EffectiveSoundVolume;
    // 设置与静音状态共同计算后的音乐音量
    public float EffectiveMusicVolume => currentSettings.EffectiveMusicVolume;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
        EnsureMusicSource();
        EnsureSoundEffectPool();
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnDestroy()
    {
        if (instance == this)
        {
            instance = null;
        }
    }

    /// <summary>
    /// 应用音频设置，并同步到当前音乐和正在播放的音效
    /// </summary>
    /// <param name="settings">音频设置数据</param>
    public void ApplySettings(SettingData settings)
    {
        if (settings == null)
        {
            settings = new SettingData();
        }

        settings.Repair();
        currentSettings = settings;

        EnsureMusicSource();
        ApplyMusicSettings();
        ApplySoundEffectSettings();
    }

    /// <summary>
    /// 播放 GameResources 中配置的默认背景音乐
    /// </summary>
    public void PlayDefaultMusic()
    {
        if (GameResources.Instance == null)
        {
            return;
        }

        PlayMusic(GameResources.Instance.GetDefaultMusicConfig());
    }

    /// <summary>
    /// 按音频 id 播放背景音乐
    /// </summary>
    /// <param name="audioId">音频配置 id</param>
    public void PlayMusic(string audioId)
    {
        if (GameResources.Instance == null)
        {
            return;
        }

        PlayMusic(GameResources.Instance.GetAudioConfig(audioId));
    }

    /// <summary>
    /// 按音频 id 播放一次音效
    /// </summary>
    /// <param name="audioId">音频配置 id</param>
    public void PlaySound(string audioId)
    {
        if (GameResources.Instance == null)
        {
            return;
        }

        PlaySound(GameResources.Instance.GetAudioConfig(audioId));
    }

    /// <summary>
    /// 停止当前背景音乐并清空音乐片段
    /// </summary>
    public void StopMusic()
    {
        EnsureMusicSource();
        musicSource.Stop();
        musicSource.clip = null;
        musicVolumeScale = 1f;
        ApplyMusicSettings();
    }

    /// <summary>
    /// 按音频配置播放背景音乐
    /// </summary>
    /// <param name="config">音乐配置</param>
    private void PlayMusic(AudioConfig config)
    {
        EnsureMusicSource();

        if (config == null || config.Clip == null || config.AudioType != AudioConfigType.Music)
        {
            StopMusic();
            return;
        }

        bool clipChanged = musicSource.clip != config.Clip;
        musicVolumeScale = config.VolumeScale;
        musicSource.clip = config.Clip;
        musicSource.loop = config.Loop;
        ApplyMusicSettings();

        if (clipChanged || !musicSource.isPlaying)
        {
            musicSource.Play();
        }
    }

    /// <summary>
    /// 按音频配置播放一次音效
    /// </summary>
    /// <param name="config">音效配置</param>
    private void PlaySound(AudioConfig config)
    {
        if (config == null || config.Clip == null || config.AudioType != AudioConfigType.SoundEffect)
        {
            return;
        }

        if (EffectiveSoundVolume <= 0f)
        {
            return;
        }

        EnsureSoundEffectPool();
        SoundEffect soundEffect = soundEffectPool.Get(Vector3.zero, Quaternion.identity, activeSoundRoot);
        activeSoundEffects.Add(soundEffect);
        soundEffect.Play(config, EffectiveSoundVolume, ReleaseSoundEffect);
    }

    /// <summary>
    /// 确保背景音乐 AudioSource 存在
    /// </summary>
    private void EnsureMusicSource()
    {
        if (musicSource != null)
        {
            return;
        }

        AudioSource[] sources = GetComponents<AudioSource>();
        musicSource = sources.Length > 0 ? sources[0] : gameObject.AddComponent<AudioSource>();
        musicSource.playOnAwake = false;
        musicSource.loop = true;
    }

    /// <summary>
    /// 确保音效对象池存在，并在首次创建时预热
    /// </summary>
    private void EnsureSoundEffectPool()
    {
        if (soundEffectPool != null)
        {
            return;
        }

        activeSoundRoot = CreateChildRoot("Active Sound Effects");
        inactiveSoundRoot = CreateChildRoot("Inactive Sound Effects");

        GameObject soundEffectPrefab = CreateSoundEffectPrefab();
        soundEffectPool = new ComponentPool<SoundEffect>(
            soundEffectPrefab,
            inactiveSoundRoot,
            EnsureSoundEffectComponents);

        PrewarmSoundEffectPool(DefaultSoundEffectPoolSize);
    }

    /// <summary>
    /// 创建音效对象池使用的运行时模板对象
    /// </summary>
    /// <returns>带有 SoundEffect 和 AudioSource 的模板对象</returns>
    private GameObject CreateSoundEffectPrefab()
    {
        GameObject prefab = new GameObject("SoundEffect");
        prefab.transform.SetParent(inactiveSoundRoot);
        prefab.SetActive(false);
        EnsureSoundEffectComponents(prefab);
        return prefab;
    }

    /// <summary>
    /// 确保音效对象上具备播放所需组件
    /// </summary>
    /// <param name="instanceObject">音效对象</param>
    /// <returns>音效播放组件。</returns>
    private SoundEffect EnsureSoundEffectComponents(GameObject instanceObject)
    {
        AudioSource source = instanceObject.GetComponent<AudioSource>();
        if (source == null)
        {
            source = instanceObject.AddComponent<AudioSource>();
        }

        source.playOnAwake = false;
        source.loop = false;

        SoundEffect soundEffect = instanceObject.GetComponent<SoundEffect>();
        if (soundEffect == null)
        {
            soundEffect = instanceObject.AddComponent<SoundEffect>();
        }

        return soundEffect;
    }

    /// <summary>
    /// 预热音效对象池
    /// </summary>
    /// <param name="count">预创建的音效对象数量</param>
    private void PrewarmSoundEffectPool(int count)
    {
        for (int i = 0; i < count; i++)
        {
            SoundEffect soundEffect = soundEffectPool.Get(Vector3.zero, Quaternion.identity, activeSoundRoot);
            soundEffectPool.Release(soundEffect);
        }
    }

    /// <summary>
    /// 创建一个子节点作为音效对象组织根节点
    /// </summary>
    /// <param name="rootName">根节点名称</param>
    /// <returns>根节点 Transform。</returns>
    private Transform CreateChildRoot(string rootName)
    {
        GameObject rootObject = new GameObject(rootName);
        rootObject.transform.SetParent(transform);
        rootObject.transform.localPosition = Vector3.zero;
        rootObject.transform.localRotation = Quaternion.identity;
        rootObject.transform.localScale = Vector3.one;
        return rootObject.transform;
    }

    /// <summary>
    /// 将音效释放回对象池
    /// </summary>
    /// <param name="soundEffect">音效播放组件</param>
    private void ReleaseSoundEffect(SoundEffect soundEffect)
    {
        if (soundEffectPool == null || soundEffect == null)
        {
            return;
        }

        activeSoundEffects.Remove(soundEffect);
        soundEffectPool.Release(soundEffect);
    }

    /// <summary>
    /// 应用当前背景音乐音量和静音状态
    /// </summary>
    private void ApplyMusicSettings()
    {
        EnsureMusicSource();
        musicSource.volume = EffectiveMusicVolume * musicVolumeScale;
        musicSource.mute = !currentSettings.musicEnabled;
    }

    /// <summary>
    /// 将当前音效音量同步到正在播放的音效对象
    /// </summary>
    private void ApplySoundEffectSettings()
    {
        foreach (SoundEffect soundEffect in activeSoundEffects)
        {
            if (soundEffect != null)
            {
                soundEffect.SetVolume(EffectiveSoundVolume);
            }
        }
    }

    /// <summary>
    /// 场景加载完成后重新应用音乐设置并播放默认音乐
    /// </summary>
    /// <param name="scene">已加载场景</param>
    /// <param name="mode">场景加载模式</param>
    private static void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Instance.ApplyMusicSettings();
        Instance.PlayDefaultMusic();
    }
}
