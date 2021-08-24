using Sandbox;

namespace procedural_world
{
	public static class WorldSettings
	{
		public static int lod { get; set; }
		public static int mapWidth { get; set; }= 65;
		public static int mapHeight { get; set; }= 65;
		
		public static int seed { get; set; }= 2;
		
		public static float scale { get; set; } = 15;

		public static float meshHeightScale { get; set; } = 250;
		
		public static int octaves { get; set; }= 4;
		public static float persistance { get; set; }= 0.5f;
		public static float lacunarity { get; set; }= 2;
		public static Vector2 offset { get; set; }= Vector2.Zero;
		public static bool NoiseMode { get; set; } = true;
	}
}
