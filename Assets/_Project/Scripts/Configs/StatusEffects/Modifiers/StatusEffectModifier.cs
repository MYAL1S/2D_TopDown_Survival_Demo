using UnityEngine;

/// <summary>
/// 状态效果修饰器基类
/// </summary>
public abstract class StatusEffectModifier : ScriptableObject
{
    /// <summary>
    /// 创建运行时修饰器实例
    /// </summary>
    /// <returns></returns>
    public abstract IStatusEffectModifierRuntime CreateRuntimeModifier();
}
