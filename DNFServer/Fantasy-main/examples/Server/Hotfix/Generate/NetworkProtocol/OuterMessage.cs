using ProtoBuf;
using System.Collections.Generic;
using Fantasy;
#pragma warning disable CS8618

namespace Fantasy
{	
	[ProtoContract]
	public partial class RegisterAccountReqest : AProto, IRequest
	{
		[ProtoIgnore]
		public RegisterAccountResponse ResponseType { get; set; }
		public uint OpCode() { return OuterOpcode.RegisterAccountReqest; }
		[ProtoMember(1)]
		public string account { get; set; }
		[ProtoMember(2)]
		public string passWorld { get; set; }
	}
	[ProtoContract]
	public partial class RegisterAccountResponse : AProto, IResponse
	{
		public uint OpCode() { return OuterOpcode.RegisterAccountResponse; }
		[ProtoMember(91, IsRequired = true)]
		public uint ErrorCode { get; set; }
		[ProtoMember(1)]
		public string account { get; set; }
		[ProtoMember(2)]
		public string passWorld { get; set; }
	}
	[ProtoContract]
	public partial class GetTokenReqest : AProto, IRequest
	{
		[ProtoIgnore]
		public GetTokenResponse ResponseType { get; set; }
		public uint OpCode() { return OuterOpcode.GetTokenReqest; }
		[ProtoMember(1)]
		public string account { get; set; }
		[ProtoMember(2)]
		public string passWorld { get; set; }
	}
	[ProtoContract]
	public partial class GetTokenResponse : AProto, IResponse
	{
		public uint OpCode() { return OuterOpcode.GetTokenResponse; }
		[ProtoMember(91, IsRequired = true)]
		public uint ErrorCode { get; set; }
		[ProtoMember(1)]
		public string token { get; set; }
		[ProtoMember(2)]
		public string address { get; set; }
		[ProtoMember(3)]
		public long accountid { get; set; }
	}
	[ProtoContract]
	public partial class LoginGateReqest : AProto, IRequest
	{
		[ProtoIgnore]
		public LoginGateResponse ResponseType { get; set; }
		public uint OpCode() { return OuterOpcode.LoginGateReqest; }
		[ProtoMember(1)]
		public long accountid { get; set; }
		[ProtoMember(2)]
		public string token { get; set; }
	}
	[ProtoContract]
	public partial class LoginGateResponse : AProto, IResponse
	{
		public uint OpCode() { return OuterOpcode.LoginGateResponse; }
		[ProtoMember(91, IsRequired = true)]
		public uint ErrorCode { get; set; }
		[ProtoMember(1)]
		public long accountid { get; set; }
		[ProtoMember(2)]
		public long level { get; set; }
		[ProtoMember(3)]
		public long gold { get; set; }
		[ProtoMember(4)]
		public long diamond { get; set; }
		[ProtoMember(5)]
		public List<RoleData> roleList = new List<RoleData>();
	}
	[ProtoContract]
	public partial class CreateRoleReqest : AProto, IRequest
	{
		[ProtoIgnore]
		public CreateRoleResponse ResponseType { get; set; }
		public uint OpCode() { return OuterOpcode.CreateRoleReqest; }
		[ProtoMember(1)]
		public int roleid { get; set; }
		[ProtoMember(2)]
		public long accountid { get; set; }
		[ProtoMember(3)]
		public string roleName { get; set; }
	}
	[ProtoContract]
	public partial class CreateRoleResponse : AProto, IResponse
	{
		public uint OpCode() { return OuterOpcode.CreateRoleResponse; }
		[ProtoMember(91, IsRequired = true)]
		public uint ErrorCode { get; set; }
		[ProtoMember(1)]
		public RoleData roleData { get; set; }
	}
	[ProtoContract]
	public partial class RoleData : AProto
	{
		[ProtoMember(1)]
		public long uid { get; set; }
		[ProtoMember(2)]
		public string roleName { get; set; }
		[ProtoMember(3)]
		public int level { get; set; }
		[ProtoMember(4)]
		public int roleid { get; set; }
		[ProtoMember(5)]
		public long accountid { get; set; }
	}
	[ProtoContract]
	public partial class SelectRoleReqest : AProto, IRequest
	{
		[ProtoIgnore]
		public SelectRoleResponse ResponseType { get; set; }
		public uint OpCode() { return OuterOpcode.SelectRoleReqest; }
		[ProtoMember(1)]
		public long roleUid { get; set; }
		[ProtoMember(2)]
		public long accountid { get; set; }
	}
	[ProtoContract]
	public partial class SelectRoleResponse : AProto, IResponse
	{
		public uint OpCode() { return OuterOpcode.SelectRoleResponse; }
		[ProtoMember(91, IsRequired = true)]
		public uint ErrorCode { get; set; }
		[ProtoMember(1)]
		public RoleData roleData { get; set; }
	}
	[ProtoContract]
	public partial class EnterMapReqest : AProto, IRequest
	{
		[ProtoIgnore]
		public EnterMapResponse ResponseType { get; set; }
		public uint OpCode() { return OuterOpcode.EnterMapReqest; }
		[ProtoMember(1)]
		public long playerid { get; set; }
		[ProtoMember(2)]
		public int curMap { get; set; }
		[ProtoMember(3)]
		public int mapType { get; set; }
		[ProtoMember(4)]
		public int doorType { get; set; }
	}
	[ProtoContract]
	public partial class EnterMapResponse : AProto, IResponse
	{
		public uint OpCode() { return OuterOpcode.EnterMapResponse; }
		[ProtoMember(91, IsRequired = true)]
		public uint ErrorCode { get; set; }
		[ProtoMember(1)]
		public long playerid { get; set; }
		[ProtoMember(2)]
		public int mapType { get; set; }
		[ProtoMember(3)]
		public int doorType { get; set; }
		[ProtoMember(4)]
		public CSVector3 roleInitPos { get; set; }
	}
	[ProtoContract]
	public partial class StateSyncReqest : AProto, IRequest
	{
		[ProtoIgnore]
		public StateSyncResponse ResponseType { get; set; }
		public uint OpCode() { return OuterOpcode.StateSyncReqest; }
		[ProtoMember(1)]
		public long statePacketId { get; set; }
		[ProtoMember(2)]
		public StateSyncData roleSyncData { get; set; }
	}
	[ProtoContract]
	public partial class StateSyncResponse : AProto, IResponse
	{
		public uint OpCode() { return OuterOpcode.StateSyncResponse; }
		[ProtoMember(91, IsRequired = true)]
		public uint ErrorCode { get; set; }
		[ProtoMember(1)]
		public long statePacketId { get; set; }
		[ProtoMember(2)]
		public StateSyncData roleSyncData { get; set; }
	}
	[ProtoContract]
	public partial class StateSyncData : AProto
	{
		[ProtoMember(1)]
		public long playerid { get; set; }
		[ProtoMember(2)]
		public int mapType { get; set; }
		[ProtoMember(3)]
		public CSVector3 position { get; set; }
		[ProtoMember(4)]
		public CSVector3 inputDir { get; set; }
		[ProtoMember(5)]
		public int state { get; set; }
		[ProtoMember(6)]
		public int roleid { get; set; }
		[ProtoMember(7)]
		public int playerMapStatus { get; set; }
	}
	[ProtoContract]
	public partial class OtherPlayerStateSyncMessage : AProto, IMessage
	{
		public uint OpCode() { return OuterOpcode.OtherPlayerStateSyncMessage; }
		[ProtoMember(1)]
		public StateSyncData roleData { get; set; }
	}
	[ProtoContract]
	public partial class CSVector3 : AProto
	{
		[ProtoMember(1)]
		public float x { get; set; }
		[ProtoMember(2)]
		public float y { get; set; }
		[ProtoMember(3)]
		public float z { get; set; }
	}
	[ProtoContract]
	public partial class CreateTeamReqest : AProto, IRequest
	{
		[ProtoIgnore]
		public CreateTeamResponse ResponseType { get; set; }
		public uint OpCode() { return OuterOpcode.CreateTeamReqest; }
		[ProtoMember(1)]
		public long playerid { get; set; }
		[ProtoMember(2)]
		public int mapType { get; set; }
	}
	[ProtoContract]
	public partial class CreateTeamResponse : AProto, IResponse
	{
		public uint OpCode() { return OuterOpcode.CreateTeamResponse; }
		[ProtoMember(91, IsRequired = true)]
		public uint ErrorCode { get; set; }
		[ProtoMember(1)]
		public int tameid { get; set; }
		[ProtoMember(2)]
		public RoleData roleData { get; set; }
	}
	[ProtoContract]
	public partial class JoinTeamReqest : AProto, IRequest
	{
		[ProtoIgnore]
		public JoinTeamResponse ResponseType { get; set; }
		public uint OpCode() { return OuterOpcode.JoinTeamReqest; }
		[ProtoMember(1)]
		public long playerid { get; set; }
		[ProtoMember(2)]
		public int mapType { get; set; }
		[ProtoMember(3)]
		public int teamid { get; set; }
	}
	[ProtoContract]
	public partial class JoinTeamResponse : AProto, IResponse
	{
		public uint OpCode() { return OuterOpcode.JoinTeamResponse; }
		[ProtoMember(91, IsRequired = true)]
		public uint ErrorCode { get; set; }
		[ProtoMember(1)]
		public int tameid { get; set; }
		[ProtoMember(2)]
		public List<RoleData> roleList = new List<RoleData>();
	}
	[ProtoContract]
	public partial class TeamStateChangeMessage : AProto, IMessage
	{
		public uint OpCode() { return OuterOpcode.TeamStateChangeMessage; }
		[ProtoMember(1)]
		public int teamState { get; set; }
		[ProtoMember(2)]
		public RoleData roleData { get; set; }
	}
	[ProtoContract]
	public partial class EnterDungeonMessage : AProto, IMessage
	{
		public uint OpCode() { return OuterOpcode.EnterDungeonMessage; }
		[ProtoMember(1)]
		public int teamid { get; set; }
		[ProtoMember(2)]
		public int dungeonCfgid { get; set; }
		[ProtoMember(3)]
		public List<RoleData> teamRoleInfoList = new List<RoleData>();
	}
	[ProtoContract]
	public partial class LoadDungeonProgresssMessage : AProto, IMessage
	{
		public uint OpCode() { return OuterOpcode.LoadDungeonProgresssMessage; }
		[ProtoMember(1)]
		public int teamid { get; set; }
		[ProtoMember(2)]
		public long playerid { get; set; }
		[ProtoMember(3)]
		public float progress { get; set; }
	}
	[ProtoContract]
	public partial class StartDungeonBattleMessage : AProto, IMessage
	{
		public uint OpCode() { return OuterOpcode.StartDungeonBattleMessage; }
		[ProtoMember(1)]
		public List<RoleData> battleRoleList = new List<RoleData>();
	}
	[ProtoContract]
	public partial class FrameOperateEventMessage_C2G : AProto, IMessage
	{
		public uint OpCode() { return OuterOpcode.FrameOperateEventMessage_C2G; }
		[ProtoMember(1)]
		public long battleid { get; set; }
		[ProtoMember(2)]
		public List<FrameOperateData> frameDataList = new List<FrameOperateData>();
	}
	[ProtoContract]
	public partial class FrameOperateEventMessage_G2C : AProto, IMessage
	{
		public uint OpCode() { return OuterOpcode.FrameOperateEventMessage_G2C; }
		[ProtoMember(1)]
		public long battleid { get; set; }
		[ProtoMember(2)]
		public List<FrameOperateData> frameDataList = new List<FrameOperateData>();
		[ProtoMember(3)]
		public long logicFrameid { get; set; }
	}
	[ProtoContract]
	public partial class FrameOperateData : AProto
	{
		[ProtoMember(1)]
		public int operateType { get; set; }
		[ProtoMember(2)]
		public CSFixIntVector3 inputDir { get; set; }
		[ProtoMember(3)]
		public int skillid { get; set; }
		[ProtoMember(4)]
		public int skillType { get; set; }
		[ProtoMember(5)]
		public CSFixIntVector3 skillPos { get; set; }
		[ProtoMember(6)]
		public long playerid { get; set; }
	}
	[ProtoContract]
	public partial class CSFixIntVector3 : AProto
	{
		[ProtoMember(1)]
		public int x { get; set; }
		[ProtoMember(2)]
		public int y { get; set; }
		[ProtoMember(3)]
		public int z { get; set; }
	}
	[ProtoContract]
	public partial class BattleEndRewardMessage_Share : AProto, IMessage
	{
		public uint OpCode() { return OuterOpcode.BattleEndRewardMessage_Share; }
		[ProtoMember(1)]
		public int cardRewardIndex { get; set; }
		[ProtoMember(2)]
		public long battleid { get; set; }
		[ProtoMember(3)]
		public long playerid { get; set; }
	}
	[ProtoContract]
	public partial class BattleEndMessage_Share : AProto, IMessage
	{
		public uint OpCode() { return OuterOpcode.BattleEndMessage_Share; }
		[ProtoMember(1)]
		public long battleid { get; set; }
		[ProtoMember(2)]
		public long playerid { get; set; }
	}
	[ProtoContract]
	public partial class ExitDungeonReqest : AProto, IRequest
	{
		[ProtoIgnore]
		public JoinTeamResponse ResponseType { get; set; }
		public uint OpCode() { return OuterOpcode.ExitDungeonReqest; }
		[ProtoMember(1)]
		public long battleid { get; set; }
		[ProtoMember(2)]
		public long playerid { get; set; }
	}
	[ProtoContract]
	public partial class ExitDungeonResponse : AProto, IResponse
	{
		public uint OpCode() { return OuterOpcode.ExitDungeonResponse; }
		[ProtoMember(91, IsRequired = true)]
		public uint ErrorCode { get; set; }
		[ProtoMember(1)]
		public long battleid { get; set; }
		[ProtoMember(2)]
		public long playerid { get; set; }
	}
	[ProtoContract]
	public partial class C2A_TestRequest : AProto, IRequest
	{
		[ProtoIgnore]
		public A2C_TestResponse ResponseType { get; set; }
		public uint OpCode() { return OuterOpcode.C2A_TestRequest; }
	}
	[ProtoContract]
	public partial class A2C_TestResponse : AProto, IResponse
	{
		public uint OpCode() { return OuterOpcode.A2C_TestResponse; }
		[ProtoMember(91, IsRequired = true)]
		public uint ErrorCode { get; set; }
	}
	[ProtoContract]
	public partial class TestLoginRequest : AProto, IRequest
	{
		[ProtoIgnore]
		public TestLoginResponse ResponseType { get; set; }
		public uint OpCode() { return OuterOpcode.TestLoginRequest; }
		[ProtoMember(1)]
		public string account { get; set; }
		[ProtoMember(2)]
		public string pass { get; set; }
	}
	[ProtoContract]
	public partial class TestLoginResponse : AProto, IResponse
	{
		public uint OpCode() { return OuterOpcode.TestLoginResponse; }
		[ProtoMember(91, IsRequired = true)]
		public uint ErrorCode { get; set; }
	}
	[ProtoContract]
	public partial class C2G_TestMessageEvent : AProto, IMessage
	{
		public uint OpCode() { return OuterOpcode.C2G_TestMessageEvent; }
		[ProtoMember(1)]
		public string message { get; set; }
		[ProtoMember(2)]
		public int frameOpCode { get; set; }
	}
	[ProtoContract]
	public partial class G2C_TestSeverToClientMessage : AProto, IMessage
	{
		public uint OpCode() { return OuterOpcode.G2C_TestSeverToClientMessage; }
		[ProtoMember(1)]
		public string message { get; set; }
		[ProtoMember(2)]
		public int frameOpCode { get; set; }
	}
}
