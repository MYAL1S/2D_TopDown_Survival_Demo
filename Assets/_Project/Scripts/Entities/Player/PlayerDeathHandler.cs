using UnityEngine;

/// <summary>
/// 玩家死亡处理组件
/// </summary>
[RequireComponent(typeof(DeathEvent))]
[DisallowMultipleComponent]
public class PlayerDeathHandler : MonoBehaviour
{
    // 玩家组件 用于在玩家死亡时访问玩家的属性和方法
    private Player player;
    // 死亡事件组件 用于监听玩家死亡事件
    private DeathEvent deathEvent;
    // 刚体组件 用于在玩家死亡时停止玩家的物理运动
    private Rigidbody2D rigidBody2D;
    // 玩家碰撞器数组
    // 用于在玩家死亡时禁用碰撞器 防止玩家继续与环境发生物理交互
    private Collider2D[] colliders;
    // 玩家控制器组件 用于在玩家死亡时禁用玩家的输入和控制逻辑
    private PlayerController playerController;
    // 玩家是否已经死亡的标志 避免重复处理死亡事件
    private bool isDead;

    private void Awake()
    {
        player = GetComponent<Player>();
        deathEvent = GetComponent<DeathEvent>();
        rigidBody2D = GetComponent<Rigidbody2D>();
        colliders = GetComponentsInChildren<Collider2D>(true);
        playerController = GetComponent<PlayerController>();
    }

    private void OnEnable()
    {
        isDead = false;
        SetCollidersEnabled(true);
        deathEvent.OnDeath += DeathEvent_OnDeath;
    }

    private void OnDisable()
    {
        deathEvent.OnDeath -= DeathEvent_OnDeath;
    }

    private void DeathEvent_OnDeath(DeathEvent eventSource)
    {
        if (isDead)
        {
            return;
        }

        isDead = true;

        if (rigidBody2D != null)
        {
            rigidBody2D.velocity = Vector2.zero;
            rigidBody2D.angularVelocity = 0f;
        }

        if (playerController != null)
        {
            playerController.enabled = false;
        }

        player.Animator.SetTrigger(Settings.defeat);
        SetCollidersEnabled(false);
    }

    /// <summary>
    /// 启用或禁用玩家的碰撞器 
    /// 在玩家死亡时禁用碰撞器 防止玩家继续与环境发生物理交互
    /// </summary>
    /// <param name="isEnabled">是否启用碰撞器</param>
    private void SetCollidersEnabled(bool isEnabled)
    {
        for (int i = 0; i < colliders.Length; i++)
        {
            if (colliders[i] != null)
            {
                colliders[i].enabled = isEnabled;
            }
        }
    }
}
