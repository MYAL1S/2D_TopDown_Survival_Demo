using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(MovementByVelocityEvent))]
[RequireComponent(typeof(SpriteRenderer))]
[DisallowMultipleComponent]
public class MovementByVelocity : MonoBehaviour
{
    private Rigidbody2D rigidBody2D;

    private SpriteRenderer spriteRenderer;
    private MovementByVelocityEvent movementByVelocityEvent;

    private void Awake()
    {
        // 缓存刚体以及MovementByVelocityEvent组件
        rigidBody2D = GetComponent<Rigidbody2D>();
        //2d游戏不需要重力
        rigidBody2D.gravityScale = 0f;
        //冻结角色旋转
        rigidBody2D.freezeRotation = true;
        // 缓存精灵渲染器组件
        spriteRenderer = GetComponent<SpriteRenderer>();
        movementByVelocityEvent = GetComponent<MovementByVelocityEvent>();
    }

    private void OnEnable()
    {
        // 订阅speed移动事件
        movementByVelocityEvent.OnMovementByVelocity += MovementByVelocityEvent_OnMovementByVelocity;
    }

    private void OnDisable()
    {
        // 取消订阅speed移动事件
        movementByVelocityEvent.OnMovementByVelocity -= MovementByVelocityEvent_OnMovementByVelocity;
    }

    /// <summary>
    /// 当触发通过速度移动事件时调用
    /// </summary>
    /// <param name="movementByVelocityEvent"></param>
    /// <param name="movementByVelocityArgs"></param>
    private void MovementByVelocityEvent_OnMovementByVelocity(MovementByVelocityEvent movementByVelocityEvent, MovementByVelocityArgs movementByVelocityArgs)
    {
        SetOrientation(movementByVelocityArgs.moveDirection);
        MoveRigidBody(movementByVelocityArgs.moveDirection, movementByVelocityArgs.moveSpeed);
    }

    /// <summary>
    /// 设置角色朝向
    /// </summary>
    /// <param name="moveDirection"></param>
    private void SetOrientation(Vector2 moveDirection)
    {
        if (moveDirection != Vector2.zero)
            spriteRenderer.flipX = moveDirection.x < 0;
    }

    /// <summary>
    /// 设置刚体的速度
    /// </summary>
    private void MoveRigidBody(Vector2 moveDirection, float moveSpeed)
    {
        rigidBody2D.velocity = moveDirection * moveSpeed;
        
    }
}