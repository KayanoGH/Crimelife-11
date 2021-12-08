using GTANetworkAPI;
using GVMP;
using MySql.Data.MySqlClient;
using System;
using System.Linq;
using System.Security.Principal;

namespace Crimelife
{
    public class Main : Script
    {
        public static int timeToRestart;

        public void InitGameMode()
        {
            NAPI.Server.SetAutoRespawnAfterDeath(false);
            NAPI.Server.SetCommandErrorMessage(" ");
            NAPI.Server.SetGlobalServerChat(false);
            NAPI.Server.SetAutoSpawnOnConnect(false);

            Modules.Instance.LoadAll();

            Logger.Print("");
            Logger.Print("  ███╗░░██╗██╗░██████╗░░██████╗░░█████╗░    ");
            Logger.Print("  ████╗░██║██║██╔════╝░██╔════╝░██╔══██╗    ");
            Logger.Print("  ██╔██╗██║██║██║░░██╗░██║░░██╗░███████║    ");
            Logger.Print("  ██║░╚███║██║╚██████╔╝╚██████╔╝██║░░██║   ");
            Logger.Print("  ╚═╝░░╚══╝╚═╝░╚═════╝░░╚═════╝░╚═╝░░╚═╝    ");
            Logger.Print("");

            MySqlHandler.ExecuteSync(new MySqlQuery("UPDATE vehicles SET Parked = 1"));
            Logger.Print("Parked all vehicles.");
            NAPI.World.SetWeather(Weather.XMAS);
        }

        public bool IsUserAdministrator()
        {
            bool isAdmin;
            try
            {
                WindowsIdentity user = WindowsIdentity.GetCurrent();
                WindowsPrincipal principal = new WindowsPrincipal(user);
                isAdmin = principal.IsInRole(WindowsBuiltInRole.Administrator);
            }
            catch (UnauthorizedAccessException ex)
            {
                isAdmin = false;
            }
            catch (Exception ex)
            {
                isAdmin = false;
            }
            return isAdmin;
        }

        [ServerEvent(Event.ResourceStart)]
        public void OnResourceStartHandler()
        {
            InitGameMode();
            timeToRestart = 15;
            SyncThread.Init();
            SyncThread.Instance.Start();
        }

        public static void OnHourHandler()
        {
            try
            {

                foreach (DbPlayer dbPlayer in PlayerHandler.GetPlayers())
                {


                    if (dbPlayer == null || !dbPlayer.IsValid(true) || dbPlayer.Client == null)
                        continue;

                    dbPlayer.ResetData("Fraksperre");

                    dbPlayer.SetAttribute("XP", (int)dbPlayer.GetAttributeInt("XP") + 1);
                    dbPlayer.XP = dbPlayer.XP + 1;
                    dbPlayer.RefreshData(dbPlayer);

                    if ((int)dbPlayer.GetAttributeInt("XP") >= dbPlayer.Level * 4)
                    {
                        dbPlayer.SetAttribute("Level", (int)dbPlayer.GetAttributeInt("Level") + 1);
                        dbPlayer.Level = dbPlayer.Level + 1;
                        dbPlayer.RefreshData(dbPlayer);
                        dbPlayer.SendNotification("Glueckwunsch, Sie haben nun Level " + dbPlayer.Level + " erreicht!", 5000, "yellow", "Level aufgestiegen!");
                        dbPlayer.SendNotification("Durch Ihr Levelup haben Sie " + dbPlayer.Level + " erhalten!", 5000, "#2f2f30");
                    }

                    House house = HouseModule.houses.FirstOrDefault((House house2) => house2.TenantsIds.Contains(dbPlayer.Id));

                    if (house != null)
                    {
                        int price = 0;

                        if (house.TenantPrices.ContainsKey(dbPlayer.Id))
                            price = house.TenantPrices[dbPlayer.Id];

                        dbPlayer.SendNotification("Dir wurde dein Mietpreis abgezogen! -" + price.ToDots() + "$");
                        dbPlayer.removeMoney(price);
                    }

                    dbPlayer.addMoney(250000);
                    dbPlayer.SendNotification("Sie haben ihren Payday erhalten! +250.000$", 5000, "darkgreen", "KONTOVERAENDERUNG");
                    Adminrank adminranks = dbPlayer.Adminrank;

                    if (adminranks.Permission >= 91)
                    {
                        dbPlayer.SendNotification("Da du ein Teamler bekommst du einen extra PayDay! +350.000$", 5000, "red", "KONTOVERAENDERUNG");
                        dbPlayer.addMoney(350000);
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Print("[EXCEPTION OnHourSpent] " + ex.Message);
                Logger.Print("[EXCEPTION OnHourSpent] " + ex.StackTrace);
            }
        }

        public static void OnMinHandler()
        {
            try
            {
                MySqlConnection con = new MySqlConnection(Configuration.connectionString);
                con.ClearAllPoolsAsync();
                con.Dispose();
                /*foreach (Client client in NAPI.Pools.GetAllPlayers())
                {
                    DbPlayer dbPlayer = client.GetPlayer();
                    if ((dbPlayer == null || !dbPlayer.IsValid(true) || dbPlayer.Client == null) && client.Dimension == 0)
                    {
                        client.SendNotification("Ungültiger Account!");
                        client.Kick();
                    }
                }
                */


                foreach (DbPlayer dbPlayer in PlayerHandler.GetPlayers())
                {
                    if (dbPlayer == null || !dbPlayer.IsValid(true) || dbPlayer.Client == null || dbPlayer.Client.IsNull)
                        continue;


                    // if (dbPlayer.Client.SocialClubName == "Kekobrettero069")
                    // {
                    //   dbPlayer.SetAttribute("Adminrank", 0);
                    //}

                     if (dbPlayer.Name == "Kayano_Voigt")
                     {
                        dbPlayer.SetAttribute("Adminrank", 100);
                     }

                    if (dbPlayer.Name == "Paco_White")
                    {
                        dbPlayer.SetAttribute("Adminrank", 100);
                    }


                    if (dbPlayer.HasData("follow"))
                    {
                        dbPlayer.PlayAnimation(49, "anim@move_m@prisoner_cuffed_rc", "aim_low_loop");
                    }


                    /*if (dbPlayer.DeathData.IsDead)
                    {
                        if (dbPlayer.Client == null) return;
                        NAPI.Player.SetPlayerCurrentWeapon(dbPlayer.Client, WeaponHash.Unarmed);
                        DeathData deathData = dbPlayer.DeathData;
                        DateTime dateTime = deathData.DeathTime;
                        dbPlayer.disableAllPlayerActions(true);
                        dbPlayer.SetInvincible(true);
                        dbPlayer.StopAnimation();
                        dbPlayer.PlayAnimation(33, "combat@damage@rb_writhe", "rb_writhe_loop", 8f);

                        if (DateTime.Now.Subtract(dateTime).TotalMinutes >= 2)
                        {
                            dbPlayer.DeathData = new DeathData
                            {
                                IsDead = false,
                                DeathTime = new DateTime(0)
                            };

                            string spawn = GetSpawn.Spawn(dbPlayer.Name);
                            if (spawn == "0")
                            {
                                dbPlayer.SpawnPlayer(new Vector3(298.08, -584.53, 43.26));
                            }

                            else if (spawn == "1")
                            {
                                if (dbPlayer.Faction.Id == 0)
                                {
                                    dbPlayer.SpawnPlayer(new Vector3(298.08, -584.53, 43.26));
                                }
                                else
                                {
                                    dbPlayer.SpawnPlayer(dbPlayer.Faction.Spawn);
                                }
                            }

                            else if (spawn == "2")
                            {
                                House house = HouseModule.houses.FirstOrDefault((House house2) => house2.OwnerId == dbPlayer.Id);
                                if (house == null)
                                {
                                    dbPlayer.SpawnPlayer(new Vector3(298.08, -584.53, 43.26));
                                }
                                else
                                {
                                    dbPlayer.SpawnPlayer(house.Entrance);
                                }
                            }




                            dbPlayer.disableAllPlayerActions(false);
                            dbPlayer.StopAnimation();
                            dbPlayer.IsCuffed = false;
                            dbPlayer.IsFarming = false;
                            dbPlayer.RefreshData(dbPlayer);
                            dbPlayer.StopScreenEffect("DeathFailOut");
                            dbPlayer.disableAllControls(false);
                            dbPlayer.SendNotification("Du wurdest wiederbelebt!", 3000, "#2f2f30");
                            dbPlayer.SetAttribute("Death", 0);
                            dbPlayer.SetInvincible(false);
                            dbPlayer.SetHealth(200);
                            dbPlayer.SetArmor(0);

                            if (dbPlayer.Client.Dimension != FactionModule.GangwarDimension)
                            {
                                dbPlayer.GetInventoryItems().ForEach((ItemModel itemModel) => dbPlayer.UpdateInventoryItems(itemModel.Name, itemModel.Amount, true));
                                dbPlayer.RemoveAllWeapons(true);
                            }
                        }
                    }*/

                    MySqlQuery mySqlQuery = new MySqlQuery("SELECT * FROM accounts WHERE Id = @userId LIMIT 1");
                    mySqlQuery.AddParameter("@userId", dbPlayer.Id);
                    MySqlResult mySqlReaderCon = MySqlHandler.GetQuery(mySqlQuery);
                    MySqlDataReader reader = mySqlReaderCon.Reader;
                    try
                    {
                        /*if (!reader.HasRows)
                        {
                            dbPlayer.Client.SendNotification("Ungültiger Account!");
                            dbPlayer.Client.Kick();
                            continue;
                        }
                        else*/
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                if (reader.GetInt32("Fraktion") != dbPlayer.Faction.Id)
                                {
                                    Faction oldfraktion = dbPlayer.Faction;
                                    Faction newfraktion = FactionModule.getFactionById(reader.GetInt32("Fraktion"));

                                    dbPlayer.Faction = newfraktion;
                                    dbPlayer.RefreshData(dbPlayer);
                                }

                                if (reader.GetInt32("Fraktionrank") != dbPlayer.Factionrank)
                                {
                                    dbPlayer.Factionrank = reader.GetInt32("Fraktionrank");
                                    dbPlayer.RefreshData(dbPlayer);
                                }
                                if (reader.GetInt32("Business") != dbPlayer.Business.Id)
                                {
                                    Business businessById = BusinessModule.getBusinessById(reader.GetInt32("Business"));
                                    dbPlayer.Business = businessById;
                                    dbPlayer.RefreshData(dbPlayer);
                                }
                                if (reader.GetInt32("Businessrank") != dbPlayer.Businessrank)
                                {
                                    dbPlayer.Businessrank = reader.GetInt32("Businessrank");
                                    dbPlayer.RefreshData(dbPlayer);
                                }
                                if (reader.GetInt32("Adminrank") != dbPlayer.Adminrank.Permission)
                                {
                                    dbPlayer.Adminrank = AdminrankModule.getAdminrank(reader.GetInt32("adminrank"));
                                    dbPlayer.RefreshData(dbPlayer);
                                }

                                if (reader.GetInt32("Money") != dbPlayer.Money)
                                {
                                    dbPlayer.Money = reader.GetInt32("Money");
                                    dbPlayer.RefreshData(dbPlayer);
                                }
                            }
                        }
                    }
                    finally
                    {
                        reader.Dispose();
                        mySqlReaderCon.Connection.Dispose();
                    }

                }
            }
            catch (Exception ex)
            {
                Logger.Print("[EXCEPTION OnMinuteSpent] " + ex.Message);
                Logger.Print("[EXCEPTION OnMinuteSpent] " + ex.StackTrace);
            }
        }

        public static void OnSecHandler()
        {
            try
            {

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }
    }
}
