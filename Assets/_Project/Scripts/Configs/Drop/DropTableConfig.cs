using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DropTableConfig_", menuName = "ScriptableObjects/DropTableConfig")]
public class DropTableConfig : ScriptableObject
{
    [SerializeField]
    private DropTableEntry[] entries;

    /// <summary>
    /// 掉落结果列表会被清空并填充新的结果
    /// </summary>
    /// <param name="results">掉落结果列表</param>
    public void RollDrops(List<ItemDropRoll> results)
    {
        if (results == null)
        {
            return;
        }

        results.Clear();

        if (entries == null)
        {
            return;
        }

        for (int i = 0; i < entries.Length; i++)
        {
            DropTableEntry entry = entries[i];
            if (entry != null && entry.TryRoll(out ItemDropRoll roll))
            {
                results.Add(roll);
            }
        }
    }
}
