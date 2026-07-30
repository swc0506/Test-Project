/*---------------------------------
 *Title:UI自动化组件生成代码生成工具
 *Author:铸梦
 *Date:2026/7/29 20:33:44
 *Description:变量需要以[Text]括号加组件类型的格式进行声明，然后右键窗口物体—— 一键生成UI数据组件脚本即可
 *注意:以下文件是自动生成的，再次生成后会以代码追加的形式新增,若手动修改后,尽量避免自动生成
---------------------------------*/

using UnityEngine;
using UnityEngine.UI;
using SuperScrollView;

namespace ZM.UI
{
    public class GetHeroCardItem : MonoBehaviour
    {
        #region 自定义字段

        private Text nameText;
        private Transform cardParent;
        private GameObject effectObj;
        private CardItem cardItem;

        #endregion


        #region 生命周期

        //脚本初始化接口 (为保证生命周期的执行顺序，请在View层调用该接口确保需要初始化的数据正常执行)
        public void OnInitialize()
        {
            //按钮事件自动注册绑定
            nameText = transform.Find("Name").GetComponent<Text>();
            cardParent = transform.Find("CardParent");
        }

        //物体设置数据接口 (请自定以你的参数，方便外部调用传参)
        public void SetItemData(int id)
        {
            var heroData = ConfigCenter.GetHeroData(id);
            GameObject itemObj =
                ZMAsset.ZMAsset.InstantiateObject(AssetPathConfig.HALL_PREFABS_PATH + "Card/CardItem", cardParent);
            cardItem = itemObj.GetComponent<CardItem>();
            cardItem.SetItemData(heroData);
            effectObj = ZMAsset.ZMAsset.InstantiateObject(
                AssetPathConfig.HALL_PREFABS_PATH + "Card/" + heroData.quality.ToString(),
                transform);
            nameText.text = heroData.nameChinese;
        }

        //物体销毁时执行 (为保证生命周期的执行顺序，请在View层调用该接口确保需要释放时的接口正常调用)
        public void OnDispose()
        {
            if (effectObj != null)
            {
                ZMAsset.ZMAsset.Release(effectObj);
                effectObj = null;
            }

            if (cardItem != null)
            {
                cardItem.OnDispose();
                cardItem = null;
            }
        }

        #endregion


        #region UI组件事件

        #endregion
    }
}