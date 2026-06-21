using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 目标系统
/// 主要负责管理游戏中所有敌人的注册与注销 以及提供高效的查询接口来寻找玩家附近的敌人
/// </summary>
public static class TargetingSystem
{
    //定义了空间划分的网格大小
    //整个游戏世界被划分成了一个个 4 x 4 的正方形网格（Cell）
    private const float SpatialCellSize = 4f;

    //存储所有当前存活/已注册的敌人的线性列表
    //初始化容量设为 256 避免前期列表扩容带来的内存分配开销
    private static readonly List<Enemy> registeredEnemies = new List<Enemy>(256);
    //记录每个敌人对应在 registeredEnemies 列表中的索引位置
    //这主要是为了在移除敌人时能迅速找到它在哪 实现 O(1) 的时间复杂度
    private static readonly Dictionary<Enemy, int> enemyIndexes = new Dictionary<Enemy, int>(256);
    //这是空间索引字典  用于根据敌人所在的网格快速查找附近的敌人
    //Key是网格的二维坐标（比如 (0, 1)） Value是处于该网格内的所有敌人的列表
    private static readonly Dictionary<Vector2Int, List<Enemy>> enemiesByCell = new Dictionary<Vector2Int, List<Enemy>>(256);
    //记录上一次重建空间索引时的游戏帧数（Time.frameCount）
    //确保同一帧内无论有多少个武器道具等请求寻找目标 空间索引最多只会重建一次
    private static int spatialIndexFrame = -1;

    public static int RegisteredEnemyCount => registeredEnemies.Count;

    /// <summary>
    /// 将新生成的敌人加入列表和字典 
    /// 并将 spatialIndexFrame 设为 -1 标记当前的空间索引已过期 需要重建
    /// </summary>
    /// <param name="enemy">需要注册的敌人</param>
    public static void RegisterEnemy(Enemy enemy)
    {
        if (enemy == null || enemyIndexes.ContainsKey(enemy))
        {
            return;
        }

        enemyIndexes.Add(enemy, registeredEnemies.Count);
        registeredEnemies.Add(enemy);
        spatialIndexFrame = -1;
    }

    /// <summary>
    /// 通过 enemyIndexes 快速找到敌人的索引
    /// 然后调用内部的 RemoveAt 方法
    /// </summary>
    /// <param name="enemy">需要取消注册的敌人</param>
    public static void UnregisterEnemy(Enemy enemy)
    {
        if (enemy == null || !enemyIndexes.TryGetValue(enemy, out int index))
        {
            return;
        }

        RemoveAt(index);
    }

    /// <summary>
    /// 返回该点附近范围内最近的一个活着的敌人
    /// 如果没有找到则返回null
    /// </summary>
    /// <param name="origin">玩家位置坐标</param>
    /// <param name="range">所搜索的范围</param>
    /// <returns>距离内最近的敌人</returns>
    public static Enemy FindNearestAliveEnemy(Vector3 origin, float range)
    {
        // 如果不限制范围
        // 则全图遍历 registeredEnemies 列表来寻找最近的敌人
        if (range <= 0f)
        {
            return FindNearestAliveEnemyInRegisteredList(origin);
        }

        // 确保本帧网格数据最新
        RebuildSpatialIndexIfNeeded();

        Enemy nearestEnemy = null;
        float maxSqrDistance = range * range;
        float nearestSqrDistance = maxSqrDistance;
        // 通过将 origin 点向左下和右上各偏移 range 的距离 来计算出覆盖搜索范围的最小和最大网格坐标
        Vector2Int minCell = WorldToCell(origin - new Vector3(range, range, 0f));
        Vector2Int maxCell = WorldToCell(origin + new Vector3(range, range, 0f));

        // 只遍历这些覆盖到的网格里的敌人
        for (int x = minCell.x; x <= maxCell.x; x++)
        {
            for (int y = minCell.y; y <= maxCell.y; y++)
            {
                Vector2Int cell = new Vector2Int(x, y);
                if (!enemiesByCell.TryGetValue(cell, out List<Enemy> enemies))
                {
                    continue;
                }

                for (int i = 0; i < enemies.Count; i++)
                {
                    Enemy enemy = enemies[i];
                    if (!IsValidTarget(enemy))
                    {
                        continue;
                    }

                    float sqrDistance = (enemy.transform.position - origin).sqrMagnitude;
                    if (sqrDistance <= nearestSqrDistance)
                    {
                        nearestSqrDistance = sqrDistance;
                        nearestEnemy = enemy;
                    }
                }
            }
        }

        return nearestEnemy;
    }

    /// <summary>
    /// 收集该点附近范围内所有活着的敌人
    /// </summary>
    /// <param name="origin">原点</param>
    /// <param name="range">范围</param>
    /// <param name="results">结果列表</param>
    /// <returns>收集到的敌人数量</returns>
    public static int CollectAliveEnemiesInRange(Vector3 origin, float range, List<Enemy> results)
    {
        return CollectAliveEnemiesInRange(origin, range, results, int.MaxValue);
    }

    public static int CollectAliveEnemiesInRange(Vector3 origin, float range, List<Enemy> results, int maxResults)
    {
        if (results == null)
        {
            return 0;
        }

        results.Clear();
        if (range <= 0f)
        {
            return 0;
        }

        if (maxResults <= 0)
        {
            return 0;
        }

        RebuildSpatialIndexIfNeeded();

        float maxSqrDistance = range * range;
        Vector2Int minCell = WorldToCell(origin - new Vector3(range, range, 0f));
        Vector2Int maxCell = WorldToCell(origin + new Vector3(range, range, 0f));

        for (int x = minCell.x; x <= maxCell.x; x++)
        {
            for (int y = minCell.y; y <= maxCell.y; y++)
            {
                Vector2Int cell = new Vector2Int(x, y);
                if (!enemiesByCell.TryGetValue(cell, out List<Enemy> enemies))
                {
                    continue;
                }

                for (int i = 0; i < enemies.Count; i++)
                {
                    Enemy enemy = enemies[i];
                    if (!IsValidTarget(enemy))
                    {
                        continue;
                    }

                    float sqrDistance = (enemy.transform.position - origin).sqrMagnitude;
                    if (sqrDistance <= maxSqrDistance)
                    {
                        results.Add(enemy);
                        if (results.Count >= maxResults)
                        {
                            return results.Count;
                        }
                    }
                }
            }
        }

        return results.Count;
    }

    /// <summary>
    /// 对全图所有敌人进行暴力遍历
    /// 查找距离该点最近的一个活着的敌人
    /// 该方法不使用空间索引 因此在范围较大时性能较差 但在范围较小时可能更快
    /// 该方法会自动清理已销毁或禁用的敌人对象以保持列表的有效性
    /// </summary>
    /// <param name="origin">玩家位置坐标</param>
    /// <returns></returns>
    private static Enemy FindNearestAliveEnemyInRegisteredList(Vector3 origin)
    {
        Enemy nearestEnemy = null;
        float nearestSqrDistance = float.MaxValue;

        for (int i = registeredEnemies.Count - 1; i >= 0; i--)
        {
            Enemy enemy = registeredEnemies[i];
            if (enemy == null || !enemy.isActiveAndEnabled)
            {
                RemoveAt(i);
                continue;
            }

            if (!enemy.IsAlive)
            {
                continue;
            }

            float sqrDistance = (enemy.transform.position - origin).sqrMagnitude;
            if (sqrDistance <= nearestSqrDistance)
            {
                nearestSqrDistance = sqrDistance;
                nearestEnemy = enemy;
            }
        }

        return nearestEnemy;
    }


    /// <summary>
    /// 检查当前帧是否已经重建过空间索引了 
    /// 如果是就直接返回 否则就重新计算每个敌人所在的网格并更新 enemiesByCell 字典
    /// </summary>
    private static void RebuildSpatialIndexIfNeeded()
    {
        // 计算如果当前帧已经重建过空间索引了 就直接返回 避免重复计算
        if (spatialIndexFrame == Time.frameCount)
        {
            return;
        }

        // 如果不是同一帧 它会清空旧的网格数据 enemiesByCell
        foreach (List<Enemy> enemies in enemiesByCell.Values)
        {
            enemies.Clear();
        }

        // 倒序遍历 registeredEnemies
        for (int i = registeredEnemies.Count - 1; i >= 0; i--)
        {
            Enemy enemy = registeredEnemies[i];
            // 如果敌人对象已经被销毁或禁用 就从 registeredEnemies 列表中移除它 并继续下一次循环
            if (enemy == null || !enemy.isActiveAndEnabled)
            {
                RemoveAt(i);
                continue;
            }

            if (!enemy.IsAlive)
            {
                continue;
            }

            // 通过 WorldToCell 计算出敌人所在的网格坐标
            Vector2Int cell = WorldToCell(enemy.transform.position);
            // 如果 enemiesByCell 中还没有这个网格的列表 就创建一个新的列表并添加到字典中
            if (!enemiesByCell.TryGetValue(cell, out List<Enemy> enemies))
            {
                enemies = new List<Enemy>(8);
                enemiesByCell.Add(cell, enemies);
            }

            // 将敌人添加到对应网格的列表中
            enemies.Add(enemy);
        }

        // 最后将 spatialIndexFrame 更新为当前帧数 以标记空间索引已经是最新的了
        spatialIndexFrame = Time.frameCount;
    }

    /// <summary>
    /// 检查敌人对象是否有效 也就是它不为 null 处于激活状态 并且还活着
    /// </summary>
    /// <param name="enemy">需要检查的敌人</param>
    /// <returns></returns>
    private static bool IsValidTarget(Enemy enemy)
    {
        return enemy != null && enemy.isActiveAndEnabled && enemy.IsAlive;
    }

    /// <summary>
    /// 将世界坐标转换为网格坐标 
    /// 通过将世界坐标除以 SpatialCellSize 并向下取整来计算出该坐标所在的网格的二维坐标
    /// </summary>
    /// <param name="position">世界坐标</param>
    /// <returns></returns>
    private static Vector2Int WorldToCell(Vector3 position)
    {
        return new Vector2Int(
            Mathf.FloorToInt(position.x / SpatialCellSize),
            Mathf.FloorToInt(position.y / SpatialCellSize));
    }

    /// <summary>
    /// 从 registeredEnemies 列表中移除指定索引的敌人
    /// </summary>
    /// <param name="index">要删除的敌人索引</param>
    private static void RemoveAt(int index)
    {
        // 记录要移除的敌人和列表中最后一个敌人
        int lastIndex = registeredEnemies.Count - 1;
        Enemy removedEnemy = registeredEnemies[index];
        Enemy lastEnemy = registeredEnemies[lastIndex];

        // 把列表最后一个元素移动到被删除元素的位置 然后删掉最后一个元素
        registeredEnemies[index] = lastEnemy;
        registeredEnemies.RemoveAt(lastIndex);

        // 如果被删除的敌人不为 null 就从 enemyIndexes 字典中移除它
        if (!ReferenceEquals(removedEnemy, null))
        {
            enemyIndexes.Remove(removedEnemy);
        }

        // 如果被删除的敌人不是列表中最后一个元素 并且最后一个元素不为 null
        // 就更新 enemyIndexes 字典中最后一个敌人的索引为被删除元素的位置
        if (index != lastIndex && !ReferenceEquals(lastEnemy, null))
        {
            enemyIndexes[lastEnemy] = index;
        }

        // 最后将 spatialIndexFrame 设为 -1 标记当前的空间索引已过期 需要重建
        spatialIndexFrame = -1;
    }
}
