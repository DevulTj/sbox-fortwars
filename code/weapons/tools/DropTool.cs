﻿using Sandbox;
using System.Collections.Generic;
using System.Linq;

namespace Fortwars
{
	[Library( "droptool", Title = "Drop tool" )]
	public partial class DropTool : Carriable
	{
		public virtual float PrimaryRate => 2.0f;

		public float DropTimeDelay = 15f;
		public override string ViewModelPath => "models/items/medkit/medkit_v.vmdl";

		public override void Spawn()
		{
			base.Spawn();

			SetModel( "models/items/medkit/medkit_w.vmdl" );
			Scale = 0.25f;

			TimeSinceLastDrop = DropTimeDelay;
		}

		[Net, Predicted]
		public TimeSince TimeSincePrimaryAttack { get; set; }

		[Net, Predicted]
		public TimeSince TimeSinceLastDrop { get; set; }

		[Net] bool Dropped { get; set; }

		public override void Simulate( Client player )
		{
			if ( !Owner.IsValid() )
				return;

			if ( CanPrimaryAttack() )
			{
				using ( LagCompensation() )
				{
					TimeSincePrimaryAttack = 0;
					AttackPrimary();
				}
			}

			if ( IsServer )
			{
				if ( TimeSinceLastDrop < DropTimeDelay )
				{
					Dropped = false;
				}

				if ( TimeSincePrimaryAttack > 0.4f && Dropped )
				{
					using ( LagCompensation() )
					{
						TimeSinceLastDrop = 0;
						DoDrop();
					}
				}
			}

			if ( IsClient )
			{
				if ( TimeSincePrimaryAttack > 0.6f )
				{
					ViewModelEntity?.SetAnimParameter( "noammo", true );
				}

				if ( TimeSinceLastDrop > DropTimeDelay )
				{
					ViewModelEntity?.SetAnimParameter( "noammo", false );
				}
			}
		}

		public virtual void SpawnPickup()
		{
		}

		public void DoDrop()
		{
			if ( IsServer )
			{
				Entity.All.OfType<Pickup>().Where( e => e.Client == this.Client ).ToList().ForEach( e => e.Delete() );
			}

			SpawnPickup();

			Dropped = false;
		}

		public virtual bool CanPrimaryAttack()
		{
			if ( !Owner.IsValid() || !Input.Down( InputButton.Attack1 ) ) return false;

			var rate = PrimaryRate;
			if ( rate <= 0 ) return true;

			return TimeSincePrimaryAttack > (1 / rate);
		}

		public virtual void AttackPrimary()
		{
			var player = Owner as FortwarsPlayer;
			player.SetAnimParameter( "b_attack", true );

			Dropped = true;

			ViewModelEntity?.SetAnimParameter( "fire", true );
		}

		public virtual IEnumerable<TraceResult> TraceHit( Vector3 start, Vector3 end, float radius = 2.0f )
		{
			bool InWater = Map.Physics.IsPointWater( start );

			var tr = Trace.Ray( start, end )
					.UseHitboxes()
					.HitLayer( CollisionLayer.Water, !InWater )
					.HitLayer( CollisionLayer.Debris )
					.Ignore( Owner )
					.Ignore( this )
					.Size( radius )
					.Run();

			yield return tr;
		}

		public override void ActiveStart( Entity ent )
		{
			base.ActiveStart( ent );
		}

		public override void SimulateAnimator( PawnAnimator anim )
		{
			if ( TimeSinceLastDrop < DropTimeDelay )
			{
				EnableDrawing = false;
				anim.SetAnimParameter( "holdtype", 0 );
				anim.SetAnimParameter( "holdtype_handedness", 0 );
				anim.SetAnimParameter( "holdtype_pose_hand", 0f );
				anim.SetAnimParameter( "holdtype_attack", 1 );
			}
			else
			{
				EnableDrawing = true;
				anim.SetAnimParameter( "holdtype", 4 );
				anim.SetAnimParameter( "holdtype_handedness", 0 );
				anim.SetAnimParameter( "holdtype_pose_hand", 0f );
				anim.SetAnimParameter( "holdtype_attack", 1 );
			}
		}
	}
}
