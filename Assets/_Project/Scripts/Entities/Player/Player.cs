using UnityEngine;

/// <summary>
/// 玩家运行时聚合类
/// 统一缓存玩家身上的常用组件引用，并在生成时补齐缺失的运行时组件
/// </summary>
#region REQUIRE COMPONENTS
[RequireComponent(typeof(MovementByVelocityEvent))]
[RequireComponent(typeof(MovementByVelocity))]
[RequireComponent(typeof(IdleEvent))]
[RequireComponent(typeof(Idle))]
[RequireComponent(typeof(PlayerController))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(PlayerAnimator))]
[RequireComponent(typeof(Health))]
[RequireComponent(typeof(InjuredEvent))]
[RequireComponent(typeof(DeathEvent))]
[RequireComponent(typeof(AttackEvent))]
[RequireComponent(typeof(PlayerDamageReceiver))]
[RequireComponent(typeof(PlayerInjuredHandler))]
[RequireComponent(typeof(PlayerHealthBar))]
[RequireComponent(typeof(PlayerDeathHandler))]
[RequireComponent(typeof(PlayerWeaponSystem))]
[RequireComponent(typeof(PlayerPickupSystem))]
[RequireComponent(typeof(PlayerProgress))]
[RequireComponent(typeof(RuntimeStats))]
[RequireComponent(typeof(StatusEffectManager))]
#endregion
[DisallowMultipleComponent]
public class Player : MonoBehaviour
{
    public SpriteRenderer SpriteRenderer { get; private set; }
    public Animator Animator { get; private set; }

    public PlayerController PlayerController { get; private set; }
    public PlayerAnimator PlayerAnimator { get; private set; }
    public MovementByVelocityEvent MovementByVelocityEvent { get; private set; }
    public MovementByVelocity MovementByVelocity { get; private set; }
    public IdleEvent IdleEvent { get; private set; }
    public Idle Idle { get; private set; }
    public Health Health { get; private set; }
    public InjuredEvent InjuredEvent { get; private set; }
    public DeathEvent DeathEvent { get; private set; }
    public AttackEvent AttackEvent { get; private set; }
    public PlayerDamageReceiver PlayerDamageReceiver { get; private set; }
    public PlayerInjuredHandler PlayerInjuredHandler { get; private set; }
    public PlayerHealthBar PlayerHealthBar { get; private set; }
    public PlayerDeathHandler PlayerDeathHandler { get; set; }
    public PlayerWeaponSystem PlayerWeaponSystem { get; private set; }
    public PlayerPickupSystem PlayerPickupSystem { get; private set; }
    public PlayerProgress PlayerProgress { get; private set; }
    public RuntimeStats RuntimeStats { get; private set; }
    public StatusEffectManager StatusEffectManager { get; private set; }

    private void Awake()
    {
        EnsureRuntimeComponents();
        CacheComponents();
    }

    /// <summary>
    /// 确保玩家对象上存在所有运行时需要的组件
    /// 这样角色预制体缺少新系统组件时，也能在生成后自动补齐
    /// </summary>
    public void EnsureRuntimeComponents()
    {
        EnsureComponent<Rigidbody2D>();
        EnsureComponent<PlayerController>();
        EnsureComponent<PlayerAnimator>();
        EnsureComponent<MovementByVelocityEvent>();
        EnsureComponent<MovementByVelocity>();
        EnsureComponent<IdleEvent>();
        EnsureComponent<Idle>();
        EnsureComponent<Health>();
        EnsureComponent<InjuredEvent>();
        EnsureComponent<DeathEvent>();
        EnsureComponent<AttackEvent>();
        EnsureComponent<PlayerDamageReceiver>();
        EnsureComponent<PlayerInjuredHandler>();
        EnsureComponent<PlayerHealthBar>();
        EnsureComponent<PlayerDeathHandler>();
        EnsureComponent<PlayerWeaponSystem>();
        EnsureComponent<PlayerPickupSystem>();
        EnsureComponent<PlayerProgress>();
        EnsureComponent<RuntimeStats>();
        EnsureComponent<StatusEffectManager>();
    }

    /// <summary>
    /// 如果当前对象缺少指定组件，则自动添加
    /// </summary>
    private void EnsureComponent<T>() where T : Component
    {
        if (GetComponent<T>() == null)
        {
            gameObject.AddComponent<T>();
        }
    }

    /// <summary>
    /// 缓存玩家对象上的常用组件引用，减少后续重复 GetComponent
    /// </summary>
    public void CacheComponents()
    {
        if (SpriteRenderer == null)
            SpriteRenderer = GetComponent<SpriteRenderer>();
        if (Animator == null)
            Animator = GetComponent<Animator>();
        if (PlayerController == null)
            PlayerController = GetComponent<PlayerController>();
        if (PlayerAnimator == null)
            PlayerAnimator = GetComponent<PlayerAnimator>();
        if (MovementByVelocityEvent == null)
            MovementByVelocityEvent = GetComponent<MovementByVelocityEvent>();
        if (MovementByVelocity == null)
            MovementByVelocity = GetComponent<MovementByVelocity>();
        if (IdleEvent == null)
            IdleEvent = GetComponent<IdleEvent>();
        if (Idle == null)
            Idle = GetComponent<Idle>();
        if (Health == null)
            Health = GetComponent<Health>();
        if (InjuredEvent == null)
            InjuredEvent = GetComponent<InjuredEvent>();
        if (DeathEvent == null)
            DeathEvent = GetComponent<DeathEvent>();
        if (AttackEvent == null)
            AttackEvent = GetComponent<AttackEvent>();
        if (PlayerDamageReceiver == null)
            PlayerDamageReceiver = GetComponent<PlayerDamageReceiver>();
        if (PlayerInjuredHandler == null)
            PlayerInjuredHandler = GetComponent<PlayerInjuredHandler>();
        if (PlayerHealthBar == null)
            PlayerHealthBar = GetComponent<PlayerHealthBar>();
        if (PlayerDeathHandler == null)
            PlayerDeathHandler = GetComponent<PlayerDeathHandler>();
        if (PlayerWeaponSystem == null)
            PlayerWeaponSystem = GetComponent<PlayerWeaponSystem>();
        if (PlayerPickupSystem == null)
            PlayerPickupSystem = GetComponent<PlayerPickupSystem>();
        if (PlayerProgress == null)
            PlayerProgress = GetComponent<PlayerProgress>();
        if (RuntimeStats == null)
            RuntimeStats = GetComponent<RuntimeStats>();
        if (StatusEffectManager == null)
            StatusEffectManager = GetComponent<StatusEffectManager>();
    }

    /// <summary>
    /// 批量启用或禁用玩家运行时组件
    /// 可用于结算、暂停或特殊状态下统一控制玩家行为
    /// </summary>
    /// <param name="enabled">是否启用组件</param>
    public void SetAllComponentsEnabled(bool enabled)
    {
        if (Animator != null)
            Animator.enabled = enabled;
        if (PlayerController != null)
            PlayerController.enabled = enabled;
        if (PlayerAnimator != null)
            PlayerAnimator.enabled = enabled;
        if (MovementByVelocityEvent != null)
            MovementByVelocityEvent.enabled = enabled;
        if (MovementByVelocity != null)
            MovementByVelocity.enabled = enabled;
        if (IdleEvent != null)
            IdleEvent.enabled = enabled;
        if (Idle != null)
            Idle.enabled = enabled;
        if (Health != null)
            Health.enabled = enabled;
        if (InjuredEvent != null)
            InjuredEvent.enabled = enabled;
        if (DeathEvent != null)
            DeathEvent.enabled = enabled;
        if (AttackEvent != null)
            AttackEvent.enabled = enabled;
        if (PlayerDamageReceiver != null)
            PlayerDamageReceiver.enabled = enabled;
        if (PlayerInjuredHandler != null)
            PlayerInjuredHandler.enabled = enabled;
        if (PlayerHealthBar != null)
            PlayerHealthBar.enabled = enabled;
        if (PlayerDeathHandler != null)
            PlayerDeathHandler.enabled = enabled;
        if (PlayerWeaponSystem != null)
            PlayerWeaponSystem.enabled = enabled;
        if (PlayerPickupSystem != null)
            PlayerPickupSystem.enabled = enabled;
        if (PlayerProgress != null)
            PlayerProgress.enabled = enabled;
        if (RuntimeStats != null)
            RuntimeStats.enabled = enabled;
        if (StatusEffectManager != null)
            StatusEffectManager.enabled = enabled;
    }
}
