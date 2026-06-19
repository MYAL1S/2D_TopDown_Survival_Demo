using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class IdleEvent : MonoBehaviour
{
    // 提供给Idle组件的事件，当角色进入Idle状态时触发
    // 这里使用Action 也是一样的 因为进入Idle状态不需要更多参数
    // 但使用Action<IdleEvent>是为了更面向对象
    public event Action<IdleEvent> OnIdle;

    /// <summary>
    /// 提供给观察者调用的函数
    /// 当需要触发Idle事件时，调用此函数
    /// 所有监听Idle事件的组件都会收到通知
    /// </summary>
    public void CallIdleEvent()
    {
        OnIdle?.Invoke(this);
    }
}
