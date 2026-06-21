using UnityEngine;

/// <summary>
/// 敌人攻击系统 
/// 负责检测玩家是否在攻击范围内并触发攻击事件
/// </summary>
[RequireComponent(typeof(AttackEvent))]
[DisallowMultipleComponent]
public class EnemyAttackSystem : MonoBehaviour
{
    [SerializeField]
    [Tooltip("Only colliders on these layers can be considered player attack targets.")]
    private LayerMask playerLayerMask;

    private Enemy enemy;
    private Player collidingPlayer;
    private float cooldownTimer;

    private void Awake()
    {
        CacheEnemy();
        EnsurePlayerLayerMask();
    }

    private void OnEnable()
    {
        CacheEnemy();
        EnsurePlayerLayerMask();
        collidingPlayer = null;
        cooldownTimer = 0f;
    }

    private void Update()
    {
        // 如果敌人没有攻击事件或者敌人已经死亡 则不进行攻击检测
        if (enemy == null || enemy.AttackEvent == null || !enemy.IsAlive)
        {
            return;
        }

        // 如果攻击冷却中 则不进行攻击检测
        if (cooldownTimer > 0f)
        {
            cooldownTimer -= Time.deltaTime;
            return;
        }

        // 如果没有碰撞的玩家或者玩家不在攻击范围内 则不进行攻击
        if (collidingPlayer == null || !IsPlayerInAttackRange(collidingPlayer))
        {
            return;
        }

        // 触发攻击事件
        Vector3 direction = collidingPlayer.transform.position - transform.position;
        AttackContext context = new AttackContext(
            gameObject,
            collidingPlayer.gameObject,
            transform.position,
            direction,
            enemy.Attack,
            enemy.AttackRange);

        enemy.AttackEvent.CallAttackEvent(context);
        cooldownTimer = enemy.AttackCooldown;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // 当有新的碰撞进入时
        // 尝试缓存玩家
        TryCacheCollidingPlayer(other);
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        // 当持续碰撞时
        // 尝试缓存玩家
        if (collidingPlayer != null)
        {
            return;
        }

        TryCacheCollidingPlayer(other);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        // 当碰撞退出时 如果退出的对象不在玩家图层掩码中 则不进行处理
        if (!IsInLayerMask(other.gameObject.layer, playerLayerMask))
        {
            return;
        }

        // 尝试获取玩家组件 如果成功 且该玩家是当前缓存的玩家 则将缓存的玩家置空
        Player player = other.GetComponentInParent<Player>();
        if (player != null && player == collidingPlayer)
        {
            collidingPlayer = null;
        }
    }

    /// <summary>
    /// 尝试缓存碰撞的玩家 
    /// 如果碰撞的对象是玩家 则将其缓存为当前攻击目标
    /// </summary>
    /// <param name="other"></param>
    private void TryCacheCollidingPlayer(Collider2D other)
    {
        // 如果已经有一个碰撞的玩家 则不进行处理
        if (collidingPlayer != null)
            return;
        // 如果该碰撞的对象不在玩家图层掩码中 则不进行处理
        if (!IsInLayerMask(other.gameObject.layer, playerLayerMask))
        {
            return;
        }

        // 尝试获取玩家组件 如果成功 则将其缓存为当前攻击目标
        Player player = other.GetComponentInParent<Player>();
        if (player != null)
        {
            collidingPlayer = player;
        }
    }

    /// <summary>
    /// 检查玩家是否在攻击范围内
    /// </summary>
    /// <param name="player">玩家</param>
    /// <returns></returns>
    private bool IsPlayerInAttackRange(Player player)
    {
        if (player == null)
        {
            return false;
        }

        float attackRange = enemy.AttackRange;
        float sqrDistance = (player.transform.position - transform.position).sqrMagnitude;
        return sqrDistance <= attackRange * attackRange;
    }

    /// <summary>
    /// 确保玩家图层掩码已正确设置 
    /// 如果未设置 则尝试通过名称获取玩家层并设置图层掩码
    /// </summary>
    private void EnsurePlayerLayerMask()
    {
        if (playerLayerMask.value != 0)
        {
            return;
        }

        int playerLayer = LayerMask.NameToLayer("Player");
        if (playerLayer >= 0)
        {
            playerLayerMask = 1 << playerLayer;
        }
    }


    /// <summary>
    /// 检查指定的层是否在图层掩码中
    /// </summary>
    /// <param name="layer">层</param>
    /// <param name="layerMask">图层掩码</param>
    /// <returns></returns>
    private static bool IsInLayerMask(int layer, LayerMask layerMask)
    {
        return (layerMask.value & (1 << layer)) != 0;
    }

    /// <summary>
    /// 缓存敌人组件
    /// </summary>
    private void CacheEnemy()
    {
        if (enemy == null)
        {
            enemy = GetComponent<Enemy>();
        }

        if (enemy != null)
        {
            enemy.CacheComponents();
        }
    }
}
