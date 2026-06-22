using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 掉落物品生成系统
/// </summary>
[DisallowMultipleComponent]
public class ItemDropGenerator : MonoBehaviour
{
    [SerializeField]
    [Tooltip("default drop table config")]
    private DropTableConfig defaultDropTableConfig;

    [SerializeField]
    [Min(0f)]
    [Tooltip("The radius within which items can be scattered when dropped.")]
    private float spawnScatterRadius = 0.35f;

    // 记录的掉落物池化系统字典
    // key为物品配置的ResourceId value为对应的组件池
    private readonly Dictionary<string, ComponentPool<PickupController>> pickupPools = new Dictionary<string, ComponentPool<PickupController>>();
    // 临时列表 用于记录掉落表的roll结果 避免重复创建列表对象
    private readonly List<ItemDropRoll> rolledDrops = new List<ItemDropRoll>();
    // 池化系统的根对象 用于组织池化对象
    private Transform poolRoot;
    // 活动掉落物的根对象 用于组织场景中的掉落物
    private Transform activeRoot;

    /// <summary>
    /// 根据敌人配置生成掉落物
    /// </summary>
    /// <param name="position">生成位置</param>
    /// <param name="enemyConfig">敌人配置</param>
    public void GenerateDrop(Vector3 position, EnemyConfig enemyConfig)
    {
        // 解析掉落表配置
        DropTableConfig dropTableConfig = ResolveDropTable(enemyConfig);
        if (dropTableConfig == null)
        {
            return;
        }

        // 进行掉落表roll 获取掉落结果
        dropTableConfig.RollDrops(rolledDrops);
        // 根据roll结果生成掉落物
        for (int i = 0; i < rolledDrops.Count; i++)
        {
            ItemDropRoll roll = rolledDrops[i];
            SpawnPickup(roll.ItemConfig, roll.Amount, position);
        }
    }

    /// <summary>
    /// 释放掉落物 将其返回对应的池中 以便下次重用
    /// </summary>
    /// <param name="pickup">需要释放的掉落物</param>
    public void ReleasePickup(PickupController pickup)
    {
        // 确保掉落物和它的物品配置有效 
        if (pickup == null || pickup.ItemConfig == null)
        {
            return;
        }

        // 获取掉落物对应的池的key 
        string poolKey = GetPoolKey(pickup.ItemConfig);
        // 如果对应的池存在 则将掉落物释放回池中
        if (pickupPools.TryGetValue(poolKey, out ComponentPool<PickupController> pool))
        {
            pool.Release(pickup);
            return;
        }

        // 否则直接禁用掉落物并将其父对象设置为池的根对象 以便在层级视图中组织和管理
        pickup.gameObject.SetActive(false);
        pickup.transform.SetParent(GetPoolRoot());
    }

    /// <summary>
    /// 生成掉落物
    /// </summary>
    /// <param name="itemConfig">物品配置</param>
    /// <param name="amount">掉落数量</param>
    /// <param name="position">掉落位置</param>
    private void SpawnPickup(ItemConfig itemConfig, int amount, Vector3 position)
    {
        // 如果物品配置无效或没有对应的拾取预制体 则不生成掉落物
        if (itemConfig == null || itemConfig.PickupPrefab == null)
        {
            return;
        }

        // 在生成位置附近随机一个点 作为掉落物的生成位置 以增加掉落的自然感
        Vector2 scatter = spawnScatterRadius > 0f ? Random.insideUnitCircle * spawnScatterRadius : Vector2.zero;
        Vector3 spawnPosition = position + new Vector3(scatter.x, scatter.y, 0f);
        // 从对应的池中获取一个拾取对象 初始化并激活它
        PickupController pickup = GetPool(itemConfig).Get(spawnPosition, Quaternion.identity, GetActiveRoot());
        pickup.Initialize(itemConfig, amount, this);
    }

    /// <summary>
    /// 解析掉落表配置 
    /// 优先使用敌人配置中的掉落表 如果没有则使用默认掉落表
    /// </summary>
    /// <param name="enemyConfig">敌人配置</param>
    /// <returns></returns>
    private DropTableConfig ResolveDropTable(EnemyConfig enemyConfig)
    {
        if (enemyConfig != null && enemyConfig.DropTableConfig != null)
        {
            return enemyConfig.DropTableConfig;
        }

        return defaultDropTableConfig;
    }

    /// <summary>
    /// 根据物品配置获取对应的组件池 如果池不存在则创建一个新的池
    /// </summary>
    /// <param name="itemConfig">物品配置</param>
    /// <returns></returns>
    private ComponentPool<PickupController> GetPool(ItemConfig itemConfig)
    {
        // 获取池的key 这里使用物品配置的ResourceId作为key 因为它是唯一标识一个物品配置的字符串
        string poolKey = GetPoolKey(itemConfig);
        if (!pickupPools.TryGetValue(poolKey, out ComponentPool<PickupController> pool))
        {
            pool = new ComponentPool<PickupController>(
                itemConfig.PickupPrefab,
                GetPoolRoot(),
                EnsurePickupRuntimeComponents);
            pickupPools.Add(poolKey, pool);
        }

        return pool;
    }

    /// <summary>
    /// 获取池的根对象 
    /// 如果不存在则创建一个新的空对象作为根对象 以便在层级视图中组织和管理池化对象
    /// </summary>
    /// <returns></returns>
    private Transform GetPoolRoot()
    {
        if (poolRoot == null)
        {
            GameObject poolRootObject = new GameObject("Pickup Pool");
            poolRootObject.transform.SetParent(transform);
            poolRoot = poolRootObject.transform;
        }

        return poolRoot;
    }

    /// <summary>
    /// 获取活动掉落物的根对象
    /// </summary>
    /// <returns></returns>
    private Transform GetActiveRoot()
    {
        if (activeRoot == null)
        {
            GameObject activeRootObject = new GameObject("Active Pickups");
            activeRootObject.transform.SetParent(transform);
            activeRoot = activeRootObject.transform;
        }

        return activeRoot;
    }

    /// <summary>
    /// 确保生成的拾取对象具有运行时所需的组件和配置 
    /// 包括Rigidbody2D Collider2D PickupController等
    /// </summary>
    /// <param name="pickupObject">拾取物对象</param>
    /// <returns></returns>
    private static PickupController EnsurePickupRuntimeComponents(GameObject pickupObject)
    {
        Rigidbody2D rigidBody2D = EnsureComponent<Rigidbody2D>(pickupObject);
        ConfigureRigidbody(rigidBody2D);
        ConfigureColliders(pickupObject);

        PickupController pickup = EnsureComponent<PickupController>(pickupObject);
        return pickup;
    }

    private static void ConfigureRigidbody(Rigidbody2D rigidBody2D)
    {
        rigidBody2D.gravityScale = 0f;
        rigidBody2D.freezeRotation = true;
        rigidBody2D.interpolation = RigidbodyInterpolation2D.Interpolate;
        rigidBody2D.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
    }

    private static void ConfigureColliders(GameObject pickupObject)
    {
        Collider2D[] colliders = pickupObject.GetComponentsInChildren<Collider2D>(true);
        if (colliders.Length == 0)
        {
            CircleCollider2D circleCollider2D = pickupObject.AddComponent<CircleCollider2D>();
            circleCollider2D.radius = 0.2f;
            circleCollider2D.isTrigger = true;
            return;
        }

        for (int i = 0; i < colliders.Length; i++)
        {
            colliders[i].isTrigger = true;
        }
    }

    private static T EnsureComponent<T>(GameObject gameObject) where T : Component
    {
        T component = gameObject.GetComponent<T>();
        return component != null ? component : gameObject.AddComponent<T>();
    }

    /// <summary>
    /// 获取池的key 
    /// 这里使用物品配置的ResourceId作为key 因为它是唯一标识一个物品配置的字符串
    /// </summary>
    /// <param name="itemConfig">物品配置</param>
    /// <returns></returns>
    private static string GetPoolKey(ItemConfig itemConfig)
    {
        return itemConfig.ResourceId;
    }
}
