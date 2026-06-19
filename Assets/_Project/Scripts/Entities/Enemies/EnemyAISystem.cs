using UnityEngine;

[RequireComponent(typeof(EnemyController))]
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
    private LayerMask separationLayerMask = ~0;

    [SerializeField]
    [Min(1)]
    private int maxSeparationChecks = 12;

    private EnemyController enemyController;
    private Rigidbody2D rigidBody2D;
    private SpriteRenderer spriteRenderer;
    private Collider2D[] separationHits;

    private void Awake()
    {
        // 缓存常用组件 避免在运行时重复查找
        enemyController = GetComponent<EnemyController>();
        rigidBody2D = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        separationHits = new Collider2D[maxSeparationChecks];

        // 敌人只做平面移动 不受重力影响 也不允许旋转
        rigidBody2D.gravityScale = 0f;
        rigidBody2D.freezeRotation = true;
        rigidBody2D.interpolation = RigidbodyInterpolation2D.Interpolate;
    }

    private void FixedUpdate()
    {
        // 获取追踪目标
        // 敌人死亡或目标为空时停止移动
        Transform target = enemyController.Target;

        if (!enemyController.IsAlive || target == null)
        {
            rigidBody2D.velocity = Vector2.zero;
            return;
        }

        // 计算朝向目标的移动方向
        Vector2 offset = target.position - transform.position;
        Vector2 targetDirection = offset.sqrMagnitude > stopDistance * stopDistance
            ? offset.normalized
            : Vector2.zero;

        // 计算与周围敌人的分离方向
        // 避免敌人重叠堆叠
        Vector2 separationDirection = CalculateSeparationDirection();
        Vector2 moveDirection = targetDirection + separationDirection * separationWeight;

        // 限制最终移动向量长度
        // 避免叠加后速度过快
        if (moveDirection.sqrMagnitude > 1f)
        {
            moveDirection.Normalize();
        }

        // 根据最终方向驱动刚体移动
        rigidBody2D.velocity = moveDirection * enemyController.MoveSpeed;

        // 根据移动方向切换朝向
        if (spriteRenderer != null && moveDirection.x != 0f)
        {
            spriteRenderer.flipX = moveDirection.x < 0f;
        }
    }

    /// <summary>
    /// 计算敌人之间的分离方向
    /// 用于给相互过近的敌人增加一个向外推开的力
    /// </summary>
    private Vector2 CalculateSeparationDirection()
    {
        if (separationDistance <= 0f || separationHits == null)
        {
            return Vector2.zero;
        }

        Vector2 currentPosition = transform.position;
        int hitCount = Physics2D.OverlapCircleNonAlloc(
            currentPosition,
            separationDistance,
            separationHits,
            separationLayerMask);

        Vector2 separationDirection = Vector2.zero;

        for (int i = 0; i < hitCount; i++)
        {
            Collider2D hit = separationHits[i];
            if (hit == null)
            {
                continue;
            }

            // 只对其他存活的敌人计算分离力
            EnemyController otherEnemy = hit.GetComponentInParent<EnemyController>();
            if (otherEnemy == null || otherEnemy == enemyController || !otherEnemy.IsAlive)
            {
                continue;
            }

            // 计算远离其他敌人的方向
            Vector2 away = currentPosition - (Vector2)otherEnemy.transform.position;
            float sqrDistance = away.sqrMagnitude;

            // 如果两个敌人完全重叠
            // 使用确定性方向避免除零和随机抖动
            if (sqrDistance <= 0.0001f)
            {
                away = GetDeterministicSeparationDirection(otherEnemy);
                sqrDistance = 0.0001f;
            }

            // 距离越近
            // 分离力越强
            float distance = Mathf.Sqrt(sqrDistance);
            float strength = 1f - Mathf.Clamp01(distance / separationDistance);
            separationDirection += away.normalized * strength;
        }

        return separationDirection;
    }

    /// <summary>
    /// 当两个敌人重叠时 生成一个稳定的分离方向
    /// 避免每帧使用随机方向导致抖动
    /// </summary>
    private Vector2 GetDeterministicSeparationDirection(EnemyController otherEnemy)
    {
        int hash = enemyController.GetInstanceID() ^ otherEnemy.GetInstanceID();
        float angle = (hash & 1023) * (Mathf.PI * 2f / 1024f);
        return new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
    }
}
