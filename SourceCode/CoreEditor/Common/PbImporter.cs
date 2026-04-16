using System.IO;
using Core;
using UnityEditor.AssetImporters;
using UnityEditor.Experimental.AssetImporters;

namespace CoreEditor
{
    [ScriptedImporter(1, ".pb")]
    public class PbImporter : ScriptedImporter
    {
        public override void OnImportAsset(AssetImportContext ctx)
        {
            byte[] bytes = File.ReadAllBytes(ctx.assetPath);
            BinaryAsset asset = BinaryAsset.Create(bytes);
            ctx.AddObjectToAsset("main obj", asset);
            ctx.SetMainObject(asset);
        }
    }
}
