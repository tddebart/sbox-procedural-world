using Sandbox;

namespace procedural_world
{
	public static class MeshDataGenerator
	{
		public static MeshData GenerateTerrainMesh( float[,] heightMap, float heightMultiplier, int levelOfDetail )
		{
			var width = heightMap.GetLength( 0 );
			var height = heightMap.GetLength( 1 );
			var topLeftX = (width - 1) / 2f;
			var topLeftY = (height - 1) / 2f;

			int simplificationIncrement = (levelOfDetail == 0)?1:levelOfDetail * 2;
			int verticesPerLine = (width - 1) / simplificationIncrement + 1;

			MeshData meshData = new MeshData(verticesPerLine, verticesPerLine);
			int vertexIndex = 0;

			for ( var x = 0; x < width; x++ )
			for ( var y = 0; y < height; y++ )
			{
				if ( heightMap[x, y] <= 0.4f ) heightMap[x, y] = 0.4f;
			}

			for (int y = 0; y < height; y += simplificationIncrement) {
				for (int x = 0; x < width; x += simplificationIncrement) {
					meshData.vertices[vertexIndex] = new Vector3( (topLeftX - x) *15, (topLeftY - y) *15, heightMap[x,y]*heightMultiplier );
					meshData.uvs [vertexIndex] = new Vector2 (x / (float)width, y / (float)height);

					if (x < width - 2 && y < height - 2) {
						meshData.AddTriangle (vertexIndex, vertexIndex + verticesPerLine + 1, vertexIndex + verticesPerLine);
						meshData.AddTriangle (vertexIndex + verticesPerLine + 1, vertexIndex, vertexIndex + 1);
					}

					vertexIndex++;
				}
			}

			return meshData;
		}
	}

	public class MeshData
	{
		public Vector3[] vertices;
		public int[] triangles;
		public Vector2[] uvs;
		public int VertexCount;

		private int triangleIndex;

		public MeshData( int meshWidth, int meshHeight )
		{
			VertexCount = meshWidth * meshHeight;
			vertices = new Vector3[VertexCount];
			uvs = new Vector2[VertexCount];
			triangles = new int[(meshWidth - 1) * (meshHeight - 1) * 6];
		}


		public void AddTriangle(int a, int b, int c) {
			triangles [triangleIndex] = a;
			triangles [triangleIndex + 1] = b;
			triangles [triangleIndex + 2] = c;
			triangleIndex += 3;
		}
	}
}
