/// <summary>
/// 设置数据管理器
/// 负责从本地读取音频设置，并在设置变化后写回本地二进制文件
/// </summary>
public class SettingDataManager
{
    // 设置数据保存文件名，不包含扩展名
    private const string SaveFileName = "SettingData";

    private static SettingDataManager instance;

    public static SettingDataManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new SettingDataManager();
            }

            return instance;
        }
    }

    // 当前内存中的设置数据
    public SettingData Data { get; private set; }

    private SettingDataManager()
    {
        Load();
    }

    /// <summary>
    /// 从本地读取设置数据
    /// 如果本地没有存档，则创建默认设置并立即保存
    /// </summary>
    public void Load()
    {
        Data = BinaryDataMgr.Instance.Load<SettingData>(SaveFileName);
        if (Data == null)
        {
            Data = new SettingData();
            Save();
            return;
        }

        Data.Repair();
    }

    /// <summary>
    /// 保存当前设置数据到本地
    /// </summary>
    public void Save()
    {
        EnsureData();
        BinaryDataMgr.Instance.Save(SaveFileName, Data);
    }

    /// <summary>
    /// 确保设置数据不为空，并在保存前修复音量范围
    /// </summary>
    private void EnsureData()
    {
        if (Data == null)
        {
            Data = new SettingData();
        }

        Data.Repair();
    }
}
