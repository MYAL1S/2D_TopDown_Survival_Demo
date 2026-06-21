using System;
using UnityEngine;

/// <summary>
/// 当需要触发攻击事件时调用的事件
/// </summary>
public class AttackEvent : MonoBehaviour
{
    // 定义一个事件 参数为攻击事件源和攻击上下文
    public event Action<AttackEvent, AttackContext> OnAttack;

    /// <summary>
    /// 调用攻击事件 传入攻击上下文参数
    /// </summary>
    /// <param name="context"></param>
    public void CallAttackEvent(AttackContext context)
    {
        OnAttack?.Invoke(this, context);
    }
}
