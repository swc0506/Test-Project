using UnityEngine;

namespace Core
{
    public class BinaryAsset :  ScriptableObject
    {
        [SerializeField] private byte[] mBytes;

        public byte[] bytes
        {
            get { return mBytes; }
        }

        public static BinaryAsset Create(byte[] bytes)
        {
            var asset = ScriptableObject.CreateInstance<BinaryAsset>();
            asset.mBytes = (byte[]) bytes.Clone();
            return asset;
        }
    }
}

