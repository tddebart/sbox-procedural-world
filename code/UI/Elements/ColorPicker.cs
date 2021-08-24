﻿using System;
using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;

namespace Instagib.UI.Elements
{
	public class ColorPicker : Panel
	{
		private class HexInput : Panel
		{
			private TextEntry hexEntry;
			
			public HexInput()
			{
				var hexPanel = Add.Panel();
				hexPanel.Add.Label( "Hex" );
				hexEntry = hexPanel.Add.TextEntry( "0" );
			}

			public void SetValue( Color color )
			{
				hexEntry.Text = color.Hex;
			}
		}

		private class PickedColor : Panel { }

		private byte[] imageData;
		private int width = 360;
		private int height = 150;
		private int stride = 4;

		private Slider valueSlider;
		private HexInput input;
		private Image image;
		private PickedColor pickedColor;

		private bool move;

		public delegate void ColorPickerChangeEvent( Color color );
		public ColorPickerChangeEvent OnValueChange;

		private bool noColorSlider;

		public bool NoColorSlider
		{
			get => noColorSlider;
			set
			{
				if ( value )
					valueSlider?.Delete();
				noColorSlider = value;
			}
		}

		public ColorPicker()
		{
			StyleSheet.Load( "/Code/UI/Elements/ColorPicker.scss" );
			
			image = Add.Image();

			CreateTexture();
			
			pickedColor = AddChild<PickedColor>();

			if ( !NoColorSlider )
			{
				var valPanel = Add.Panel();
				// valPanel.Add.Label( "Value" );
				valueSlider = valPanel.AddChild<Slider>();
				valueSlider.SnapRate = 5;
				valueSlider.Value = 1.0f;
				valueSlider.OnValueChange += value =>
				{
					CreateTexture( value.CeilToInt() );
					UpdateColor();
				};
			}
		}

		private void CreateTexture( int value = 100 )
		{
			float fValue = value / 100f;

			var hslColor = Color.Red.ToHsv();

			hslColor.Value = fValue;

			var data = new byte[width * height * stride];
			imageData = data;

			for ( int i = 0; i < data.Length; ++i )
				data[i] = 255;

			void SetPixel( int x, int y, Color col )
			{
				data[((x + (y * width)) * stride) + 0] = ColorUtils.ComponentToByte( col.r );
				data[((x + (y * width)) * stride) + 1] = ColorUtils.ComponentToByte( col.g );
				data[((x + (y * width)) * stride) + 2] = ColorUtils.ComponentToByte( col.b );
				data[((x + (y * width)) * stride) + 3] = 255;
			}

			for (int y = 0; y < height; y++)
			{
				hslColor.Hue = 0;
				
				for (int x = 0; x < width; x++)
				{
					var hsvConvert = hslColor.ToColor();
					SetPixel( x, y, hsvConvert );
					hslColor.Hue += 1;
				}
				hslColor.Saturation -= (y * 0.0001f);
			}

			var texture = Texture.Create( width, height ).WithStaticUsage().WithData( data ).WithName( "hsvColor" ).Finish();

			image.Texture = texture;
		}

		private void UpdateColor()
		{
			Color GetPixel( int x, int y )
			{
				var col = new Color
				{
					r = imageData[((x + (y * width)) * stride) + 0] / 255f,
					g = imageData[((x + (y * width)) * stride) + 1] / 255f,
					b = imageData[((x + (y * width)) * stride) + 2] / 255f,
					a = imageData[((x + (y * width)) * stride) + 3] / 255f
				};

				return col;
			}

			var localPos = Mouse.Position - image.Box.Rect.Position;

			var normalizedPos = localPos / image.Box.Rect.Size;

			var arrayPos = normalizedPos * new Vector2( width, height );
			var arrayEntry = GetPixel( (int)arrayPos.x, (int)arrayPos.y );
			
			pickedColor.Style.Left = MousePosition.x * ScaleFromScreen;
			pickedColor.Style.Top = MousePosition.y * ScaleFromScreen;
			pickedColor.Style.Opacity = 1;
			pickedColor.Style.Dirty();
			pickedColor.Style.BackgroundColor = arrayEntry;

			OnValueChange?.Invoke( arrayEntry );
		}

		protected override void OnMouseDown( MousePanelEvent e )
		{
			base.OnMouseDown( e );
			move = true;
		}

		protected override void OnMouseUp( MousePanelEvent e )
		{
			base.OnMouseUp( e );
			move = false;
		}

		protected override void OnClick( MousePanelEvent e )
		{
			base.OnClick( e );
			UpdateColor();
		}

		public override void Tick()
		{
			base.Tick();
			if ( move )
				UpdateColor();
		}
	}
}
