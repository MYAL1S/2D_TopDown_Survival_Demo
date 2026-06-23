using UnityEngine;

#region REQUIRE COMPONENTS
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Health))]
[RequireComponent(typeof(InjuredEvent))]
[RequireComponent(typeof(DeathEvent))]
[RequireComponent(typeof(EnemyDropHandler))]
[RequireComponent(typeof(EnemyInjuredHandler))]
[RequireComponent(typeof(EnemyDeathHandler))]
[RequireComponent(typeof(AttackEvent))]
[RequireComponent(typeof(EnemyAttackSystem))]
[RequireComponent(typeof(EnemyMeleeAttackHandler))]
[RequireComponent(typeof(RuntimeStats))]
[RequireComponent(typeof(StatusEffectManager))]
#endregion
[DisallowMultipleComponent]
public class EnemyController : MonoBehaviour
{
    /// 敌人配置数据 包含敌人的属性和动画控制器等信息
    [SerializeField]
    private EnemyConfig enemyConfig;
    // 敌人移动速度
    private float speed = 2f;
    // 敌人攻击力
    private float atk = 1f;
    // 敌人防御力
    private float def = 0f;
    private float attackCooldown = 1f;
    private float attackRange = 0.75f;
    // 敌人刚体组件
    private Rigidbody2D rigidBody2D;
    // 敌人生命值组件
    private Health health;
    private RuntimeStats runtimeStats;
    // 敌人的目标 Transform 组件 用于敌人追踪和攻击目标
    public Transform Target { get; private set; }

    public EnemyConfig Config => enemyConfig;
    public float MoveSpeed => runtimeStats != null ? runtimeStats.CurrentMoveSpeed : speed;
    public float Attack => atk;
    public float Defense => def;
    public float AttackCooldown => attackCooldown;
    public float AttackRange => attackRange;

    // 敌人是否存活 如果生命值组件为空 则认为敌人存活
    public bool IsAlive => health == null || health.IsAlive;
    public Health Health => health;

    private void Awake()
    {
        // 缓存组件引用
        rigidBody2D = GetComponent<Rigidbody2D>();
        health = GetComponent<Health>();
        runtimeStats = GetComponent<RuntimeStats>();
        // 配置刚体属性
        rigidBody2D.gravityScale = 0f;
        rigidBody2D.freezeRotation = true;
        rigidBody2D.interpolation = RigidbodyInterpolation2D.Interpolate;
        rigidBody2D.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        // 配置敌人子物体的碰撞器为触发器 避免物理碰撞干扰敌人的移动和攻击逻辑
        ConfigureColliders();
    }

    /// <summary>
    /// 应用敌人配置数据到敌人控制器中 设置敌人的属性和动画控制器
    /// </summary>
    /// <param name="config">敌人配置数据</param>
    private void ApplyConfig(EnemyConfig config)
    {
        if (config != null)
        {
            speed = Mathf.Max(0f, config.speed);
            atk = Mathf.Max(0f, config.atk);
            def = Mathf.Max(0f, config.def);
            attackCooldown = Mathf.Max(0.01f, config.AttackCooldownSeconds);
            attackRange = Mathf.Max(0.01f, config.AttackRange);
            if (runtimeStats == null)
            {
                runtimeStats = GetComponent<RuntimeStats>();
            }

            if (runtimeStats != null)
            {
                runtimeStats.InitializeMoveSpeed(speed);
            }

            health.Initialize(config.health, def);

            Animator animator = GetComponentInChildren<Animator>();
            if (animator != null && config.animatorController != null)
            {
                animator.runtimeAnimatorController = config.animatorController;
                animator.speed = config.speed / Settings.baseSpeedForEnemyAnimations;
            }
        }
    }

    /// <summary>
    /// 初始化敌人控制器 使用指定的敌人配置数据和目标 Transform
    /// </summary>
    /// <param name="config">敌人配置数据</param>
    /// <param name="target">目标 Transform</param>
    public void Initialize(EnemyConfig config, Transform target)
    {
        enemyConfig = config;
        ApplyConfig(enemyConfig);
        Target = target;
    }

    /// <summary>
    /// 设置敌人的追踪目标 Transform 组件 
    /// 以便敌人可以追踪和攻击目标
    /// </summary>
    /// <param name="target">目标Transform</param>
    public void SetTarget(Transform target)
    {
        Target = target;
    }

    /// <summary>
    /// 将敌人所有子物体的Collider2D组件设置为触发器
    /// 以避免物理碰撞干扰敌人的移动和攻击逻辑
    /// </summary>
    private void ConfigureColliders()
    {
        Collider2D[] colliders = GetComponentsInChildren<Collider2D>(true);
        for (int i = 0; i < colliders.Length; i++)
        {
            colliders[i].isTrigger = true;
        }
    }
}
