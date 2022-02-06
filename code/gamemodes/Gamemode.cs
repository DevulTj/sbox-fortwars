using Sandbox;

namespace Fortwars.Gamemodes;

public partial class Gamemode : Entity
{
	public Gamemode()
	{
		Transmit = TransmitType.Always;
	}
	
	// Start

	public void Start()
	{
		Host.AssertServer();
		
		Log.Info( $"Starting Gamemode {ClassInfo.Name}" );
		OnStart();
		Started.Fire( this );
	}

	protected virtual void OnStart() { }
	
	// Finish
	
	public void Finish() 
	{
		Host.AssertServer();
		
		Log.Info($"Finishing Gamemode {ClassInfo.Name}");
		OnFinish();
		Finished.Fire(this);
	}
	
	protected virtual void OnFinish() { }
	
	// Cleanup
	
	public void Cleanup() {
		Log.Info($"Cleaning up Gamemode {ClassInfo.Name}");
		OnCleanup();
	}

	protected virtual void OnCleanup() { }
	
	// Validation
	
	public virtual bool Validate(out string reason) {
		reason = null;
		return true;
	}
	
	// Outputs
			
	private Output Started { get; set; }
	private Output Finished { get; set; }

	//
	// Round
	//

	[Net] public Round Round { get; private set; }

	public void SetRound( Round newRound )
	{
		Host.AssertServer();
		
		Round?.Finish();
		Round = newRound;
		Round?.Start();
	}
}
