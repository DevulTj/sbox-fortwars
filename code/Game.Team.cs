﻿namespace Fortwars
{
	partial class Game
	{
		// shit tier singleton hack since we can't init these in Game ctor
		// CreatePlayer is called before Game ctor
		private RedTeam redTeam;
		private BlueTeam blueTeam;

		public RedTeam RedTeam
		{
			get
			{
				if ( redTeam == null )
					redTeam = new RedTeam();
				return redTeam;
			}
		}

		public BlueTeam BlueTeam
		{
			get
			{
				if ( blueTeam == null )
					blueTeam = new BlueTeam();
				return blueTeam;
			}
		}
	}
}
