using Sandbox;

namespace procedural_world
{
	partial class OldCube
	{
		public Texture TextureFromColorMap( Color[] colorMap, int width, int height )
		{
			var img = new byte[width * height * 4];
			for ( int y = 0; y < height; y++ )
			{
				for ( int x = 0; x < width; x++ )
				{
					img[(y * width + x) * 4] = ColorUtils.ComponentToByte( colorMap[y*width+x].r );
					img[(y * width + x) * 4 + 1] = ColorUtils.ComponentToByte( colorMap[y*width+x].g );
					img[(y * width + x) * 4 + 2] = ColorUtils.ComponentToByte( colorMap[y*width+x].b );
					img[(y * width + x) * 4 + 3] = 255;
				}
			}

			return Texture.Create( width, height ).WithData( img ).WithName( "colorTexture" ).Finish();
		}

		public Texture TextureFromHeightMap( float[,] heightMap )
		{
			if (!IsClient) return null;
			int width = heightMap.GetLength( 0 );
			int height = heightMap.GetLength( 1 );
			
			var img = new byte[width * height * 4];
			var colorMap = new Color[width * height];
			for ( int y = 0; y < height; y++ )
			{
				for ( int x = 0; x < width; x++ )
				{
					colorMap[y * width + x] = Color.Lerp( Color.Black, Color.White, heightMap[x, y] );
				}
			}

			return TextureFromColorMap( colorMap, width, height );
		}
	}
}
