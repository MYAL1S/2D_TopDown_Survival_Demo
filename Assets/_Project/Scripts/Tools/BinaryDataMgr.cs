using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using UnityEngine;

/// <summary>
/// 二进制数据管理器
/// 负责读取 Excel 工具生成的二进制表数据，也负责保存和读取玩家本地存档数据
/// </summary>
public class BinaryDataMgr
{
    /// <summary>
    /// 本地存档保存目录
    /// </summary>
    private string Save_Path = Application.persistentDataPath + "/Data/";

    /// <summary>
    /// Excel 表生成的二进制文件目录
    /// </summary>
    public static string Data_Binary_Path = Application.streamingAssetsPath + "/DataBinary/";

    /// <summary>
    /// 已加载到内存中的 Excel 表数据
    /// key 为数据容器类名，value 为数据容器对象
    /// </summary>
    public Dictionary<string, object> tableDic = new Dictionary<string, object>();

    private static BinaryDataMgr instance = new BinaryDataMgr();
    public static BinaryDataMgr Instance => instance;

    private BinaryDataMgr()
    {
        InitData();
    }

    /// <summary>
    /// 初始化表数据
    /// 当前项目按需加载表数据，因此这里暂时不主动加载
    /// </summary>
    public void InitData()
    {

    }

    /// <summary>
    /// 获取已经加载到内存中的表数据容器
    /// </summary>
    /// <typeparam name="T">数据容器类型</typeparam>
    /// <returns>数据容器对象，未加载时返回 null</returns>
    public T GetTable<T>() where T : class
    {
        if (tableDic.ContainsKey(typeof(T).Name))
        {
            return (T)tableDic[typeof(T).Name];
        }

        return null;
    }

    /// <summary>
    /// 从本地二进制文件加载 Excel 表数据到内存
    /// </summary>
    /// <typeparam name="T">数据容器类</typeparam>
    /// <typeparam name="K">单行数据结构类</typeparam>
    public void LoadTable<T, K>()
    {
        using (FileStream fs = File.Open(Data_Binary_Path + typeof(K).Name + ".zzz", FileMode.Open, FileAccess.Read))
        {
            byte[] data = new byte[fs.Length];
            fs.Read(data, 0, data.Length);

            int index = 0;
            int count = BitConverter.ToInt32(data, index);
            index += 4;

            int keyNameLength = BitConverter.ToInt32(data, index);
            index += 4;

            string keyName = Encoding.UTF8.GetString(data, index, keyNameLength);
            index += keyNameLength;

            Type dataContainerType = typeof(T);
            object dataContainerObj = Activator.CreateInstance(dataContainerType);

            Type dataType = typeof(K);
            FieldInfo[] fieldInfos = dataType.GetFields();

            for (int i = 0; i < count; i++)
            {
                object dataObj = Activator.CreateInstance(dataType);
                foreach (FieldInfo info in fieldInfos)
                {
                    if (info.FieldType == typeof(int))
                    {
                        info.SetValue(dataObj, BitConverter.ToInt32(data, index));
                        index += 4;
                    }
                    else if (info.FieldType == typeof(float))
                    {
                        info.SetValue(dataObj, BitConverter.ToSingle(data, index));
                        index += 4;
                    }
                    else if (info.FieldType == typeof(bool))
                    {
                        info.SetValue(dataObj, BitConverter.ToBoolean(data, index));
                        index += 1;
                    }
                    else if (info.FieldType == typeof(string))
                    {
                        int strLength = BitConverter.ToInt32(data, index);
                        index += 4;
                        info.SetValue(dataObj, Encoding.UTF8.GetString(data, index, strLength));
                        index += strLength;
                    }
                }

                object dataDicObj = dataContainerType.GetField("dataDic").GetValue(dataContainerObj);
                MethodInfo methodInfo = dataDicObj.GetType().GetMethod("Add");
                object keyValue = dataType.GetField(keyName).GetValue(dataObj);
                methodInfo.Invoke(dataDicObj, new object[] { keyValue, dataObj });
            }

            tableDic.Add(typeof(T).Name, dataContainerObj);
        }
    }

    /// <summary>
    /// 将数据对象保存到本地二进制文件
    /// </summary>
    /// <param name="name">文件名，不包含扩展名</param>
    /// <param name="data">需要保存的数据对象</param>
    public void Save(string name, object data)
    {
        if (!Directory.Exists(Save_Path))
        {
            Directory.CreateDirectory(Save_Path);
        }

        using (FileStream fs = new FileStream(Save_Path + name + ".zzz", FileMode.OpenOrCreate, FileAccess.Write))
        {
            BinaryFormatter bf = new BinaryFormatter();
            bf.Serialize(fs, data);
            fs.Flush();
        }
    }

    /// <summary>
    /// 从本地二进制文件读取数据对象
    /// </summary>
    /// <typeparam name="T">数据对象类型</typeparam>
    /// <param name="name">文件名，不包含扩展名</param>
    /// <returns>读取到的数据对象，文件不存在时返回 null</returns>
    public T Load<T>(string name) where T : class
    {
        if (!File.Exists(Save_Path + name + ".zzz"))
        {
            Debug.LogWarning("未找到指定名称的文件");
            return null;
        }

        using (FileStream fs = new FileStream(Save_Path + name + ".zzz", FileMode.Open, FileAccess.Read))
        {
            BinaryFormatter bs = new BinaryFormatter();
            return bs.Deserialize(fs) as T;
        }
    }
}
