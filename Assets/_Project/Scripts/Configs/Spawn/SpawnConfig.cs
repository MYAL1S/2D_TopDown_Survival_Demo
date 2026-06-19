using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 敌人刷怪配置
/// 通过动画曲线定义随游戏进度变化的刷怪压力（速率、最大数量、批量数量）
/// 支持基础敌人资源池和基于时间段的动态敌人资源 在 Inspector 中可视化编辑
/// </summary>
[CreateAssetMenu(fileName = "SpawnConfig", menuName = "ScriptableObjects/SpawnConfig")]
public class SpawnConfig : ScriptableObject
{
    /// <summary>
    /// 时间段敌人资源 在指定时间范围内可用的敌人配置
    /// </summary>
    [Serializable]
    private class TimedEnemyResource
    {
        // 敌人资源的唯一标识
        [SerializeField]
        private string enemyResourceId;

        // 该敌人资源开始可用的时间（秒）
        [SerializeField]
        [Min(0f)]
        private float startTime;

        // 该敌人资源停止可用的时间（秒）
        // 若为 0 则表示无结束时间
        [SerializeField]
        [Min(0f)]
        private float endTime;

        public string EnemyResourceId => enemyResourceId;

        /// <summary>
        /// 判断该敌人资源在指定时间是否活跃（在有效时间范围内）
        /// </summary>
        public bool IsActive(float elapsedTime)
        {
            bool hasEndTime = endTime > 0f;
            return elapsedTime >= startTime && (!hasEndTime || elapsedTime < endTime);
        }
    }

    // 始终可用的敌人资源 ID 列表
    [Header("Enemy Selection")]
    [SerializeField]
    private string[] enemyResourceIds;

    // 在特定时间范围内可用的敌人资源列表
    // 用于实现难度阶段性提升
    [SerializeField]
    private TimedEnemyResource[] timedEnemyResources;

    // 刷怪压力相关参数
    // 随游戏进度动态调整难度
    [Header("Spawn Pressure")]
    // 每秒生成敌人的目标数量
    // 随时间推移而增加
    [SerializeField]
    private AnimationCurve spawnRateByTime = AnimationCurve.Linear(0f, 0.5f, 600f, 6f);

    // 场景中允许同时存在的最大敌人数
    // 随时间推移而增加
    [SerializeField]
    private AnimationCurve maxAliveByTime = AnimationCurve.Linear(0f, 30f, 600f, 150f);

    // 每次批量生成的敌人数量
    // 随时间推移而增加
    [SerializeField]
    private AnimationCurve batchCountByTime = AnimationCurve.Linear(0f, 1f, 600f, 8f);

    // 每帧最多生成的敌人数量
    // 用于控制单帧的性能
    [SerializeField]
    [Min(1)]
    private int maxSpawnPerFrame = 12;

    // 敌人出生点相关参数
    [Header("Spawn Position")]
    // 敌人出生点相对视野边缘的安全边距
    [SerializeField]
    [Min(0.1f)]
    private float spawnMargin = 1.5f;

    // 敌人生成时使用的 Z 轴坐标基准值。
    [SerializeField]
    private float spawnZ = 0f;

    // Z 轴随机偏移范围，最终生成的 Z 值会在 spawnZ 上叠加 [-spawnZOffset, spawnZOffset]
    [SerializeField]
    [Min(0f)]
    private float spawnZOffset = 0.1f;

    // 运行时缓存 当前活跃的敌人资源 ID 列表（合并基础池与时间段敌人）
    private readonly List<string> activeEnemyResourceIds = new List<string>();

    public string[] EnemyResourceIds => enemyResourceIds;
    public int MaxSpawnPerFrame => maxSpawnPerFrame;
    public float SpawnMargin => spawnMargin;
    public float SpawnZ => spawnZ + UnityEngine.Random.Range(-spawnZOffset, spawnZOffset);

    /// <summary>
    /// 根据已经过的时间获取当前活跃的敌人资源 ID 列表
    /// 包括始终可用的资源和当前时间范围内的时间段资源
    /// </summary>
    public IReadOnlyList<string> GetEnemyResourceIds(float elapsedTime)
    {
        activeEnemyResourceIds.Clear();
        AddEnemyResourceIds(enemyResourceIds);

        // 添加当前时间内活跃的时间段敌人资源
        if (timedEnemyResources != null)
        {
            for (int i = 0; i < timedEnemyResources.Length; i++)
            {
                TimedEnemyResource timedEnemyResource = timedEnemyResources[i];
                if (timedEnemyResource == null || !timedEnemyResource.IsActive(elapsedTime))
                {
                    continue;
                }

                AddEnemyResourceId(timedEnemyResource.EnemyResourceId);
            }
        }

        return activeEnemyResourceIds;
    }

    /// <summary>
    /// 根据经过的时间获取当前的刷怪速率（每秒敌人数）
    /// </summary>
    public float GetSpawnRate(float elapsedTime)
    {
        return Mathf.Max(0f, spawnRateByTime.Evaluate(elapsedTime));
    }

    /// <summary>
    /// 根据经过的时间获取当前允许的最大活跃敌人数
    /// </summary>
    public int GetMaxAlive(float elapsedTime)
    {
        return Mathf.Max(1, Mathf.RoundToInt(maxAliveByTime.Evaluate(elapsedTime)));
    }

    /// <summary>
    /// 根据经过的时间获取当前每次批量生成的敌人数量
    /// </summary>
    public int GetBatchCount(float elapsedTime)
    {
        return Mathf.Max(1, Mathf.RoundToInt(batchCountByTime.Evaluate(elapsedTime)));
    }

    /// <summary>
    /// 将指定的敌人资源 ID 列表添加到活跃列表中（去重）
    /// </summary>
    private void AddEnemyResourceIds(IReadOnlyList<string> resourceIds)
    {
        if (resourceIds == null)
        {
            return;
        }

        for (int i = 0; i < resourceIds.Count; i++)
        {
            AddEnemyResourceId(resourceIds[i]);
        }
    }

    /// <summary>
    /// 将单个敌人资源 ID 添加到活跃列表中（若未重复且有效则添加）
    /// </summary>
    private void AddEnemyResourceId(string resourceId)
    {
        if (string.IsNullOrWhiteSpace(resourceId) || activeEnemyResourceIds.Contains(resourceId))
        {
            return;
        }

        activeEnemyResourceIds.Add(resourceId);
    }
}
