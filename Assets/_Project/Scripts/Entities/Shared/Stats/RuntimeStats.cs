using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class RuntimeStats : MonoBehaviour
{
    private readonly Dictionary<object, float> moveSpeedMultipliers = new Dictionary<object, float>();

    [SerializeField]
    [Min(0f)]
    private float baseMoveSpeed;

    private float moveSpeedMultiplierSum;

    public float BaseMoveSpeed => baseMoveSpeed;
    public float MoveSpeedMultiplier => Mathf.Max(0f, 1f + moveSpeedMultiplierSum);
    public float CurrentMoveSpeed => baseMoveSpeed * MoveSpeedMultiplier;

    /// <summary>
    /// 初始化移动速度 
    /// 基础移动速度是实体在没有任何状态效果或其他影响时的默认移动速度
    /// </summary>
    /// <param name="moveSpeed"></param>
    public void InitializeMoveSpeed(float moveSpeed)
    {
        baseMoveSpeed = Mathf.Max(0f, moveSpeed);
        ClearMoveSpeedModifiers();
    }

    /// <summary>
    /// 设置移动速度乘数 
    /// 允许外部系统（如状态效果）为实体添加或更新移动速度乘数
    /// </summary>
    /// <param name="source">来源</param>
    /// <param name="additiveMultiplier">附加乘数</param>
    public void SetMoveSpeedMultiplier(object source, float additiveMultiplier)
    {
        // 如果来源为null 则不处理
        if (source == null)
        {
            return;
        }

        // 如果已经存在来自同一来源的乘数 则先移除它的影响 
        if (moveSpeedMultipliers.TryGetValue(source, out float existingMultiplier))
        {
            moveSpeedMultiplierSum -= existingMultiplier;
        }

        // 添加新的乘数
        moveSpeedMultipliers[source] = additiveMultiplier;
        moveSpeedMultiplierSum += additiveMultiplier;
    }

    /// <summary>
    /// 移除移动速度乘数
    /// </summary>
    /// <param name="source">来源</param>
    public void RemoveMoveSpeedMultiplier(object source)
    {
        // 如果来源为null 则不处理
        if (source == null)
        {
            return;
        }

        // 如果存在来自该来源的乘数 则移除它的影响并从字典中删除
        if (moveSpeedMultipliers.TryGetValue(source, out float existingMultiplier))
        {
            moveSpeedMultipliers.Remove(source);
            moveSpeedMultiplierSum -= existingMultiplier;
        }
    }

    /// <summary>
    /// 清除所有移动速度乘数 
    /// 主要用于重置状态效果引起的移动速度变化
    /// </summary>
    public void ClearMoveSpeedModifiers()
    {
        moveSpeedMultipliers.Clear();
        moveSpeedMultiplierSum = 0f;
    }
}
