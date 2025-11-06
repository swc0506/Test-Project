using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using ZMUIFrameWork.Scripts.Runtime.Core;

namespace ZMUIFrameWork.Scripts.Runtime.Base
{
    public class WindowBase : WindowBehaviour
    {
        private List<Button> mAllButtonList = new List<Button>();
        private List<Toggle> mToggleList = new List<Toggle>();
        private List<InputField> mInputList = new List<InputField>();

        private CanvasGroup mUIMask;
        private CanvasGroup mCanvasGroup;
        protected Transform mUIContent;
        protected bool mDisableAnim = false;

        /// <summary>
        /// 初始化基类组件
        /// </summary>
        private void InitializeBaseComponent()
        {
            mUIMask = Transform.Find("UIMask").GetComponent<CanvasGroup>();
            mCanvasGroup = Transform.Find("CanvasGroup").GetComponent<CanvasGroup>();
            mUIContent = Transform.Find("UIContent").transform;
        }

        #region 生命周期

        public override void OnAwake()
        {
            base.OnAwake();
            InitializeBaseComponent();
        }

        public override void OnShow()
        {
            base.OnShow();
            ShowAnimation();
        }

        public override void OnUpdate()
        {
            base.OnUpdate();
        }

        public override void OnHide()
        {
            base.OnHide();
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
            RemoveAllButtonListener();
            RemoveAllToggleListener();
            RemoveAllInputListener();
            mAllButtonList.Clear();
            mToggleList.Clear();
            mInputList.Clear();
        }

        #endregion

        public override void SetVisible(bool isVisible)
        {
            //GameObject.SetActive(isVisible);//临时代码
            mCanvasGroup.alpha = isVisible ? 1 : 0;
            mCanvasGroup.blocksRaycasts = isVisible;
            visible = isVisible;
        }

        public void SetMaskVisible(bool isVisible)
        {
            if (!UISetting.Instance.SINGMASK_SYSTRM)
            {
                return;
            }

            mUIMask.alpha = isVisible ? 1 : 0;
        }

        public void HideWindow()
        {
            //UIModule.Instance.HideWindow(Name);
            HideAnimation();
        }

        #region 事件管理

        public void AddButtonClickListener(Button btn, UnityAction action)
        {
            if (btn != null)
            {
                if (!mAllButtonList.Contains(btn))
                {
                    mAllButtonList.Add(btn);
                }

                btn.onClick.RemoveAllListeners();
                btn.onClick.AddListener(action);
            }
        }

        public void AddToggleClickListener(Toggle toggle, UnityAction<bool, Toggle> action)
        {
            if (toggle != null)
            {
                if (!mToggleList.Contains(toggle))
                {
                    mToggleList.Add(toggle);
                }

                toggle.onValueChanged.RemoveAllListeners();
                toggle.onValueChanged.AddListener((isOn) => { action?.Invoke(isOn, toggle); });
            }
        }

        public void AddInputFieldListener(InputField input, UnityAction<string> onChangeAction,
            UnityAction<string> endAction)
        {
            if (input != null)
            {
                if (!mInputList.Contains(input))
                {
                    mInputList.Add(input);
                }

                input.onValueChanged.RemoveAllListeners();
                input.onEndEdit.RemoveAllListeners();
                input.onValueChanged.AddListener(onChangeAction);
                input.onEndEdit.AddListener(endAction);
            }
        }

        public void RemoveAllButtonListener()
        {
            foreach (var item in mAllButtonList)
            {
                item.onClick.RemoveAllListeners();
            }
        }

        public void RemoveAllToggleListener()
        {
            foreach (var item in mToggleList)
            {
                item.onValueChanged.RemoveAllListeners();
            }
        }

        public void RemoveAllInputListener()
        {
            foreach (var item in mInputList)
            {
                item.onValueChanged.RemoveAllListeners();
                item.onEndEdit.RemoveAllListeners();
            }
        }

        #endregion
        
        #region 动画管理

        private void ShowAnimation()
        {
            //基础弹窗不需要动画
            if (Canvas.sortingOrder > 90 && mDisableAnim == false)
            {
                //Mask动画
                //mUIMask.alpha = 0;
                //mUIMask.DOFade(1, 0.2f);
                //缩放动画
                mUIContent.localScale = Vector3.one * 0.8f;
                mUIContent.DOScale(Vector3.one, 0.3f).SetEase(Ease.OutBack);
            }
        }

        private void HideAnimation()
        {
            if (Canvas.sortingOrder > 90 && mDisableAnim == false)
            {
                mUIContent.DOScale(Vector3.one * 1.1f, 0.2f).SetEase(Ease.OutBack).OnComplete(() =>
                {
                    UIModule.Instance.HideWindow(Name);
                });
            }
            else
            {
                UIModule.Instance.HideWindow(Name);
            }
        }

        #endregion
    }
}