using System;
using System.Collections.Generic;
using MinimalExample;
using procedural_world.Utils;
using Sandbox;
using Sandbox.Internal;

namespace procedural_world
{
	public class ChunkGenerator : Entity
	{
		public static float maxViewDst = 450;
		public ProcPlayer viewer;

		private static Dictionary<Vector2, TerrainChunk> terrainChunkDictionary = new Dictionary<Vector2, TerrainChunk>();
		private List<TerrainChunk> terrainChunksVisibleLastUpdate = new List<TerrainChunk>();

		public static Vector2 viewerPosition;
		private int chunkSize;
		private int chunkVisibleInViewDst;

		public ChunkGenerator()
		{
			Start();
		}

		private void Start()
		{
			chunkSize = Chunk.mapChunkSize;
			chunkVisibleInViewDst = (int)Math.Round(maxViewDst / chunkSize);
		}

		public override void Simulate( Client cl )
		{
			viewerPosition = new Vector2( viewer.Position.x, viewer.Position.y );
			UpdateVisibleChunks();
			base.Simulate( cl );
		}

		void UpdateVisibleChunks()
		{
			for ( int i = 0; i < terrainChunksVisibleLastUpdate.Count; i++ )
			{
				terrainChunksVisibleLastUpdate[i].SetVisible( false );
			}

			terrainChunksVisibleLastUpdate.Clear();
			
			var currentChunkCoordX = (int)Math.Round( viewerPosition.x / chunkSize );
			var currentChunkCoordY = (int)Math.Round( viewerPosition.y / chunkSize );

			for ( int yOffset = -chunkVisibleInViewDst; yOffset <= chunkVisibleInViewDst; yOffset++ )
			{
				for ( int xOffset = -chunkVisibleInViewDst; xOffset <= chunkVisibleInViewDst; xOffset++ )
				{
					var viewedChunkCoord = new Vector2( currentChunkCoordX + xOffset, currentChunkCoordY + yOffset );
					
					if(terrainChunkDictionary.ContainsKey( viewedChunkCoord ))
					{
						terrainChunkDictionary[viewedChunkCoord].UpdateTerrainChunk();
						if ( terrainChunkDictionary[viewedChunkCoord].IsVisible() )
						{
							terrainChunksVisibleLastUpdate.Add( terrainChunkDictionary[viewedChunkCoord] );
						}
					}
					else
					{
						terrainChunkDictionary.Add( viewedChunkCoord, new TerrainChunk(viewedChunkCoord, chunkSize) );
					}
				}
			}
		}
		public class TerrainChunk
		{
			private ModelEntity meshObject;
			private Vector2 position;
			//private BBox bounds;
			
			public TerrainChunk( Vector2 coord, int size )
			{
				position = coord * size;
				//bounds = new BBox(position-Vector2.One*size, position+Vector2.One*size);
				Vector3 positionV3 = new Vector3( position.x, position.y, 0 );

				var modelEnt = new ModelEntity();
				modelEnt.SetModel( Model.Load( "models/plane.vmdl" ) );
				meshObject = modelEnt;
				// meshObject = new Chunk();
				// meshObject.GenerateMap();
				meshObject.Position = positionV3;
				meshObject.LocalScale = 1 * size/100f;
				SetVisible( true );
			}

			public void UpdateTerrainChunk()
			{
				var viewerDstFromNearestEdge = MathF.Sqrt( meshObject.WorldSpaceBounds.SqrDistance( viewerPosition ) );
				bool visible = viewerDstFromNearestEdge >= maxViewDst;
				SetVisible( visible ); 
			}

			public void SetVisible(bool visible)
			{
				// if ( visible )
				// {
				// 	var meshObjectPosition = meshObject.Position;
				// 	meshObjectPosition.z = 0;
				// 	meshObject.Position = meshObjectPosition;
				// }
				// else
				// {
				// 	var meshObjectPosition = meshObject.Position;
				// 	meshObjectPosition.z = 500;
				// 	meshObject.Position = meshObjectPosition;
				// }
				meshObject.EnableDrawing = visible;
				//meshObject.DeleteAsync(0);
				//terrainChunkDictionary.Remove( position );
			}

			public bool IsVisible()
			{
				return meshObject.EnableDrawing;
			}

		}
	}
}
