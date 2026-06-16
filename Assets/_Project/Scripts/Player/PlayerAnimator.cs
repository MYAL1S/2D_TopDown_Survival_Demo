using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Player))]
[DisallowMultipleComponent]
public class PlayerAnimator : MonoBehaviour
{
    private Player player;

    void Awake()
    {
        player = GetComponent<Player>();
    }

    void OnEnable()
    {
        //订阅移动事件
        player.movementByVelocityEvent.OnMovementByVelocity += MovementByVelocityEvent_OnMovementByVelocity;

        // 订阅待机事件
        player.idleEvent.OnIdle += IdleEvent_OnIdle;
    }


    void OnDisable()
    {
        // 取消订阅移动事件
        player.movementByVelocityEvent.OnMovementByVelocity -= MovementByVelocityEvent_OnMovementByVelocity;

        // 取消订阅待机事件
        player.idleEvent.OnIdle -= IdleEvent_OnIdle;
    }


    /// <summary>
    /// 当玩家通过速度移动时的事件处理程序
    /// </summary>
    private void MovementByVelocityEvent_OnMovementByVelocity(MovementByVelocityEvent movementByVelocityEvent, MovementByVelocityArgs movementByVelocityArgs)
    {
        SetMovementAnimationParameters(movementByVelocityArgs.moveSpeed);
    }



    /// <summary>
    /// 当玩家待机时的事件处理程序
    /// </summary>
    private void IdleEvent_OnIdle(IdleEvent idleEvent)
    {
        SetIdleAnimationParameters();
    }


 /// <summary>
 /// 设置移动动画参数
 /// </summary>
 private void SetMovementAnimationParameters(float speed)
 {
     player.animator.SetFloat(Settings.speed, speed);
 }



 /// <summary>
 /// 设置待机动画参数
 /// </summary>
 private void SetIdleAnimationParameters()
 {
     player.animator.SetFloat(Settings.speed, 0f);
 }
}
