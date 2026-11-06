using Fantasy;
using System.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;

namespace Hotfix
{
    public class MapConfigConter
    {
        private static MapConfigConter? _instance;
        public static MapConfigConter? Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new MapConfigConter();
                    _instance.LoadConfig();
                }
                return _instance;
            }
        }

        private readonly Dictionary<MapType,MapConfig> mMapCfgDic= new Dictionary<MapType, MapConfig>();

        private string mapCfgPath = "D:\\GitHub Project\\DNF2Fantasy-Server\\examples\\Server\\Hotfix\\Helper\\MapConfig\\MapConfig.json";
        public void LoadConfig()
        {
            mMapCfgDic.Clear();
            string json = File.ReadAllText(mapCfgPath);
            List<MapConfig> mapConfigList= Newtonsoft.Json.JsonConvert.DeserializeObject<List<MapConfig>>(json);

            foreach (var item in mapConfigList)
            {
                mMapCfgDic.Add(item.mapType,item);
            }
        }
        /// <summary>
        /// 获取地图配置
        /// </summary>
        /// <param name="mapType">地图类型</param>
        /// <returns></returns>
        public MapConfig GetMapConfig(MapType mapType)
        {
            mMapCfgDic.TryGetValue(mapType, out var mapConfig);
            return mapConfig;
        }
    }
}
