#if UNITY_EDITOR
using DG.Tweening;
using Fantasy;
using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class ExportMapConfigToJson
{

    [MenuItem("Tools/µº≥ˆµÿÕº≈‰÷√")]
    public static void ExportMapConfig()
    {
        string mapCfgPath = Path.Combine(Application.dataPath, "GameData/Hall/Prefabs/Map");
        string[] filePathArr = Directory.GetFiles(mapCfgPath);
        List<MapConfig> mapConfigs = new List<MapConfig>();
        foreach (var path in filePathArr)
        {
            if (!path.EndsWith(".meta"))
            {
                Map map = AssetDatabase.LoadAssetAtPath<Map>(path.Replace(Application.dataPath, "Assets"));
                map.transform.position=map.mapInitPos;
                MapConfig mapConfig = new MapConfig();
                mapConfig.mapType = (MapType)map.mapType;
                mapConfig.mapName=map.mapType.ToString();
                mapConfig.maxPosition= ToCSVector3(map.maxPosition);
                mapConfig.minPosition= ToCSVector3(map.minPosition);
                mapConfig.mapDoorDataList = new List<MapDoorDataCfg>();
                foreach (MapEntry mapEntry in map.mapEntry)
                {
                    MapDoorDataCfg doorCfg = new MapDoorDataCfg ();
                    doorCfg.doorType = (DoorType)mapEntry.doorType;
                    doorCfg.roleInitPos = ToCSVector3(mapEntry.roleInitPos.position);
                    mapConfig.mapDoorDataList.Add(doorCfg);
                }
                mapConfigs.Add(mapConfig);
            }
        }
        string json= JsonConvert.SerializeObject(mapConfigs, Formatting.Indented);
        Debug.Log(json);
        string savePath= Path.Combine(Application.dataPath, "Editor/MapConfig/MapConfig.json");
        File.WriteAllText(savePath, json);
        Debug.Log("MapConfig Generate Success Path:" + savePath);
        AssetDatabase.Refresh();
    }

    public static CSVector3 ToCSVector3(Vector3 ve3)
    {
        CSVector3 csVector3 = new CSVector3();
        csVector3.x = ve3.x;
        csVector3.y = ve3.y;
        csVector3.z = ve3.z;
        return csVector3;
    }

}
#endif