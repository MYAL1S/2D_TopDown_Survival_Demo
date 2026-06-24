using System;
using System.Collections;
using UnityEngine;

/// <summary>
/// 单个音效播放组件
/// 由 AudioManager 的对象池复用，负责播放一次音效并在播放结束后释放自身
/// </summary>
[RequireComponent(typeof(AudioSource))]
public class SoundEffect : MonoBehaviour, IPoolable
{
    // 实际负责播放音效的 AudioSource
    private AudioSource audioSource;
    // 等待音效播放结束后释放对象的协程
    private Coroutine releaseRoutine;
    // 释放回调，由 AudioManager 传入
    private Action<SoundEffect> releaseCallback;
    // 单个音效配置的音量缩放
    private float volumeScale = 1f;

    private void Awake()
    {
        EnsureAudioSource();
    }

    /// <summary>
    /// 播放指定音效配置。
    /// </summary>
    /// <param name="config">音频配置</param>
    /// <param name="volume">全局音效音量</param>
    /// <param name="onRelease">播放结束后的释放回调</param>
    public void Play(AudioConfig config, float volume, Action<SoundEffect> onRelease)
    {
        if (config == null || config.Clip == null)
        {
            onRelease?.Invoke(this);
            return;
        }

        EnsureAudioSource();
        releaseCallback = onRelease;
        volumeScale = config.VolumeScale;

        audioSource.Stop();
        audioSource.clip = config.Clip;
        audioSource.loop = false;
        audioSource.playOnAwake = false;
        SetVolume(volume);
        audioSource.Play();

        if (releaseRoutine != null)
        {
            StopCoroutine(releaseRoutine);
        }

        releaseRoutine = StartCoroutine(ReleaseAfterClip(config.Clip.length, config.IgnoreTimeScale));
    }

    /// <summary>
    /// 设置当前音效音量
    /// </summary>
    /// <param name="volume">全局音效音量</param>
    public void SetVolume(float volume)
    {
        EnsureAudioSource();
        audioSource.volume = Mathf.Clamp01(volume * volumeScale);
        audioSource.mute = volume <= 0f;
    }

    /// <summary>
    /// 从对象池取出时确保组件引用有效
    /// </summary>
    public void OnSpawnedFromPool()
    {
        EnsureAudioSource();
    }

    /// <summary>
    /// 放回对象池时停止协程和播放，并清理回调与音频片段引用
    /// </summary>
    public void OnReturnedToPool()
    {
        if (releaseRoutine != null)
        {
            StopCoroutine(releaseRoutine);
            releaseRoutine = null;
        }

        releaseCallback = null;
        volumeScale = 1f;
        audioSource.Stop();
        audioSource.clip = null;
        audioSource.mute = false;
    }

    /// <summary>
    /// 确保当前对象带有 AudioSource 组件
    /// </summary>
    private void EnsureAudioSource()
    {
        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
        }

        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
    }

    /// <summary>
    /// 等待音效片段播放结束后释放对象
    /// 暂停期间仍需释放的 UI 音效可使用 realtime 等待；受游戏暂停影响的音效使用普通等待
    /// </summary>
    /// <param name="clipLength">音频片段长度</param>
    /// <param name="ignoreTimeScale">是否忽略 Time.timeScale</param>
    private IEnumerator ReleaseAfterClip(float clipLength, bool ignoreTimeScale)
    {
        float waitTime = Mathf.Max(0.01f, clipLength);
        if (ignoreTimeScale)
        {
            yield return new WaitForSecondsRealtime(waitTime);
        }
        else
        {
            yield return new WaitForSeconds(waitTime);
        }

        releaseRoutine = null;

        Action<SoundEffect> callback = releaseCallback;
        releaseCallback = null;
        callback?.Invoke(this);
    }
}
