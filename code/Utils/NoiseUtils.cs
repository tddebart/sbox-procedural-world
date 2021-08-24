using System;
using Sandbox;

namespace procedural_world.Utils
{
	public static class NoiseUtils
	{
		public static float[,] GenerateNoiseMap( int mapWidth, int mapHeight,int seed, float scale, int octaves, float persistance, float lacunarity, Vector2 offset )
		{
			float[,] noiseMap = new float[mapWidth, mapHeight];

			Rand.SetSeed( seed );
			Vector2[] octavesOffsets = new Vector2[octaves];
			for ( var index = 0; index < octavesOffsets.Length; index++ )
			{
				float offsetX = Rand.Int( -100000, 100000 ) + offset.x;
				float offsetY = Rand.Int( -100000, 100000 ) + offset.y;
				octavesOffsets[index] = new Vector2( offsetX, offsetY );
			}

			if ( scale <= 0 ) scale = 0.0001f;

			float maxNoiseHeight = float.MinValue;
			float minNoiseHeight = float.MaxValue;

			float halfWidth = mapWidth / 2;
			float halfHeight = mapHeight / 2;

			for ( var y = 0; y < mapHeight; y++ )
			{
				for ( var x = 0; x < mapWidth; x++ )
				{

					float amplitude = 1;
					float frequency = 1;
					float noiseHeight = 0;
					
					for ( int i = 0; i < octaves; i++ )
					{
						float sampleX = (x-halfWidth) / scale * frequency + octavesOffsets[i].x;
						float sampleY = (y-halfHeight) / scale * frequency + octavesOffsets[i].y;

						float perlinValue = Noise.Perlin( sampleX, sampleY, 0 ) * 2 - 1;
						noiseHeight += perlinValue * amplitude;

						amplitude *= persistance;
						frequency *= lacunarity;
					}

					if ( noiseHeight > maxNoiseHeight )
					{
						maxNoiseHeight = noiseHeight;
					}
					else if ( noiseHeight < minNoiseHeight )
					{
						minNoiseHeight = noiseHeight;
					}
					noiseMap[x, y] = noiseHeight;
				}		
			}

			for ( var y = 0; y < mapHeight; y++ )
			{
				for ( var x = 0; x < mapWidth; x++ )
				{
					noiseMap[x, y] = noiseMap[x, y].LerpInverse( minNoiseHeight, maxNoiseHeight);
				}
			}

			return noiseMap;
		}
	}
}
