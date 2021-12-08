using GTANetworkAPI;
using Crimelife.Module;
using System;
using System.Collections.Generic;
using System.Linq;
using GVMP;

namespace Crimelife
{
    internal class RathausModule : Module<RathausModule>
    {
        public static List<Rathaus> RathausList = new List<Rathaus>();
        public static List<Rathaus> TenantsIdss = new List<Rathaus>();

        protected override bool OnLoad()
        {

            RathausList.Add(new Rathaus
            {
                Id = 1,
                Location = new Vector3(-545.22, -203.78, 38.22)
            });
            foreach (Rathaus rathaus in RathausList)
            {
                ColShape val = NAPI.ColShape.CreateCylinderColShape(rathaus.Location, 1.4f, 1.4f, 0u);
                ((Entity)val).SetData("FUNCTION_MODEL", (object)new FunctionModel("openRathausmenus"));
                ((Entity)val).SetData("MESSAGE", (object)new Message("Benutze E um mit dem Rathaus zu interagieren.", "RATHAUS", "orange"));
                NAPI.Marker.CreateMarker(1, rathaus.Location.Subtract(new Vector3(0f, 0f, 1f)), new Vector3(), new Vector3(), 1f, new Color(255, 165, 0), false, 0u);
                NAPI.Blip.CreateBlip(419, rathaus.Location, 1f, (byte)0, "Rathaus", byte.MaxValue, 0f, true, (short)0, 0u);
            }
            return true;
        }


        [RemoteEvent("openRathausmenus")]
        public static void ShowRathausMenu(Player c)
        {
            try
            {
                DbPlayer dbPlayer = c.GetPlayer();
                if (dbPlayer == null || !dbPlayer.IsValid(true) || dbPlayer.Client == null)
                    return;
                dbPlayer.ShowNativeMenu(new NativeMenu("RathausMenu", "Menu", new List<NativeItem>()
                {
                    new NativeItem("Name ändern", "namechange"),
                    new NativeItem("Wunsch-Handynummer", "changenumber"),
                    new NativeItem("Mietvertrag kündigen", "stoprent")
                }));
            }
            catch (Exception ex) { }
        }

        [RemoteEvent("nM-RathausMenu")]
        public static void RathausMenu(Player c, string arg)
        {
            try
            {

                DbPlayer dbPlayer = c.GetPlayer();
                if (!dbPlayer.CanInteractAntiFlood(2)) return;
                if (dbPlayer == null || !dbPlayer.IsValid(true) || dbPlayer.Client == null)
                    return;
                House house = HouseModule.houses.FirstOrDefault((House house2) => house2.OwnerId != 0);
                //house.TenantsIds.Remove(dbPlayer.Id);
                //if (house.TenantPrices.ContainsKey(dbPlayer.Id))
                if (arg == "namechange")
                {
                    dbPlayer.SendNotification("Diese Funktion ist derzeit deaktiviert!", 3000, "red", "RATHAUS");
                    return;
                    if (!((Entity)(object)c == (Entity)null))
                    {
                        DbPlayer player = c.GetPlayer();
                        if (player != null && player.IsValid(ignorelogin: true) && !((Entity)(object)player.Client == (Entity)null))
                        {
                            player.CloseNativeMenu();
                            player.OpenTextInputBox(new TextInputBoxObject
                            {
                                Title = "Namensänderung beantragen | Kosten: $" + 25000 * dbPlayer.Level,
                                Message = "Die Kosten sind 25.000$ * Visumsstufe Grundgebuehr (>!Titel sind verboten!<) Bei erfolgreicher Namensänderung wirst du automatisch vom Server getrennt. Vorname_Nachname und zwar auch hier mit einem Unterstrich.",
                                Callback = "changethePlayerName",
                                CloseCallback = ""
                            });
                        }
                    }
                }

                if (arg == "changenumber")
                {
                    dbPlayer.SendNotification("Diese Funktion ist derzeit deaktiviert!", 3000, "red", "RATHAUS");
                    return;
                }

                if (arg == "stoprent")
                {
                    House houseById = dbPlayer.GetHouse();
                    if (houseById == null) return;
                    if (houseById.OwnerId != dbPlayer.Id && houseById.TenantsIds.Contains(dbPlayer.Id))
                    {
                        dbPlayer.Position = houseById.Entrance;
                        HouseModule.houses.Remove(houseById);
                        houseById.TenantsIds.Remove(dbPlayer.Id);
                        if (houseById.TenantPrices.ContainsKey(dbPlayer.Id))
                        {
                            houseById.TenantPrices.Remove(dbPlayer.Id);
                        }
                        HouseModule.houses.Add(houseById);
                        MySqlQuery mySqlQuery5 = new MySqlQuery("UPDATE houses SET TenantsId = @tenantsid, TenantPrices = @tenantprices WHERE Id = @id");
                        mySqlQuery5.AddParameter("@tenantsid", NAPI.Util.ToJson((object)houseById.TenantsIds));
                        mySqlQuery5.AddParameter("@tenantprices", NAPI.Util.ToJson((object)houseById.TenantPrices));
                        mySqlQuery5.AddParameter("@id", houseById.Id);
                        MySqlHandler.ExecuteSync(mySqlQuery5);
                        dbPlayer.SendNotification("Du hast den Mietvertrag verlassen!", 3000, "red");
                    }
                    return;
                }
            }
            catch (Exception ex) { }
        }

        /*list35.Add(new Command(u003cu003e9_132, "delete", 98, 1));
            List<Command> list36 = CommandModule.commandList;
		Action<DbPlayer, string[]> u003cu003e9_133 = CommandModule.<> c.<> 9__1_33;
            if (u003cu003e9_133 == null)
            {
                u003cu003e9_133 = new Action<DbPlayer, string[]>(CommandModule.<>c.<>9, (DbPlayer dbPlayer, string[] args) => {
                    if (!dbPlayer.HasData("IN_HOUSE"))
                    {
                        dbPlayer.SendNotification("Du befindest dich nicht in einem Haus!", 3000, "gray", "");
                    }
                    else
                    {
                        if (CommandModule.<>o__1.<>p__34 == null)
                        {
                            CommandModule.<>o__1.<>p__34 = CallSite<Func<CallSite, object, int>>.Create(Binder.Convert(CSharpBinderFlags.None, typeof(int), typeof(CommandModule)));
                        }
int num = CommandModule.<> o__1.<> p__34.Target.Invoke(CommandModule.<> o__1.<> p__34, dbPlayer.GetData("IN_HOUSE"));
if (num == 0)
{
	dbPlayer.SendNotification("Du befindest dich nicht in einem Haus!", 3000, "gray", "");
}
else
{
	House houseById = HouseModule.getHouseById(num);
	if ((houseById.OwnerId == dbPlayer.Id ? true : !houseById.TenantsIds.Contains(dbPlayer.Id)))
	{
		return;
	}
	dbPlayer.Position = houseById.Entrance;
	HouseModule.houses.Remove(houseById);
	houseById.TenantsIds.Remove(dbPlayer.Id);
	if (houseById.TenantPrices.ContainsKey(dbPlayer.Id))
	{
		houseById.TenantPrices.Remove(dbPlayer.Id);
	}
	HouseModule.houses.Add(houseById);
	MySqlQuery mySqlQuery = new MySqlQuery("UPDATE houses SET TenantsId = @tenantsid, TenantPrices = @tenantprices WHERE Id = @id");
	mySqlQuery.AddParameter("@tenantsid", NAPI.Util.ToJson(houseById.TenantsIds));
	mySqlQuery.AddParameter("@tenantprices", NAPI.Util.ToJson(houseById.TenantPrices));
	mySqlQuery.AddParameter("@id", houseById.Id);
	MySqlHandler.ExecuteSync(mySqlQuery);
	dbPlayer.SendNotification("Du hast den Mietvertrag verlassen!", 3000, "red", "");
}
                    }
                });
CommandModule.<> c.<> 9__1_33 = u003cu003e9_133;
            }
            list36.Add(new Command(u003cu003e9_133, "cancelrental", 0, 0));*/






        /*	[RemoteEvent("openRathaus")]
            public void openRathaus(Client c)
            {
                try
                {
                    if (!((Entity)(object)c == (Entity)null))
                    {
                        DbPlayer player = c.GetPlayer();
                        if (player != null && player.IsValid(ignorelogin: true) && !((Entity)(object)player.Client == (Entity)null))
                        {
                            NativeMenu nativeMenu = new NativeMenu("Rathaus", "", new List<NativeItem>
                        {
                            new NativeItem("Name ändern - 1.000.000$", "changename")
                        });
                            player.ShowNativeMenu(nativeMenu);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Logger.Print("[EXCEPTION openRathaus] " + ex.Message);
                    Logger.Print("[EXCEPTION openRathaus] " + ex.StackTrace);
                }
            }*/

        /*[RemoteEvent("nM-Rathaus")]
              public void Rathaus(Client c)
              {
                  try
                  {
                      if (!((Entity)(object)c == (Entity)null))
                      {
                          DbPlayer player = c.GetPlayer();
                          if (player != null && player.IsValid(ignorelogin: true) && !((Entity)(object)player.Client == (Entity)null))
                          {
                              player.CloseNativeMenu();
                              player.OpenTextInputBox(new TextInputBoxObject
                              {
                                  Title = "Name ändern",
                                  Message = "Gebe bitte deinen neuen Namen ein, im Anschluss wird dir der Betrag in Höhe von 1.000.000$ vom Staat entfernt. Beispiel: Alpha_Kleinkopf",
                                  Callback = "changeName",
                                  CloseCallback = ""
                              });
                          }
                      }
                  }
                  catch (Exception ex)
                  {
                      Logger.Print("[EXCEPTION nM-Rathaus] " + ex.Message);
                      Logger.Print("[EXCEPTION nM-Rathaus] " + ex.StackTrace);
                  }
              }*/

        [RemoteEvent("changethePlayerName")]
        public void changethePlayerName(Player c, string username)
        {
            SyncThread.Process(username);

            try
            {
                if ((Entity)(object)c == (Entity)null)
                {
                    return;
                }
                DbPlayer player = c.GetPlayer();
                if (player == null || !player.IsValid(ignorelogin: true) || (Entity)(object)player.Client == (Entity)null)
                {
                    return;
                }
                if (player.Money >= 1000000)
                {
                    bool flag = false;
                    foreach (char c2 in username)
                    {
                        if (c2 == '_')
                        {
                            flag = true;
                        }
                    }
                    if (username.Length > 32)
                    {
                        flag = false;
                    }
                    if (username != username.RemoveSpecialCharacters())
                    {
                        flag = false;
                    }
                    if (flag)
                    {
                        MySqlQuery mySqlQuery = new MySqlQuery($"SELECT * FROM accounts WHERE Username = '{username}'");
                        MySqlResult query = MySqlHandler.GetQuery(mySqlQuery);
                        if ((query.Reader).HasRows)
                        {
                            while ((query.Reader).Read())
                            {
                                int @int = query.Reader.GetInt32("Adminrank");
                                if (@int > 0)
                                {
                                    player.BanPlayer();
                                    return;
                                }
                            }
                        }
                        flag = !(query.Reader).HasRows;
                        query.Reader.Dispose();
                        query.Connection.Dispose();
                    }
                    if (!flag)
                    {
                        c.SendNotification("Der kann nur aus einem Vor- und Nachnamen bestehen, nur 32 Zeichen lang und nicht vergeben sein", true);
                        player.OpenTextInputBox(new TextInputBoxObject
                        {
                            Title = "Namensänderung beantragen | Kosten: $" + 25000 * player.Level,
                            Message = "Die Kosten sind 25.000$ * Visumsstufe Grundgebuehr (>!Titel sind verboten!<) Bei erfolgreicher Namensänderung wirst du automatisch vom Server getrennt. Vorname_Nachname und zwar auch hier mit einem Unterstrich.",
                            Callback = "changethePlayerName",
                            CloseCallback = ""
                        });
                        return;
                    }
                    WebhookSender.SendMessage("Spieler benennt sich um", "Der Spieler " + player.Name + " ID " + player.Id + " nennt sich zu " + username + " um.", Webhooks.rathauslogs, "Rathaus");
                    //player.SendNotification("Name geändert, deine Verbindung wurde nun getrennt!", 6000, "green", "RATHAUS");
                    player.removeMoney(25000 * player.Level);
                    player.Name = username;
                    player.RefreshData(player);
                    player.SetAttribute("Username", username);
                    player.Client.Name = (username);
                    c.SendNotification("Du wurdest gekickt! Grund: Namensänderung", true);
                    player.Client.Kick();
                }
                else
                {
                    player.SendNotification("Du hast nicht genug Geld!", 3000, "red", "RATHAUS");
                }
            }
            catch (Exception ex)
            {
                Logger.Print("[EXCEPTION changeName] " + ex.Message);
                Logger.Print("[EXCEPTION changeName] " + ex.StackTrace);
            }
        }
    }
}
