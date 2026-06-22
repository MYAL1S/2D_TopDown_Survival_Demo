using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 玩家拾取系统
/// </summary>
[RequireComponent(typeof(Player))]
[DisallowMultipleComponent]
public class PlayerPickupSystem : MonoBehaviour
{
    [SerializeField]
    [Min(0.01f)]
    [Tooltip("the radius of pickup detection, within which attract will be applied")]
    private float pickupRadius = 2.5f;

    [SerializeField]
    [Min(0.01f)]
    [Tooltip("the distance within which pickups can be collected")]
    private float collectDistance = 0.35f;

    [SerializeField]
    [Min(0f)]
    [Tooltip("the speed at which pickups are attracted to the player")]
    private float attractSpeed = 8f;

    private Player player;

    private void Awake()
    {
        player = GetComponent<Player>();
    }

    private void Update()
    {
        // 获取所有活动的拾取物
        IReadOnlyList<PickupController> activePickups = PickupSystem.ActivePickups;
        float radiusSqr = pickupRadius * pickupRadius;

        // 从后往前遍历拾取物列表
        // 以便在尝试收集时安全地移除已收集的拾取物
        for (int i = activePickups.Count - 1; i >= 0; i--)
        {
            PickupController pickup = activePickups[i];
            if (pickup == null)
            {
                continue;
            }

            // 计算玩家与拾取物之间的距离平方
            float distanceSqr = (pickup.transform.position - transform.position).sqrMagnitude;
            // 如果距离小于等于收集距离 则尝试收集
            if (distanceSqr <= collectDistance * collectDistance)
            {
                pickup.TryCollect(player);
            }
            // 否则如果距离小于等于拾取半径 则开始吸引
            else if (distanceSqr <= radiusSqr)
            {
                pickup.BeginAttract(player, attractSpeed, collectDistance);
            }
        }
    }

    /// <summary>
    /// 根据玩家配置初始化拾取系统参数
    /// </summary>
    /// <param name="config">玩家配置</param>
    public void Initialize(PlayerConfig config)
    {
        if (config == null)
        {
            return;
        }

        pickupRadius = config.PickupRadius;
        collectDistance = config.PickupCollectDistance;
        attractSpeed = config.PickupAttractSpeed;
    }
}
