namespace Core.UI
{
    public interface IUIController
    {
        string UIName { get; }
        object GUI { get; }
        IUIController Parent { get; }
        bool IsFullScreen { get; }
        bool IsFocus { get; }

        void Initial(UIProxy uiProxy);

        void Start();

        void Open(params object[] args);

        void Focus(params object[] args);

        void Update(float deltaTime);

        void Blur();

        void Close();

        void Destroy();

        UIChildPanel OpenChild(string uiName, params object[] args);
        void CloseChild(string uiName);
        UIChildPanel GetChild(string uiName);
        bool IsOpenChild(string uiName);
    }
}