﻿using Sandbox;

namespace Fortwars
{
	[Hammer.EntityTool( "Shop", "FortWars" )]
	[Hammer.EditorModel( "models/items/shopterminal/shopterminal.vmdl" )]
	[Library( "info_shop" )]
	public class InfoShop : ModelEntity, IUse
	{
		public override void Spawn()
		{
			base.Spawn();

			SetModel( "models/items/shopterminal/shopterminal.vmdl" );
			SetupPhysicsFromModel( PhysicsMotionType.Static );
		}

		public bool IsUsable( Entity user ) => true;

		public bool OnUse( Entity user )
		{
			throw new System.NotImplementedException();
		}
	}
}
