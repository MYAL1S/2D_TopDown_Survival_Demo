using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ProjectileController 是一个组件类 
/// 负责管理单个投射物的行为和生命周期
/// </summary>
[RequireComponent(typeof(Rigidbody2D))]
[DisallowMultipleComponent]
public class ProjectileController : MonoBehaviour, IPoolable
{
    [SerializeField]
    [Tooltip("Only colliders on these layers can be damaged by projectiles.")]
    private LayerMask enemyLayerMask;

    private readonly Dictionary<Enemy, float> enemyHitTimes = new Dictionary<Enemy, float>(16);
    private readonly List<Enemy> areaTargets = new List<Enemy>(32);

    private Enemy target;
    private Rigidbody2D rigidBody2D;
    private Action<ProjectileController> releaseAction;
    private Vector3 spawnPosition;
    private Vector2 moveDirection;
    private float damage;
    private float speed;
    private float maxTravelDistance;
    private float hitRadius;
    private bool isLaunched;
    private WeaponConfig weaponConfig;

    public WeaponConfig WeaponConfig => weaponConfig;
    public float Damage => damage;
    public ProjectileHitStrategy HitStrategy => weaponConfig != null ? weaponConfig.ProjectileHitStrategy : null;

    private void Awake()
    {
        rigidBody2D = GetComponent<Rigidbody2D>();
        EnsureEnemyLayerMask();
        ConfigureRigidbody();
        ConfigureCollider();
    }

    private void FixedUpdate()
    {
        // 如果投射物尚未发射 则不执行任何操作
        if (!isLaunched)
        {
            return;
        }

        // 获取当前武器配置的命中策略 
        ProjectileHitStrategy hitStrategy = HitStrategy;
        // 如果没有配置 或者没有命中策略 则记录错误并释放投射物
        if (weaponConfig == null || hitStrategy == null)
        {
            Debug.LogError($"{nameof(ProjectileController)} requires a {nameof(ProjectileHitStrategy)} on its weapon config.", this);
            ReleaseProjectile();
            return;
        }

        // 如果命中策略要求必须有一个存活的目标 但当前目标不存在或已死亡 则释放投射物
        if (hitStrategy.RequiresAliveTarget && (target == null || !target.IsAlive))
        {
            ReleaseProjectile();
            return;
        }

        // 如果命中策略使用追踪目标 且当前有一个有效目标 
        if (hitStrategy.UsesHomingTarget && target != null)
        {
            // 计算投射物与目标之间的距离
            Vector2 toTarget = target.transform.position - transform.position;
            // 如果距离小于等于命中半径 则处理命中并返回
            if (toTarget.sqrMagnitude <= hitRadius * hitRadius)
            {
                hitStrategy.HandleHit(this, target);
                return;
            }

            // 重新计算移动方向 使投射物朝向目标移动
            moveDirection = toTarget.normalized;
        }

        rigidBody2D.velocity = moveDirection * speed;

        // 计算投射物从出生位置到当前位置的平方距离 如果超过最大飞行距离 则释放投射物
        float traveledSqrDistance = (transform.position - spawnPosition).sqrMagnitude;
        if (traveledSqrDistance >= maxTravelDistance * maxTravelDistance)
        {
            ReleaseProjectile();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        TryHitCollider(other);
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        TryHitCollider(other);
    }

    /// <summary>
    /// 尝试处理与碰撞器的接触 
    /// </summary>
    /// <param name="other"></param>
    private void TryHitCollider(Collider2D other)
    {
        // 如果投射物尚未发射 则不执行任何操作
        if (!isLaunched)
        {
            return;
        }

        // 获取当前武器配置的命中策略 如果没有配置 或者没有命中策略 则不执行任何操作
        ProjectileHitStrategy hitStrategy = HitStrategy;
        if (hitStrategy == null)
        {
            return;
        }

        // 如果命中物体不在敌人层级掩码中 则不执行任何操作
        if (!IsInLayerMask(other.gameObject.layer, enemyLayerMask))
        {
            return;
        }

        // 获取碰撞器所在的敌人对象 
        Enemy enemy = other.GetComponentInParent<Enemy>();
        // 如果没有找到 或者敌人已死亡 则不执行任何操作
        if (enemy == null || !enemy.IsAlive)
        {
            return;
        }

        // 如果命中策略要求只能命中初始目标 且当前目标不为null 且命中对象不是当前目标 则不执行任何操作
        if (hitStrategy.AcceptsOnlyInitialTarget && target != null && enemy != target)
        {
            return;
        }

        // 处理命中事件 由命中策略决定具体的处理方式
        hitStrategy.HandleHit(this, enemy);
    }


    /// <summary>
    /// 发射投射物
    /// </summary>
    /// <param name="target">目标敌人</param>
    /// <param name="context">攻击上下文</param>
    /// <param name="releaseAction">释放动作</param>
    public void Launch(Enemy target, AttackContext context, Action<ProjectileController> releaseAction)
    {
        this.target = target;
        weaponConfig = context.WeaponConfig;
        damage = context.Damage;
        speed = Mathf.Max(0.01f, context.ProjectileSpeed);
        maxTravelDistance = Mathf.Max(0.01f, context.Range);
        hitRadius = Mathf.Max(0.01f, context.ProjectileHitRadius);
        this.releaseAction = releaseAction;

        spawnPosition = context.Origin;
        moveDirection = context.Direction.sqrMagnitude > 0f ? context.Direction.normalized : Vector2.right;
        isLaunched = true;
        enemyHitTimes.Clear();
        rigidBody2D.velocity = moveDirection * speed;
    }

    /// <summary>
    /// 当投射物从对象池中被获取时调用
    /// </summary>
    public void OnSpawnedFromPool()
    {
        if (rigidBody2D == null)
        {
            rigidBody2D = GetComponent<Rigidbody2D>();
        }

        rigidBody2D.velocity = Vector2.zero;
        rigidBody2D.angularVelocity = 0f;
        isLaunched = false;
    }

    /// <summary>
    /// 当投射物被释放回对象池时调用
    /// </summary>
    public void OnReturnedToPool()
    {
        if (rigidBody2D != null)
        {
            rigidBody2D.velocity = Vector2.zero;
            rigidBody2D.angularVelocity = 0f;
        }

        target = null;
        weaponConfig = null;
        releaseAction = null;
        enemyHitTimes.Clear();
        isLaunched = false;
    }

    /// <summary>
    /// 尝试对敌人应用配置的伤害 如果成功应用伤害 则返回true 否则返回false
    /// </summary>
    /// <param name="enemy">目标敌人</param>
    /// <param name="impactPosition">命中位置</param>
    /// <returns></returns>
    public bool TryApplyConfiguredDamage(Enemy enemy, Vector3 impactPosition)
    {
        // 首先检查是否可以对敌人造成伤害
        // 如果不能造成伤害 则直接返回false
        if (!CanDamage(enemy))
        {
            return false;
        }

        // 使用WeaponDamageApplier来应用配置的伤害
        // 传入敌人 对应的命中位置 当前投射物的伤害 武器配置 和一个用于存储范围伤害目标的列表
        bool appliedDamage = WeaponDamageApplier.ApplyConfiguredDamage(
            enemy,
            impactPosition,
            damage,
            weaponConfig,
            areaTargets);

        // 如果成功应用了伤害
        // 则记录当前时间作为对该敌人的最后命中时间 以便后续判断穿透伤害间隔
        if (appliedDamage)
        {
            enemyHitTimes[enemy] = Time.time;
        }

        return appliedDamage;
    }

    /// <summary>
    /// 释放投射物 
    /// 停止投射物的移动 并调用释放动作 将投射物返回对象池
    /// </summary>
    public void ReleaseProjectile()
    {
        if (!isLaunched)
        {
            return;
        }

        isLaunched = false;
        rigidBody2D.velocity = Vector2.zero;
        releaseAction?.Invoke(this);
    }

    /// <summary>
    /// 检查是否可以对敌人造成伤害 
    /// 根据敌人的状态和之前的命中时间来决定是否允许再次造成伤害
    /// </summary>
    /// <param name="enemy">目标敌人</param>
    /// <returns></returns>
    private bool CanDamage(Enemy enemy)
    {
        // 如果敌人不存在 或者敌人已死亡 或者没有配置武器 则不能造成伤害
        if (enemy == null || !enemy.IsAlive || weaponConfig == null)
        {
            return false;
        }

        // 如果之前没有命中过这个敌人 则可以造成伤害
        if (!enemyHitTimes.TryGetValue(enemy, out float lastHitTime))
        {
            return true;
        }

        // 如果之前命中过这个敌人 但已经超过穿透伤害间隔时间 则可以造成伤害 否则不能造成伤害
        return Time.time - lastHitTime >= weaponConfig.PiercingDamageInterval;
    }

    /// <summary>
    /// 配置刚体属性 
    /// 使投射物不受重力影响 不旋转 使用插值平滑移动 并启用连续碰撞检测以避免高速穿透问题
    /// </summary>
    private void ConfigureRigidbody()
    {
        rigidBody2D.gravityScale = 0f;
        rigidBody2D.freezeRotation = true;
        rigidBody2D.interpolation = RigidbodyInterpolation2D.Interpolate;
        rigidBody2D.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
    }

    /// <summary>
    /// 配置碰撞器属性
    /// </summary>
    private void ConfigureCollider()
    {
        Collider2D collider2D = GetComponent<Collider2D>();
        if (collider2D == null)
        {
            collider2D = gameObject.AddComponent<CircleCollider2D>();
        }

        collider2D.isTrigger = true;
    }

    /// <summary>
    /// 确保敌人层级掩码被正确配置 
    /// 如果当前掩码值为0 则尝试通过层级名称获取敌人层级并设置掩码值
    /// </summary>
    private void EnsureEnemyLayerMask()
    {
        if (enemyLayerMask.value != 0)
        {
            return;
        }

        int enemyLayer = LayerMask.NameToLayer("Enemy");
        if (enemyLayer >= 0)
        {
            enemyLayerMask = 1 << enemyLayer;
        }
    }

    /// <summary>
    /// 检查给定的层级是否在指定的层级掩码中
    /// </summary>
    /// <param name="layer"></param>
    /// <param name="layerMask"></param>
    /// <returns></returns>
    private static bool IsInLayerMask(int layer, LayerMask layerMask)
    {
        return (layerMask.value & (1 << layer)) != 0;
    }
}
