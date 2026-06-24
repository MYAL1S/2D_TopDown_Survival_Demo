using UnityEngine;

/// <summary>
/// 角色选择系统
/// 根据角色配置 id 生成当前玩家，并维护场景中唯一的活动玩家引用
/// </summary>
[RequireComponent(typeof(CharacterSpawnManager))]
[DisallowMultipleComponent]
public class CharacterSelectionSystem : MonoBehaviour
{
    // 具体执行角色预制体实例化的生成器
    private CharacterSpawnManager characterSpawnManager;

    // 当前场景中处于活动状态的玩家对象
    public Player ActivePlayer { get; private set; }

    private void Awake()
    {
        characterSpawnManager = GetComponent<CharacterSpawnManager>();
    }

    /// <summary>
    /// 使用默认出生点选择并生成角色
    /// </summary>
    /// <param name="playerResourceId">玩家角色配置 id</param>
    /// <returns>新生成的玩家对象</returns>
    public Player SelectCharacter(string playerResourceId)
    {
        return SelectCharacter(playerResourceId, Vector3.zero);
    }

    /// <summary>
    /// 选择并生成角色
    /// 生成新玩家前会销毁旧玩家，并将摄像机目标切换到新玩家
    /// </summary>
    /// <param name="playerResourceId">玩家角色配置 id</param>
    /// <param name="spawnPosition">玩家出生位置</param>
    /// <returns>新生成的玩家对象</returns>
    public Player SelectCharacter(string playerResourceId, Vector3 spawnPosition)
    {
        if (ActivePlayer != null)
        {
            if (CameraTargetBinder.Instance != null)
            {
                CameraTargetBinder.Instance.ClearTarget();
            }

            Destroy(ActivePlayer.gameObject);
        }

        ActivePlayer = characterSpawnManager.SpawnCharacter(playerResourceId, spawnPosition);
        if (ActivePlayer != null && CameraTargetBinder.Instance != null)
        {
            CameraTargetBinder.Instance.SetTarget(ActivePlayer.transform);
        }

        return ActivePlayer;
    }
}
