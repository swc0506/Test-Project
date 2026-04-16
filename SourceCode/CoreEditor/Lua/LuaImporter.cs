#if LUA
using System;
using System.IO;
using Core;
using UnityEditor.AssetImporters;
using UnityEditor.Experimental.AssetImporters;

namespace CoreEditor.Lua
{
    [ScriptedImporter(1, ".lua")]
    public class LuaImporter : ScriptedImporter
    {
        public static Action<string> importAction;

        public override void OnImportAsset(AssetImportContext ctx)
        {
            byte[] bytes = File.ReadAllBytes(ctx.assetPath);
            LuaAsset asset = LuaAsset.Create(bytes);
            ctx.AddObjectToAsset("main obj", asset);
            ctx.SetMainObject(asset);

            importAction?.Invoke(ctx.assetPath);
        }
    }
}
#endif