using UnityEngine;

/// <summary>
/// 状态效果上下文
/// </summary>
public sealed class StatusEffectContext
{
    public StatusEffectContext(GameObject owner)
    {
        Owner = owner;
        Transform = owner != null ? owner.transform : null;
        Health = owner != null ? owner.GetComponent<Health>() : null;
        RuntimeStats = owner != null ? owner.GetComponent<RuntimeStats>() : null;
    }

    /// <summary>
    /// 状态效果的拥有者 通常是一个实体对象 例如玩家或敌人
    /// </summary>
    public GameObject Owner { get; }
    /// <summary>
    /// 状态效果拥有者的 Transform 组件 用于获取位置 旋转和缩放等信息
    /// </summary>
    public Transform Transform { get; }
    /// <summary>
    /// 状态效果拥有者的 Health 组件 用于获取生命值和处理伤害等相关逻辑
    /// </summary>
    public Health Health { get; }
    /// <summary>
    /// 状态效果拥有者的 RuntimeStats 组件 用于获取和修改运行时属性 例如移动速度攻击力等
    /// </summary>
    public RuntimeStats RuntimeStats { get; }
}
