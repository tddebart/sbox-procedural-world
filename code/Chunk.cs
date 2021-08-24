using System;
using procedural_world.Utils;
using Sandbox;
using Game = MinimalExample.Game;

namespace procedural_world
{
	public partial class Chunk : ModelEntity
	{
		public Texture texture;

		private Mesh _mesh;
		private Model _model;
		private float[,] noiseMap;
		private MeshData meshData;
		
		

		public void GenerateMapSingle()
		{
			noiseMap = NoiseUtils.GenerateNoiseMap( mapChunkSize, mapChunkSize, seed,scale, octaves , persistance, lacunarity, offset  );
			meshData = MeshDataGenerator.GenerateTerrainMesh( noiseMap, meshHeightScale, levelOfDetail );
			//Log.Info( "height: " + meshHeightScale );
			
			_mesh = new Mesh(Material.Load( "materials/world.vmat"));
			_mesh.CreateVertexBuffer<SimpleVertex>( meshData.VertexCount, SimpleVertex.Layout );
			_mesh.CreateIndexBuffer( meshData.triangles.Length );
			
			RebuildTriangles();
			
			EnableAllCollisions = true;
			EnableShadowCasting = true;
			
			_mesh.LockVertexBuffer<SimpleVertex>( vertices =>
			{
				var index = 0;
				for ( var y = 0; y <= mapChunkSize-1; y+= simplificationIncrement )
				{
					for ( var x = 0; x <= mapChunkSize-1; x+= simplificationIncrement )
					{
						//Log.Info( noiseMap[x,y] );
			
						vertices[index] = new SimpleVertex
						{
							position = meshData.vertices[index],
							 normal = Vector3.One,
							 tangent = Vector3.Left,
							 texcoord = meshData.uvs[index]
						};
			
						index++;
					}
				}
				
				for ( var i = 0; i < meshData.triangles.Length; i += 3 )
				{
					ref var a = ref vertices[meshData.triangles[i + 0]];
					ref var b = ref vertices[meshData.triangles[i + 1]];
					ref var c = ref vertices[meshData.triangles[i + 2]];
				
					var surfaceNormal = Vector3.Cross( c.position - b.position, a.position - b.position ).Normal;
				
					a.normal += surfaceNormal;
					b.normal += surfaceNormal;
					c.normal += surfaceNormal;
				}
				
				foreach ( ref var vertex in vertices )
				{
					vertex.normal = vertex.normal.Normal;
				}
			} );
			_model = new ModelBuilder()
				.AddMesh( _mesh )
				//.AddCollisionMesh( meshData.vertices, meshData.triangles )
				.Create();
			
			SetModel( _model );
			PhysicsClear();
			//SetupPhysicsFromModel( PhysicsMotionType.Static );
			
			SetModelTexture();
		}
		private void RebuildTriangles()
		{
			_mesh.LockIndexBuffer( _triangles =>
			{
				var i = 0;
				var vert = 0;
				for ( var y = 0; y < verticesPerLine-1; y++ )
				{
					for ( var x = 0; x < verticesPerLine-1; x++ )
					{
						_triangles[i] = meshData.triangles[i];
						i++;

						_triangles[i] = meshData.triangles[i];
						i++;

						_triangles[i] = meshData.triangles[i];
						i++;

						_triangles[i] = meshData.triangles[i];
						i++;

						_triangles[i] = meshData.triangles[i];
						i++;
			
						_triangles[i] = meshData.triangles[i]; 
						i++;
			
						vert++;
					}
			
					vert++;
				}
			} );
		}
	}
}
