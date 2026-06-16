using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#region REQUIRE COMPONENTS
[RequireComponent(typeof(MovementByVelocityEvent))]
[RequireComponent(typeof(MovementByVelocity))]
[RequireComponent(typeof(IdleEvent))]
[RequireComponent(typeof(Idle))]
[RequireComponent(typeof(PlayerController))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(PlayerAnimator))]
#endregion
[DisallowMultipleComponent]
public class Player : MonoBehaviour
{
    [HideInInspector] public PlayerController playerController;
    [HideInInspector] public MovementByVelocityEvent movementByVelocityEvent;
    [HideInInspector] public MovementByVelocity movementByVelocity;
    [HideInInspector] public IdleEvent idleEvent;
    [HideInInspector] public Idle idle;
    [HideInInspector] public SpriteRenderer spriteRenderer;
    [HideInInspector] public Animator animator;


    void Awake()
    {
        playerController = GetComponent<PlayerController>();
        movementByVelocityEvent = GetComponent<MovementByVelocityEvent>();
        movementByVelocity = GetComponent<MovementByVelocity>();
        idleEvent = GetComponent<IdleEvent>();
        idle = GetComponent<Idle>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
    }
}
