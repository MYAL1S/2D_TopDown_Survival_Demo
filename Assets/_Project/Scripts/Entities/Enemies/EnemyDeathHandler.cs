using System.Collections;
using UnityEngine;

/// <summary>
/// 敌人死亡处理组件
/// 监听 DeathEvent，触发击杀统计、播放死亡特效，并将敌人释放回对象池
/// </summary>
[RequireComponent(typeof(DeathEvent))]
[DisallowMultipleComponent]
public class EnemyDeathHandler : MonoBehaviour
{
    // 敌人死亡时生成的特效预制体
    [SerializeField]
    private GameObject deathEffectPrefab;

    // 死亡后延迟回收到对象池的时间
    [SerializeField]
    [Min(0f)]
    private float destroyDelay = 0.15f;

    private DeathEvent deathEvent;
    private Rigidbody2D rigidBody2D;
    private Collider2D[] colliders;
    private Enemy enemy;
    // 防止同一个敌人的死亡逻辑被重复处理
    private bool isHandlingDeath;

    private void Awake()
    {
        deathEvent = GetComponent<DeathEvent>();
        rigidBody2D = GetComponent<Rigidbody2D>();
        enemy = GetComponent<Enemy>();
        colliders = GetComponentsInChildren<Collider2D>(true);
    }

    private void OnEnable()
    {
        isHandlingDeath = false;
        SetCollidersEnabled(true);
        deathEvent.OnDeath += DeathEvent_OnDeath;
    }

    private void OnDisable()
    {
        deathEvent.OnDeath -= DeathEvent_OnDeath;
    }

    private void DeathEvent_OnDeath(DeathEvent eventSource)
    {
        if (isHandlingDeath)
        {
            return;
        }

        isHandlingDeath = true;
        // 通知关卡管理器更新击杀数量
        EventCenter.Instance.EventTrigger(E_EventType.EnemyKilled, enemy);
        StartCoroutine(DeathRoutine());

        Generator generator = Generator.Instance;
        if (generator != null && generator.SpawnSystem != null && enemy != null)
        {
            generator.SpawnSystem.AddToDeadEnemies(enemy.GetInstanceID());
        }
    }

    /// <summary>
    /// 执行敌人死亡后的物理停止、碰撞禁用、特效播放和对象池回收
    /// </summary>
    private IEnumerator DeathRoutine()
    {
        if (rigidBody2D != null)
        {
            rigidBody2D.velocity = Vector2.zero;
        }

        SetCollidersEnabled(false);

        if (deathEffectPrefab != null)
        {
            Instantiate(deathEffectPrefab, transform.position, Quaternion.identity);
        }

        if (destroyDelay > 0f)
        {
            yield return new WaitForSeconds(destroyDelay);
        }

        ReturnToPool();
    }

    /// <summary>
    /// 批量启用或禁用敌人的碰撞器
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

    /// <summary>
    /// 优先将敌人释放回 EnemySpawnManager 的对象池，没有池时退化为禁用对象
    /// </summary>
    private void ReturnToPool()
    {
        if (enemy == null)
        {
            enemy = GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.CacheComponents();
            }
        }

        Generator generator = Generator.Instance;
        if (generator != null && generator.EnemySpawnManager != null && enemy != null)
        {
            generator.EnemySpawnManager.ReleaseEnemy(enemy);
            return;
        }

        gameObject.SetActive(false);
    }
}
