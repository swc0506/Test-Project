using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System.IO;
using Excel;
using System.Data;
using Newtonsoft.Json;

public class ExcelToJson
{
    private static string excelPath;
    [MenuItem("Tools/配置表/英雄配置生成")]
    public static void LoadHeroExcel()
    {
        Object selection = Selection.activeObject;
        if (selection == null)
        {
            return;
        }
        excelPath = AssetDatabase.GetAssetPath(selection);
        ConvertJson(Application.dataPath + "/GameData/HallWorld/CfgData/Hero.json");
    }
    [MenuItem("Tools/配置表/关卡配置生成")]
    public static void LoadLevelExcel()
    {
        Object selection = Selection.activeObject;
        if (selection == null)
        {
            return;
        }
        excelPath = AssetDatabase.GetAssetPath(selection);
        ConvertJson(Application.dataPath + "/GameData/HallWorld/CfgData/levelConfig.json");
    }
    public static void ConvertJson(string savePath = "")
    { 
        // excelPath = Application.dataPath.Replace("Assets", "") + excelPath;
        Debug.Log(Application.dataPath.Replace("Assets", "") +"/ConfigExcels/LevelConfig.xlsx");
        //读取XML文件 转未数据表
        FileStream stream = File.Open(excelPath, FileMode.Open, FileAccess.Read,FileShare.ReadWrite);
        IExcelDataReader excelDataReader = ExcelReaderFactory.CreateOpenXmlReader(stream);
        
        DataSet dataSet = excelDataReader.AsDataSet();
        //开始转换数据
        if (dataSet.Tables.Count == 0) return;
        //读取Excel文件中的第一个sheet表
        DataTable sheet = dataSet.Tables[0];
        //如果表中没内容，不进行处理
        if (sheet.Rows.Count == 0) return;

        //读取表中的行数和列数
        int rowCount = sheet.Rows.Count;
        int colCount = sheet.Columns.Count;
        //准备一个表储存全部数据
        List<Dictionary<string, object>> dataTable = new List<Dictionary<string, object>>();
        //遍历列表读取数据
        for (int i = 2; i < rowCount; i++)
        {
            Dictionary<string, object> row = new Dictionary<string, object>();
            for (int j = 0; j < colCount; j++)
            {
                //读取第一行数据作为表头字段
                string field = sheet.Rows[1][j].ToString();
                // key value 格式的数据
                if (string.Equals(sheet.Rows[0][j].ToString(), "Array")) //单独处理数组数据
                {
                    row[field] = "[" + sheet.Rows[i][j] + "]"; ;
                }
                else
                {
                    row[field] = sheet.Rows[i][j];
                }

            }
            dataTable.Add(row);
        }
        //生成Json字符串
        string json = JsonConvert.SerializeObject(dataTable, Formatting.Indented);
        //处理数组
        json = json.Replace("\"[","[").Replace("]\"","]");
        string filePath = savePath;
        //写入文件
        using (FileStream fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write))
        {
            using (TextWriter textWriter = new StreamWriter(fileStream, System.Text.Encoding.UTF8))
            {
                textWriter.Write(json);
                textWriter.Close();
                textWriter.Dispose();
            }
            fileStream.Close();
            fileStream.Dispose();
        }
        AssetDatabase.Refresh();
    }

}
