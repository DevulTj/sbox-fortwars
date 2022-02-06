using System;
using Sandbox;

namespace Fortwars.Gamemodes;

public abstract partial class TimedRound : Round
{
	public virtual TimeSpan RoundDuration => TimeSpan.FromSeconds(60);
	public TimeSpan TimeLeft => !IsFinished ? RoundDuration - ElapsedTime : TimeSpan.Zero;

	[Net] private float EndTime { get; set; }

	public override void Start() {
		if(RoundDuration.TotalSeconds > 0) {
			EndTime = Time.Now + (float)RoundDuration.TotalSeconds;
		}

		base.Start();
	}

	public override void Finish() {
		if(Host.IsServer) {
			EndTime = 0f;
		}

		base.Finish();
	}

	protected override void OnThink() {
		if(!(EndTime > 0) || !(Time.Now >= EndTime)) {
			return;
		}

		EndTime = 0f;
		OnTimeUp();
	}

	protected virtual void OnTimeUp() {
		Finish();
	}
}
