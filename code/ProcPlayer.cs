using Sandbox;
using System;
using System.Collections.Generic;
using System.Linq;
using procedural_world;
using procedural_world.Utils;
using Sandbox.ScreenShake;

namespace MinimalExample
{
	public partial class ProcPlayer : Player
	{
		public OldCube OldCube;
		public Chunk Cube;
		public ChunkGenerator chunkGenerator;
		
		public override void Respawn()
		{
			SetModel( "models/citizen/citizen.vmdl" );

			//
			// Use WalkController for movement (you can make your own PlayerController for 100% control)
			//
			Controller = new WalkController();

			//
			// Use StandardPlayerAnimator  (you can make your own PlayerAnimator for 100% control)
			//
			Animator = new StandardPlayerAnimator();

			//
			// Use ThirdPersonCamera (you can make your own Camera for 100% control)
			//
			Camera = new FirstPersonCamera();

			EnableAllCollisions = true;
			EnableDrawing = true;
			EnableHideInFirstPerson = true;
			EnableShadowInFirstPerson = true;

			base.Respawn();
		}

		/// <summary>
		/// Called every tick, clientside and serverside.
		/// </summary>
		public override void Simulate( Client cl )
		{
			base.Simulate( cl );

			//
			// If you have active children (like a weapon etc) you should call this to 
			// simulate those too.
			//
			SimulateActiveChild( cl, ActiveChild );

			//
			// If we're running serverside and Attack1 was just pressed, spawn a ragdoll
			//
			if (Input.Pressed( InputButton.Attack1 ))
			{
				// if ( OldCube != null ) return;
				// var cube = new Cube();
				// cube.Initialize();
				//
				// cube.Position = EyePos + EyeRot.Forward * 40;
				//
				// OldCube = cube;
				
				if ( Cube != null ) return;
				var cube = new Chunk();
				cube.GenerateMapSingle();
				cube.Position = EyePos + EyeRot.Forward * 40;
				Cube = cube;
				
				// if ( chunkGenerator != null ) return;
				// var generator = new ChunkGenerator {Position = Vector3.Zero + new Vector3( 0,0 ,100 ), viewer = this};
				// chunkGenerator = generator;
			}

			// if (Cube != null && Cube.lastScale != Cube.scale )
			// {
			// 	Cube.Initialize();
			// 	if(IsClient)
			// 		if ( WorldSettings != null )
			// 		{
			// 			Cube.UpdateScale( Cube.scale );
			// 		}
			// }

			if ( Cube != null )
			{
				Cube.Simulate( cl );
			}

			if ( chunkGenerator != null )
			{
				chunkGenerator.Simulate(cl);
			}
			
			if (Input.Down(InputButton.Attack2))
			{
				Cube?.GenerateMapSingle();
				// OldCube?.Initialize();
				
				// Log.Info( "pressed" );
				// var tr = Trace.Ray( EyePos, EyePos + EyeRot.Forward * 500 )
				// 	.UseHitboxes()
				// 	.Ignore( this )
				// 	.Run();
				// if ( tr.Hit && tr.Entity is ModelEntity ent )
				// {
				// 	Log.Info( "did" );
				// 	if ( IsClient )
				// 	{
				// 		Log.Info( "client" );
				// 		//var mat = Material.Create("test", "Complex");
				// 		//mat.OverrideTexture( "Normal", DrawNoiseMap() );
				// 		ent.SceneObject.SetMaterialOverride( Material.Load( "materials/test.vmat" ) );
				// 		_texture = Cube?.DrawNoiseMap(NoiseUtils.GenerateNoiseMap( 200,200, 0, 25, 4, 0.5f, 2f, Vector2.Zero ));
				// 		//_texture = CreateColorTexture( 100,100 );
				// 		//Log.Info(_texture.Height + " | " + _texture.Width);
				// 		ent.SceneObject.SetValue( "test" , _texture );
				// 		//ent.SceneObject.ColorTint = Color.White;
				// 	}
				// }
			}
			//Log.Info(WorldSettings?.scale  );
		}
		
		Texture CreateColorTexture( int w, int h)
		{
			if ( w <= 0 || h <= 0 ) return null;

			var img = new byte[w * h * 4];

			void SetColor( int x, int y, Color col )
			{
				img[(x + y * w) * 4] = ColorUtils.ComponentToByte( col.r );
				img[(x + y * w) * 4 + 1] = ColorUtils.ComponentToByte( col.g );
				img[(x + y * w) * 4 + 2] = ColorUtils.ComponentToByte( col.b );
				img[(x + y * w) * 4 + 3] = 100;
			}

			for ( int x = 0; x < w; x++ )
			{
				for ( int y = 0; y < h; y++ )
				{
					SetColor( x, y, Color.Random );
				}
			}

			return Texture.Create( w, h ).WithData( img ).WithName( "color" ).Finish();
		}

		private Texture _texture;

		public override void OnKilled()
		{
			base.OnKilled();

			EnableDrawing = false;
		}
	}
}
