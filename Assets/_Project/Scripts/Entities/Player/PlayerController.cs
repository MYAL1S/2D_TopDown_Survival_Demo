using UnityEngine;

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
    private bool isInitialized;

    private void Awake()
    {
        player = GetComponent<Player>();
    }

    private void Start()
    {
        Initialize(playerConfig);
    }

    private void Update()
    {
        // 如果玩家控制器已经初始化，获取输入并处理
        if (isInitialized)
        {
            moveInput = InputSystemAdapter.Instance.MoveInput;

            if (moveInput.sqrMagnitude > 1f)
                moveInput = moveInput.normalized;
        }
    }

    private void FixedUpdate()
    {
        // 只有在玩家控制器已经初始化时才处理移动和待机状态
        if (isInitialized)
        {

            bool isMoving = moveInput.sqrMagnitude > 0.0001f;

            if (isMoving)
            {
                player.MovementByVelocityEvent.CallMovementByVelocityEvent(moveInput.normalized, moveInput.magnitude * moveSpeed);
            }
            else if (wasMoving)
            {
                player.IdleEvent.CallIdleEvent();
            }

            wasMoving = isMoving;
        }
    }

    /// <summary>
    /// 初始化玩家控制器
    /// 设置移动速度并启用组件
    /// </summary>
    /// <param name="config"></param>
    public void Initialize(PlayerConfig config)
    {
        if (config == null)
        {
            Debug.LogError($"{nameof(PlayerController)} is missing a PlayerConfig reference.", this);
            enabled = false;
            return;
        }

        playerConfig = config;
        moveSpeed = playerConfig.MoveSpeed;
        isInitialized = true;
        enabled = true;
        SetPlayerAnimationSpeed();
    }

    /// <summary>
    /// 根据玩家的移动速度设置动画播放速度
    /// </summary>
    private void SetPlayerAnimationSpeed()
    {
        if (player != null && player.Animator != null)
        {
            player.Animator.speed = moveSpeed / Settings.baseSpeedForPlayerAnimations;
        }
    }
}
