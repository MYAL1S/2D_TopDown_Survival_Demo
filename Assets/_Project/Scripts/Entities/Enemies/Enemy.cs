using UnityEngine;

/// <summary>
/// 敌人类 代表游戏中的敌人实体 
/// 包含敌人的属性 组件和行为逻辑
/// </summary>
#region REQUIRE COMPONENTS
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Health))]
[RequireComponent(typeof(InjuredEvent))]
[RequireComponent(typeof(DeathEvent))]
[RequireComponent(typeof(AttackEvent))]
[RequireComponent(typeof(EnemyController))]
[RequireComponent(typeof(EnemyAISystem))]
[RequireComponent(typeof(EnemyAttackSystem))]
[RequireComponent(typeof(EnemyMeleeAttackHandler))]
[RequireComponent(typeof(EnemyInjuredHandler))]
[RequireComponent(typeof(EnemyDeathHandler))]
[RequireComponent(typeof(EnemyDropHandler))]
[RequireComponent(typeof(RuntimeStats))]
[RequireComponent(typeof(StatusEffectManager))]
#endregion
[DisallowMultipleComponent]
public class Enemy : MonoBehaviour, IPoolable
{
    public EnemyController Controller { get; private set; }
    public EnemyAISystem AISystem { get; private set; }
    public EnemyAttackSystem AttackSystem { get; private set; }
    public EnemyMeleeAttackHandler MeleeAttackHandler { get; private set; }
    public EnemyInjuredHandler InjuredHandler { get; private set; }
    public EnemyDeathHandler DeathHandler { get; private set; }
    public EnemyDropHandler DropHandler { get; private set; }
    public Health Health { get; private set; }
    public InjuredEvent InjuredEvent { get; private set; }
    public DeathEvent DeathEvent { get; private set; }
    public AttackEvent AttackEvent { get; private set; }
    public Rigidbody2D Rigidbody2D { get; private set; }
    public RuntimeStats RuntimeStats { get; private set; }
    public StatusEffectManager StatusEffectManager { get; private set; }
    public EnemyConfig Config => Controller != null ? Controller.Config : null;
    public Transform Target => Controller != null ? Controller.Target : null;
    public float MoveSpeed => Controller != null ? Controller.MoveSpeed : 0f;
    public float Attack => Controller != null ? Controller.Attack : 0f;
    public float Defense => Controller != null ? Controller.Defense : 0f;
    public float AttackCooldown => Controller != null ? Controller.AttackCooldown : 1f;
    public float AttackRange => Controller != null ? Controller.AttackRange : 0f;
    public bool IsAlive => Controller == null || Controller.IsAlive;

    private void Awake()
    {
        // 先确保所有必要的组件存在 然后缓存它们的引用 以便快速访问
        EnsureRuntimeComponents();
        CacheComponents();
    }

    /// <summary>
    /// 注册敌人到TargetingSystem
    /// 以便它可以被玩家或其他系统找到
    /// </summary>
    private void OnDisable()
    {
        TargetingSystem.UnregisterEnemy(this);
    }

    /// <summary>
    /// 取消注册敌人从TargetingSystem
    /// </summary>
    private void OnDestroy()
    {
        TargetingSystem.UnregisterEnemy(this);
    }

    /// <summary>
    /// 缓存所有必要的组件引用 以便快速访问
    /// </summary>
    public void CacheComponents()
    {
        if (Controller == null)
            Controller = GetComponent<EnemyController>();
        if (AISystem == null)
            AISystem = GetComponent<EnemyAISystem>();
        if (AttackSystem == null)
            AttackSystem = GetComponent<EnemyAttackSystem>();
        if (MeleeAttackHandler == null)
            MeleeAttackHandler = GetComponent<EnemyMeleeAttackHandler>();
        if (InjuredHandler == null)
            InjuredHandler = GetComponent<EnemyInjuredHandler>();
        if (DeathHandler == null)
            DeathHandler = GetComponent<EnemyDeathHandler>();
        if (DropHandler == null)
            DropHandler = GetComponent<EnemyDropHandler>();
        if (Health == null)
            Health = GetComponent<Health>();
        if (InjuredEvent == null)
            InjuredEvent = GetComponent<InjuredEvent>();
        if (DeathEvent == null)
            DeathEvent = GetComponent<DeathEvent>();
        if (AttackEvent == null)
            AttackEvent = GetComponent<AttackEvent>();
        if (Rigidbody2D == null)
            Rigidbody2D = GetComponent<Rigidbody2D>();
        if (RuntimeStats == null)
            RuntimeStats = GetComponent<RuntimeStats>();
        if (StatusEffectManager == null)
            StatusEffectManager = GetComponent<StatusEffectManager>();
    }

    /// <summary>
    /// 初始化敌人实例 使用提供的配置和目标
    /// </summary>
    /// <param name="config">敌人配置</param>
    /// <param name="target">目标Transform</param>
    public void Initialize(EnemyConfig config, Transform target)
    {
        CacheComponents();
        Controller.Initialize(config, target);
        TargetingSystem.RegisterEnemy(this);
    }

    /// <summary>
    /// 设置敌人的目标Transform 以便追踪和攻击目标
    /// </summary>
    /// <param name="target"></param>
    public void SetTarget(Transform target)
    {
        Controller.SetTarget(target);
    }

    /// <summary>
    /// 当敌人从对象池中生成时调用 
    /// 重置敌人的状态和属性以确保它处于初始状态
    /// </summary>
    public void OnSpawnedFromPool()
    {
        ResetRigidbody();
    }

    /// <summary>
    /// 当敌人返回对象池时调用
    /// 重置敌人的状态和属性以确保它处于初始状态
    /// </summary>
    public void OnReturnedToPool()
    {
        if (StatusEffectManager != null)
        {
            StatusEffectManager.ClearAll();
        }

        TargetingSystem.UnregisterEnemy(this);
        SetTarget(null);
        ResetRigidbody();
    }

    /// <summary>
    /// 重置敌人的刚体状态 包括速度和角速度 
    /// 以确保它在从对象池中生成或返回时处于静止状态
    /// </summary>
    private void ResetRigidbody()
    {
        if (Rigidbody2D == null)
        {
            Rigidbody2D = GetComponent<Rigidbody2D>();
        }

        if (Rigidbody2D == null)
        {
            return;
        }

        Rigidbody2D.velocity = Vector2.zero;
        Rigidbody2D.angularVelocity = 0f;
    }

    /// <summary>
    /// 确保敌人对象上存在Health InjuredEvent DeathEvent组件 
    /// 如果不存在则添加这些组件 以保证敌人具有生命值和受伤死亡事件的功能
    /// </summary>
    public void EnsureRuntimeComponents()
    {
        EnsureComponent<Rigidbody2D>();
        EnsureComponent<EnemyController>();
        EnsureComponent<EnemyAISystem>();
        EnsureComponent<Health>();
        EnsureComponent<InjuredEvent>();
        EnsureComponent<DeathEvent>();
        EnsureComponent<EnemyInjuredHandler>();
        EnsureComponent<EnemyDeathHandler>();
        EnsureComponent<EnemyDropHandler>();
        EnsureComponent<AttackEvent>();
        EnsureComponent<EnemyAttackSystem>();
        EnsureComponent<EnemyMeleeAttackHandler>();
        EnsureComponent<RuntimeStats>();
        EnsureComponent<StatusEffectManager>();
    }

    private void EnsureComponent<T>() where T : Component
    {
        if (GetComponent<T>() == null)
            gameObject.AddComponent<T>();
    }
}
