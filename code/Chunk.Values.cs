using System;
using Sandbox;
using Game = MinimalExample.Game;

namespace procedural_world
{
	public partial class Chunk
	{
		public const int mapChunkSize = 121;
		public int simplificationIncrement => levelOfDetail == 0 ? 1:levelOfDetail * 2;
		public int verticesPerLine => (mapChunkSize - 1) / simplificationIncrement + 1;
		public int levelOfDetail => WorldSettings.lod;
		public int lastLevelOfDetail;
		public static int seed => WorldSettings.seed;
		public static float lastSeed{ get; set; }
		public static float scale => WorldSettings.scale;
		public static float lastScale{ get; set; }
		public static float meshHeightScale => WorldSettings.meshHeightScale;
		public static float lastMeshHeightScale;
		public static int octaves { get; set; }= 4;
		public static float persistance { get; set; }= 0.5f;
		public static float lacunarity { get; set; }= 2;
		public Vector2 offset => WorldSettings.offset;
		public Vector2 lastOffset;
		public static bool lastNoiseMode;
		
		public struct TerrainType
		{
			public string name;
			public float height;
			public Color color;
		}

		public TerrainType[] regions = {
			new TerrainType
			{
				name = "Deep Water",
				height = 0.3f,
				color = new Color( 0, 0.203f, 1 )
			},
			new TerrainType
			{
				name = "Water",
				height = 0.4f,
				color = new Color( 0, 0.556f, 1 )
			},
			new TerrainType
			{
				name = "Sand",
				height = 0.45f,
				color = new Color( 1, 0.878f, 0.360f )
			},
			new TerrainType
			{
				name = "Grass",
				height = 0.55f,
				color = new Color( 0.062f, 0.901f, 0 )
			},
			new TerrainType
			{
				name = "Grass2",
				height = 0.6f,
				color = new Color( 0.047f, 0.678f, 0 )
			},
			new TerrainType
			{
				name = "Rock",
				height = 0.7f,
				color = new Color( 0.847f, 0.423f, 0.333f )
			},
			new TerrainType
			{
				name = "Rock2",
				height = 0.9f,
				color = new Color( 0.388f, 0.239f, 0.172f )
			},
			new TerrainType
			{
				name = "Snow",
				height = 1,
				color = Color.White
			}
		};
		
		// TODO: This is scuffed
		[ServerCmd]
		public static void UpdateSeed( int seed )
		{
			WorldSettings.seed = seed;
		}
		[ServerCmd]
		public static void UpdateScale( float scale )
		{
			WorldSettings.scale = scale;
		}
		[ServerCmd]
		public static void UpdateOffset( Vector2 offset )
		{
			WorldSettings.offset = offset;
		}
		[ServerCmd]
		public static void UpdateHeightScale( float scale )
		{
			WorldSettings.meshHeightScale = scale;
		}
		[ServerCmd]
		public static void Updatelod( int lod )
		{
			WorldSettings.lod = lod;
		}
		
		public override void Simulate( Client cl )
		{
			if ( lastScale != scale ||
			     lastLevelOfDetail != levelOfDetail ||
			     lastSeed != seed ||
			     lastOffset != offset ||
			     lastNoiseMode != WorldSettings.NoiseMode ||
			     lastMeshHeightScale != meshHeightScale)
			{
				if ( IsClient )
				{
					UpdateScale( scale );
					UpdateSeed( seed );
					UpdateOffset( offset );
					UpdateHeightScale( meshHeightScale );
					Updatelod( levelOfDetail );
				}
				GenerateMapSingle();
			}

			//mapWidth = WorldSettings.mapWidth;
			//mapHeight = WorldSettings.mapHeight;
			octaves = WorldSettings.octaves;
			persistance = WorldSettings.persistance;
			lacunarity = WorldSettings.lacunarity;

			WorldSettings.lod = Math.Clamp(WorldSettings.lod, 0, 6 );
			// TODO: this is also dumb
			lastSeed = seed;
			lastScale = scale;
			lastOffset = offset;
			lastNoiseMode = WorldSettings.NoiseMode;
			lastMeshHeightScale = meshHeightScale;
			lastLevelOfDetail = levelOfDetail;
			
			base.Simulate( cl );
		}
	}
}
