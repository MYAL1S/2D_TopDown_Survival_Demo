using UnityEngine;

/// <summary>
/// Controls player input, movement events, and basic player runtime settings.
/// </summary>
[RequireComponent(typeof(Player))]
[DisallowMultipleComponent]
public class PlayerController : MonoBehaviour
{
    [SerializeField]
    [Tooltip("The scriptable object that contains player info.")]
    private PlayerConfig playerConfig;

    private Player player;
    private float moveSpeed;
    private Vector2 moveInput;
    private bool wasMoving;

    private void Awake()
    {
        player = GetComponent<Player>();
    }

    private void Start()
    {
        if (playerConfig == null)
        {
            Debug.LogError($"{nameof(PlayerController)} is missing a PlayerConfig reference.", this);
            enabled = false;
            return;
        }

        moveSpeed = playerConfig.MoveSpeed;
        SetPlayerAnimationSpeed();
    }

    private void Update()
    {
        moveInput = InputSystemAdapter.Instance.MoveInput;

        if (moveInput.sqrMagnitude > 1f)
        {
            moveInput = moveInput.normalized;
        }
    }

    private void FixedUpdate()
    {
        bool isMoving = moveInput.sqrMagnitude > 0.0001f;

        if (isMoving)
        {
            player.movementByVelocityEvent.CallMovementByVelocityEvent(moveInput.normalized, moveInput.magnitude * moveSpeed);
        }
        else if (wasMoving)
        {
            player.idleEvent.CallIdleEvent();
        }

        wasMoving = isMoving;
    }

    private void SetPlayerAnimationSpeed()
    {
        player.animator.speed = moveSpeed / Settings.baseSpeedForPlayerAnimations;
    }
}
