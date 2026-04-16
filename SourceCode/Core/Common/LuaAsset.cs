using System.Text;
using UnityEngine;

namespace Core
{
    public class LuaAsset : ScriptableObject
    {
        [SerializeField] private byte[] mBytes;

        public byte[] bytes
        {
            get { return mBytes; }
        }

        public string text
        {
            get { return Encoding.UTF8.GetString(mBytes); }
        }

        public static LuaAsset Create(string text)
        {
            var asset = ScriptableObject.CreateInstance<LuaAsset>();
            asset.mBytes = Encoding.UTF8.GetBytes(text);
            return asset;
        }

        public static LuaAsset Create(byte[] bytes)
        {
            var asset = ScriptableObject.CreateInstance<LuaAsset>();
            asset.mBytes = (byte[]) bytes.Clone();
            return asset;
        }
    }
}