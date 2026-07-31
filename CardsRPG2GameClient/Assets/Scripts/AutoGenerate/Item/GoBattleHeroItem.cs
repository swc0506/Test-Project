/*---------------------------------
 *Title:UI自动化组件生成代码生成工具
 *Author:铸梦
 *Date:2026/7/31 11:49:35
 *Description:变量需要以[Text]括号加组件类型的格式进行声明，然后右键窗口物体—— 一键生成UI数据组件脚本即可
 *注意:以下文件是自动生成的，再次生成后会以代码追加的形式新增,若手动修改后,尽量避免自动生成
---------------------------------*/

using UnityEngine;
using UnityEngine.UI;
using SuperScrollView;

namespace ZM.UI
{
    public class GoBattleHeroItem : MonoBehaviour, IZMUIViewListItem
    {
        #region 自定义字段

        public GameObject choosedBgGameObject;

        public Button iconButton;

        public Image attributeImage;

        public Text lvText;

        public Slider HPSlider;

        public Image starImage;

        public GameObject choosedGameObject;

        #endregion


        #region 生命周期

        public void InitListItem()
        {
            //按钮事件自动注册绑定
            iconButton.onClick.AddListener(OniconButtonClick);
        }

        public void SetListItemShowData(int index, params object[] data)
        {
            HeroData heroData = (HeroData)data[0];
            iconButton.image.sprite =
                ZMAsset.ZMAsset.LoadSprite($"{AssetsPathConfig.HALL_TEXTURE_PATH}HeroHead/X1_icon_{heroData.name}");
            starImage.sprite =
                ZMAsset.ZMAsset.LoadAtlasSprite($"{AssetsPathConfig.HALL_TEXTURE_PATH}Card/Card",
                    "start" + (int)heroData.quality);
            attributeImage.sprite = ZMAsset.ZMAsset.LoadAtlasSprite($"{AssetsPathConfig.HALL_TEXTURE_PATH}Card/Card",
                "Attribute" + (int)heroData.type);
        }

        public void OnRelease()
        {
            //按钮事件自动注册绑定
            iconButton.onClick.RemoveListener(OniconButtonClick);
        }

        #endregion


        #region UI组件事件

        private void OniconButtonClick()
        {
        }

        #endregion
    }
}