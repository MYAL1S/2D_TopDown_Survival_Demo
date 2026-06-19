using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 当需要触发死亡事件时调用的事件
/// </summary>
public class DeathEvent : MonoBehaviour
{
    // 定义一个事件 当敌人死亡时触发 事件参数为死亡事件对象本身
    public event Action<DeathEvent> OnDeath;

    /// <summary>
    /// 提供给观察者调用的函数 当需要触发死亡事件时调用这个函数
    /// </summary>
    public void CallDeathEvent()
    {
        OnDeath?.Invoke(this);
    }
}
