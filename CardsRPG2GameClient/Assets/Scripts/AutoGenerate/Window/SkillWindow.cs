/*---------------------------------
 *Title:UI表现层脚本自动化生成工具
 *Author:ZM 铸梦
 *Date:2026/8/7 17:55:13
 *Description:UI 表现层，该层只负责界面的交互、表现相关的更新，不允许编写任何业务逻辑代码
 *注意:以下文件是自动生成的，再次生成不会覆盖原有的代码，会在原有的代码上进行新增，可放心使用
---------------------------------*/

using DG.Tweening;
using UnityEngine.UI;
using UnityEngine;

namespace ZM.UI
{
    public class SkillWindow : WindowBase
    {
        public SkillWindowDataComponent dataCompt;

        #region 生命周期函数

        //调用机制与Mono Awake一致
        public override void OnAwake()
        {
            dataCompt = gameObject.GetComponent<SkillWindowDataComponent>();
            dataCompt.InitComponent(this);
            base.OnAwake();
        }

        //物体显示时执行
        public override void OnShow()
        {
            base.OnShow();
        }

        //物体隐藏时执行
        public override void OnHide()
        {
            base.OnHide();
        }

        //物体销毁时执行
        public override void OnDestroy()
        {
            base.OnDestroy();
        }

        #endregion

        #region API Function

        public void PlayAnim(SkillConfig skill, int heroId)
        {
            dataCompt.SkillNameText.text = skill.skillName;
            dataCompt.iconImage.sprite = skill.skillIcon;
            //dataCompt.iconImage.sprite = ResourcesManager.Instance.LoadAsset<Sprite>("Texture/" + heroId);
            dataCompt.SkillTipsTransform.localScale = Vector3.one;
            dataCompt.SkillTipsTransform.transform.localPosition = new Vector3(500, 0, 0);

            dataCompt.SkillTipsTransform.DOLocalMoveX(0, 0.1f).OnComplete(() =>
            {
                dataCompt.SkillTipsTransform.DOLocalMoveY(10, 0.5f).SetLoops(-1, LoopType.Yoyo);
            });
        
            dataCompt.SkillTipsTransform.DOLocalMoveX(500, 0.1f).SetDelay(1.5f);
        }
        
        #endregion

        #region UI组件事件

        public void OnCloseButtonClick()
        {
            HideWindow();
        }

        #endregion
    }
}