using System;
using System.Data;
using System.IO;
using System.Text;
using Excel;
using UnityEditor;
using UnityEngine;

/// <summary>
/// Excel 数据生成工具。
/// 根据 Excel 表生成二进制数据文件、单行数据类和数据容器类。
/// </summary>
public class ExcelTool
{
    /// <summary>
    /// Excel 表所在目录。
    /// </summary>
    public static string Excel_Path = Application.dataPath + "/Data/Excel/";

    /// <summary>
    /// 自动生成的数据类目录。
    /// </summary>
    public static string Data_Class_Path = Application.dataPath + "/Scripts/Data/ExcelData/DataClass/";

    /// <summary>
    /// 自动生成的数据容器类目录。
    /// </summary>
    public static string Data_Container_Path = Application.dataPath + "/Scripts/Data/ExcelData/DataContainer/";

    /// <summary>
    /// Excel 表数据起始行。
    /// 前四行分别为变量名、变量类型、主键标记和字段描述。
    /// </summary>
    public static int BEGIN_INDEX = 4;

    /// <summary>
    /// 从菜单触发 Excel 数据生成。
    /// </summary>
    [MenuItem("GameTool/GenerateExcelInfo")]
    private static void GenerateExcelInfo()
    {
        DirectoryInfo dirInfo = Directory.CreateDirectory(Excel_Path);
        FileInfo[] fileInfos = dirInfo.GetFiles();
        DataTableCollection tableCollection;

        for (int i = 0; i < fileInfos.Length; i++)
        {
            if (fileInfos[i].Extension != ".xlsx" && fileInfos[i].Extension != ".xls")
            {
                continue;
            }

            using (FileStream fs = fileInfos[i].Open(FileMode.Open, FileAccess.Read))
            {
                IExcelDataReader excelReader = ExcelReaderFactory.CreateOpenXmlReader(fs);
                tableCollection = excelReader.AsDataSet().Tables;
            }

            foreach (DataTable tableInfo in tableCollection)
            {
                GenerateBinaryFile(tableInfo);
                GenerateExcelDataClass(tableInfo);
                GenerateExcelDataContainer(tableInfo);
            }
        }
    }

    /// <summary>
    /// 根据 Excel 表头生成单行数据类。
    /// </summary>
    /// <param name="data">Excel 工作表数据。</param>
    private static void GenerateExcelDataClass(DataTable data)
    {
        DataRow variableName = GetVariableNameRow(data);
        DataRow variableType = GetVariableTypeRow(data);

        if (!Directory.Exists(Data_Class_Path))
        {
            Directory.CreateDirectory(Data_Class_Path);
        }

        string fileContent = "public class " + data.TableName + "\n{\n";

        for (int i = 0; i < data.Columns.Count; i++)
        {
            fileContent += "    " + "public " + variableType[i].ToString() + " " + variableName[i].ToString() + ";\n";
        }

        fileContent += "}";

        File.WriteAllText(Data_Class_Path + data.TableName + ".cs", fileContent);
        AssetDatabase.Refresh();
    }

    /// <summary>
    /// 根据 Excel 表生成数据容器类。
    /// 容器内使用主键类型到数据类的字典保存所有行。
    /// </summary>
    /// <param name="data">Excel 工作表数据。</param>
    public static void GenerateExcelDataContainer(DataTable data)
    {
        DataRow rowType = GetVariableTypeRow(data);
        int keyIndex = GetKeyIndex(data);

        if (!Directory.Exists(Data_Container_Path))
        {
            Directory.CreateDirectory(Data_Container_Path);
        }

        string fileContent = "using System.Collections.Generic;\n";
        fileContent += "public class " + data.TableName + "Container\n{\n";
        fileContent += "    " + "public Dictionary<" + rowType[keyIndex].ToString() + "," + data.TableName + "> dataDic = ";
        fileContent += "new Dictionary<" + rowType[keyIndex].ToString() + "," + data.TableName + ">();\n";
        fileContent += "}";

        File.WriteAllText(Data_Container_Path + data.TableName + "Container.cs", fileContent);
        AssetDatabase.Refresh();
    }

    /// <summary>
    /// 根据 Excel 表内容生成二进制数据文件。
    /// </summary>
    /// <param name="data">Excel 工作表数据。</param>
    private static void GenerateBinaryFile(DataTable data)
    {
        if (!Directory.Exists(BinaryDataMgr.Data_Binary_Path))
        {
            Directory.CreateDirectory(BinaryDataMgr.Data_Binary_Path);
        }

        DataRow rowInfo;
        DataRow rowType = GetVariableTypeRow(data);

        using (FileStream fs = new FileStream(BinaryDataMgr.Data_Binary_Path + data.TableName + ".zzz", FileMode.OpenOrCreate, FileAccess.Write))
        {
            fs.Write(BitConverter.GetBytes(data.Rows.Count - BEGIN_INDEX), 0, 4);

            string keyName = GetVariableNameRow(data)[GetKeyIndex(data)].ToString();
            byte[] keyNameBytes = Encoding.UTF8.GetBytes(keyName);
            fs.Write(BitConverter.GetBytes(keyNameBytes.Length), 0, 4);
            fs.Write(keyNameBytes, 0, keyNameBytes.Length);

            for (int i = BEGIN_INDEX; i < data.Rows.Count; i++)
            {
                rowInfo = data.Rows[i];
                for (int j = 0; j < data.Columns.Count; j++)
                {
                    switch (rowType[j].ToString())
                    {
                        case "int":
                            fs.Write(BitConverter.GetBytes(int.Parse(rowInfo[j].ToString())), 0, 4);
                            break;
                        case "float":
                            fs.Write(BitConverter.GetBytes(float.Parse(rowInfo[j].ToString())), 0, 4);
                            break;
                        case "string":
                            byte[] bytes = Encoding.UTF8.GetBytes(rowInfo[j].ToString());
                            fs.Write(BitConverter.GetBytes(bytes.Length), 0, 4);
                            fs.Write(bytes, 0, bytes.Length);
                            break;
                        case "boolean":
                            fs.Write(BitConverter.GetBytes(bool.Parse(rowInfo[j].ToString())), 0, 1);
                            break;
                    }
                }
            }
        }

        AssetDatabase.Refresh();
    }

    /// <summary>
    /// 获取变量名所在行。
    /// </summary>
    /// <param name="data">Excel 工作表数据。</param>
    /// <returns>变量名行。</returns>
    private static DataRow GetVariableNameRow(DataTable data)
    {
        return data.Rows[0];
    }

    /// <summary>
    /// 获取变量类型所在行。
    /// </summary>
    /// <param name="data">Excel 工作表数据。</param>
    /// <returns>变量类型行。</returns>
    private static DataRow GetVariableTypeRow(DataTable data)
    {
        return data.Rows[1];
    }

    /// <summary>
    /// 获取主键所在列索引。
    /// </summary>
    /// <param name="data">Excel 工作表数据。</param>
    /// <returns>主键列索引，未配置时返回 0。</returns>
    private static int GetKeyIndex(DataTable data)
    {
        DataRow rowKey = data.Rows[2];
        for (int i = 0; i < data.Columns.Count; i++)
        {
            if (rowKey[i].ToString() == "key")
            {
                return i;
            }
        }

        return 0;
    }
}
