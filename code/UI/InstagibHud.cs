using Instagib.UI.Menus;
using Sandbox;
using Sandbox.UI;

namespace Instagib.UI
{
	public partial class InstagibHud : Sandbox.HudEntity<RootPanel>
	{
		public static Panel TiltingHudPanel;
		public static Panel StaticHudPanel;
		private SettingsMenu settingsMenu;
		
		public static InstagibHud CurrentHud;
		
		public InstagibHud()
		{
			if ( IsClient )
			{
				SetCurrentMenu( null );

				CurrentHud = this;
			}
		}

		private BaseMenu currentMenu;
		public void SetCurrentMenu( BaseMenu newMenu = null )
		{
			currentMenu?.Delete();
			currentMenu = null;
			StaticHudPanel?.Delete();
			TiltingHudPanel?.Delete();

			if ( newMenu == null )
			{
				// Show standard hud
				StaticHudPanel = RootPanel.Add.Panel( "staticpanel" );
				StaticHudPanel.StyleSheet.Load( "/Code/UI/MainPanel.scss" );
				StaticHudPanel.AddChild<ClassicChatBox>();
				//StaticHudPanel.AddChild<NameTags>();
				//StaticHudPanel.AddChild<KillFeed>();

				//TiltingHudPanel = RootPanel.AddChild<MainPanel>();
			}
			else
			{
				newMenu.Parent = RootPanel;
				currentMenu = newMenu;
			}
		}

		[Event.Tick.Client]
		public void OnTick()
		{
			if ( Input.Pressed( InputButton.Menu ) )
			{
				if ( currentMenu is MainMenu )
					SetCurrentMenu();
				else
					SetCurrentMenu( new MainMenu() );
			}
		}
	}
}
