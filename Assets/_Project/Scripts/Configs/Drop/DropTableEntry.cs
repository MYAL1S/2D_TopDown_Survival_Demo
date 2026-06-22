using System;
using UnityEngine;

/// <summary>
/// 掉落表条目类 
/// 用于定义单个掉落项的配置 包含掉落物品、掉落概率和数量范围等信息
/// </summary>
[Serializable]
public class DropTableEntry
{
    [SerializeField]
    private ItemConfig itemConfig;

    [SerializeField]
    [Range(0f, 1f)]
    private float dropChance = 1f;

    [SerializeField]
    [Min(1)]
    private int minAmount = 1;

    [SerializeField]
    [Min(1)]
    private int maxAmount = 1;

    public ItemConfig ItemConfig => itemConfig;
    public float DropChance => dropChance;
    public int MinAmount => Mathf.Max(1, minAmount);
    public int MaxAmount => Mathf.Max(MinAmount, maxAmount);

    /// <summary>
    /// 尝试进行一次掉落判定 
    /// 根据配置的掉落概率和数量范围生成一个掉落结果
    /// </summary>
    /// <param name="roll">掉落物</param>
    /// <returns></returns>
    public bool TryRoll(out ItemDropRoll roll)
    {
        roll = default;

        if (itemConfig == null || UnityEngine.Random.value > dropChance)
        {
            return false;
        }

        int amount = UnityEngine.Random.Range(MinAmount, MaxAmount + 1);
        roll = new ItemDropRoll(itemConfig, amount);
        return true;
    }
}
