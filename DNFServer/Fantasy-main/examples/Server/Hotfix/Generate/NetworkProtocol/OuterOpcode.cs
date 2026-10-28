namespace Fantasy
{
	public static partial class OuterOpcode
	{
		 public const int RegisterAccountReqest = 110000001;
		 public const int RegisterAccountResponse = 160000001;
		 public const int GetTokenReqest = 110000002;
		 public const int GetTokenResponse = 160000002;
		 public const int LoginGateReqest = 110000003;
		 public const int LoginGateResponse = 160000003;
		 public const int CreateRoleReqest = 110000004;
		 public const int CreateRoleResponse = 160000004;
		 public const int SelectRoleReqest = 110000005;
		 public const int SelectRoleResponse = 160000005;
		 public const int EnterMapReqest = 110000006;
		 public const int EnterMapResponse = 160000006;
		 public const int StateSyncReqest = 110000007;
		 public const int StateSyncResponse = 160000007;
		 public const int OtherPlayerStateSyncMessage = 100000001;
		 public const int CreateTeamReqest = 110000008;
		 public const int CreateTeamResponse = 160000008;
		 public const int JoinTeamReqest = 110000009;
		 public const int JoinTeamResponse = 160000009;
		 public const int TeamStateChangeMessage = 100000002;
		 public const int EnterDungeonMessage = 100000003;
		 public const int LoadDungeonProgresssMessage = 100000004;
		 public const int StartDungeonBattleMessage = 100000005;
		 public const int FrameOperateEventMessage_C2G = 100000006;
		 public const int FrameOperateEventMessage_G2C = 100000007;
		 public const int BattleEndRewardMessage_Share = 100000008;
		 public const int BattleEndMessage_Share = 100000009;
		 public const int ExitDungeonReqest = 110000010;
		 public const int ExitDungeonResponse = 160000010;
		 public const int C2A_TestRequest = 110000011;
		 public const int A2C_TestResponse = 160000011;
		 public const int TestLoginRequest = 110000012;
		 public const int TestLoginResponse = 160000012;
		 public const int C2G_TestMessageEvent = 100000010;
		 public const int G2C_TestSeverToClientMessage = 100000011;
	}
}
