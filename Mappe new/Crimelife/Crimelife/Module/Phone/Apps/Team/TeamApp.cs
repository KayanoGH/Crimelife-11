using GTANetworkAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GVMP;

namespace Crimelife
{
    class TeamApp : Crimelife.Module.Module<TeamApp>
    {
        [RemoteEvent("requestTeamMembers")]
        public static void requestFraktionMembers(Player c)
        {
            try
            {
                if (c == null) return;
                DbPlayer dbPlayer = c.GetPlayer();
                if (dbPlayer == null || !dbPlayer.IsValid(true) || dbPlayer.Client == null)
                    return;

                List<TeamMember> TeamMemberList = new List<TeamMember>();

                int ManagePermission = 0;

                if (dbPlayer.Factionrank > 9 && dbPlayer.Factionrank != 12)
                {
                    ManagePermission = 1;
                }
                else if (dbPlayer.Factionrank == 12)
                {
                    ManagePermission = 2;
                }

                foreach (DbPlayer fraktionPlayer in dbPlayer.Faction.GetFactionPlayers())
                {
                    int manage = 0;

                    int medic = 0;

                    if (fraktionPlayer.Factionrank > 10 && fraktionPlayer.Factionrank != 12)
                    {
                        manage = 1;
                    }
                    else if (fraktionPlayer.Factionrank == 12)
                    {
                        manage = 2;
                    }
                    if (fraktionPlayer.Medic)
                    {
                        medic = 1;
                    }

                    TeamMemberList.Add(new TeamMember
                    {
                        Id = fraktionPlayer.Id,
                        Name = fraktionPlayer.Name,
                        Rank = fraktionPlayer.Factionrank,
                        Manage = (fraktionPlayer.Factionrank > 9 ? 2 : 0),
                        Medic = (fraktionPlayer.Medic ? 1 : 0),
                        Number = fraktionPlayer.Id
                    });
                }

                TeamMemberList = TeamMemberList.OrderBy(obj => obj.Rank).ToList();
                TeamMemberList.Reverse();

                object JSONobject = new
                {
                    TeamMemberList = TeamMemberList,
                    ManagePermission = ManagePermission
                };

                dbPlayer.TriggerEvent("componentServerEvent", "TeamListApp", "responseTeamMembers",
                    NAPI.Util.ToJson(JSONobject));
            }
            catch (Exception ex)
            {
                Logger.Print("[EXCEPTION requestTeamMembers] " + ex.Message);
                Logger.Print("[EXCEPTION requestTeamMembers] " + ex.StackTrace);
            }
        }

        [RemoteEvent("leaveFrak")]
        public void LeaveFrak(Player c)
        {
            if (c == null) return;
            DbPlayer dbPlayer = c.GetPlayer();
            if (dbPlayer == null || !dbPlayer.IsValid(true) || dbPlayer.Client == null)
                return;

            try
            {

                if (dbPlayer.Faction.Id == 0)
                    return;

                foreach (DbPlayer target in dbPlayer.Faction.GetFactionPlayers())
                {
                    target.SendNotification("Der Spieler " + c.Name + " hat die Fraktion verlassen.", 3000, dbPlayer.Faction.GetRGBStr(), dbPlayer.Faction.Name);
                }

                dbPlayer.SetAttribute("Fraktion", 0);
                dbPlayer.SetAttribute("Fraktionrank", 0);

                dbPlayer.Faction = FactionModule.getFactionById(0);
                dbPlayer.Factionrank = 0;
                dbPlayer.SetData("Fraksperre", true);
                dbPlayer.RefreshData(dbPlayer);
                c.TriggerEvent("updateTeamId", 0);
                c.TriggerEvent("updateTeamRank", 0);
                c.TriggerEvent("updateJob", "Zivilist");

                dbPlayer.SendNotification("Du hast die Fraktion verlassen.", 3000, "orange", "fraktionssystem");
                dbPlayer.SendNotification("Du hast nun eine Fraksperre (Eine Stunde)", 3000, "orange", "fraktionssystem");
                c.TriggerEvent("hatNudeln", false);



            }
            catch (Exception ex)
            {
                Logger.Print("[EXCEPTION leaveFrak] " + ex.Message);
                Logger.Print("[EXCEPTION leaveFrak] " + ex.StackTrace);
            }
        }

        [RemoteEvent("editTeamMember")]
        public void editTeamMember(Player c, string name, int newrank)
        {
            if (c == null) return;
            DbPlayer dbPlayer = c.GetPlayer();
            if (dbPlayer == null || !dbPlayer.IsValid(true) || dbPlayer.Client == null)
                return;

            try
            {
                if (dbPlayer.Faction.Id == 0)
                    return;

                if (dbPlayer.Factionrank > 9)
                {

                    DbPlayer dbPlayer2 = PlayerHandler.GetPlayer(name);
                    if (dbPlayer2 == null || !dbPlayer2.IsValid(true))
                    {
                        dbPlayer.SendNotification("Spieler nicht online!", 3000, "red");
                        return;
                    }

                    if (dbPlayer == dbPlayer2)
                    {
                        dbPlayer.SendNotification("Du kannst dich nicht selber verwalten", 3000, "red");
                        return;
                    }

                    if (dbPlayer.Factionrank <= newrank)
                    {
                        dbPlayer.SendNotification("Du hast keine Berechtigung, um die Rechte für diesen Spieler zu verändern", 3000, "red");
                        return;
                    }

                    if (dbPlayer2.Faction.Id == dbPlayer.Faction.Id)
                    {
                        if (dbPlayer2.Factionrank < dbPlayer.Factionrank)
                        {
                            dbPlayer.SendNotification("Du hast den Spieler " + name + " auf Rang " + newrank + " gestuft.", 3000, dbPlayer.Faction.GetRGBStr(), dbPlayer.Faction.Name);

                            dbPlayer2.SetAttribute("Fraktionrank", newrank);
                            dbPlayer2.Factionrank = newrank;
                            dbPlayer2.RefreshData(dbPlayer2);
                            dbPlayer.Faction.GetFactionPlayers().ForEach(delegate (DbPlayer target)
                            {
                                target.SendNotification(dbPlayer.Name + " hat " + dbPlayer2.Name + " auf den Rang " + newrank + " gestuft.", 3000, dbPlayer.Faction.GetRGBStr(), dbPlayer.Faction.Name);
                            });
                        }
                        else
                        {
                            dbPlayer.SendNotification("Du hast keine Berechtigung, um die Rechte für diesen Spieler zu verändern.", 3000, dbPlayer.Faction.GetRGBStr(), dbPlayer.Faction.Name);
                        }
                    }
                    else
                    {
                        dbPlayer.SendNotification("Dieser Spieler ist nicht in deiner Fraktion.", 3000, dbPlayer.Faction.GetRGBStr(), dbPlayer.Faction.Name);
                    }
                }
                else
                {
                    dbPlayer.SendNotification("Du hast dazu keine Berechtigung.", 3000, dbPlayer.Faction.GetRGBStr(), dbPlayer.Faction.Name);
                }
            }
            catch (Exception ex)
            {
                Logger.Print("[EXCEPTION editTeamMember] " + ex.Message);
                Logger.Print("[EXCEPTION editTeamMember] " + ex.StackTrace);
            }
        }

        /* [RemoteEvent("parkFraktionVehicles")]
         public void parkCars(Client c)
         {
             if (c == null) return;
             DbPlayer dbPlayer = c.GetPlayer();
             if (dbPlayer == null || !dbPlayer.IsValid(true) || dbPlayer.Client == null)
                 return;

             try
             {
                 if (dbPlayer.Faction == null || dbPlayer.Faction.Id == 0)
                     return;

                 dbPlayer.Faction.GetFactionPlayers().ForEach((DbPlayer target) => target.SendNotification("Alle Fraktionsfahrzeuge wurden von " + c.Name + " eingeparkt.", 3000, dbPlayer.Faction.GetRGBStr(), dbPlayer.Faction.Name));

                 foreach (Vehicle veh in NAPI.Pools.GetAllVehicles())
                     if (((Faction)veh.GetData("VEHICLE_FRAKTION")).Id == dbPlayer.Faction.Id)
                         veh.Delete();
             }
             catch (Exception ex)
             {
                 Logger.Print("[EXCEPTION parkCars] " + ex.Message);
                 Logger.Print("[EXCEPTION parkCars] " + ex.StackTrace);
             }
         }*/

        [RemoteEvent("parkFraktionVehicles")]
        public void parkCars(Player c)
        {
            if ((Entity)(object)c == null)
            {
                return;
            }
            DbPlayer dbPlayer = c.GetPlayer();
            if (dbPlayer == null || !dbPlayer.IsValid(ignorelogin: true) || (Entity)(object)dbPlayer.Client == null || !dbPlayer.CanInteractAntiFlood(1))
            {
                return;
            }
            try
            {
                if (dbPlayer.Faction != null && dbPlayer.Faction.Id != 0)
                {
                    dbPlayer.Faction.GetFactionPlayers().ForEach(delegate (DbPlayer target)
                    {
                        target.SendNotification("Alle Fraktionsfahrzeuge wurden von " + c.Name + " eingeparkt.", 3000, dbPlayer.Faction.GetRGBStr(), dbPlayer.Faction.Name);
                    });
                    NAPI.Pools.GetAllVehicles().FindAll((Vehicle veh) => veh.GetVehicle() != null && veh.GetVehicle().Fraktion != null && veh.GetVehicle().Fraktion.Id == dbPlayer.Faction.Id).ForEach(delegate (Vehicle veh)
                    {
                        ((Entity)veh).Delete();
                    });
                }
            }
            catch (Exception ex)
            {
                Logger.Print("[EXCEPTION parkCars] " + ex.Message);
                Logger.Print("[EXCEPTION parkCars] " + ex.StackTrace);
            }
        }

        [RemoteEvent("kickMember")]
        public void PhoneUninvite(Player c, string name)
        {
            if (c == null) return;
            DbPlayer dbPlayer = c.GetPlayer();
            if (dbPlayer == null || !dbPlayer.IsValid(true) || dbPlayer.Client == null)
                return;

            try
            {

                if (dbPlayer.Faction == null || dbPlayer.Faction.Id == 0)
                    return;

                if (dbPlayer.Factionrank > 9)
                {
                    DbPlayer dbPlayer2 = PlayerHandler.GetPlayer(name);
                    if (dbPlayer2 == null || !dbPlayer2.IsValid(true))
                    {
                        dbPlayer.SendNotification("Spieler nicht online!", 3000, "red");
                        return;
                    }

                    if (dbPlayer2.Faction.Id == dbPlayer.Faction.Id)
                    {
                        if (dbPlayer2.Factionrank < dbPlayer.Factionrank)
                        {
                            dbPlayer.SendNotification("Du hast den Spieler " + name + " uninvited.", 3000,
                                dbPlayer.Faction.GetRGBStr(), dbPlayer.Faction.Name);

                            dbPlayer2.SetAttribute("Fraktion", 0);
                            dbPlayer2.SetAttribute("Fraktionrank", 0);
                            dbPlayer2.SetAttribute("Medic", 0);

                            dbPlayer2.TriggerEvent("updateTeamId", 0);
                            dbPlayer2.TriggerEvent("updateTeamRank", 0);
                            dbPlayer2.TriggerEvent("updateJob", "Zivilist");

                            dbPlayer2.Faction = FactionModule.getFactionById(0);
                            dbPlayer2.Factionrank = 0;
                            dbPlayer2.SetData("Fraksperre", true);
                            dbPlayer2.RefreshData(dbPlayer2);
                            dbPlayer2.SendNotification("Du wurdest aus der Fraktion " + dbPlayer.Faction.Name + " gekickt.", 3000, dbPlayer.Faction.GetRGBStr(), dbPlayer.Faction.Name);
                        }
                        else
                        {
                            dbPlayer.SendNotification("Du hast keine Berechtigung, um diesen Spieler zu uninviten.", 3000, dbPlayer.Faction.GetRGBStr(), dbPlayer.Faction.Name);
                        }
                    }
                    else
                    {
                        dbPlayer.SendNotification("Dieser Spieler ist nicht in deiner Fraktion.", 3000, dbPlayer.Faction.GetRGBStr(), dbPlayer.Faction.Name);
                    }
                }
                else
                {
                    dbPlayer.SendNotification("Du hast dazu keine Berechtigung.", 3000, dbPlayer.Faction.GetRGBStr(), dbPlayer.Faction.Name);
                }
            }
            catch (Exception ex)
            {
                Logger.Print("[EXCEPTION kickMember] " + ex.Message);
                Logger.Print("[EXCEPTION kickMember] " + ex.StackTrace);
            }
        }

        [RemoteEvent("medicgeben")]
        public void Phonemedic(Player c, string name)
        {
            DbPlayer dbPlayer = c.GetPlayer();
            if (dbPlayer == null) return;
            List<DbPlayer> list = dbPlayer.Faction.GetFactionPlayers().FindAll((DbPlayer player) => player.Medic);
            if (list == null) return;
            Player client2 = dbPlayer.Client;
            if (client2 == null) return;
            StringBuilder stringBuilder = new StringBuilder();
            try
            {
                if (dbPlayer.Faction.Id != 0 && dbPlayer.Factionrank >= 11)
                {
                    DbPlayer player2 = PlayerHandler.GetPlayer(name);
                    if (player2 == null || !player2.IsValid(ignorelogin: true))
                    {
                        dbPlayer.SendNotification("Spieler nicht gefunden.");
                    }
                    else if (dbPlayer.Faction.Id != player2.Faction.Id)
                    {
                        dbPlayer.SendNotification("Ihr seit nicht in der gleichen Fraktion!");
                    }
                    else if (player2.Medic)
                    {
                        Faction fraktion = FactionModule.getFactionById(Convert.ToInt32(player2.Faction.Id));
                        player2.Medic = false;
                        player2.RefreshData(player2);
                        player2.SetAttribute("Medic", 0);
                        player2.SendNotification("Du bist nun kein Frak-Medic mehr!", 5000, fraktion.GetRGBStr(), dbPlayer.Faction.Name);
                        dbPlayer.Faction.GetFactionPlayers().ForEach((DbPlayer dbPlayer) => dbPlayer.SendNotification(player2.Name + " ist nun kein Frak-Medic mehr!", 5000, fraktion.GetRGBStr(), dbPlayer.Faction.Name));
                    }
                    else
                    {
                        if (list.Count >= 5)
                        {
                            stringBuilder = new StringBuilder();
                            list.ForEach(delegate (DbPlayer player)
                            {
                                stringBuilder.Append(player.Name + ", ");
                            });
                            dbPlayer.SendNotification("Es sind bereits 5 Medics in deiner Fraktion. " + stringBuilder.ToString());
                        }
                        else
                        {
                            Faction fraktion = FactionModule.getFactionById(Convert.ToInt32(player2.Faction.Id));
                            if (fraktion == null || fraktion.Id == 0) return;
                            player2.Medic = true;
                            player2.RefreshData(player2);
                            player2.SetAttribute("Medic", 1);
                            player2.SendNotification("Du bist nun Frak-Medic!", 5000, "red", dbPlayer.Faction.Name);
                            dbPlayer.Faction.GetFactionPlayers().ForEach((DbPlayer dbPlayer) => dbPlayer.SendNotification(player2.Name + " ist nun Frak-Medic!", 5000, fraktion.GetRGBStr(), dbPlayer.Faction.Name));
                        }
                    }
                }
            }
            catch (Exception ex2)
            {
                Logger.Print("[EXCEPTION setmedic] " + ex2.Message);
                Logger.Print("[EXCEPTION setmedic] " + ex2.StackTrace);
            }
        }

        [RemoteEvent("addPlayer")]
        public void PhoneInvite(Player c, string name)
        {
            if (c == null) return;
            DbPlayer dbPlayer = c.GetPlayer();
            if (dbPlayer == null || !dbPlayer.IsValid(true) || dbPlayer.Client == null)
                return;
            int fraktion = dbPlayer.Faction.Id;
            MySqlQuery mySqlQuery = new MySqlQuery($"SELECT COUNT(*) as Anzahl FROM accounts WHERE Fraktion = '{fraktion}'");
            // MySqlResult mySqlReaderCon = MySqlHandler.GetQuery(mySqlQuery);
            MySqlResult result = MySqlHandler.GetQuery(mySqlQuery);
            try
            {
                if (result.Reader.HasRows)
                {
                    result.Reader.Read();
                    int test = result.Reader.GetInt32("Anzahl");
                    if (dbPlayer.Faction == null || dbPlayer.Faction.Id == 0)
                        return;

                    if (dbPlayer.Factionrank > 9)
                    {
                        DbPlayer dbPlayer2 = PlayerHandler.GetPlayer(name);

                        if (dbPlayer2 == null || !dbPlayer2.IsValid(true))
                        {
                            dbPlayer.SendNotification("Spieler nicht online!", 3000, "red");
                            return;
                        }

                        if (test >= 25)
                        {
                            dbPlayer.SendNotification("Du kannst keinen mehr in deine Fraktion einladen, da das Limit bei 25 Mitglieder liegt!", 7000, "red");
                            return;
                        }


                        /* List<DbPlayer> list = dbPlayer.Faction.GetFactionPlayers().FindAll((DbPlayer player) => player.SpielerFraktion);
                         if (list.Count >= 35)
                         {
                             dbPlayer.SendNotification("Deine Fraktion hat mehr als 35 Member, daher kannst du keinen Mehr einladen!", 5000, dbPlayer.Faction.GetRGBStr(), dbPlayer.Faction.Name);
                         }*/

                        /* List<DbPlayer> factionPlayers = dbPlayer.Faction.GetFactionPlayers();

                         if (factionPlayers.FindAll(dbPlayer2.Id).Count >= 35)
                         {
                             dbPlayer.SendNotification("Deine Fraktion hat mehr als 35 Member, daher kannst du keinen Mehr einladen!", 5000, dbPlayer.Faction.GetRGBStr(), dbPlayer.Faction.Name);
                         }*/


                        if (dbPlayer2.Faction.Id == dbPlayer.Faction.Id)
                        {
                            dbPlayer.SendNotification("Der Spieler ist bereits in deiner Fraktion.", 3000, dbPlayer.Faction.GetRGBStr(), dbPlayer.Faction.Name);
                        }
                        else
                        {
                            if (dbPlayer2.HasData("Fraksperre"))
                            {
                                dbPlayer.SendNotification("Der Spieler hat eine Fraksperre.", 3000, dbPlayer.Faction.GetRGBStr(), dbPlayer.Faction.Name);
                                return;
                            }

                            if (dbPlayer2.Faction.Id == 0)
                            {
                                
                                dbPlayer2.TriggerEvent("openWindow", "Confirmation", "{\"confirmationObject\":{\"Title\":\"" + dbPlayer.Faction.Name + "\",\"Message\":\"Möchtest du die Einladung von " + c.Name + " annehmen?\",\"Callback\":\"acceptInvite\",\"Arg1\":" + dbPlayer.Faction.Id + ",\"Arg2\":\"\"}}");
                                dbPlayer.SendNotification("Du hast " + name + " eine Einladung gesendet.", 3000, dbPlayer.Faction.GetRGBStr(), dbPlayer.Faction.Name);
                            }
                            else
                            {
                                dbPlayer.SendNotification("Dieser Spieler ist bereits in einer Fraktion.", 3000, dbPlayer.Faction.GetRGBStr(), dbPlayer.Faction.Name);
                            }
                        }
                    }
                    else
                    {
                        dbPlayer.SendNotification("Du hast dazu keine Berechtigung.", 3000, dbPlayer.Faction.GetRGBStr(), dbPlayer.Faction.Name);
                    }
                    result.Connection.Dispose();
                }
            }
            catch (Exception ex)
            {
                Logger.Print("[EXCEPTION addPlayer] " + ex.Message);
                Logger.Print("[EXCEPTION addPlayer] " + ex.StackTrace);
            }
        }


        [RemoteEvent("acceptInvite")]
        public void PhoneJoinfrak(Player c, string frak, object unused)
        {
            if (c == null) return;
            DbPlayer dbPlayer = c.GetPlayer();
            if (dbPlayer == null || !dbPlayer.IsValid(true) || dbPlayer.Client == null)
                return;

            try
            {
                Faction fraktion = FactionModule.getFactionById(Convert.ToInt32(frak));
                dbPlayer.Faction = fraktion;
                dbPlayer.Factionrank = 0;
                dbPlayer.RefreshData(dbPlayer);

                dbPlayer.SetAttribute("Fraktion", fraktion.Id);
                dbPlayer.SetAttribute("Fraktionrank", 0);
                dbPlayer.SetAttribute("Medic", 0);

                c.TriggerEvent("updateTeamId", fraktion.Id);
                c.TriggerEvent("updateTeamRank", 0);
                c.TriggerEvent("updateJob", fraktion.Name);

                foreach (DbPlayer target in fraktion.GetFactionPlayers())
                {
                    target.SendNotification("" + c.Name + " ist jetzt ein Mitglied", 3000, fraktion.GetRGBStr(), fraktion.Name);
                }

                dbPlayer.SendNotification("Du bist der Fraktion " + fraktion.Name + " beigetreten.", 3000, fraktion.GetRGBStr(), fraktion.Name);
            }
            catch (Exception ex)
            {
                Logger.Print("[EXCEPTION acceptInvite] " + ex.Message);
                Logger.Print("[EXCEPTION acceptInvite] " + ex.StackTrace);
            }
        }


    }
}
