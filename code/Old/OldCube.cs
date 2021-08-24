using System;
using procedural_world.Utils;
using Sandbox;
using Game = MinimalExample.Game;

namespace procedural_world
{
	public partial class OldCube : ModelEntity
	{
		private static int HeightDataResolution => 64;

		private static int VertexCount => mapWidth * mapHeight;
		private static int triangles => (mapWidth-1) * (mapHeight-1) * 6;

		private Vector3[] _vertices;
		private int[] _triangles;
		private Mesh _mesh;
		private Model _model;
		
		public struct TerrainType
		{
			public string name;
			public float height;
			public Color color;
		}

		public TerrainType[] regions;
		
		
		public static int mapWidth => WorldSettings.mapWidth;
		public static float lastWidth{ get; set; }
		public static int mapHeight => WorldSettings.mapHeight;
		public static float lastHeight{ get; set; }
		public static int seed => WorldSettings.seed;
		public static float lastSeed{ get; set; }
		public static float scale => WorldSettings.scale;
		public static float lastScale{ get; set; }
		public static int octaves { get; set; }= 4;
		public static float persistance { get; set; }= 0.5f;
		public static float lacunarity { get; set; }= 2;
		public static Vector2 offset => WorldSettings.offset;
		public static Vector2 lastOffset;
		public static bool lastNoiseMode;

		public void Initialize()
		{
			regions = new[]
			{
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

			_vertices = new Vector3[VertexCount];
			_triangles = new int[triangles];
			
			_mesh = new Mesh( Material.Load( "materials/world.vmat") );


			_mesh.CreateVertexBuffer<SimpleVertex>( VertexCount, SimpleVertex.Layout );
			_mesh.CreateIndexBuffer( triangles );

			RebuildIndices();

			EnableAllCollisions = true;
			EnableShadowCasting = true;
			
			Rebuild();
		}
		
		// TODO: This is scuffed
		[ServerCmd]
		public static void UpdateWidth( int width )
		{
			WorldSettings.mapWidth = width;
		}
		[ServerCmd]
		public static void UpdateHeight( int height )
		{
			WorldSettings.mapHeight = height;
		}
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

		public override void Simulate( Client cl )
		{
			if ( lastScale != scale || lastWidth != mapWidth || lastHeight != mapHeight || lastSeed != seed || lastOffset != offset || lastNoiseMode != WorldSettings.NoiseMode )
			{
				if ( IsClient )
				{
					UpdateScale( scale );
					UpdateWidth( mapWidth );
					UpdateHeight( mapHeight );
					UpdateSeed( seed );
					UpdateOffset( offset );
				}
				Initialize();
			}

			//mapWidth = WorldSettings.mapWidth;
			//mapHeight = WorldSettings.mapHeight;
			octaves = WorldSettings.octaves;
			persistance = WorldSettings.persistance;
			lacunarity = WorldSettings.lacunarity;

			// TODO: this is also dumb
			lastWidth = mapWidth;
			lastHeight =mapHeight;
			lastSeed = seed;
			lastScale = scale;
			lastOffset = offset;
			lastNoiseMode = WorldSettings.NoiseMode;
			
			base.Simulate( cl );
		}

		protected override void OnDestroy()
		{
			base.OnDestroy();
			
			_mesh = null;
			_model = null;
		}


		private void Rebuild()
		{
			//var noiseMap = new float[mapWidth, mapHeight];
			var noiseMap = NoiseUtils.GenerateNoiseMap( mapWidth, mapHeight, 0,scale, octaves , persistance, lacunarity, offset  );
			_mesh.LockVertexBuffer<SimpleVertex>( vertices =>
			{
				var index = 0;
				for ( var y = 0; y <= mapHeight-1; y++ )
				{
					for ( var x = 0; x <= mapWidth-1; x++ )
					{
						//Log.Info( noiseMap[x,y] );
						var position = new Vector3( x * 20, y * 20, noiseMap[x,y]*200 );
			
						vertices[index] = new SimpleVertex
						{
							position = new Vector2( x,y)
							// normal = Vector3.One,
							// tangent = Vector3.Left,
							// texcoord = new Vector2( x,y )/HeightDataResolution
						};
			
						_vertices[index] = position;
			
						index++;
					}
				}
			
				// for ( var i = 0; i < _triangles.Length; i += 3 )
				// {
				// 	ref var a = ref vertices[_triangles[i + 0]];
				// 	ref var b = ref vertices[_triangles[i + 1]];
				// 	ref var c = ref vertices[_triangles[i + 2]];
				//
				// 	var surfaceNormal = Vector3.Cross( c.position - b.position, a.position - b.position ).Normal;
				//
				// 	a.normal += surfaceNormal;
				// 	b.normal += surfaceNormal;
				// 	c.normal += surfaceNormal;
				// }
				//
				// foreach ( ref var vertex in vertices )
				// {
				// 	vertex.normal = vertex.normal.Normal;
				// }
			} );

			// TODO: delete the old model or something
			_model = new ModelBuilder()
				.AddMesh( _mesh )
				.AddCollisionMesh( _vertices, _triangles )
				.Create();
			
			SetModel( _model );
			PhysicsClear();
			SetupPhysicsFromModel( PhysicsMotionType.Static );
			
			if ( IsClient )
			{
				//var noiseMap = NoiseUtils.GenerateNoiseMap( mapWidth, mapHeight, 0,WorldSettings.scale, octaves , persistance, lacunarity, offset  );

				Color[] colorMap = new Color[mapWidth * mapHeight];
				for ( int y = 0; y < mapHeight; y++ )
				{
					for ( int x = 0; x < mapWidth; x++ )
					{
						float currentHeight = noiseMap[x, y];
						for ( int i = 0; i < regions.Length; i++ )
						{
							if ( currentHeight <= regions[i].height )
							{
								colorMap[y * mapWidth + x] = regions[i].color;
								break;
							}
						}	
					}	
				}

				if ( WorldSettings.NoiseMode )
				{
					_texture = TextureFromHeightMap( noiseMap );
					
				}
				else
				{
					_texture = TextureFromColorMap( colorMap, mapWidth, mapHeight );
				}

				//_texture = Texture.Load( "https://i.imgur.com/fxEZlwr.png" );
				SceneObject.SetValue( "test" , _texture );
			}
		}

		private Texture _texture;

		private void RebuildIndices()
		{
			_mesh.LockIndexBuffer( indices =>
			{
				var i = 0;
				var vert = 0;
				for ( var y = 0; y < Math.Min(mapHeight-1, mapWidth-1); y++ )
				{
					for ( var x = 0; x < Math.Min(mapHeight-1, mapWidth-1); x++ )
					{
						indices[i] = _triangles[i] = vert + HeightDataResolution + 2;
						i++;

						indices[i] = _triangles[i] = vert + Math.Min(mapHeight-1, mapWidth-1) + 1;
						i++;

						indices[i] = _triangles[i] = vert + 1;
						i++;

						indices[i] = _triangles[i] = vert + 1;
						i++;

						indices[i] = _triangles[i] = vert + Math.Min(mapHeight-1, mapWidth-1) + 1;
						i++;

						indices[i] = _triangles[i] = vert + 0;
						i++;

						vert++;
					}

					vert++;
				}
			} );
		}
	}
}
