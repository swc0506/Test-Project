/*---------------------------------
 *Title:UI自动化组件生成代码生成工具
 *Author:铸梦
 *Date:2026/7/30 15:23:43
 *Description:变量需要以[Text]括号加组件类型的格式进行声明，然后右键窗口物体—— 一键生成UI数据组件脚本即可
 *注意:以下文件是自动生成的，再次生成后会以代码追加的形式新增,若手动修改后,尽量避免自动生成
---------------------------------*/

using UnityEngine;
using UnityEngine.UI;
using SuperScrollView;

namespace ZM.UI
{
    public class HeroListItem : MonoBehaviour, IZMUIViewListItem
    {
        #region 自定义字段

        public Button CardButton;

        public Image bgImage;

        public Image iconImage;

        public Image frameImage;

        public Image attributeImage;

        public Text levelText;

        public Image coreImage;

        public Image startImage;

        #endregion


        #region 生命周期

        public void InitListItem()
        {
            //按钮事件自动注册绑定
            CardButton.onClick.AddListener(OnCardButtonClick);
        }

        public void SetListItemShowData(int index, params object[] data)
        {
            HeroData heroData = (HeroData)data[0];
            
            coreImage.gameObject.SetVisible(heroData.quality == QualityEnum.Red);
            iconImage.sprite =
                ZMAsset.ZMAsset.LoadSprite($"{AssetsPathConfig.HALL_TEXTURE_PATH}HeroIcon/X1_card_{heroData.name}");
            startImage.sprite =
                ZMAsset.ZMAsset.LoadAtlasSprite($"{AssetsPathConfig.HALL_TEXTURE_PATH}Card/Card",
                    "start" + (int)heroData.quality);
            frameImage.sprite = ZMAsset.ZMAsset.LoadAtlasSprite($"{AssetsPathConfig.HALL_TEXTURE_PATH}Card/Card",
                "CardFrame" + (int)heroData.quality);
            bgImage.sprite = ZMAsset.ZMAsset.LoadAtlasSprite($"{AssetsPathConfig.HALL_TEXTURE_PATH}Card/Card",
                "Cardbg" + (int)heroData.quality);
            attributeImage.sprite = ZMAsset.ZMAsset.LoadAtlasSprite($"{AssetsPathConfig.HALL_TEXTURE_PATH}Card/Card",
                "Attribute" + (int)heroData.type);
        }

        public void OnRelease()
        {
            //按钮事件自动注册绑定
            CardButton.onClick.RemoveListener(OnCardButtonClick);
        }

        #endregion


        #region UI组件事件

        private void OnCardButtonClick()
        {
            
        }

        #endregion
    }
}