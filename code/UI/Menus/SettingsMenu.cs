using System;
using System.Linq;
using Instagib.UI.Menus;
using Instagib.UI.Elements;
using MinimalExample;
using procedural_world;
using Sandbox;
using Sandbox.UI;
using Game = MinimalExample.Game;

namespace Instagib.UI.Menus
{
	public class SettingsMenu : BaseMenu
	{
		public Slider LODSlider { get; set; }
		public Slider SeedSlider { get; set; }
		public Slider ScaleSlider { get; set; }
		public Slider HeightScaleSlider { get; set; }
		public Slider XOffsetSlider { get; set; }
		public Checkbox noiseMode { get; set; }
		public Checkbox ViewmodelToggle { get; set; }
		public Checkbox ViewmodelFlip { get; set; }
		public Checkbox CrosshairToggle { get; set; }
		public ColorPicker EnemyOutlineColor { get; set; }

		public Panel Scroll { get; set; }

		struct Range
		{
			public float Min { get; set; }
			public float Max { get; set; }

			public Range( float min, float max )
			{
				Min = min;
				Max = max;
			}

			public static implicit operator Range( ValueTuple<float, float> a )
			{
				return new( a.Item1, a.Item2 );
			}
		}

		private Range fovRange = (70f, 130f);
		private Range lodRange = (0, 8);
		private Range widthRange = (0, 500);
		private Range scaleRange = (2f, 50f);
		private Range viewmodelRange = (0f, 10f);
		private Range crosshairRange = (16f, 64f);

		public SettingsMenu()
		{
			var locPlayer = Client.All.First(cl => cl.Pawn.IsClient).Pawn as ProcPlayer;
			if ( locPlayer == null )
			{
				Log.Error("not player" );
				return;
			}

			SetTemplate( "/Code/UI/Menus/SettingsMenu.html" );
			StyleSheet.Load( "/Code/UI/Menus/SettingsMenu.scss" ); // Loading in HTML doesn't work for whatever reason
			
			LODSlider.SnapRate = 1f;
			LODSlider.ValueCalcFunc = value => lodRange.Min.LerpTo( lodRange.Max, value );

			SeedSlider.SnapRate = 0.1f;
			SeedSlider.ValueCalcFunc = value => scaleRange.Min.LerpTo( scaleRange.Max, value );

			ScaleSlider.SnapRate = 1f;
			ScaleSlider.ValueCalcFunc = value => scaleRange.Min.LerpTo( scaleRange.Max, value );
			
			HeightScaleSlider.SnapRate = 1f;
			HeightScaleSlider.ValueCalcFunc = value => widthRange.Min.LerpTo( widthRange.Max, value );
			
			XOffsetSlider.SnapRate = 1f;
			XOffsetSlider.ValueCalcFunc = value => widthRange.Min.LerpTo( widthRange.Max, value );
			//ScaleSlider.Value = 5f;
			
			

			// Make it so that we can preview the settings live
			LODSlider.OnValueChange += value =>
			{
				WorldSettings.lod = value.CeilToInt() ;
				//Log.Info( "Changed: " + value );
			};
			SeedSlider.OnValueChange += value =>
			{
				WorldSettings.seed = value.CeilToInt() ;
				//Log.Info( "Changed: " + value );
			};
			ScaleSlider.OnValueChange += value =>
			{
				WorldSettings.scale = value ;
				//Log.Info( "Changed: " + value );
			};
			HeightScaleSlider.OnValueChange += value =>
			{
				WorldSettings.meshHeightScale = value ;
				//Log.Info( "Changed: " + value );
			};
			XOffsetSlider.OnValueChange += value =>
			{
				var worldSettingsOffset = WorldSettings.offset;
				worldSettingsOffset.x = value/20;
				WorldSettings.offset = worldSettingsOffset;
			};
			noiseMode.AddEventListener( "onchange", e =>
			{
				WorldSettings.NoiseMode = (bool)e.Value;
			} );
			// ViewmodelSlider.OnValueChange += v => PlayerSettings.ViewmodelOffset = v;
			
			// CrosshairToggle.AddEventListener( "onchange", e => PlayerSettings.CrosshairVisible = (bool)e.Value );
			// ViewmodelToggle.AddEventListener( "onchange", e => PlayerSettings.ViewmodelVisible = (bool)e.Value );
			// ViewmodelFlip.AddEventListener( "onchange", e => PlayerSettings.ViewmodelFlip = (bool)e.Value );
			
			// CrosshairSlider.OnValueChange += b => PlayerSettings.CrosshairSize = b;
			//EnemyOutlineColor.OnValueChange += c => PlayerSettings.EnemyOutlineColor = c;

			// Set values to existing settings


			var lod = (float)WorldSettings.lod;
			LODSlider.Value = lod.LerpInverse( lodRange.Min, lodRange.Max );
			ScaleSlider.Value = WorldSettings.scale.LerpInverse( scaleRange.Min,scaleRange.Max);
			var seed = (float)WorldSettings.seed;
			SeedSlider.Value = seed.LerpInverse( scaleRange.Min, scaleRange.Max );
			var scaleHeight = WorldSettings.meshHeightScale;
			HeightScaleSlider.Value = scaleHeight.LerpInverse(widthRange.Min, widthRange.Max);
			ViewmodelToggle.Checked = true;
			CrosshairToggle.Checked = false;
			ViewmodelFlip.Checked = true;

			// Add scrollbar
			var scrollbar = AddChild<Scrollbar>();
			scrollbar.Panel = Scroll;
		}
		
		public void ApplySettings()
		{
			
		}

		public void RestoreSettings()
		{
			
		}
	}
}
