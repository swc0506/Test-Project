using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace ZM.AssetFrameWork
{
    public partial class AssetsFrame : FrameBase
    {
        private IHotAssets mHotAssets = null;
        private IDecompressAssets mDecompressAssets = null;

        /// <summary>
        /// 初始化框架
        /// </summary>
        public void InitFrameWork()
        {
            mHotAssets = new HotAssetsManager();
            mDecompressAssets = new AssetsDecompressManager();
        }
        
        protected void Update()
        {
            mHotAssets?.OnMainThreadUpdate();
        }
    }
}