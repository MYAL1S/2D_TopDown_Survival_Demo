using System;
using System.Collections.Generic;

/// <summary>
/// 玩家本地数据模型
/// 只保存适合持久化的简单数据，例如金币、已解锁角色、已解锁关卡和当前选中角色
/// </summary>
[Serializable]
public class PlayerData
{
    // 玩家当前持有金币数量
    public int gold;
    // 已解锁角色 id 列表，对应 PlayerConfig.ResourceId
    public List<string> unlockedCharacterIds = new List<string>();
    // 已解锁关卡 id 列表，对应 StageConfig.ResourceId
    public List<string> unlockedStageIds = new List<string>();
    // 当前选中的角色 id，用于进入关卡时生成对应角色
    public string selectedCharacterId;

    /// <summary>
    /// 判断指定角色是否已经解锁
    /// </summary>
    /// <param name="characterId">角色配置 id</param>
    /// <returns>已解锁返回 true</returns>
    public bool IsCharacterUnlocked(string characterId)
    {
        return ContainsId(unlockedCharacterIds, characterId);
    }

    /// <summary>
    /// 判断指定角色是否为当前选中角色
    /// </summary>
    /// <param name="characterId">角色配置 id</param>
    /// <returns>当前选中该角色返回 true</returns>
    public bool IsCharacterSelected(string characterId)
    {
        return !string.IsNullOrWhiteSpace(characterId) && selectedCharacterId == characterId;
    }

    /// <summary>
    /// 判断指定关卡是否已经解锁
    /// </summary>
    /// <param name="stageId">关卡配置 id</param>
    /// <returns>已解锁返回 true</returns>
    public bool IsStageUnlocked(string stageId)
    {
        return ContainsId(unlockedStageIds, stageId);
    }

    /// <summary>
    /// 增加金币
    /// </summary>
    /// <param name="amount">增加数量，小于等于 0 时忽略</param>
    public void AddGold(int amount)
    {
        if (amount <= 0)
        {
            return;
        }

        gold += amount;
    }

    /// <summary>
    /// 消费金币
    /// </summary>
    /// <param name="amount">消费数量，小于等于 0 时视为成功</param>
    /// <returns>金币足够并扣除成功返回 true</returns>
    public bool SpendGold(int amount)
    {
        if (amount <= 0)
        {
            return true;
        }

        if (gold < amount)
        {
            return false;
        }

        gold -= amount;
        return true;
    }

    /// <summary>
    /// 解锁角色
    /// </summary>
    /// <param name="characterId">角色配置 id</param>
    /// <returns>本次新增解锁返回 true</returns>
    public bool UnlockCharacter(string characterId)
    {
        return AddUniqueId(unlockedCharacterIds, characterId);
    }

    /// <summary>
    /// 选择已解锁角色
    /// </summary>
    /// <param name="characterId">角色配置 id</param>
    /// <returns>选中角色发生变化返回 true</returns>
    public bool SelectCharacter(string characterId)
    {
        if (!IsCharacterUnlocked(characterId) || selectedCharacterId == characterId)
        {
            return false;
        }

        selectedCharacterId = characterId;
        return true;
    }

    /// <summary>
    /// 解锁关卡
    /// </summary>
    /// <param name="stageId">关卡配置 id</param>
    /// <returns>本次新增解锁返回 true</returns>
    public bool UnlockStage(string stageId)
    {
        return AddUniqueId(unlockedStageIds, stageId);
    }

    /// <summary>
    /// 确保玩家至少拥有默认角色和默认关卡
    /// </summary>
    /// <param name="defaultCharacterId">默认角色 id</param>
    /// <param name="defaultStageId">默认关卡 id</param>
    public void EnsureDefaultUnlocks(string defaultCharacterId, string defaultStageId)
    {
        UnlockCharacter(defaultCharacterId);
        UnlockStage(defaultStageId);

        if (string.IsNullOrWhiteSpace(selectedCharacterId) || !IsCharacterUnlocked(selectedCharacterId))
        {
            selectedCharacterId = defaultCharacterId;
        }
    }

    /// <summary>
    /// 向 id 列表中添加一个不重复的有效 id
    /// </summary>
    /// <param name="ids">目标 id 列表</param>
    /// <param name="id">要添加的 id</param>
    /// <returns>成功添加返回 true</returns>
    private static bool AddUniqueId(List<string> ids, string id)
    {
        if (ids == null || string.IsNullOrWhiteSpace(id) || ids.Contains(id))
        {
            return false;
        }

        ids.Add(id);
        return true;
    }

    /// <summary>
    /// 判断 id 列表中是否存在指定 id
    /// </summary>
    /// <param name="ids">目标 id 列表</param>
    /// <param name="id">要查找的 id</param>
    /// <returns>存在返回 true</returns>
    private static bool ContainsId(List<string> ids, string id)
    {
        return ids != null && !string.IsNullOrWhiteSpace(id) && ids.Contains(id);
    }
}
