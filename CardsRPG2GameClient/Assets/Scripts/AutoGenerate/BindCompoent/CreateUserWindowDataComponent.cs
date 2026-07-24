/*---------------------------------
 *Title:UI自动化组件生成代码生成工具
 *Author:铸梦
 *Date:2026/7/24 16:09:25
 *Description:变量需要以[Text]括号加组件类型的格式进行声明，然后右键窗口物体—— 一键生成UI数据组件脚本即可
 *注意:以下文件是自动生成的，任何手动修改都会被下次生成覆盖,若手动修改后,尽量避免自动生成
---------------------------------*/

using UnityEngine;
using UnityEngine.UI;
using SuperScrollView;

namespace ZM.UI
{
    public class CreateUserWindowDataComponent : MonoBehaviour
    {
        public GameObject maleUncheckedGameObject;

        public GameObject femaleSelectIconGameObject;

        public GameObject femaleUncheckedGameObject;

        public GameObject maleSelectIconGameObject;

        public Button maleButton;

        public Button femaleButton;

        public InputField nicknameInputField;

        public Button CreateButton;

        public void InitComponent(WindowBase target)
        {
            //组件事件绑定
            CreateUserWindow mWindow = (CreateUserWindow)target;
            target.AddButtonClickListener(maleButton, mWindow.OnmaleButtonClick);
            target.AddButtonClickListener(femaleButton, mWindow.OnfemaleButtonClick);
            target.AddInputFieldListener(nicknameInputField, mWindow.OnnicknameInputChange, mWindow.OnnicknameInputEnd);
            target.AddButtonClickListener(CreateButton, mWindow.OnCreateButtonClick);
        }
    }
}