using System;
using System.Threading.Tasks;
using Sandbox;

namespace Fortwars.Gamemodes;

public abstract partial class Round : BaseNetworkable
{
	[Net] public bool IsFinished { get; private set; }
	[Net] public float StartTime { get; private set; }

	// Helpers
	public TimeSpan ElapsedTime => !IsFinished ? TimeSpan.FromSeconds( Time.Now - StartTime ) : TimeSpan.Zero;

	public virtual void Start()
	{
		Host.AssertServer();

		Log.Info( $"Round started {GetType()}" );

		_ = ThinkTimer();

		StartTime = Time.Now;
		OnStart();
	}

	public virtual void Finish()
	{
		Host.AssertServer();

		if ( IsFinished )
		{
			return;
		}

		Log.Info( $"Round ended {GetType()}" );

		IsFinished = true;
		OnFinish();
	}

	private async Task ThinkTimer()
	{
		while ( !IsFinished )
		{
			OnThink();
			await GameTask.DelaySeconds( 1 );
		}
	}

	protected virtual void OnStart() { }
	protected virtual void OnFinish() { }
	protected virtual void OnThink() { }
}
