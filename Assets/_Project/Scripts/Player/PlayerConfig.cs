using UnityEngine;

/// <summary>
/// 角色配置类 通过ScriptableObject创建实例 用于存储角色的基础信息和属性
/// </summary>
[CreateAssetMenu(fileName = "PlayerConfig_", menuName = "ScriptableObjects/PlayerConfig")]
public class PlayerConfig : ScriptableObject
{

    // 角色名
    #region Header PLAYER BASE DETAILS
    [Space(10)]
    [Header("PLAYER BASE DETAILS")]
    #endregion
    #region Tooltip
    [Tooltip("Player character name.")]
    #endregion
    public string playerCharacterName;


    // 角色关联的预制体
    #region Tooltip
    [Tooltip("Prefab gameobject for the player")]
    #endregion
    public GameObject playerPrefab;


    // 角色关联的动画控制器
    #region Tooltip
    [Tooltip("Player runtime animator controller")]
    #endregion
    public RuntimeAnimatorController runtimeAnimatorController;


    // 角色的基础移动速度
    // 如果有升级提升移速的话可以配置速率
    // 也可以根据该速度除以一个标准速度配置动画播放速率 避免出现奇怪的不连贯运动等现象
    #region Tooltip
    [Tooltip("Player base movement speed.")]
    #endregion
    [Min(0f)]
    [SerializeField]
    private float baseSpeed = 8f;

    // 通过属性暴露给外部使用速度
    // 避免外部改基础移速
    public float MoveSpeed => baseSpeed;
}
