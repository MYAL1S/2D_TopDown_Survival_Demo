using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 当需要触发受伤事件时调用的事件
/// </summary>
public class InjuredEvent : MonoBehaviour
{
    // 受伤事件 当敌人受到伤害时触发 传递两个参数 一个是事件源对象 一个是伤害值
    public event Action<InjuredEvent,int> OnInjured;

    /// <summary>
    /// 提供给观察者调用的函数 当需要触发受伤事件时调用这个函数
    /// </summary>
    public void CallInjuredEvent(int damage)
    {
        OnInjured?.Invoke(this, damage);
    }
}
