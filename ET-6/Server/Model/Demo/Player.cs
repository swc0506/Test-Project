namespace ET
{
	public enum PlayerState
	{
		Disconnect,
		Gate,
		Game
	}

    [ObjectSystem]
    public class PlayerAwakeSystem : AwakeSystem<Player, long, long>
    {
        public override void Awake(Player self, long a, long roleId)
        {
			self.AccountId = a;
			self.UnitId = roleId;
        }
    }

    public sealed class Player : Entity, IAwake<string>, IAwake<long, long>
	{
		public long AccountId { get; set; }

		public string Account { get; set; }
		
		public long UnitId { get; set; }

		public PlayerState PlayerState { get; set; }

		public Session ClientSession { get; set; }
	}
}