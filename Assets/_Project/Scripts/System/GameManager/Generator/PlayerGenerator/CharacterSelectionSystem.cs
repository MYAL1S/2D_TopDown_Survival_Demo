using UnityEngine;

/// <summary>
/// 角色选择系统
/// 负责切换当前玩家角色
/// 并维护活动玩家引用
/// </summary>
[RequireComponent(typeof(CharacterSpawnManager))]
[DisallowMultipleComponent]
public class CharacterSelectionSystem : MonoBehaviour
{
    // 依赖角色生成器完成实际的玩家实例化
    private CharacterSpawnManager characterSpawnManager;

    // 当前场景中激活的玩家对象
    public Player ActivePlayer { get; private set; }

    /// <summary>
    /// 缓存同物体上的角色生成管理器
    /// </summary>
    private void Awake()
    {
        characterSpawnManager = GetComponent<CharacterSpawnManager>();
    }

    /// <summary>
    /// 使用默认出生点选择角色
    /// </summary>
    public Player SelectCharacter(string playerResourceId)
    {
        return SelectCharacter(playerResourceId, Vector3.zero);
    }

    /// <summary>
    /// 选择角色
    /// 先销毁旧玩家
    /// 再生成新玩家并更新活动引用
    /// </summary>
    public Player SelectCharacter(string playerResourceId, Vector3 spawnPosition)
    {
        if (ActivePlayer != null)
        {
            CameraTargetBinder.Instance.ClearTarget();
            Destroy(ActivePlayer.gameObject);
        }

        ActivePlayer = characterSpawnManager.SpawnCharacter(playerResourceId, spawnPosition);
        CameraTargetBinder.Instance.SetTarget(ActivePlayer.transform);
        return ActivePlayer;
    }
}
