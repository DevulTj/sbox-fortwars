﻿using Sandbox;
using Sandbox.Hooks;
using Sandbox.UI;
using Sandbox.UI.Construct;

namespace Fortwars;

public partial class ChatBox : Panel
{
	public static ChatBox Instance { get; private set; }

	public Panel Canvas { get; protected set; }
	public TextEntry Input { get; protected set; }

	public ChatBox()
	{
		Instance = this;

		StyleSheet.Load( "/ui/hud/ChatBox.scss" );

		Canvas = Add.Panel( "canvas" );
		Canvas.PreferScrollToBottom = true;

		Input = Add.TextEntry( "" );
		Input.AddEventListener( "onsubmit", () => Submit() );
		Input.AddEventListener( "onblur", () => Close() );
		Input.AcceptsFocus = true;
		Input.AllowEmojiReplace = true;

		Chat.OnOpenChat += Open;
	}

	void Open()
	{
		AddClass( "open" );
		Input.Focus();

		foreach ( ChatEntry message in Canvas.Children )
		{
			if ( message.HasClass( "hide" ) )
			{
				message.AddClass( "show" );
			}
		}

		Canvas.TryScrollToBottom();
	}

	void Close()
	{
		RemoveClass( "open" );
		Input.Blur();

		foreach ( ChatEntry message in Canvas.Children )
		{
			if ( message.HasClass( "show" ) )
			{
				message.RemoveClass( "show" );
				message.AddClass( "expired" );
			}
		}

		Canvas.TryScrollToBottom();
	}

	void Submit()
	{
		Close();

		var msg = Input.Text.Trim();
		Input.Text = "";

		if ( string.IsNullOrWhiteSpace( msg ) )
			return;

		Say( msg );

		Canvas.TryScrollToBottom();
	}

	public void AddEntry( string name, string message, string avatar, string additionalClass = null )
	{
		var e = Canvas.AddChild<ChatEntry>();
		e.Message.Text = message;
		e.NameLabel.Text = name;
		e.Avatar.SetTexture( avatar );

		e.SetClass( "noname", string.IsNullOrEmpty( name ) );
		e.SetClass( "noavatar", string.IsNullOrEmpty( avatar ) );

		if ( !string.IsNullOrEmpty( additionalClass ) )
			e.AddClass( additionalClass );

		Canvas.TryScrollToBottom();
	}

	[ClientCmd( "chat_add", CanBeCalledFromServer = true )]
	public static void AddChatEntry( string name, string message, string avatar = null, string className = null )
	{
		Instance?.AddEntry( name, message, avatar, className );

		// Only log clientside if we're not the listen server host
		if ( !Global.IsListenServer )
		{
			Log.Info( $"{name}: {message}" );
		}
	}

	[ClientCmd( "chat_addinfo", CanBeCalledFromServer = true )]
	public static void AddInformation( string message, string avatar = null, bool important = false )
	{
		Instance?.AddEntry( null, message, avatar, important ? "information" : null );
	}

	[ServerCmd( "say" )]
	public static void Say( string message )
	{
		Assert.NotNull( ConsoleSystem.Caller );

		if ( message.StartsWith( "!rtv" ) )
		{
			Game.RockTheVote();
		}

		// todo - reject more stuff
		if ( message.Contains( '\n' ) || message.Contains( '\r' ) )
			return;

		string className = "";

		if ( ConsoleSystem.Caller.Pawn is FortwarsPlayer player )
			className = $"{player.TeamID}-team";

		Log.Info( $"{ConsoleSystem.Caller}: {message}" );
		AddChatEntry( To.Everyone, ConsoleSystem.Caller.Name, message, $"avatar:{ConsoleSystem.Caller.PlayerId}", className );
	}
}
