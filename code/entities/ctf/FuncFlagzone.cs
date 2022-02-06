using Sandbox;

namespace Fortwars
{
	/// <summary>
	/// Players capture the flag in this area.
	/// </summary>
	[Library( "func_flagzone" )]
	[Hammer.Solid]
	[Hammer.RenderFields]
	[Hammer.VisGroup( Hammer.VisGroup.Dynamic )]
	public partial class FuncFlagzone : BrushEntity
	{
		[Property]
		public Team Team { get; set; }

		public override void Spawn()
		{
			base.Spawn();

			SetupPhysicsFromModel( PhysicsMotionType.Static );
			CollisionGroup = CollisionGroup.Trigger;
			EnableSolidCollisions = false;
			EnableTouch = true;

			Transmit = TransmitType.Never;
		}

		public override void StartTouch( Entity other )
		{
			if ( other is FortwarsPlayer player )
				Event.Run( "gamemodes.ctf.flagzone.touched", player, Team );
		}

		public override void EndTouch( Entity other )
		{
			base.EndTouch( other );
		}
	}
}
