﻿using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;

namespace Fortwars
{
	public class BaseMessage : Panel
	{
		Panel progress;

		TimeUntil remainingLifetime;

		public BaseMessage( string icon, string title, string message )
		{
			StyleSheet.Load( "/ui/elements/generic/BaseMessage.scss" );

			progress = Add.Panel( "inner" );

			var left = Add.Panel();
			left.Add.Icon( icon, "icon" );
			left.Add.Label( title, "title" );

			Add.Label( message, "message" );

			remainingLifetime = 5;
		}

		public override void Tick()
		{
			base.Tick();
			progress.Style.Width = Length.Fraction( remainingLifetime / 5.0f );

			if ( remainingLifetime <= 0 )
				Delete();
		}
	}

	public static class BaseMessageExtensions
	{
		public static BaseMessage Message( this PanelCreator self, string icon, string title, string message )
		{
			var control = new BaseMessage( icon, title, message );
			control.Parent = self.panel;

			return control;
		}
	}
}
