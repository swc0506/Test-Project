using System.Collections.Generic;
using System.Linq;
using UnityEngine.Events;
using UnityEngine.UI;

namespace ZMUIFrameWork.Scripts.Runtime.Base
{
    public class WindowBase : WindowBehaviour
    {
        private List<Button> mAllButtonList = new List<Button>();
        private List<Toggle> mToggleList = new List<Toggle>();
        private List<InputField> mInputList = new List<InputField>();

        #region 生命周期

        public override void OnAwake()
        {
            base.OnAwake();
        }

        public override void OnShow()
        {
            base.OnShow();
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
            GameObject.SetActive(isVisible);//临时代码
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
    }
}