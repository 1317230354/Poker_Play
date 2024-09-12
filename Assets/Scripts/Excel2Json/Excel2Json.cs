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
    //�������ļ�ת��Ϊjson
    public string excelFilePath;
    public string jsonOutputPath;
    //���ļ���·��������excelת��Ϊjson�ļ�
    public string folderPath;
    public string outputFolder;

    // ��Unity�༭������Ӳ˵�ѡ��
    [MenuItem("Tools/������Excelת��ΪJson")]
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
    [MenuItem("Tools/���ļ�����Excelת��ΪJson")]
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
    /// ���ļ���·��������excelת��Ϊjson�ļ�
    /// </summary>
    /// <param name="folderPath"></param>
    /// <param name="outputFolder"></param>
    void ConvertExcelFilesToJson(string folderPath, string outputFolder)
    {
        try
        {
            // �������ļ����Ƿ���ڣ��������򴴽�
            if (!Directory.Exists(outputFolder))
            {
                Directory.CreateDirectory(outputFolder);
            }

            // �����ļ��������е�Excel�ļ�
            foreach (string filePath in Directory.GetFiles(folderPath, "*.xlsx"))
            {
                // �������JSON�ļ�·��
                string jsonFilePath = Path.Combine(outputFolder, Path.GetFileNameWithoutExtension(filePath) + ".json");
                // ת����ǰExcel�ļ�ΪJSON
                ConvertExcelToJson(filePath, jsonFilePath);
            }

            Debug.Log("ת�������ļ��ɹ�.");
        }
        catch (Exception ex)
        {
            Debug.LogError($"ExcelתJson����: {ex.Message}");
        }
    }
    /// <summary>
    /// �������ļ�ת��Ϊjson
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
                    Debug.Log("ת�������ļ��ɹ�");
                }
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"ExcelתJson����: {ex.Message}");
        }
    }
    /// <summary>
    /// excel�ļ���ת������
    /// </summary>
    /// <param name="dataSet"></param>
    /// <returns></returns>

    string ProcessDataSet(DataSet dataSet)
    {
        var table = dataSet.Tables[0];
        var rows = table.Rows.Cast<DataRow>();
        var columns = table.Columns.Cast<DataColumn>();

        // ��ȡ�ڶ��У�����ȷ����������
        var typeRow = table.Rows[1];

        var jsonArray = rows.Skip(2).Select(row => columns.ToDictionary(column => table.Rows[0][column].ToString(), column =>
        {
            var cellValue = row[column].ToString();
            var dataType = typeRow[column].ToString(); // �ڶ��б�ʾ����������

            // ���ݵڶ��е����ͽ�������ת��
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
                case "intarray": // int���͵����飬�Զ��ŷָ�
                    return cellValue.Split('|')
                        .Select(item => int.TryParse(item.Trim(), out int intResult) ? intResult : (int?)null)
                        .Where(item => item.HasValue)
                        .Select(item => item.Value)
                        .ToArray();

                case "floatarray": // float���͵����飬�Զ��ŷָ�
                    return cellValue.Split('|')
                        .Select(item => float.TryParse(item.Trim(), out float floatResult) ? floatResult : (float?)null)
                        .Where(item => item.HasValue)
                        .Select(item => item.Value)
                        .ToArray();

                case "stringarray": // string���͵����飬�Զ��ŷָ�
                    return cellValue.Split('|')
                        .Select(item => item.Trim())
                        .Where(item => !string.IsNullOrEmpty(item))
                        .ToArray();

                case "custom": // �Զ������ʹ���
                    return CustomExcelArray((value) =>
                    {
                        // ���������Ҫ�����Զ�������
                        return new List<string>(); // ���ﷵ������Ҫ���Զ������
                    });


                default: // Ĭ�ϵ����ַ�������
                    return cellValue;
            }

            // ���ת��ʧ�ܣ�����ԭʼ�ַ���
            return cellValue;

            // �Զ����������ʹ�����
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
    //        //�������ͻ򸡵�
    //        if (double.TryParse(cellValue, out double number)) // ����Ƿ�Ϊ����
    //        {
    //            if (number % 1 == 0) // ���������
    //                return Convert.ToInt32(number);
    //            else // ����Ǹ�����
    //                return number;
    //        }
    //        //����boolֵ
    //        else if (bool.TryParse(cellValue, out bool boolValue)) // ����Ƿ�Ϊ��������
    //        {
    //            return boolValue;
    //        }
    //    // ����Ƿ�Ϊ�Զ�������
    //    switch (table.Rows[0][column].ToString())
    //    {

    //        //Excel���򣺷ֺ�Ϊ���飬����Ϊ�Զ�����ķָ�
    //        //--------------------------------------------------
    //        //�������ʹӴ˴����в���
    //        case "example!!":
    //            //��ֵ����
    //            return CostomExcelArray((value) =>
    //            {
    //                //���и�ֵ
    //                //EffectBean effectBean = new EffectBean();
    //                //effectBean.EffectType = value[0];
    //                //effectBean.EffectValue = float.Parse(value[1]);
    //                //return effectBean;
    //                return new List<string>();//��ʱ����Ҫ
    //            });
    //                    //---------------------------------------------------
    //    }
    //            //�����ַ�������
    //            if (cellValue.Contains("|"))
    //        {
    //            return cellValue
    //           .Split('|')
    //           .Select(item => item.Trim())
    //           .Where(item => !string.IsNullOrEmpty(item))
    //           .ToArray();
    //        }
    //        //�����ַ���
    //        return cellValue;

    //        object CostomExcelArray<T>(Func<string[], T> setValue)
    //        {
    //            //����;��ֵ����Ϊ�ַ�������
    //            if (cellValue.Contains("|"))
    //            {
    //                string[] customString = cellValue
    //         .Split('|')
    //         .Select(item => item.Trim())
    //         .Where(item => !string.IsNullOrEmpty(item))
    //         .ToArray();
    //                //��ǰֵΪ����
    //                List<T> TArray = new List<T>();
    //                foreach (var oneString in customString)
    //                {
    //                    var Elements = oneString.Split(',');
    //                    //��Ӹ�ֵ�߼�
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