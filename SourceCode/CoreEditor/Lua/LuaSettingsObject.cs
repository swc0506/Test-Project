#if LUA
using System.Collections.Generic;

namespace CoreEditor.Lua
{
    public class LuaSettingsObject : ConfigObject
    {
        public string ideOpenFileArgs;
        public string luaExtensionName;
        public List<string> luaSearchDir;

        public string fairyGUIPanelDir;
        public string fairyGUIExtensionName;
        public string generateLuaDir;
        public string luaRequirePrefix;
        public List<string> commonPkgs;

        protected override void OnInitial()
        {
            ideOpenFileArgs = string.Empty;
            luaExtensionName = ".lua";
            luaSearchDir = new List<string>();

            fairyGUIPanelDir = string.Empty;
            fairyGUIExtensionName = ".bytes";
            generateLuaDir = string.Empty;
            luaRequirePrefix = string.Empty;
            commonPkgs = new List<string>();
        }

        public static LuaSettingsObject Get()
        {
            return Load<LuaSettingsObject>("Lua");
        }
    }
}
#endif