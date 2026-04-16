#if MAP
using System;
using System.Collections.Generic;
using Core.Map;

namespace CoreEditor.Map
{
    internal class MapConfigObject : ConfigObject
    {
        public List<ThemeInfo> themes;

        protected override void OnInitial()
        {
            themes = new List<ThemeInfo>();
        }

        public static MapConfigObject Get()
        {
            return Load<MapConfigObject>("Map");
        }
    }

    [Serializable]
    public class ThemeInfo
    {
        public string name;
        public List<GroupInfo> groups;
    }

    [Serializable]
    public class GroupInfo
    {
        public string name;
        public bool foldout;
        public int partType;
        public List<string> partPaths;
    }
}
#endif