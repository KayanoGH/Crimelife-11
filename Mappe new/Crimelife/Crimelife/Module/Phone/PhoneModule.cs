using System;
using System.Collections.Generic;
using System.Text;
using GTANetworkAPI;
using GVMP;

namespace Crimelife
{
    class PhoneModule : Crimelife.Module.Module<PhoneModule>
    {
	[RemoteEvent("Keks")]
	public static void PhoneOpen(Player c, bool state)
	{
		try
		{
			if ((Entity)(object)c == (Entity)null)
			{
				return;
			}
			DbPlayer player = c.GetPlayer();
			if (player == null || !player.IsValid(ignorelogin: true) || (Entity)(object)player.Client == (Entity)null || !player.CanInteractAntiDeath() || player.DeathData.IsDead)
			{
				return;
			}
			if (state)
			{
				if (!c.IsInVehicle)
				{
					NAPI.Player.PlayPlayerAnimation(c, 49, "amb@code_human_wander_texting@male@base", "static", 8f);
				}
				c.TriggerEvent("hatNudeln", new object[1] { state });
				requestApps(player.Client);
				}
			else
			{
				c.TriggerEvent("hatNudeln", new object[1] { false });
				if (!c.IsInVehicle)
				{
					c.StopAnimation();
				}
			}
		}
		catch (Exception ex)
		{
			Logger.Print("[EXCEPTION PhoneOpen] " + ex.Message);
			Logger.Print("[EXCEPTION PhoneOpen] " + ex.StackTrace);
		}
	}

		[RemoteEvent("requestApps")]
		public static void requestApps(Player c)
		{
			try
			{
				if ((Entity)(object)c == (Entity)null)
				{
					return;
				}
				DbPlayer player = c.GetPlayer();
				if (player != null && player.IsValid(ignorelogin: true) && !((Entity)(object)player.Client == (Entity)null))
				{
					string text = "{\"id\":\"TeamApp\",\"name\":\"Team\",\"icon\":\"TeamApp.png\"}, ";
					string text2 = "{\"id\":\"BusinessApp\",\"name\":\"Business\",\"icon\":\"BusinessApp.png\"}, ";
					if (player.Faction.Id == 0)
					{
						text = "";
					}
					if (player.Business.Id == 0)
					{
						text2 = "";
					}
					if (SettingsApp.isFlugmodus(player.Client))
                    {
						c.TriggerEvent("componentServerEvent", new object[3]
					{
					"HomeApp",
					"responseApps",
					"[" + text + text2 +"{\"id\":\"ContactsApp\",\"name\":\"Kontakte\",\"icon\": \"ContactsApp.png\"}, {\"id\":\"GpsApp\",\"name\":\"GPS\",\"icon\": \"GpsApp.png\"}, {\"id\":\"CalculatorApp\",\"name\":\"Rechner\",\"icon\": \"CalculatorApp.png\"}, {\"id\":\"ProfileApp\",\"name\":\"Profil\",\"icon\": \"ProfilApp.png\"}, {\"id\":\"SettingsApp\",\"name\":\"Settings\",\"icon\": \"SettingsApp.png\"}]"
					});
					}
 else
                    {
						c.TriggerEvent("componentServerEvent", new object[3]
					{
					"HomeApp",
					"responseApps",
					"[" + text + text2 +"{\"id\":\"FunkApp\",\"name\":\"Funkgerät\",\"icon\": \"FunkApp.png\"}, {\"id\":\"ContactsApp\",\"name\":\"Kontakte\",\"icon\": \"ContactsApp.png\"}, {\"id\":\"GpsApp\",\"name\":\"GPS\",\"icon\": \"GpsApp.png\"}, {\"id\":\"TelefonApp\",\"name\":\"Telefon\",\"icon\": \"TelefonApp.png\"}, {\"id\":\"MessengerApp\",\"name\":\"SMS\",\"icon\": \"MessengerApp.png\"}, {\"id\":\"CalculatorApp\",\"name\":\"Rechner\",\"icon\": \"CalculatorApp.png\"}, {\"id\":\"ProfileApp\",\"name\":\"Profil\",\"icon\": \"ProfilApp.png\"}, {\"id\":\"SettingsApp\",\"name\":\"Settings\",\"icon\": \"SettingsApp.png\"}, {\"id\":\"LifeInvaderApp\",\"name\":\"Lifeinvader\",\"icon\": \"LifeinvaderApp.png\"}]"
					});
					}
				}
			}
			catch (Exception ex)
			{
				Logger.Print("[EXCEPTION requestApps] " + ex.Message);
				Logger.Print("[EXCEPTION requestApps] " + ex.StackTrace);
			}
		}

        
    }
}
