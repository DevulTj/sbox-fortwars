using Sandbox;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace Fortwars;

[Library("fwblock"), AutoGenerate]
public class BlockAsset : Asset
{
	//
	// Static 
	//
	public static List<BlockAsset> All { get; protected set; } = new();
	public static Dictionary<string, BlockAsset> RecognitionDictionary { get; protected set; } = new();
	public static List<string> RecognitionChoices { get; protected set; } = new();

	//
	// Meta
	//
	[Property, Category("Meta")]
	public string BlockName { get; set; } = "My block";

	[Property, Category("Meta")]
	public string BlockDescription { get; set; } = "It does stuff, I guess";

	[Property, Category("Meta")]
	public BlockMaterial.Type BlockType { get; set; } = BlockMaterial.Type.Wood;

	[Property, Category("Meta"), ResourceType("vmdl")]
	public string WorldModel { get; set; }

	[Property, Category("Meta"), ResourceType("png")]
	public string IconPath { get; set; }

	[Property, Category("Meta"), Range(1f, 2048f)]
	public float MaxHealth { get; set; } = 1.0f;

	//
	// Speech Recognition
	//
	[Property, Category("Speech Recognition")]
	public string[] SpokenNames { get; set; }

	//
	// Methods
	//
    protected override void PostLoad()
    {
        base.PostLoad();

		if ( !All.Contains( this ) )
        {
			All.Add( this );

			Log.Info($"[AssetRegistry] Loading new block asset: {BlockName}");

			foreach( var name in SpokenNames )
            {
				RecognitionDictionary.Add( name, this );
				// Requirement for the Speech Recognition library
				RecognitionChoices.Add( name );
			}
        }
    }

	public static BlockAsset FromString( string blockName ) => All.First( x => x.BlockName == blockName );

	public RadialWheel.Item AsRadialWheelItem()
    {
		return new()
		{
			Icon = IconPath,
			Name = BlockName,
			Description = BlockDescription
		};
    }
}
