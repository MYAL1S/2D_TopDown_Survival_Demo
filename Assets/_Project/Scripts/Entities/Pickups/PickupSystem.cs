using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

/// <summary>
/// 拾取系统 
/// 负责管理游戏中的拾取物品的注册 注销 效果应用和视觉反馈
/// </summary>
public static class PickupSystem
{
    private static readonly List<PickupController> activePickups = new List<PickupController>();
    private static readonly Dictionary<int, ComponentPool<PickupEffectController>> effectPools = new Dictionary<int, ComponentPool<PickupEffectController>>();

    private static Transform effectPoolRoot;
    private static Transform activeEffectRoot;

    public static IReadOnlyList<PickupController> ActivePickups => activePickups;

    /// <summary>
    /// 注册一个新的拾取物品到系统中 
    /// 将其添加到活动拾取物品列表中 以便系统能够跟踪和管理它
    /// </summary>
    /// <param name="pickup"></param>
    public static void RegisterPickup(PickupController pickup)
    {
        if (pickup != null && !activePickups.Contains(pickup))
        {
            activePickups.Add(pickup);
        }
    }

    /// <summary>
    /// 从系统中注销一个拾取物品
    /// </summary>
    /// <param name="pickup"></param>
    public static void UnregisterPickup(PickupController pickup)
    {
        activePickups.Remove(pickup);
    }

    /// <summary>
    /// 完成拾取物品的效果应用和视觉反馈
    /// </summary>
    /// <param name="player">玩家</param>
    /// <param name="itemConfig">物品配置</param>
    /// <param name="amount">数量</param>
    public static void CompletePickup(Player player, ItemConfig itemConfig, int amount)
    {
        ApplyPickup(player, itemConfig, amount);
        PlayPickupEffect(player, itemConfig);
    }

    /// <summary>
    /// 根据物品配置和数量应用拾取效果到玩家
    /// </summary>
    /// <param name="player">玩家</param>
    /// <param name="itemConfig">物品配置</param>
    /// <param name="amount">数量</param>
    public static void ApplyPickup(Player player, ItemConfig itemConfig, int amount)
    {
        // 确保玩家和物品配置有效
        if (player == null || itemConfig == null)
        {
            return;
        }

        // 计算总效果量 确保至少为1
        float totalAmount = Mathf.Max(1, amount) * itemConfig.EffectAmount;

        // 根据物品效果类型应用对应的效果
        switch (itemConfig.EffectType)
        {
            // 经验值效果
            case ItemEffectType.Experience:
                if (player.PlayerProgress != null)
                {
                    // 将总效果量四舍五入为整数并添加到玩家的经验值中
                    player.PlayerProgress.AddExperience(Mathf.RoundToInt(totalAmount));
                }
                break;
            case ItemEffectType.Score:
                if (player.PlayerProgress != null)
                {
                    // 将总效果量四舍五入为整数并添加到玩家的经验值中
                    player.PlayerProgress.AddScore(Mathf.RoundToInt(totalAmount));
                }
                break;

            // 治疗效果
            case ItemEffectType.Heal:
                if (player.Health != null)
                {
                    // 将总效果量应用为治疗量 恢复玩家的生命值
                    player.Health.Heal(totalAmount);
                }
                break;
        }
    }

    /// <summary>
    /// 根据物品配置播放拾取效果的视觉反馈
    /// </summary>
    /// <param name="player">玩家</param>
    /// <param name="itemConfig">物品配置</param>
    private static void PlayPickupEffect(Player player, ItemConfig itemConfig)
    {
        // 确保玩家和物品配置有效且配置中包含拾取效果预制体
        if (player == null || itemConfig == null || itemConfig.PickupEffectPrefab == null)
        {
            return;
        }

        // 从物品配置中获取拾取效果预制体
        GameObject effectPrefab = itemConfig.PickupEffectPrefab;
        // 从对象池中获取一个拾取效果实例 将其放置在玩家位置并设置为活动状态
        PickupEffectController effect = GetEffectPool(effectPrefab).Get(
            player.transform.position,
            Quaternion.identity,
            GetActiveEffectRoot());

        effect.PlayAndRelease(effectController => ReleasePickupEffect(effectPrefab, effectController));
    }

    /// <summary>
    /// 将拾取效果实例释放回对象池 
    /// 或者如果没有池则将其禁用并放回场景中
    /// </summary>
    /// <param name="effectPrefab">效果预制体</param>
    /// <param name="effect">效果控制器</param>
    private static void ReleasePickupEffect(GameObject effectPrefab, PickupEffectController effect)
    {
        // 确保效果控制器有效
        if (effect == null)
        {
            return;
        }

        // 获取效果预制体的池键  
        int poolKey = GetEffectPoolKey(effectPrefab);
        // 并尝试从池字典中获取对应的对象池 如果找到对应的池 则将效果实例释放回池中
        if (effectPools.TryGetValue(poolKey, out ComponentPool<PickupEffectController> pool))
        {
            pool.Release(effect);
            return;
        }

        // 如果没有找到对应的池
        // 则将效果实例禁用并将其父对象设置为效果池根对象 以便在场景中组织和管理
        effect.gameObject.SetActive(false);
        effect.transform.SetParent(GetEffectPoolRoot());
    }

    /// <summary>
    /// 获取指定效果预制体的对象池 如果池不存在则创建一个新的池
    /// </summary>
    /// <param name="effectPrefab">效果预制体</param>
    /// <returns></returns>
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
    /// 确保效果对象具有拾取效果控制器组件 
    /// 如果对象已经具有该组件 则返回现有组件 否则添加一个新的组件并返回它
    /// </summary>
    /// <param name="effectObject">效果对象</param>
    /// <returns></returns>
    private static PickupEffectController EnsurePickupEffectRuntimeComponents(GameObject effectObject)
    {
        PickupEffectController controller = effectObject.GetComponent<PickupEffectController>();
        return controller != null ? controller : effectObject.AddComponent<PickupEffectController>();
    }

    /// <summary>
    /// 获取效果池根对象 用于组织场景中的效果实例 
    /// 如果根对象不存在则创建一个新的对象作为根对象
    /// </summary>
    /// <returns></returns>
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
    /// 获取活动的拾取效果根对象 用于组织场景中的拾取效果实例
    /// </summary>
    /// <returns></returns>
    private static Transform GetActiveEffectRoot()
    {
        // 如果活动效果根对象不存在 则创建一个新的对象并将其作为根对象
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
    /// 获取效果预制体的池键 
    /// 通过预制体的实例ID生成一个唯一的整数键 如果预制体为null 则返回0
    /// </summary>
    /// <param name="effectPrefab">效果预制体</param>
    /// <returns></returns>
    private static int GetEffectPoolKey(GameObject effectPrefab)
    {
        return effectPrefab != null ? effectPrefab.GetInstanceID() : 0;
    }
}
