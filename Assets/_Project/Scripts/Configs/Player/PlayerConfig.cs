using UnityEngine;

/// <summary>
/// 玩家角色配置
/// 用于描述角色选择界面展示信息，以及进入关卡后生成角色所需的基础属性
/// </summary>
[CreateAssetMenu(fileName = "PlayerConfig_", menuName = "ScriptableObjects/PlayerConfig")]
public class PlayerConfig : ScriptableObject
{
    // 稳定角色 id，用于 GameResources 缓存、存档解锁和关卡生成
    [SerializeField]
    [Tooltip("Stable id used by GameResources and runtime spawners.")]
    private string resourceId;

    [Header("PLAYER BASE DETAILS")]
    // 角色显示名称
    [Tooltip("Player character name.")]
    public string playerCharacterName;

    // 角色选择界面显示的头像
    [SerializeField]
    [Tooltip("Icon shown in character selection UI.")]
    private Sprite characterIcon;

    // 解锁该角色需要消耗的金币数量
    [SerializeField]
    [Min(0)]
    [Tooltip("Gold cost required to unlock this character.")]
    private int unlockPrice;

    // 进入关卡时实际生成的玩家预制体
    [Tooltip("Prefab gameobject for the player")]
    public GameObject playerPrefab;

    // 玩家运行时动画控制器
    [Tooltip("Player runtime animator controller")]
    public RuntimeAnimatorController runtimeAnimatorController;

    // 玩家基础移动速度
    [Tooltip("Player base movement speed.")]
    [Min(0f)]
    [SerializeField]
    private float baseSpeed = 8f;

    // 角色出生时默认装备的武器配置
    [SerializeField]
    [Tooltip("Weapon equipped when the character is spawned.")]
    private WeaponConfig startingWeaponConfig;

    // 角色界面 Ability 图片使用的默认武器图标
    [SerializeField]
    [Tooltip("Icon shown as this character's default weapon ability in character selection UI.")]
    private Sprite defaultWeaponIcon;

    [Header("PLAYER COMBAT DETAILS")]
    // 玩家最大生命值
    [SerializeField]
    [Min(1f)]
    private float maxHealth = 100f;

    // 玩家防御力，会在受伤计算中抵消部分伤害
    [SerializeField]
    [Min(0f)]
    private float defense = 0f;

    // 玩家受伤后的短暂无敌时间
    [SerializeField]
    [Min(0f)]
    private float injuredInvincibilityDuration = 0.75f;

    [Header("PLAYER PICKUP DETAILS")]
    // 玩家拾取吸附半径
    [SerializeField]
    [Min(0.01f)]
    private float pickupRadius = 2.5f;

    // 拾取物距离玩家多近时判定被收集
    [SerializeField]
    [Min(0.01f)]
    private float pickupCollectDistance = 0.35f;

    // 拾取物被吸附到玩家身边时的移动速度
    [SerializeField]
    [Min(0f)]
    private float pickupAttractSpeed = 8f;

    // 未手动配置 id 时使用角色名作为默认 id
    public string ResourceId => string.IsNullOrWhiteSpace(resourceId) ? playerCharacterName : resourceId;
    // 角色头像
    public Sprite CharacterIcon => characterIcon;
    // 解锁价格
    public int UnlockPrice => unlockPrice;
    // 移动速度
    public float MoveSpeed => baseSpeed;
    // 出生默认武器
    public WeaponConfig StartingWeaponConfig => startingWeaponConfig;
    // 默认武器图标
    public Sprite DefaultWeaponIcon => defaultWeaponIcon;
    // 最大生命值
    public float MaxHealth => maxHealth;
    // 防御力
    public float Defense => defense;
    // 受伤无敌时间
    public float InjuredInvincibilityDuration => injuredInvincibilityDuration;
    // 拾取半径
    public float PickupRadius => pickupRadius;
    // 拾取完成距离
    public float PickupCollectDistance => pickupCollectDistance;
    // 拾取吸附速度
    public float PickupAttractSpeed => pickupAttractSpeed;
}
