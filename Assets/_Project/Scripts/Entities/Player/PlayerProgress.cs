using System;
using UnityEngine;

/// <summary>
/// 玩家进度系统
/// </summary>
[DisallowMultipleComponent]
public class PlayerProgress : MonoBehaviour
{
    /// <summary>
    /// 当经验值发生变化时触发的事件 参数1: 当前经验值 参数2: 增加的经验值
    /// </summary>
    public event Action<int, int> OnExperienceChanged;
    /// <summary>
    /// 当分数发生变化时触发的事件 参数1: 当前分数 参数2: 增加的分数
    /// </summary>
    public event Action<int, int> OnScoreChanged;

    /// <summary>
    /// 玩家的当前经验值 通过AddExperience方法增加
    /// </summary>
    public int Experience { get; private set; }
    /// <summary>
    /// 玩家的当前分数 通过AddExperience和AddScore方法增加
    /// </summary>
    public int Score { get; private set; }

    /// <summary>
    /// 增加经验值 触发OnExperienceChanged事件
    /// </summary>
    /// <param name="amount">经验值</param>
    public void AddExperience(int amount)
    {
        int addedAmount = Mathf.Max(0, amount);
        if (addedAmount == 0)
        {
            return;
        }

        Experience += addedAmount;
        OnExperienceChanged?.Invoke(Experience, addedAmount);
    }

    /// <summary>
    /// 增加分数 触发OnScoreChanged事件
    /// </summary>
    /// <param name="amount">分数</param>
    public void AddScore(int amount)
    {
        int addedAmount = Mathf.Max(0, amount);
        if (addedAmount == 0)
        {
            return;
        }

        Score += addedAmount;
        OnScoreChanged?.Invoke(Score, addedAmount);
    }

    /// <summary>
    /// 重置玩家的经验值和分数 将它们都设置为0 并触发相应的事件通知UI更新
    /// </summary>
    public void ResetProgress()
    {
        Experience = 0;
        Score = 0;
        OnExperienceChanged?.Invoke(Experience, 0);
        OnScoreChanged?.Invoke(Score, 0);
    }
}
