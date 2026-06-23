using System.Collections.Generic;
using UnityEngine;

public static class AOESystem
{
    // AOE区域对象池字典
    // 键为AOE区域预制体 值为对应的组件池
    private static readonly Dictionary<GameObject, ComponentPool<AOEZoneController>> zonePools =
        new Dictionary<GameObject, ComponentPool<AOEZoneController>>(8);
    // 活动的AOE区域对象池字典
    private static readonly Dictionary<AOEZoneController, ComponentPool<AOEZoneController>> activeZonePools =
        new Dictionary<AOEZoneController, ComponentPool<AOEZoneController>>(32);
    // 活动的AOE区域列表
    private static readonly List<AOEZoneController> activeZones = new List<AOEZoneController>(32);

    // AOE区域对象池的根节点
    private static Transform poolRoot;
    // 活动AOE区域的根节点
    private static Transform activeRoot;
    // 运行时AOE系统的Runner组件
    private static AOESystemRunner runner;

    public static AOEZoneController SpawnAOE(AOEConfig config, Vector3 position, GameObject owner = null)
    {
        return SpawnAOE(config, position, owner, null);
    }

    /// <summary>
    /// 生成一个AOE区域 并将其添加到活动区域列表中
    /// 如果配置中指定了ZonePrefab
    /// 则从对象池中获取该区域 否则在运行时创建一个新的区域。
    /// </summary>
    /// <param name="config">AOE配置</param>
    /// <param name="position">生成位置</param>
    /// <param name="owner">拥有者</param>
    /// <param name="sourceWeaponConfig">源武器配置</param>
    /// <returns></returns>
    public static AOEZoneController SpawnAOE(
        AOEConfig config,
        Vector3 position,
        GameObject owner,
        WeaponConfig sourceWeaponConfig)
    {
        // 如果配置为空 则直接返回null
        if (config == null)
        {
            return null;
        }

        // 根据配置中的ZonePrefab是否为空 来决定是从对象池中获取还是在运行时创建一个新的区域
        AOEZoneController zone = config.ZonePrefab != null
            ? SpawnPooledZone(config, position)
            : SpawnRuntimeZone(config, position);

        // 初始化AOE区域 
        StatusEffectSourceContext sourceContext = new StatusEffectSourceContext(
            zone.gameObject,
            owner,
            owner,
            sourceWeaponConfig,
            config);

        zone.Initialize(config, owner, sourceContext, ReleaseAOE);
        // 如果AOE区域正在运行 则将其添加到活动区域列表中 并确保Runner组件存在
        if (zone.IsRunning)
        {
            activeZones.Add(zone);
            EnsureRunner();
        }

        return zone;
    }

    /// <summary>
    /// 从对象池中获取一个AOE区域 
    /// 并将其添加到活动区域列表中
    /// </summary>
    /// <param name="config">AOE配置</param>
    /// <param name="position">生成位置</param>
    /// <returns></returns>
    private static AOEZoneController SpawnPooledZone(AOEConfig config, Vector3 position)
    {
        // 获取对应的组件池
        ComponentPool<AOEZoneController> pool = GetPool(config.ZonePrefab);
        // 从组件池中获取一个AOE区域 
        AOEZoneController zone = pool.Get(position, Quaternion.identity, GetActiveRoot());
        // 并将其添加到活动区域列表中
        activeZonePools[zone] = pool;
        return zone;
    }

    /// <summary>
    /// 在运行时创建一个新的AOE区域
    /// </summary>
    /// <param name="config">AOE配置</param>
    /// <param name="position">生成位置</param>
    /// <returns></returns>
    private static AOEZoneController SpawnRuntimeZone(AOEConfig config, Vector3 position)
    {
        // 创建一个新的GameObject作为AOE区域
        GameObject zoneObject = new GameObject($"{config.ResourceId} Runtime AOE");
        // 设置其父对象为活动AOE区域的根节点
        zoneObject.transform.SetParent(GetActiveRoot());
        // 设置其位置为指定的位置
        zoneObject.transform.position = position;
        return EnsureZoneRuntimeComponents(zoneObject);
    }

    /// <summary>
    /// 释放一个AOE区域
    /// </summary>
    /// <param name="zone">AOE区域控制器</param>
    private static void ReleaseAOE(AOEZoneController zone)
    {
        if (zone == null)
        {
            return;
        }

        // 如果该AOE区域是从对象池中获取的 则将其释放回对象池
        if (activeZonePools.TryGetValue(zone, out ComponentPool<AOEZoneController> pool))
        {
            activeZonePools.Remove(zone);
            activeZones.Remove(zone);
            pool.Release(zone);
            return;
        }

        // 否则 直接从活动区域列表中移除 并销毁该AOE区域的GameObject
        activeZones.Remove(zone);
        UnityEngine.Object.Destroy(zone.gameObject);
    }

    /// <summary>
    /// 更新所有活动的AOE区域
    /// </summary>
    /// <param name="deltaTime">时间增量</param>
    private static void UpdateActiveZones(float deltaTime)
    {
        for (int i = activeZones.Count - 1; i >= 0; i--)
        {
            AOEZoneController zone = activeZones[i];
            if (zone == null || !zone.IsRunning)
            {
                activeZones.RemoveAt(i);
                continue;
            }

            zone.Tick(deltaTime);
        }
    }

    /// <summary>
    /// 获取指定预制体的组件池 如果不存在则创建一个新的组件池
    /// </summary>
    /// <param name="prefab">AOE预制体</param>
    /// <returns>该AOE组件池</returns>
    private static ComponentPool<AOEZoneController> GetPool(GameObject prefab)
    {
        if (zonePools.TryGetValue(prefab, out ComponentPool<AOEZoneController> pool))
        {
            return pool;
        }

        pool = new ComponentPool<AOEZoneController>(prefab, GetPoolRoot(), EnsureZoneRuntimeComponents);
        zonePools.Add(prefab, pool);
        return pool;
    }

    /// <summary>
    /// 确保物体上有AOEZoneController组件 
    /// 如果没有则添加一个
    /// </summary>
    /// <param name="zoneObject">AOE区域对象</param>
    /// <returns>AOEZoneController组件</returns>
    private static AOEZoneController EnsureZoneRuntimeComponents(GameObject zoneObject)
    {
        AOEZoneController controller = zoneObject.GetComponent<AOEZoneController>();
        if (controller == null)
        {
            controller = zoneObject.AddComponent<AOEZoneController>();
        }

        return controller;
    }

    /// <summary>
    /// 获取AOE区域对象池的根节点 
    /// 如果不存在则创建一个新的GameObject作为根节点
    /// </summary>
    /// <returns>AOE区域对象池的根节点</returns>
    private static Transform GetPoolRoot()
    {
        if (poolRoot == null)
        {
            GameObject rootObject = new GameObject("AOE Pool");
            poolRoot = rootObject.transform;
        }

        return poolRoot;
    }

    /// <summary>
    /// 获取活动AOE区域的根节点
    /// </summary>
    /// <returns>活跃AOE区域的根节点</returns>
    private static Transform GetActiveRoot()
    {
        if (activeRoot == null)
        {
            GameObject rootObject = new GameObject("Active AOEs");
            activeRoot = rootObject.transform;
        }

        return activeRoot;
    }

    /// <summary>
    /// 确保AOE系统的Runner组件存在
    /// </summary>
    private static void EnsureRunner()
    {
        if (runner != null)
        {
            return;
        }

        GameObject runnerObject = new GameObject("AOE System Runner");
        UnityEngine.Object.DontDestroyOnLoad(runnerObject);
        runner = runnerObject.AddComponent<AOESystemRunner>();
    }

    /// <summary>
    /// AOE系统的Runner组件 用于在每帧更新时调用UpdateActiveZones方法
    /// </summary>
    private sealed class AOESystemRunner : MonoBehaviour
    {
        private void Update()
        {
            UpdateActiveZones(Time.deltaTime);
        }
    }
}
