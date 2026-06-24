using UnityEngine;

/// <summary>
/// 玩家死亡处理组件
/// 监听玩家 DeathEvent，通知关卡管理器结束关卡，并禁用玩家控制与碰撞
/// </summary>
[RequireComponent(typeof(DeathEvent))]
[DisallowMultipleComponent]
public class PlayerDeathHandler : MonoBehaviour
{
    // 玩家聚合组件，用于访问动画器等玩家运行时引用
    private Player player;
    // 死亡事件组件
    private DeathEvent deathEvent;
    // 刚体组件，死亡时停止玩家物理运动
    private Rigidbody2D rigidBody2D;
    // 玩家及子节点碰撞器，死亡后禁用以避免继续参与碰撞
    private Collider2D[] colliders;
    // 玩家控制器，死亡后禁用输入控制
    private PlayerController playerController;
    // 防止死亡事件重复执行
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
        // 通知 LevelManager 以失败状态结束关卡并打开结算宝箱面板
        EventCenter.Instance.EventTrigger(E_EventType.PlayerDied);

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
    /// 批量启用或禁用玩家碰撞器
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
