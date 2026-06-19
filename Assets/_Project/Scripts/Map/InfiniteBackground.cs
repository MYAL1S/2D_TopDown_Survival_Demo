using UnityEngine;

/// <summary>
/// 无限背景系统
/// 使用网格瓦片复用实现可无限滚动的背景 通过追踪相机位置动态重定位瓦片
/// 当相机移动时 若跨越网格边界则更新所有瓦片位置
/// </summary>
[DisallowMultipleComponent]
public class InfiniteBackground : MonoBehaviour
{
    // 背景瓦片的预制体
    [SerializeField]
    private GameObject backgroundTilePrefab;

    // 追踪的目标相机 若未设置则使用 Camera.main
    [SerializeField]
    private Camera targetCamera;

    // 网格半径 相机中心周围的瓦片数
    // 范围为 [-gridRadius, gridRadius]
    [SerializeField]
    [Min(1)]
    private int gridRadius = 2;

    // 是否自动扩展网格以覆盖相机完整视图
    [SerializeField]
    private bool expandToCoverCameraView = true;

    // 单个瓦片的宽高尺寸
    // 若为 (0, 0) 则从预制体的 sprite 自动推断
    [SerializeField]
    private Vector2 tileSize;

    // 所有瓦片的 Z 轴位置
    [SerializeField]
    private float zPosition;

    // 已创建的瓦片 Transform 数组（可复用）
    private Transform[] tileTransforms;
    // 实际使用的瓦片尺寸（由 tileSize 或 sprite 推断得出）
    private Vector2 resolvedTileSize;
    // 相机当前所在的网格坐标
    private Vector2Int currentCenterCoord;
    // 运行时计算的有效网格半径（考虑 expandToCoverCameraView）
    private int runtimeGridRadius;
    // 初始化完成标志
    private bool isInitialized;

    /// <summary>
    /// 游戏启动时尝试初始化背景系统
    /// </summary>
    private void Start()
    {
        Initialize();
    }

    /// <summary>
    /// 每帧检测相机网格位置变化，若改变则重新定位所有瓦片
    /// </summary>
    private void Update()
    {
        // 尝试初始化（如果未成功，每帧重试）
        if (!isInitialized && !Initialize())
        {
            return;
        }

        // 尝试获取目标相机
        if (targetCamera == null)
        {
            targetCamera = Camera.main;
            if (targetCamera == null)
            {
                return;
            }
        }

        // 检测相机是否跨越网格边界
        Vector2Int cameraCoord = GetTileCoord(targetCamera.transform.position);
        if (cameraCoord != currentCenterCoord)
        {
            currentCenterCoord = cameraCoord;
            RepositionTiles();
        }
    }

    /// <summary>
    /// 初始化背景系统
    /// 解析瓦片尺寸、计算网格半径、创建瓦片池、摆放初始位置
    /// 返回 true 表示初始化成功  false 表示存在配置错误
    /// </summary>
    public bool Initialize()
    {
        if (backgroundTilePrefab == null)
        {
            Debug.LogError($"{nameof(InfiniteBackground)} needs a background tile prefab.", this);
            return false;
        }

        if (targetCamera == null)
        {
            targetCamera = Camera.main;
        }

        if (targetCamera == null)
        {
            Debug.LogError($"{nameof(InfiniteBackground)} could not find a target camera.", this);
            return false;
        }

        // 解析瓦片尺寸（若未明确设置则从 sprite 推断）
        resolvedTileSize = ResolveTileSize();
        if (resolvedTileSize.x <= 0f || resolvedTileSize.y <= 0f)
        {
            Debug.LogError($"{nameof(InfiniteBackground)} could not resolve a valid tile size.", this);
            return false;
        }

        // 根据相机视野与是否需要完全覆盖来计算实际网格半径
        runtimeGridRadius = GetRuntimeGridRadius();
        // 创建或复用瓦片对象池
        CreateTilePool();

        // 初始化相机位置与瓦片摆放。
        currentCenterCoord = GetTileCoord(targetCamera.transform.position);
        RepositionTiles();
        isInitialized = true;
        return true;
    }

    /// <summary>
    /// 创建瓦片对象池或调整其大小
    /// 若已有足够的瓦片则复用 否则实例化新的瓦片对象
    /// </summary>
    private void CreateTilePool()
    {
        int gridSize = runtimeGridRadius * 2 + 1;
        int tileCount = gridSize * gridSize;

        // 若已有正确数量的瓦片则复用
        if (tileTransforms != null && tileTransforms.Length == tileCount)
        {
            return;
        }

        tileTransforms = new Transform[tileCount];

        // 实例化所需数量的瓦片对象
        for (int i = 0; i < tileTransforms.Length; i++)
        {
            GameObject tileObject = Instantiate(backgroundTilePrefab, transform);
            tileObject.name = $"{backgroundTilePrefab.name}_{i}";
            tileTransforms[i] = tileObject.transform;
        }
    }

    /// <summary>
    /// 根据当前相机网格坐标重新摆放所有瓦片到周围网格位置
    /// </summary>
    private void RepositionTiles()
    {
        int index = 0;
        for (int y = -runtimeGridRadius; y <= runtimeGridRadius; y++)
        {
            for (int x = -runtimeGridRadius; x <= runtimeGridRadius; x++)
            {
                Vector2Int tileCoord = currentCenterCoord + new Vector2Int(x, y);
                tileTransforms[index].position = GetTileWorldPosition(tileCoord);
                index++;
            }
        }
    }

    /// <summary>
    /// 将世界坐标转换为网格坐标。
    /// </summary>
    private Vector2Int GetTileCoord(Vector3 worldPosition)
    {
        return new Vector2Int(
            Mathf.FloorToInt(worldPosition.x / resolvedTileSize.x),
            Mathf.FloorToInt(worldPosition.y / resolvedTileSize.y));
    }

    /// <summary>
    /// 将网格坐标转换为世界坐标（网格左下角位置）
    /// </summary>
    private Vector3 GetTileWorldPosition(Vector2Int tileCoord)
    {
        return new Vector3(
            tileCoord.x * resolvedTileSize.x,
            tileCoord.y * resolvedTileSize.y,
            zPosition);
    }

    /// <summary>
    /// 计算运行时的有效网格半径
    /// 若启用 expandToCoverCameraView 则自动扩展至覆盖相机完整视图
    /// </summary>
    private int GetRuntimeGridRadius()
    {
        int radius = Mathf.Max(1, gridRadius);
        // 若未启用或相机非正交，则直接返回配置的 gridRadius。
        if (!expandToCoverCameraView || targetCamera == null || !targetCamera.orthographic)
        {
            return radius;
        }

        // 计算覆盖相机视野所需的网格半径
        float halfHeight = targetCamera.orthographicSize;
        float halfWidth = halfHeight * targetCamera.aspect;
        int xRadius = Mathf.CeilToInt(halfWidth / resolvedTileSize.x) + 1;
        int yRadius = Mathf.CeilToInt(halfHeight / resolvedTileSize.y) + 1;
        return Mathf.Max(radius, xRadius, yRadius);
    }

    /// <summary>
    /// 解析瓦片尺寸
    /// 优先使用显式设置的 tileSize 否则从预制体的 sprite 推断
    /// 返回 Vector2.zero 时表示无法解析有效尺寸
    /// </summary>
    private Vector2 ResolveTileSize()
    {
        // 若显式设置了有效的瓦片尺寸则直接使用
        if (tileSize.x > 0f && tileSize.y > 0f)
        {
            return tileSize;
        }

        // 尝试从预制体的 sprite 推断尺寸（sprite bounds * 渲染器的 lossyScale）
        SpriteRenderer spriteRenderer = backgroundTilePrefab.GetComponentInChildren<SpriteRenderer>(true);
        if (spriteRenderer != null && spriteRenderer.sprite != null)
        {
            Vector2 spriteSize = spriteRenderer.sprite.bounds.size;
            Vector3 scale = spriteRenderer.transform.lossyScale;
            return new Vector2(
                Mathf.Abs(spriteSize.x * scale.x),
                Mathf.Abs(spriteSize.y * scale.y));
        }

        return Vector2.zero;
    }
}
