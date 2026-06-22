using UnityEngine;

/// <summary>
/// 拾取物控制器
/// </summary>
[DisallowMultipleComponent]
public class PickupController : MonoBehaviour, IPoolable
{
    // 吸引物品时的默认收集距离
    [SerializeField]
    [Min(0.01f)]
    private float collectDistance = 0.35f;

    // 当前物品配置
    private ItemConfig itemConfig;
    // 当前物品数量
    private int amount;
    // 生成该物品的掉落系统
    private ItemDropGenerator owner;
    // 吸引目标
    private Transform attractTarget;
    // 吸引速度
    private float attractSpeed;
    // 是否已被收集
    private bool isCollected;
    // 刚体
    private Rigidbody2D rigidBody2D;
    // 碰撞器数组
    private Collider2D[] colliders;

    public ItemConfig ItemConfig => itemConfig;
    public int Amount => amount;

    private void Awake()
    {
        CacheComponents();
    }

    private void Update()
    {
        // 如果已被收集或没有吸引目标 则不进行移动
        if (isCollected || attractTarget == null)
        {
            return;
        }

        // 向吸引目标移动
        transform.position = Vector3.MoveTowards(
            transform.position,
            attractTarget.position,
            attractSpeed * Time.deltaTime);

        // 如果移动后与吸引目标的距离小于等于收集距离 则尝试收集
        if ((transform.position - attractTarget.position).sqrMagnitude <= collectDistance * collectDistance)
        {
            TryCollect(attractTarget.GetComponentInParent<Player>());
        }
    }

    /// <summary>
    /// 初始化拾取物
    /// </summary>
    /// <param name="config">物品配置</param>
    /// <param name="dropAmount">掉落数量</param>
    /// <param name="dropOwner">掉落系统</param>
    public void Initialize(ItemConfig config, int dropAmount, ItemDropGenerator dropOwner)
    {
        itemConfig = config;
        amount = Mathf.Max(1, dropAmount);
        owner = dropOwner;
        isCollected = false;
        attractTarget = null;
        attractSpeed = 0f;
        SetCollidersEnabled(true);
    }

    /// <summary>
    /// 开始吸引物品
    /// </summary>
    /// <param name="player">玩家</param>
    /// <param name="speed">吸引速度</param>
    /// <param name="pickupCollectDistance">拾取距离</param>
    public void BeginAttract(Player player, float speed, float pickupCollectDistance)
    {
        if (isCollected || player == null)
        {
            return;
        }

        // 设置吸引目标为玩家的Transform 
        attractTarget = player.transform;
        // 吸引速度为指定速度 
        attractSpeed = Mathf.Max(0f, speed);
        // 收集距离为指定距离
        collectDistance = Mathf.Max(0.01f, pickupCollectDistance);
    }

    /// <summary>
    /// 尝试收集物品
    /// </summary>
    /// <param name="player">玩家</param>
    public void TryCollect(Player player)
    {
        // 如果已被收集、玩家为null或物品配置为null 则不进行收集
        if (isCollected || player == null || itemConfig == null)
        {
            return;
        }

        // 标记为已收集 禁用碰撞器 完成拾取逻辑
        isCollected = true;
        SetCollidersEnabled(false);
        PickupSystem.CompletePickup(player, itemConfig, amount);

        // 如果记录了掉落系统 则通知掉落系统释放该物品 否则直接禁用该物品
        if (owner != null)
        {
            owner.ReleasePickup(this);
        }
        else
        {
            gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// 当从对象池中生成时调用
    /// </summary>
    public void OnSpawnedFromPool()
    {
        // 重新缓存组件 启用碰撞器 注册到拾取系统
        CacheComponents();
        SetCollidersEnabled(true);
        PickupSystem.RegisterPickup(this);
    }

    /// <summary>
    /// 当返回对象池时调用
    /// </summary>
    public void OnReturnedToPool()
    {
        // 取消注册 重置状态 禁用碰撞器
        PickupSystem.UnregisterPickup(this);
        itemConfig = null;
        amount = 0;
        owner = null;
        attractTarget = null;
        attractSpeed = 0f;
        isCollected = false;

        if (rigidBody2D != null)
        {
            rigidBody2D.velocity = Vector2.zero;
            rigidBody2D.angularVelocity = 0f;
        }

        SetCollidersEnabled(false);
    }

    /// <summary>
    /// 失活时取消注册到拾取系统 以防止在失活期间被误操作
    /// </summary>
    private void OnDisable()
    {
        PickupSystem.UnregisterPickup(this);
    }

    /// <summary>
    /// 缓存组件
    /// </summary>
    private void CacheComponents()
    {
        if (rigidBody2D == null)
        {
            rigidBody2D = GetComponent<Rigidbody2D>();
        }

        colliders = GetComponentsInChildren<Collider2D>(true);
    }

    /// <summary>
    /// 启用或禁用碰撞器
    /// </summary>
    /// <param name="isEnabled">是否启用碰撞器</param>
    private void SetCollidersEnabled(bool isEnabled)
    {
        if (colliders == null)
        {
            return;
        }

        for (int i = 0; i < colliders.Length; i++)
        {
            if (colliders[i] != null)
            {
                colliders[i].enabled = isEnabled;
            }
        }
    }
}
