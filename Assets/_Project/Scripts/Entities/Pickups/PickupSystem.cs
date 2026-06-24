using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 拾取系统。
/// 负责记录场景中的活动拾取物，应用拾取效果，并通过对象池播放拾取反馈特效
/// </summary>
public static class PickupSystem
{
    // 当前场景中可被玩家吸附或拾取的物品列表
    private static readonly List<PickupController> activePickups = new List<PickupController>();
    // 按特效预制体缓存对象池，避免拾取频繁实例化特效对象
    private static readonly Dictionary<int, ComponentPool<PickupEffectController>> effectPools = new Dictionary<int, ComponentPool<PickupEffectController>>();

    // 未使用拾取特效对象的根节点
    private static Transform effectPoolRoot;
    // 正在播放拾取特效对象的根节点
    private static Transform activeEffectRoot;

    public static IReadOnlyList<PickupController> ActivePickups => activePickups;

    /// <summary>
    /// 注册一个拾取物，供玩家拾取系统扫描
    /// </summary>
    /// <param name="pickup">拾取物控制器</param>
    public static void RegisterPickup(PickupController pickup)
    {
        if (pickup != null && !activePickups.Contains(pickup))
        {
            activePickups.Add(pickup);
        }
    }

    /// <summary>
    /// 注销一个拾取物
    /// </summary>
    /// <param name="pickup">拾取物控制器</param>
    public static void UnregisterPickup(PickupController pickup)
    {
        activePickups.Remove(pickup);
    }

    /// <summary>
    /// 完成拾取物收集流程
    /// 先应用数值效果，再播放拾取视觉反馈
    /// </summary>
    /// <param name="player">拾取该物品的玩家。</param>
    /// <param name="itemConfig">物品配置。</param>
    /// <param name="amount">拾取数量。</param>
    public static void CompletePickup(Player player, ItemConfig itemConfig, int amount)
    {
        ApplyPickup(player, itemConfig, amount);
        PlayPickupEffect(player, itemConfig);
    }

    /// <summary>
    /// 根据物品配置将拾取效果应用到玩家或关卡统计
    /// </summary>
    /// <param name="player">拾取该物品的玩家</param>
    /// <param name="itemConfig">物品配置</param>
    /// <param name="amount">拾取数量</param>
    public static void ApplyPickup(Player player, ItemConfig itemConfig, int amount)
    {
        if (player == null || itemConfig == null)
        {
            return;
        }

        float totalAmount = Mathf.Max(1, amount) * itemConfig.EffectAmount;

        switch (itemConfig.EffectType)
        {
            case ItemEffectType.Experience:
                if (player.PlayerProgress != null)
                {
                    player.PlayerProgress.AddExperience(Mathf.RoundToInt(totalAmount));
                }
                break;

            case ItemEffectType.Score:
                if (player.PlayerProgress != null)
                {
                    player.PlayerProgress.AddScore(Mathf.RoundToInt(totalAmount));
                }
                break;

            case ItemEffectType.Heal:
                if (player.Health != null)
                {
                    player.Health.Heal(totalAmount);
                }
                break;

            case ItemEffectType.Gold:
                // 金币属于本局收益，由 LevelManager 监听后刷新 GamePanel 并在结算时持久化
                EventCenter.Instance.EventTrigger(E_EventType.GoldPickedUp, Mathf.RoundToInt(totalAmount));
                break;
        }
    }

    /// <summary>
    /// 播放拾取物配置中的视觉反馈特效
    /// </summary>
    /// <param name="player">拾取该物品的玩家</param>
    /// <param name="itemConfig">物品配置</param>
    private static void PlayPickupEffect(Player player, ItemConfig itemConfig)
    {
        if (player == null || itemConfig == null || itemConfig.PickupEffectPrefab == null)
        {
            return;
        }

        GameObject effectPrefab = itemConfig.PickupEffectPrefab;
        PickupEffectController effect = GetEffectPool(effectPrefab).Get(
            player.transform.position,
            Quaternion.identity,
            GetActiveEffectRoot());

        effect.PlayAndRelease(effectController => ReleasePickupEffect(effectPrefab, effectController));
    }

    /// <summary>
    /// 将拾取特效实例释放回对应对象池
    /// </summary>
    /// <param name="effectPrefab">特效预制体</param>
    /// <param name="effect">特效控制器实例</param>
    private static void ReleasePickupEffect(GameObject effectPrefab, PickupEffectController effect)
    {
        if (effect == null)
        {
            return;
        }

        int poolKey = GetEffectPoolKey(effectPrefab);
        if (effectPools.TryGetValue(poolKey, out ComponentPool<PickupEffectController> pool))
        {
            pool.Release(effect);
            return;
        }

        effect.gameObject.SetActive(false);
        effect.transform.SetParent(GetEffectPoolRoot());
    }

    /// <summary>
    /// 获取指定特效预制体对应的对象池，不存在时创建
    /// </summary>
    /// <param name="effectPrefab">特效预制体</param>
    /// <returns>拾取特效对象池</returns>
    private static ComponentPool<PickupEffectController> GetEffectPool(GameObject effectPrefab)
    {
        int poolKey = GetEffectPoolKey(effectPrefab);
        if (!effectPools.TryGetValue(poolKey, out ComponentPool<PickupEffectController> pool))
        {
            pool = new ComponentPool<PickupEffectController>(
                effectPrefab,
                GetEffectPoolRoot(),
                EnsurePickupEffectRuntimeComponents);
            effectPools.Add(poolKey, pool);
        }

        return pool;
    }

    /// <summary>
    /// 确保特效实例上存在 PickupEffectController
    /// </summary>
    /// <param name="effectObject">特效实例对象</param>
    /// <returns>拾取特效控制器</returns>
    private static PickupEffectController EnsurePickupEffectRuntimeComponents(GameObject effectObject)
    {
        PickupEffectController controller = effectObject.GetComponent<PickupEffectController>();
        return controller != null ? controller : effectObject.AddComponent<PickupEffectController>();
    }

    /// <summary>
    /// 获取未使用特效实例的对象池根节点
    /// </summary>
    /// <returns>对象池根节点</returns>
    private static Transform GetEffectPoolRoot()
    {
        if (effectPoolRoot == null)
        {
            GameObject poolRootObject = new GameObject("Pickup Effect Pool");
            effectPoolRoot = poolRootObject.transform;
        }

        return effectPoolRoot;
    }

    /// <summary>
    /// 获取正在播放的拾取特效根节点
    /// </summary>
    /// <returns>活动特效根节点</returns>
    private static Transform GetActiveEffectRoot()
    {
        if (activeEffectRoot == null)
        {
            GameObject activeRootObject = new GameObject("Active Pickup Effects");
            Player player = Generator.Instance.CharacterSelectionSystem.ActivePlayer;
            activeRootObject.transform.SetParent(player.gameObject.transform);
            activeEffectRoot = activeRootObject.transform;
        }

        return activeEffectRoot;
    }

    /// <summary>
    /// 根据特效预制体实例 id 生成对象池键
    /// </summary>
    /// <param name="effectPrefab">特效预制体</param>
    /// <returns>对象池键</returns>
    private static int GetEffectPoolKey(GameObject effectPrefab)
    {
        return effectPrefab != null ? effectPrefab.GetInstanceID() : 0;
    }
}
