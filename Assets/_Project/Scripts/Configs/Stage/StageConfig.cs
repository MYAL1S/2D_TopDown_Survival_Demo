using UnityEngine;

/// <summary>
/// 关卡配置数据
/// 保存主菜单展示信息、怪物生成配置以及关卡通关时长
/// </summary>
[CreateAssetMenu(fileName = "StageConfig_", menuName = "ScriptableObjects/StageConfig")]
public class StageConfig : ScriptableObject
{
    // 稳定关卡 id，用于存档、解锁状态和运行时查找
    [SerializeField]
    [Tooltip("Stable id used by save data and runtime lookup.")]
    private string resourceId;

    // 主菜单中显示的关卡名称
    [SerializeField]
    [Tooltip("Displayed stage name.")]
    private string stageName;

    // 主菜单中显示的关卡等级
    [SerializeField]
    [Min(1)]
    [Tooltip("Displayed stage level.")]
    private int stageLevel = 1;

    // 主菜单中显示的关卡图片
    [SerializeField]
    [Tooltip("Displayed stage image in the main menu.")]
    private Sprite stageImage;

    // 当前关卡使用的怪物生成波次配置
    [SerializeField]
    [Tooltip("Enemy spawn wave configuration used by this stage.")]
    private SpawnConfig spawnConfig;

    // 关卡持续时间，达到该时间后判定通关
    [SerializeField]
    [Min(1f)]
    [Tooltip("Stage duration in seconds. The stage is cleared when this time is reached.")]
    private float durationSeconds = 300f;

    // 未手动配置 id 时使用 stageLevel 生成一个稳定默认值
    public string ResourceId => string.IsNullOrWhiteSpace(resourceId) ? $"stage_{stageLevel}" : resourceId;
    // 关卡显示名称
    public string StageName => stageName;
    // 关卡显示等级
    public int StageLevel => stageLevel;
    // 关卡显示图片
    public Sprite StageImage => stageImage;
    // 关卡怪物生成配置
    public SpawnConfig SpawnConfig => spawnConfig;
    // 关卡通关所需时间
    public float DurationSeconds => durationSeconds;
}
