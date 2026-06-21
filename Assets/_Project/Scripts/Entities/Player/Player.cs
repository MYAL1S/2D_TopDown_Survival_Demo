using System.Collections;
using System.Collections.Generic;
using UnityEditor.SceneManagement;
using UnityEngine;

/// <summary>
/// 玩家类 主要用于管理玩家的各个组件
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
[RequireComponent(typeof(PlayerDeathHandler))]
[RequireComponent(typeof(PlayerWeaponSystem))]
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
    public PlayerDeathHandler PlayerDeathHandler { get; set; }
    public PlayerWeaponSystem PlayerWeaponSystem { get; private set; }


    void Awake()
    {
        EnsureRuntimeComponents();
        CacheComponents();
    }

    /// <summary>
    /// 确保玩家对象上存在所有必要的组件 如果缺少则添加
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
        EnsureComponent<PlayerDeathHandler>();
        EnsureComponent<PlayerWeaponSystem>();
    }

    private void EnsureComponent<T>() where T : Component
    {
        if (GetComponent<T>() == null)
            gameObject.AddComponent<T>();
    }
    /// <summary>
    /// 缓存玩家对象上所有必要组件的引用 以便快速访问
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
        if (MovementByVelocityEvent== null)
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
        if (PlayerDeathHandler == null)
            PlayerDeathHandler = GetComponent<PlayerDeathHandler>();
        if (PlayerWeaponSystem == null)
            PlayerWeaponSystem = GetComponent<PlayerWeaponSystem>();
    }

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
        if (PlayerDeathHandler != null)
            PlayerDeathHandler.enabled = enabled;
        if (PlayerWeaponSystem != null)
            PlayerWeaponSystem.enabled = enabled;
    }
}
