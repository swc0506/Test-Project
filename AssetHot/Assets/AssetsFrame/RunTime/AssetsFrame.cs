using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace ZM.AssetFrameWork
{
    public partial class AssetsFrame : FrameBase
    {
        public static Transform RecycleObjRoot{ get; private set; }
        private IHotAssets mHotAssets = null;
        private IDecompressAssets mDecompressAssets = null;
        private IResourceInterface mResource = null;

        /// <summary>
        /// 初始化框架
        /// </summary>
        public void InitFrameWork()
        {
            var root = new GameObject("RecycleObjRoot");
            RecycleObjRoot = root.transform;
            DontDestroyOnLoad(root);
            mHotAssets = new HotAssetsManager();
            mDecompressAssets = new AssetsDecompressManager();
            mResource = new ResourceManager();
            mResource.Initialize();
        }
        
        protected void Update()
        {
            mHotAssets?.OnMainThreadUpdate();
        }
    }
}