using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 通用组件对象池
/// 用于复用实现 IPoolable 接口的组件，减少频繁创销对象的性能开销
/// </summary>
public class ComponentPool<T> where T : Component, IPoolable
{
    // 用于实例化新对象的预制体
    private readonly GameObject prefab;
    // 不活跃对象的父容器，便于层级管理
    private readonly Transform inactiveParent;
    // 可选的自定义组件解析器，用于从实例化对象中获取目标组件
    private readonly Func<GameObject, T> componentResolver;
    // 队列结构，存储当前不活跃对象实例，采用 FIFO 顺序便于均匀复用
    private readonly Queue<T> inactiveInstances = new Queue<T>();

    /// <summary>
    /// 初始化对象池
    /// </summary>
    /// <param name="prefab">用于实例化新对象的预制体</param>
    /// <param name="inactiveParent">用于存放不活跃对象的父节点</param>
    /// <param name="componentResolver">可选的自定义组件解析委托，若为 null 则采用默认方式</param>
    public ComponentPool(GameObject prefab, Transform inactiveParent, Func<GameObject, T> componentResolver = null)
    {
        this.prefab = prefab;
        this.inactiveParent = inactiveParent;
        this.componentResolver = componentResolver;
    }

    /// <summary>
    /// 从池中获取一个对象，若无可用则创建新对象
    /// 激活该对象、设置位置和旋转，并调用其 OnSpawnedFromPool 回调
    /// </summary>
    public T Get(Vector3 position, Quaternion rotation, Transform activeParent)
    {
        T instance = GetInactiveOrCreate();
        Transform instanceTransform = instance.transform;
        instanceTransform.SetParent(activeParent);
        instanceTransform.SetPositionAndRotation(position, rotation);
        instance.gameObject.SetActive(true);
        instance.OnSpawnedFromPool();
        return instance;
    }

    /// <summary>
    /// 将对象归还到池中
    /// 调用 OnReturnedToPool 回调、禁用游戏对象并移回池容器
    /// </summary>
    public void Release(T instance)
    {
        if (instance == null)
        {
            return;
        }

        GameObject instanceObject = instance.gameObject;
        instance.OnReturnedToPool();
        instanceObject.SetActive(false);
        instance.transform.SetParent(inactiveParent);
        inactiveInstances.Enqueue(instance);
    }

    /// <summary>
    /// 清空对象池，销毁所有不活跃的对象。
    /// </summary>
    public void Clear()
    {
        while (inactiveInstances.Count > 0)
        {
            T instance = inactiveInstances.Dequeue();
            if (instance != null)
            {
                UnityEngine.Object.Destroy(instance.gameObject);
            }
        }
    }

    /// <summary>
    /// 从不活跃实例队列中取出可用对象，若无则创建新实例
    /// 过程中会跳过已销毁的对象引用
    /// </summary>
    private T GetInactiveOrCreate()
    {
        while (inactiveInstances.Count > 0)
        {
            T instance = inactiveInstances.Dequeue();
            if (instance != null)
            {
                return instance;
            }
        }

        GameObject instanceObject = UnityEngine.Object.Instantiate(prefab, inactiveParent);
        T component = ResolveComponent(instanceObject);
        instanceObject.SetActive(false);
        return component;
    }

    /// <summary>
    /// 从实例化的游戏对象中获取或添加目标组件
    /// 优先使用自定义解析器（若提供），否则采用默认获取或添加逻辑
    /// </summary>
    private T ResolveComponent(GameObject instanceObject)
    {
        if (componentResolver != null)
        {
            return componentResolver(instanceObject);
        }

        T component = instanceObject.GetComponent<T>();
        if (component == null)
        {
            component = instanceObject.AddComponent<T>();
        }

        return component;
    }
}
