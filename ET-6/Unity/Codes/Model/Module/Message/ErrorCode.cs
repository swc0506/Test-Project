namespace ET
{
    public static partial class ErrorCode
    {
        public const int ERR_Success = 0;

        // 1-11004 是SocketError请看SocketError定义
        //-----------------------------------
        // 100000-109999是Core层的错误

        // 110000以下的错误请看ErrorCore.cs

        // 这里配置逻辑层的错误码
        // 110000 - 200000是抛异常的错误
        // 200001以上不抛异常

        public const int ERR_NetworkError = 200002;
        public const int ERR_LoginInfoError = 200003;
        public const int ERR_RequestRepeatedly = 200004;
        public const int ERR_AccountEmpty = 200005;
        public const int ERR_NameIsNotMatch = 200006;
        public const int ERR_PasswordIsNotMatch = 200007;
        public const int ERR_NameIsError = 200008;
        public const int ERR_PasswordError = 200009;
        public const int ERR_TokenError = 200010;
        public const int ERR_RequestSceneTypeError = 200014;

        public const int ERR_RoleNameError = 200011;
        public const int ERR_RoleNameSame = 200012;
        public const int ERR_RoleNotExist = 200013;

        public const int ERR_ConnectGateKeyError = 200015;
        public const int ERR_OtherAccountLogin = 200016;
        public const int ERR_SessionPlayerError = 200017;
        public const int ERR_NonePlayerError = 200018;
        public const int ERR_SessionStateError = 200019;
        public const int ERR_EnterGameError = 200020;
        public const int ERR_ReEnterGameError = 200021;
        public const int ERR_ReEnterGameError2 = 200022;

        public const int ERR_NumericTypeNotExist = 200023;
        public const int ERR_NumericTypeNotAddPoint = 200024;
        public const int ERR_AddPointNotEnough = 200025;
    }
}