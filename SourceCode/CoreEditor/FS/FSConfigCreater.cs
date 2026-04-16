using Core.FS;
using UnityEditor;
using UnityEngine;

namespace CoreEditor.FS
{
    public class FSConfigCreater
    {
        [MenuItem("Assets/Create/Core/FSConfig")]
        private static void CreateFSConfig()
        {
            FSConfig obj = ScriptableObject.CreateInstance<FSConfig>();
            AssetUtils.CreateAsset(obj, "Assets/Core/Resources/FS/Key.asset");
        }
    }
}