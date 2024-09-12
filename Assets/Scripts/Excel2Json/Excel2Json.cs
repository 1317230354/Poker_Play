using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using ExcelDataReader;
using Newtonsoft.Json;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

public class Excel2Json : MonoBehaviour
{
    //将单个文件转换为json
    public string excelFilePath;
    public string jsonOutputPath;
    //将文件夹路径下所有excel转换为json文件
    public string folderPath;
    public string outputFolder;

    // 在Unity编辑器中添加菜单选项
    [MenuItem("Tools/将单个Excel转换为Json")]
    static void ConvertExcelToJsonFromMenu()
    {
        var instance = FindObjectOfType<Excel2Json>();
        if (instance != null)
        {
            instance.ConvertExcelToJson(instance.excelFilePath, instance.jsonOutputPath);
        }
        else
        {
            Debug.LogError("Excel2Json component not found in scene.");
        }
    }
    [MenuItem("Tools/将文件夹下Excel转换为Json")]
    static void ConvertAllExcelToJsonFromMenu()
    {

        var instance = FindObjectOfType<Excel2Json>();
        if (instance != null)
        {
            instance.ConvertExcelFilesToJson(instance.folderPath, instance.outputFolder);
        }
        else
        {
            Debug.LogError("Excel2Json component not found in scene.");
        }

    }

    /// <summary>
    /// 将文件夹路径下所有excel转换为json文件
    /// </summary>
    /// <param name="folderPath"></param>
    /// <param name="outputFolder"></param>
    void ConvertExcelFilesToJson(string folderPath, string outputFolder)
    {
        try
        {
            // 检查输出文件夹是否存在，不存在则创建
            if (!Directory.Exists(outputFolder))
            {
                Directory.CreateDirectory(outputFolder);
            }

            // 遍历文件夹下所有的Excel文件
            foreach (string filePath in Directory.GetFiles(folderPath, "*.xlsx"))
            {
                // 构造输出JSON文件路径
                string jsonFilePath = Path.Combine(outputFolder, Path.GetFileNameWithoutExtension(filePath) + ".json");
                // 转换当前Excel文件为JSON
                ConvertExcelToJson(filePath, jsonFilePath);
            }

            Debug.Log("转换所有文件成功.");
        }
        catch (Exception ex)
        {
            Debug.LogError($"Excel转Json出错: {ex.Message}");
        }
    }
    /// <summary>
    /// 将单个文件转换为json
    /// </summary>
    /// <param name="excelPath"></param>
    /// <param name="jsonPath"></param>
    void ConvertExcelToJson(string excelPath, string jsonPath)
    {
        try
        {
            using (var stream = File.Open(excelPath, FileMode.Open, FileAccess.Read))
            {
                using (var reader = ExcelReaderFactory.CreateReader(stream))
                {
                    var result = reader.AsDataSet();
                    var json = ProcessDataSet(result);
                    File.WriteAllText(jsonPath, json);
                    Debug.Log("转换单个文件成功");
                }
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"Excel转Json出错: {ex.Message}");
        }
    }
    /// <summary>
    /// excel文件的转换规则
    /// </summary>
    /// <param name="dataSet"></param>
    /// <returns></returns>

    string ProcessDataSet(DataSet dataSet)
    {
        var table = dataSet.Tables[0];
        var rows = table.Rows.Cast<DataRow>();
        var columns = table.Columns.Cast<DataColumn>();

        // 获取第二行，用于确定数据类型
        var typeRow = table.Rows[1];

        var jsonArray = rows.Skip(2).Select(row => columns.ToDictionary(column => table.Rows[0][column].ToString(), column =>
        {
            var cellValue = row[column].ToString();
            var dataType = typeRow[column].ToString(); // 第二行表示的数据类型

            // 根据第二行的类型进行数据转换
            switch (dataType.ToLower())
            {
                case "int":
                    if (int.TryParse(cellValue, out int intValue))
                        return intValue;
                    break;

                case "float":
                    if (float.TryParse(cellValue, out float floatValue))
                        return floatValue;
                    break;
                case "string":
                    return cellValue;

                case "bool":
                    if (bool.TryParse(cellValue, out bool boolValue))
                        return boolValue;
                    break;
                case "intarray": // int类型的数组，以逗号分隔
                    return cellValue.Split('|')
                        .Select(item => int.TryParse(item.Trim(), out int intResult) ? intResult : (int?)null)
                        .Where(item => item.HasValue)
                        .Select(item => item.Value)
                        .ToArray();

                case "floatarray": // float类型的数组，以逗号分隔
                    return cellValue.Split('|')
                        .Select(item => float.TryParse(item.Trim(), out float floatResult) ? floatResult : (float?)null)
                        .Where(item => item.HasValue)
                        .Select(item => item.Value)
                        .ToArray();

                case "stringarray": // string类型的数组，以逗号分隔
                    return cellValue.Split('|')
                        .Select(item => item.Trim())
                        .Where(item => !string.IsNullOrEmpty(item))
                        .ToArray();

                case "custom": // 自定义类型处理
                    return CustomExcelArray((value) =>
                    {
                        // 这里根据需要处理自定义类型
                        return new List<string>(); // 这里返回你需要的自定义对象
                    });


                default: // 默认当做字符串处理
                    return cellValue;
            }

            // 如果转换失败，返回原始字符串
            return cellValue;

            // 自定义数组类型处理方法
            object CustomExcelArray<T>(Func<string[], T> setValue)
            {
                if (cellValue.Contains("|"))
                {
                    string[] customString = cellValue
                        .Split('|')
                        .Select(item => item.Trim())
                        .Where(item => !string.IsNullOrEmpty(item))
                        .ToArray();
                    List<T> TArray = new List<T>();
                    foreach (var oneString in customString)
                    {
                        var Elements = oneString.Split(',');
                        T element = setValue.Invoke(Elements);
                        TArray.Add(element);
                    }
                    return TArray;
                }
                else
                {
                    var Elements = cellValue.Split(',');
                    return setValue.Invoke(Elements);
                }
            }

        })).ToArray();

        return JsonConvert.SerializeObject(jsonArray, Formatting.Indented);
    }

    //string ProcessDataSet(DataSet dataSet)
    //{
    //    var table = dataSet.Tables[0];
    //    var rows = table.Rows.Cast<DataRow>();
    //    var columns = table.Columns.Cast<DataColumn>();

    //    var jsonArray = rows.Skip(2).Select(row => columns.ToDictionary(column => table.Rows[0][column].ToString(), column =>
    //    {
    //        var cellValue = row[column].ToString();
    //        //返回整型或浮点
    //        if (double.TryParse(cellValue, out double number)) // 检测是否为数字
    //        {
    //            if (number % 1 == 0) // 如果是整数
    //                return Convert.ToInt32(number);
    //            else // 如果是浮点数
    //                return number;
    //        }
    //        //返回bool值
    //        else if (bool.TryParse(cellValue, out bool boolValue)) // 检测是否为布尔类型
    //        {
    //            return boolValue;
    //        }
    //    // 检测是否为自定义类型
    //    switch (table.Rows[0][column].ToString())
    //    {

    //        //Excel规则：分号为数组，逗号为自定义类的分割
    //        //--------------------------------------------------
    //        //更多类型从此处进行补充
    //        case "example!!":
    //            //赋值函数
    //            return CostomExcelArray((value) =>
    //            {
    //                //进行赋值
    //                //EffectBean effectBean = new EffectBean();
    //                //effectBean.EffectType = value[0];
    //                //effectBean.EffectValue = float.Parse(value[1]);
    //                //return effectBean;
    //                return new List<string>();//暂时不需要
    //            });
    //                    //---------------------------------------------------
    //    }
    //            //返回字符串数组
    //            if (cellValue.Contains("|"))
    //        {
    //            return cellValue
    //           .Split('|')
    //           .Select(item => item.Trim())
    //           .Where(item => !string.IsNullOrEmpty(item))
    //           .ToArray();
    //        }
    //        //返回字符串
    //        return cellValue;

    //        object CostomExcelArray<T>(Func<string[], T> setValue)
    //        {
    //            //根据;将值划分为字符串数组
    //            if (cellValue.Contains("|"))
    //            {
    //                string[] customString = cellValue
    //         .Split('|')
    //         .Select(item => item.Trim())
    //         .Where(item => !string.IsNullOrEmpty(item))
    //         .ToArray();
    //                //当前值为数组
    //                List<T> TArray = new List<T>();
    //                foreach (var oneString in customString)
    //                {
    //                    var Elements = oneString.Split(',');
    //                    //添加赋值逻辑
    //                    T element = setValue.Invoke(Elements);
    //                    TArray.Add(element);
    //                }
    //                return TArray;
    //            }
    //            else
    //            {
    //                var Elements = cellValue.Split(',');
    //                return setValue.Invoke(Elements);
    //            }
    //        }

    //    })).ToArray();

    //    return JsonConvert.SerializeObject(jsonArray, Formatting.Indented);

    //}
}