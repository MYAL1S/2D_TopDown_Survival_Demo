using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 敌人AI系统 负责控制敌人的移动和行为
/// </summary>
[RequireComponent(typeof(Rigidbody2D))]
[DisallowMultipleComponent]
public class EnemyAISystem : MonoBehaviour
{
    [SerializeField]
    [Min(0f)]
    [Tooltip("the distance that enemy will stop to pursue the target.")]
    private float stopDistance = 0.15f;

    [SerializeField]
    [Min(0f)]
    [Tooltip("the minimum distance between enemies, if too close will apply separation force.")]
    private float separationDistance = 0.1f;

    [SerializeField]
    [Min(0f)]
    [Tooltip("the weight coefficient of the separation force applied to the pursuit direction.")]
    private float separationWeight = 1.25f;

    [SerializeField]
    [Min(0.02f)]
    [Tooltip("how often this enemy recalculates separation. Staggered per instance.")]
    private float separationUpdateInterval = 0.1f;

    [SerializeField]
    [Min(1)]
    private int maxSeparationChecks = 12;

    private readonly List<Enemy> separationCandidates = new List<Enemy>(16);

    private Enemy enemy;
    private Rigidbody2D rigidBody2D;
    private SpriteRenderer spriteRenderer;
    private Vector2 cachedSeparationDirection;
    private float nextSeparationUpdateTime;

    private void Awake()
    {
        enemy = GetComponent<Enemy>();
        rigidBody2D = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();

        rigidBody2D.gravityScale = 0f;
        rigidBody2D.freezeRotation = true;
        rigidBody2D.interpolation = RigidbodyInterpolation2D.Interpolate;
    }

    private void OnEnable()
    {
        cachedSeparationDirection = Vector2.zero;
        nextSeparationUpdateTime = Time.time + GetStaggeredInitialDelay();
    }

    private void FixedUpdate()
    {
        // 如果敌人没有目标或者目标已经死亡 则停止移动
        Transform target = enemy != null ? enemy.Target : null;
        if (enemy == null || !enemy.IsAlive || target == null)
        {
            rigidBody2D.velocity = Vector2.zero;
            return;
        }

        // 计算敌人移动方向
        Vector2 offset = target.position - transform.position;
        // 如果敌人距离目标小于停止距离 则不再移动
        Vector2 targetDirection = offset.sqrMagnitude > stopDistance * stopDistance
            ? offset.normalized
            : Vector2.zero;

        // 计算分离方向
        Vector2 separationDirection = GetSeparationDirection();
        Vector2 moveDirection = targetDirection + separationDirection * separationWeight;
        if (moveDirection.sqrMagnitude > 1f)
        {
            moveDirection.Normalize();
        }

        rigidBody2D.velocity = moveDirection * enemy.MoveSpeed;

        if (spriteRenderer != null && moveDirection.x != 0f)
        {
            spriteRenderer.flipX = moveDirection.x < 0f;
        }
    }

    /// <summary>
    /// 计算敌人分离方向
    /// </summary>
    /// <returns></returns>
    private Vector2 GetSeparationDirection()
    {
        // 如果分离距离或分离权重为0 则不需要计算分离方向
        if (separationDistance <= 0f || separationWeight <= 0f)
        {
            cachedSeparationDirection = Vector2.zero;
            return cachedSeparationDirection;
        }

        // 如果当前时间小于下次分离更新的时间 则返回缓存的分离方向
        if (Time.time < nextSeparationUpdateTime)
        {
            return cachedSeparationDirection;
        }

        // 计算分离方向 并缓存结果
        nextSeparationUpdateTime = Time.time + Mathf.Max(0.02f, separationUpdateInterval);
        cachedSeparationDirection = CalculateSeparationDirectionFromSpatialGrid();
        return cachedSeparationDirection;
    }

    /// <summary>
    /// 计算敌人分离方向 使用空间网格优化
    /// </summary>
    /// <returns></returns>
    private Vector2 CalculateSeparationDirectionFromSpatialGrid()
    {
        // 使用TargetingSystem收集范围内的敌人
        Vector2 currentPosition = transform.position;
        TargetingSystem.CollectAliveEnemiesInRange(
            currentPosition,
            separationDistance,
            separationCandidates,
            maxSeparationChecks + 1);

        Vector2 separationDirection = Vector2.zero;
        int checkedEnemies = 0;

        // 遍历收集到的敌人 并计算分离方向
        for (int i = 0; i < separationCandidates.Count && checkedEnemies < maxSeparationChecks; i++)
        {
            // 排除自身和无效敌人
            Enemy otherEnemy = separationCandidates[i];
            if (otherEnemy == null || otherEnemy == enemy || !otherEnemy.IsAlive)
            {
                continue;
            }

            // 计算分离方向
            checkedEnemies++;
            Vector2 away = currentPosition - (Vector2)otherEnemy.transform.position;
            float sqrDistance = away.sqrMagnitude;
            if (sqrDistance <= 0.0001f)
            {
                away = GetDeterministicSeparationDirection(otherEnemy);
                sqrDistance = 0.0001f;
            }

            // 计算距离和强度
            float distance = Mathf.Sqrt(sqrDistance);
            // 计算强度 根据距离和分离距离进行归一化
            float strength = 1f - Mathf.Clamp01(distance / separationDistance);
            separationDirection += away.normalized * strength;
        }

        return separationDirection;
    }

    /// <summary>
    /// 计算每个敌人的初始延迟
    /// 以避免所有敌人同时更新分离方向
    /// </summary>
    /// <returns></returns>
    private float GetStaggeredInitialDelay()
    {
        float interval = Mathf.Max(0.02f, separationUpdateInterval);
        // 通过使用实例ID生成一个稳定的随机数来计算初始延迟
        int stableId = Mathf.Abs(GetInstanceID());
        return (stableId % 1000) / 1000f * interval;
    }

    /// <summary>
    /// 计算两个敌人之间的确定性分离方向
    /// </summary>
    /// <param name="otherEnemy">另一个敌人</param>
    /// <returns></returns>
    private Vector2 GetDeterministicSeparationDirection(Enemy otherEnemy)
    {
        // 通过使用两个敌人的实例ID生成一个稳定的随机方向
        int hash = enemy.GetInstanceID() ^ otherEnemy.GetInstanceID();
        float angle = (hash & 1023) * (Mathf.PI * 2f / 1024f);
        return new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
    }
}
