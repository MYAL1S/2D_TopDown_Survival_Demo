using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[DisallowMultipleComponent]
public class MovementByVelocityEvent : MonoBehaviour
{
    // 当需要通过速度移动时触发的事件
    public event Action<MovementByVelocityEvent, MovementByVelocityArgs> OnMovementByVelocity;

    /// <summary>
    /// 提供给观察者调用的函数
    /// </summary>
    /// <param name="moveDirection"></param>
    /// <param name="moveSpeed"></param>
    public void CallMovementByVelocityEvent(Vector2 moveDirection, float moveSpeed)
    {
        OnMovementByVelocity?.Invoke(this, new MovementByVelocityArgs() { moveDirection = moveDirection, moveSpeed = moveSpeed });
    }

}

/// <summary>
/// 通过MovementByVelocityEvent传递的参数
/// 移动时需要传递两个参数
/// 一个是移动方向，一个是移动速度
/// </summary>
public class MovementByVelocityArgs : EventArgs
{
    public Vector2 moveDirection;
    public float moveSpeed;
}

