using System;

namespace procedural_world.Utils
{
	public static class BboxExtension
	{
		public static Vector3 ClosestPoint( this BBox bbox, Vector3 point )
		{
			var dx = Math.Max( bbox.Mins.x - point.x, point.x - bbox.Maxs.x  );
			var dy = Math.Max( bbox.Mins.y - point.y, point.y - bbox.Maxs.y );
			var dz = Math.Max( bbox.Mins.z - point.z, point.z - bbox.Maxs.z );

			return new Vector3( dx, dy, dz );
		}
		
		public static float SqrDistance ( this BBox bbox, Vector3 point )
		{
			var smallestDistance = Vector3.DistanceBetween(point, bbox.ClosestPoint( point ));

			return smallestDistance * smallestDistance;
		}
	}
}
