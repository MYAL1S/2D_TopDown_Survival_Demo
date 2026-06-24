using System.Collections.Generic;

/// <summary>
/// 玩家数据管理器类 用于加载和保存玩家数据
/// </summary>
public class PlayerDataManager
{
    // 保存文件名
    private const string SaveFileName = "PlayerData";
    // 默认解锁的角色id
    private const string DefaultCharacterId = "player_mage";
    // 默认解锁的关卡id
    private const string DefaultStageId = "stage_1";

    private static PlayerDataManager instance;

    public static PlayerDataManager Instance
    {
        get 
        {
            if (instance == null)
                instance = new PlayerDataManager();
            return instance;
        }
    }

    // 玩家数据对象
    public PlayerData Data { get; private set; }

    private PlayerDataManager()
    {
        Load(false);
    }

    /// <summary>
    /// 加载玩家数据 
    /// 如果没有数据则创建默认数据
    /// </summary>
    public void Load()
    {
        Load(true);
    }

    private void Load(bool notifyChanged)
    {
        // 尝试从二进制文件中加载玩家数据
        Data = BinaryDataMgr.Instance.Load<PlayerData>(SaveFileName);
        // 如果加载失败
        if (Data == null)
        {
            // 则创建默认数据并保存
            Data = CreateDefaultData();
            Save();
        }
        else
        {
            // 加载成功后修复数据
            // 确保数据完整性
            RepairData(Data);
        }

        // 通知金币数量变化和数据变化
        if (notifyChanged)
        {
            NotifyDataChanged();
        }
    }

    /// <summary>
    /// 保存玩家数据到二进制文件
    /// </summary>
    public void Save()
    {
        BinaryDataMgr.Instance.Save(SaveFileName, Data);
    }

    /// <summary>
    /// 重置玩家数据为默认数据
    /// </summary>
    public void ResetToDefault()
    {
        Data = CreateDefaultData();
        Save();
        NotifyGoldChanged();
        NotifyDataChanged();
    }

    /// <summary>
    /// 增加金币的方法
    /// </summary>
    /// <param name="amount">金币数</param>
    /// <param name="saveImmediately">是否立即保存</param>
    public void AddGold(int amount, bool saveImmediately = true)
    {
        // 记录增加前的金币数    
        int before = Data.gold;
        Data.AddGold(amount);

        if (Data.gold == before)
        {
            return;
        }

        SaveIfNeeded(saveImmediately);
        NotifyGoldChanged();
        NotifyDataChanged();
    }

    /// <summary>
    /// 消费金币的方法
    /// </summary>
    /// <param name="amount">金币数</param>
    /// <param name="saveImmediately">是否立即保存</param>
    /// <returns></returns>
    public bool SpendGold(int amount, bool saveImmediately = true)
    {
        int before = Data.gold;
        // 尝试消费金币 如果金币不足则返回false
        bool success = Data.SpendGold(amount);
        if (!success)
        {
            return false;
        }

        SaveIfNeeded(saveImmediately);
        // 如果金币数发生变化
        // 则触发金币变化事件
        if (Data.gold != before)
        {
            NotifyGoldChanged();
        }

        NotifyDataChanged();
        return true;
    }

    /// <summary>
    /// 解锁角色的方法
    /// </summary>
    /// <param name="characterId">角色ID</param>
    /// <param name="saveImmediately">是否立即保存</param>
    /// <returns></returns>
    public bool UnlockCharacter(string characterId, bool saveImmediately = true)
    {
        // 尝试解锁角色
        bool unlocked = Data.UnlockCharacter(characterId);
        if (!unlocked)
        {
            return false;
        }

        SaveIfNeeded(saveImmediately);
        // 触发角色解锁事件
        EventCenter.Instance.EventTrigger(E_EventType.CharacterUnlocked, characterId);
        // 通知数据变化
        NotifyDataChanged();
        return true;
    }

    /// <summary>
    /// 尝试购买并解锁角色
    /// 已解锁角色直接返回成功；未解锁时会先扣金币，再写入解锁列表并保存
    /// </summary>
    /// <param name="config">要购买的角色配置</param>
    /// <returns>购买或已解锁返回 true，金币不足或配置无效返回 false</returns>
    public bool TryPurchaseCharacter(PlayerConfig config)
    {
        if (config == null || string.IsNullOrWhiteSpace(config.ResourceId))
        {
            return false;
        }

        string characterId = config.ResourceId;
        if (IsCharacterUnlocked(characterId))
        {
            return true;
        }

        int price = config.UnlockPrice;
        if (!Data.SpendGold(price))
        {
            return false;
        }

        if (!Data.UnlockCharacter(characterId))
        {
            Data.AddGold(price);
            return false;
        }

        Save();
        NotifyGoldChanged();
        EventCenter.Instance.EventTrigger(E_EventType.CharacterUnlocked, characterId);
        NotifyDataChanged();
        return true;
    }

    /// <summary>
    /// 选择一个已经解锁的角色，并根据参数决定是否立即持久化
    /// </summary>
    /// <param name="characterId">要选择的角色 id</param>
    /// <param name="saveImmediately">是否立即保存到本地</param>
    /// <returns>角色选择发生变化返回 true</returns>
    public bool SelectCharacter(string characterId, bool saveImmediately = true)
    {
        if (Data == null || !Data.SelectCharacter(characterId))
        {
            return false;
        }

        SaveIfNeeded(saveImmediately);
        NotifyDataChanged();
        return true;
    }

    /// <summary>
    ///  解锁关卡的方法
    /// </summary>
    /// <param name="stageId">关卡ID</param>
    /// <param name="saveImmediately">是否立即保存</param>
    /// <returns></returns>
    public bool UnlockStage(string stageId, bool saveImmediately = true)
    {
        bool unlocked = Data.UnlockStage(stageId);
        if (!unlocked)
        {
            return false;
        }

        SaveIfNeeded(saveImmediately);
        EventCenter.Instance.EventTrigger(E_EventType.StageUnlocked, stageId);
        NotifyDataChanged();
        return true;
    }

    /// <summary>
    /// 检查角色是否已解锁
    /// </summary>
    /// <param name="characterId">角色id</param>
    /// <returns></returns>
    public bool IsCharacterUnlocked(string characterId)
    {
        return Data != null && Data.IsCharacterUnlocked(characterId);
    }

    /// <summary>
    /// 检查关卡是否已解锁
    /// </summary>
    /// <param name="stageId">关卡id</param>
    /// <returns></returns>
    public bool IsStageUnlocked(string stageId)
    {
        return Data != null && Data.IsStageUnlocked(stageId);
    }

    /// <summary>
    /// 创建默认的玩家数据
    /// </summary>
    /// <returns></returns>
    private static PlayerData CreateDefaultData()
    {
        PlayerData data = new PlayerData();
        data.EnsureDefaultUnlocks(DefaultCharacterId, DefaultStageId);
        return data;
    }

    /// <summary>
    /// 修复玩家数据
    /// 确保数据完整性
    /// </summary>
    /// <param name="data">玩家数据</param>
    private static void RepairData(PlayerData data)
    {
        if (data.unlockedCharacterIds == null)
        {
            data.unlockedCharacterIds = new List<string>();
        }

        if (data.unlockedStageIds == null)
        {
            data.unlockedStageIds = new List<string>();
        }

        if (data.gold < 0)
        {
            data.gold = 0;
        }

        data.EnsureDefaultUnlocks(DefaultCharacterId, DefaultStageId);
    }

    /// <summary>
    /// 根据参数决定是否立即保存玩家数据
    /// </summary>
    /// <param name="saveImmediately">是否立即保存</param>
    private void SaveIfNeeded(bool saveImmediately)
    {
        if (saveImmediately)
        {
            Save();
        }
    }

    /// <summary>
    /// 通知金币数量发生变化
    /// </summary>
    private void NotifyGoldChanged()
    {
        EventCenter.Instance.EventTrigger(E_EventType.PlayerGoldChanged, Data.gold);
    }

    /// <summary>
    /// 通知玩家数据发生变化
    /// </summary>
    private void NotifyDataChanged()
    {
        EventCenter.Instance.EventTrigger(E_EventType.PlayerDataChanged, Data);
    }
}
