using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(IdleEvent))]
[DisallowMultipleComponent]
public class Idle : MonoBehaviour
{
    private Rigidbody2D rigidBody2D;
    private IdleEvent idleEvent;

    private void Awake()
    {
        // 缓存刚体和IdleEvent组件
        rigidBody2D = GetComponent<Rigidbody2D>();
        //2d游戏不需要重力
        rigidBody2D.gravityScale = 0f;
        //冻结角色旋转
        rigidBody2D.freezeRotation = true;
        rigidBody2D.interpolation = RigidbodyInterpolation2D.Interpolate;
        rigidBody2D.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        idleEvent = GetComponent<IdleEvent>();

    }

    private void OnEnable()
    {
        // 订阅Idle事件
        idleEvent.OnIdle += IdleEvent_OnIdle;
    }

    private void OnDisable()
    {
        // 取消订阅 
        idleEvent.OnIdle -= IdleEvent_OnIdle;
    }

    /// <summary>
    /// 当收到Idle事件时调用的函数
    /// </summary>
    /// <param name="idleEvent"></param>
    private void IdleEvent_OnIdle(IdleEvent idleEvent)
    {
        MoveRigidBody();
    }

    /// <summary>
    /// 在进入Idle状态时
    /// 通过设置刚体速度为0
    /// 确保角色停止移动
    /// </summary>
    private void MoveRigidBody()
    {
        rigidBody2D.velocity = Vector2.zero;
    }
}
