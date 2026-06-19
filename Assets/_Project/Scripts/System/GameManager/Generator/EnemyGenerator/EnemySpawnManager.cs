using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 敌人生成管理器
/// 使用对象池模式管理敌人生成与回收
/// 以降低频繁创销的性能开销
/// </summary>
[DisallowMultipleComponent]
public class EnemySpawnManager : MonoBehaviour
{
    // 按敌人配置维护多个独立的对象池
    private readonly Dictionary<string, ComponentPool<EnemyController>> enemyPools = new Dictionary<string, ComponentPool<EnemyController>>();
    // 所有对象池的父节点 便于层级管理
    private Transform poolRoot;

    /// <summary>
    /// 使用默认朝向和无父对象生成敌人
    /// </summary>
    public EnemyController SpawnEnemy(EnemyConfig config, Vector3 position, Transform target)
    {
        return SpawnEnemy(config, position, Quaternion.identity, null, target);
    }

    /// <summary>
    /// 按指定位置、旋转和父对象从对象池获取敌人 并初始化其配置与追踪目标
    /// </summary>
    public EnemyController SpawnEnemy(EnemyConfig config, Vector3 position, Quaternion rotation, Transform parent, Transform target)
    {
        if (config == null || config.enemyPrefab == null)
        {
            Debug.LogError($"{nameof(EnemySpawnManager)} could not spawn enemy. Missing config or prefab.", this);
            return null;
        }

        EnemyController enemyController = GetPool(config).Get(position, rotation, parent);
        enemyController.Initialize(config, target);
        return enemyController;
    }

    /// <summary>
    /// 将敌人归还到对象池
    /// 若敌人配置丢失或无对应的池 则直接禁用
    /// </summary>
    public void ReleaseEnemy(EnemyController enemyController)
    {
        if (enemyController == null)
        {
            return;
        }

        EnemyConfig config = enemyController.Config;
        if (config == null)
        {
            enemyController.gameObject.SetActive(false);
            enemyController.transform.SetParent(GetPoolRoot());
            return;
        }

        string poolKey = GetPoolKey(config);
        if (enemyPools.TryGetValue(poolKey, out ComponentPool<EnemyController> pool))
        {
            pool.Release(enemyController);
            return;
        }

        enemyController.gameObject.SetActive(false);
        enemyController.transform.SetParent(GetPoolRoot());
    }

    /// <summary>
    /// 获取或创建指定配置对应的对象池
    /// </summary>
    private ComponentPool<EnemyController> GetPool(EnemyConfig config)
    {
        string poolKey = GetPoolKey(config);
        if (!enemyPools.TryGetValue(poolKey, out ComponentPool<EnemyController> pool))
        {
            pool = new ComponentPool<EnemyController>(
                config.enemyPrefab,
                GetPoolRoot(),
                EnsureEnemyRuntimeComponents);
            enemyPools.Add(poolKey, pool);
        }

        return pool;
    }

    /// <summary>
    /// 获取或创建对象池根节点 用作所有敌人池的容器
    /// </summary>
    private Transform GetPoolRoot()
    {
        if (poolRoot == null)
        {
            GameObject poolRootObject = new GameObject("Enemy Pool");
            poolRootObject.transform.SetParent(transform);
            poolRoot = poolRootObject.transform;
        }

        return poolRoot;
    }

    /// <summary>
    /// 根据敌人配置生成对象池的唯一键
    /// </summary>
    private static string GetPoolKey(EnemyConfig config)
    {
        return config.ResourceId;
    }

    /// <summary>
    /// 确保敌人物体具备运行所需的所有基础组件
    /// </summary>
    private static EnemyController EnsureEnemyRuntimeComponents(GameObject enemyObject)
    {
        Rigidbody2D rigidBody2D = EnsureComponent<Rigidbody2D>(enemyObject);
        ConfigureRigidbody(rigidBody2D);
        ConfigureColliders(enemyObject);

        EnsureComponent<InjuredEvent>(enemyObject);
        EnsureComponent<DeathEvent>(enemyObject);
        EnsureComponent<Health>(enemyObject);
        EnemyController enemyController = EnsureComponent<EnemyController>(enemyObject);
        EnsureComponent<EnemyAISystem>(enemyObject);

        EnsureComponent<EnemyInjuredHandler>(enemyObject);
        EnsureComponent<EnemyDeathHandler>(enemyObject);
        EnsureComponent<EnemyDropHandler>(enemyObject);

        return enemyController;
    }

    /// <summary>
    /// 配置敌人的二维刚体参数
    /// 确保移动和碰撞表现符合预期
    /// </summary>
    private static void ConfigureRigidbody(Rigidbody2D rigidBody2D)
    {
        rigidBody2D.gravityScale = 0f;
        rigidBody2D.freezeRotation = true;
        rigidBody2D.interpolation = RigidbodyInterpolation2D.Interpolate;
        rigidBody2D.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
    }

    /// <summary>
    /// 将敌人及其子物体上的碰撞体设置为触发器
    /// </summary>
    private static void ConfigureColliders(GameObject enemyObject)
    {
        Collider2D[] colliders = enemyObject.GetComponentsInChildren<Collider2D>(true);
        for (int i = 0; i < colliders.Length; i++)
        {
            colliders[i].isTrigger = true;
        }
    }

    /// <summary>
    /// 确保目标组件存在于游戏对象上
    /// 若不存在则自动添加
    /// </summary>
    private static T EnsureComponent<T>(GameObject gameObject) where T : Component
    {
        T component = gameObject.GetComponent<T>();
        return component != null ? component : gameObject.AddComponent<T>();
    }
}
