using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 玩家武器系统 
/// 负责管理玩家的武器库存和攻击逻辑
/// </summary>
[RequireComponent(typeof(AttackEvent))]
[DisallowMultipleComponent]
public class PlayerWeaponSystem : MonoBehaviour, IWeaponInventory
{
    // 初始武器配置数组
    // 用于在游戏开始时为玩家装备默认的武器
    [SerializeField]
    private WeaponConfig[] startingWeaponConfigs;

    // 发射点变换 用于确定武器发射子弹的位置
    [SerializeField]
    private Transform firePoint;

    // 活跃的武器实例列表 包含玩家当前装备的所有武器实例
    private readonly List<WeaponRuntimeInstance> activeWeapons = new List<WeaponRuntimeInstance>(4);
    // 投射物池字典 用于管理不同类型的投射物对象池
    private readonly Dictionary<GameObject, ComponentPool<ProjectileController>> projectilePools =
        new Dictionary<GameObject, ComponentPool<ProjectileController>>(4);
    // 活跃的投射物池字典 用于跟踪当前正在使用的投射物及其对应的对象池
    private readonly Dictionary<ProjectileController, ComponentPool<ProjectileController>> activeProjectilePools =
        new Dictionary<ProjectileController, ComponentPool<ProjectileController>>(32);

    // 攻击事件组件 用于处理攻击相关的事件和回调
    private AttackEvent attackEvent;
    // 攻击服务类 提供攻击执行的辅助方法和逻辑
    private WeaponAttackServices attackServices;
    // 投射物池根变换 用于组织所有投射物对象池的父对象
    private Transform projectilePoolRoot;
    // 标记是否已经初始化了起始武器 避免重复添加
    private bool hasInitializedStartingWeapons;

    // 活跃的武器列表 只读属性 提供外部访问当前玩家装备的所有武器实例
    public IReadOnlyList<WeaponRuntimeInstance> ActiveWeapons => activeWeapons;

    private void Awake()
    {
        // 获取攻击事件组件并创建攻击服务实例
        attackEvent = GetComponent<AttackEvent>();
        attackServices = new WeaponAttackServices(this);
    }

    private void OnEnable()
    {
        // 如果尚未初始化起始武器且当前没有任何活跃武器 则添加起始武器配置中的武器实例
        if (!hasInitializedStartingWeapons && activeWeapons.Count == 0)
        {
            AddStartingWeapons();
            hasInitializedStartingWeapons = true;
        }

        // 订阅攻击事件的回调方法 当攻击事件被触发时调用AttackEvent_OnAttack方法处理攻击逻辑
        if (attackEvent != null)
        {
            attackEvent.OnAttack += AttackEvent_OnAttack;
        }
    }

    private void OnDisable()
    {
        // 取消订阅攻击事件的回调方法 避免在对象禁用时继续处理攻击事件
        if (attackEvent != null)
        {
            attackEvent.OnAttack -= AttackEvent_OnAttack;
        }
    }

    private void Update()
    {
        // 在每帧更新中处理武器的冷却逻辑
        // 遍历所有活跃的武器实例 更新它们的冷却时间 并尝试执行攻击
        float deltaTime = Time.deltaTime;
        for (int i = 0; i < activeWeapons.Count; i++)
        {
            WeaponRuntimeInstance weapon = activeWeapons[i];
            if (weapon == null || weapon.Config == null || !weapon.IsEnabled)
            {
                continue;
            }

            weapon.TickCooldown(deltaTime);
            if (weapon.IsCoolingDown)
            {
                continue;
            }

            if (TryAttack(weapon))
            {
                weapon.ResetCooldown();
            }
        }
    }

    /// <summary>
    /// 初始化起始武器方法 用于在游戏开始时为玩家装备默认的武器实例
    /// </summary>
    /// <param name="config">武器配置</param>
    internal void InitializeStartingWeapon(WeaponConfig config)
    {
        // 清除当前活跃的武器实例列表 确保只有起始武器被添加
        activeWeapons.Clear();
        if (config != null)
        {
            AddWeapon(config);
        }

        // 设置标记表示已经初始化了起始武器 避免重复添加
        hasInitializedStartingWeapons = true;
    }

    /// <summary>
    /// 添加武器方法 用于将新的武器实例添加到玩家的活跃武器列表中
    /// </summary>
    /// <param name="config">武器配置</param>
    /// <returns></returns>
    public bool AddWeapon(WeaponConfig config)
    {
        return AddWeapon(config, true);
    }

    /// <summary>
    /// 添加武器方法 重载版本 允许指定是否重置武器的冷却时间
    /// </summary>
    /// <param name="config">武器配置</param>
    /// <param name="resetCooldown">是否重置冷却时间</param>
    /// <returns></returns>
    public bool AddWeapon(WeaponConfig config, bool resetCooldown)
    {
        // 如果配置为null或者玩家已经拥有该武器 则返回false表示添加失败
        if (config == null || HasWeapon(config))
        {
            return false;
        }

        // 根据resetCooldown参数决定初始冷却时间
        // 如果需要重置则设置为0 否则使用配置中的默认冷却时间
        float initialCooldown = resetCooldown ? 0f : config.Cooldown;
        activeWeapons.Add(new WeaponRuntimeInstance(config, initialCooldown));
        return true;
    }

    /// <summary>
    /// 移除武器方法 用于从玩家的活跃武器列表中移除指定的武器实例
    /// </summary>
    /// <param name="config">武器配置</param>
    /// <returns></returns>
    public bool RemoveWeapon(WeaponConfig config)
    {
        if (config == null)
        {
            return false;
        }

        return RemoveWeapon(config.ResourceId);
    }

    /// <summary>
    /// 移除武器方法 重载版本 允许通过资源ID来指定要移除的武器实例
    /// </summary>
    /// <param name="resourceId">武器资源ID</param>
    /// <returns></returns>
    public bool RemoveWeapon(string resourceId)
    {
        if (string.IsNullOrWhiteSpace(resourceId))
        {
            return false;
        }

        for (int i = activeWeapons.Count - 1; i >= 0; i--)
        {
            WeaponRuntimeInstance weapon = activeWeapons[i];
            if (weapon?.Config == null)
            {
                activeWeapons.RemoveAt(i);
                continue;
            }

            if (weapon.Config.ResourceId == resourceId)
            {
                activeWeapons.RemoveAt(i);
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// 检查玩家是否拥有指定的武器实例
    /// </summary>
    /// <param name="config">武器配置</param>
    /// <returns></returns>
    public bool HasWeapon(WeaponConfig config)
    {
        if (config == null)
        {
            return false;
        }

        return HasWeapon(config.ResourceId);
    }

    /// <summary>
    /// 检查玩家是否拥有指定资源ID的武器实例
    /// </summary>
    /// <param name="resourceId">武器资源ID</param>
    /// <returns></returns>
    public bool HasWeapon(string resourceId)
    {
        if (string.IsNullOrWhiteSpace(resourceId))
        {
            return false;
        }

        for (int i = 0; i < activeWeapons.Count; i++)
        {
            WeaponConfig config = activeWeapons[i]?.Config;
            if (config != null && config.ResourceId == resourceId)
            {
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// 发射投射物方法 
    /// 用于根据攻击上下文中的信息创建并发射一个新的投射物实例
    /// </summary>
    /// <param name="context">攻击上下文</param>
    /// <param name="target">目标敌人</param>
    internal void FireProjectile(AttackContext context, Enemy target)
    {
        // 如果攻击上下文中的投射物预制体为null或者目标敌人不存在或已死亡
        // 则不执行发射逻辑 直接返回
        if (context.ProjectilePrefab == null || target == null || !target.IsAlive)
        {
            return;
        }

        // 获取对应投射物预制体的对象池 
        ComponentPool<ProjectileController> pool = GetProjectilePool(context.ProjectilePrefab);
        // 从池中获取一个投射物实例 
        ProjectileController projectile = pool.Get(context.Origin, Quaternion.identity, null);
        // 并将其与当前活跃的投射物池关联起来
        activeProjectilePools[projectile] = pool;
        projectile.Launch(target, context, ReleaseProjectile);
    }

    /// <summary>
    /// 添加起始武器方法 
    /// 用于在游戏开始时根据配置数组为玩家装备默认的武器实例
    /// </summary>
    private void AddStartingWeapons()
    {
        if (startingWeaponConfigs == null)
        {
            return;
        }

        for (int i = 0; i < startingWeaponConfigs.Length; i++)
        {
            AddWeapon(startingWeaponConfigs[i]);
        }
    }

    /// <summary>
    /// 尝试攻击方法
    /// </summary>
    /// <param name="weapon">武器运行时实例</param>
    /// <returns></returns>
    private bool TryAttack(WeaponRuntimeInstance weapon)
    {
        WeaponConfig config = weapon.Config;
        Enemy target = TargetingSystem.FindNearestAliveEnemy(transform.position, config.Range);
        if (target == null)
        {
            return false;
        }

        Vector3 firePosition = firePoint != null ? firePoint.position : transform.position;
        Vector3 direction = target.transform.position - firePosition;
        AttackContext context = new AttackContext(
            gameObject,
            target.gameObject,
            firePosition,
            direction,
            config.Damage,
            config.Range,
            config.ProjectilePrefab,
            config.ProjectileSpeed,
            config.ProjectileHitRadius,
            config);

        attackEvent.CallAttackEvent(context);
        return true;
    }

    /// <summary>
    /// 攻击事件回调方法
    /// </summary>
    /// <param name="eventSource">攻击事件源</param>
    /// <param name="context">攻击上下文</param>
    private void AttackEvent_OnAttack(AttackEvent eventSource, AttackContext context)
    {
        if (context.AttackerObject != gameObject)
        {
            return;
        }

        WeaponConfig contextWeaponConfig = context.WeaponConfig;
        if (contextWeaponConfig == null)
        {
            return;
        }

        if (contextWeaponConfig.AttackStrategy == null)
        {
            Debug.LogError($"{nameof(WeaponConfig)} '{contextWeaponConfig.name}' requires a {nameof(WeaponAttackStrategy)}.", contextWeaponConfig);
            return;
        }

        contextWeaponConfig.AttackStrategy.TryExecute(context, attackServices);
    }

    /// <summary>
    /// 获取投射物对象池方法
    /// </summary>
    /// <param name="projectilePrefab">投射物预设体</param>
    /// <returns></returns>
    private ComponentPool<ProjectileController> GetProjectilePool(GameObject projectilePrefab)
    {
        if (projectilePools.TryGetValue(projectilePrefab, out ComponentPool<ProjectileController> pool))
        {
            return pool;
        }

        pool = new ComponentPool<ProjectileController>(
            projectilePrefab,
            GetProjectilePoolRoot(),
            EnsureProjectileRuntimeComponents);
        projectilePools.Add(projectilePrefab, pool);
        return pool;
    }

    /// <summary>
    /// 释放投射物方法 
    /// 用于将不再需要的投射物实例返回到对应的对象池中以供后续重用
    /// </summary>
    /// <param name="projectile">投射物控制器</param>
    private void ReleaseProjectile(ProjectileController projectile)
    {
        if (projectile == null)
        {
            return;
        }

        if (activeProjectilePools.TryGetValue(projectile, out ComponentPool<ProjectileController> pool))
        {
            activeProjectilePools.Remove(projectile);
            pool.Release(projectile);
        }
    }

    /// <summary>
    /// 得到投射物对象池根物体
    /// </summary>
    /// <returns></returns>
    private Transform GetProjectilePoolRoot()
    {
        if (projectilePoolRoot == null)
        {
            GameObject poolRootObject = new GameObject($"{name} Projectile Pool");
            projectilePoolRoot = poolRootObject.transform;
        }

        return projectilePoolRoot;
    }

    /// <summary>
    /// 确保投射物运行时组件方法
    /// </summary>
    /// <param name="projectileObject">投射物gameobject</param>
    /// <returns></returns>
    private static ProjectileController EnsureProjectileRuntimeComponents(GameObject projectileObject)
    {
        Rigidbody2D rigidBody2D = EnsureComponent<Rigidbody2D>(projectileObject);
        rigidBody2D.gravityScale = 0f;
        rigidBody2D.freezeRotation = true;
        rigidBody2D.interpolation = RigidbodyInterpolation2D.Interpolate;
        rigidBody2D.collisionDetectionMode = CollisionDetectionMode2D.Continuous;

        Collider2D collider2D = projectileObject.GetComponent<Collider2D>();
        if (collider2D == null)
        {
            collider2D = projectileObject.AddComponent<CircleCollider2D>();
        }

        collider2D.isTrigger = true;
        return EnsureComponent<ProjectileController>(projectileObject);
    }

    /// <summary>
    /// 确保组件方法
    /// </summary>
    /// <typeparam name="T">需要添加的组件</typeparam>
    /// <param name="gameObject">需要添加组件的GameObject</param>
    /// <returns></returns>
    private static T EnsureComponent<T>(GameObject gameObject) where T : Component
    {
        T component = gameObject.GetComponent<T>();
        return component != null ? component : gameObject.AddComponent<T>();
    }
}
