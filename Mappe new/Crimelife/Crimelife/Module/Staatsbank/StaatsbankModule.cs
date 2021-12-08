using GTANetworkAPI;
using GVMP.Handlers;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using GVMP;

namespace Crimelife.Module.Staatsbank
{
    class StaatsbankModule : Crimelife.Module.Module<StaatsbankModule>
    {
        public static string fillstaatsbank()
        {
            try
            {
                int random = new Random().Next(6, 20);
                JObject admin = new JObject(
                    new JProperty("Id", (557)),
                     new JProperty("Slot", (1)),
                      new JProperty("Weight", (0)),
                       new JProperty("Name", ("Goldbarren")),
                        new JProperty("ImagePath", ("goldbarren.png")),
                         new JProperty("Amount", (random))
                    );
                return "[" + JsonConvert.SerializeObject(admin) + "]";
            }
            catch (Exception)
            {

                throw;
            }
        }

        protected override bool OnLoad()
        {
            try
            {
                ColShape val = NAPI.ColShape.CreateCylinderColShape(new Vector3(254, 225.27, 101.88), 1.4f, 1.4f, 0);
                ColShape valback = NAPI.ColShape.CreateCylinderColShape(new Vector3(253.26, 223.13, 101.68), 1.4f, 1.4f, 0);
                val.SetData("FUNCTION_MODEL", new FunctionModel("openstaatsbank"));
                valback.SetData("FUNCTION_MODEL", new FunctionModel("leavestaatsbank", 1, 1));
                val.SetData("MESSAGE", new Message("Benutze E um die Staatsbank aufzubrechen", "Staatsbank", "yellow", 5000));
                val.SetData("staatscol", true);
                Marker mark = NAPI.Marker.CreateMarker(1, new Vector3(254, 225.27, 101.88 - 0.62), new Vector3(), new Vector3(), 1.0f, new Color(255, 140, 0), false, 0);
                mark.SetData("staatsmark", true);
                Marker markback = NAPI.Marker.CreateMarker(1, new Vector3(253.26, 223.13, 101.68 - 0.62), new Vector3(), new Vector3(), 1.0f, new Color(255, 140, 0), false, 0);
                markback.SetData("staatsmark", true);
                Blip blip = NAPI.Blip.CreateBlip(207, new Vector3(231.99, 215, 106.28), 1f, 46, "Staatsbank", 255, 0.0f, true, 0, 0);
                blip.SetData("staatsblip", true);

                ColShape schrank1 = NAPI.ColShape.CreateCylinderColShape(new Vector3(258.36, 214.62, 101.68), 1.4f, 1.4f, 0);
                schrank1.SetData("FUNCTION_MODEL", new FunctionModel("openstaatsbankschrank", 1));
                schrank1.SetData("Schrank", 1);


                ColShape schrank2 = NAPI.ColShape.CreateCylinderColShape(new Vector3(259.41, 217.77, 101.68), 1.4f, 1.4f, 0);
                schrank2.SetData("FUNCTION_MODEL", new FunctionModel("openstaatsbankschrank", 2));
                schrank2.SetData("Schrank", 2);

                ColShape schrank3 = NAPI.ColShape.CreateCylinderColShape(new Vector3(264.6, 215.85, 101.68), 1.4f, 1.4f, 0);
                schrank3.SetData("FUNCTION_MODEL", new FunctionModel("openstaatsbankschrank", 3));
                schrank3.SetData("Schrank", 3);

                ColShape schrank4 = NAPI.ColShape.CreateCylinderColShape(new Vector3(265.98, 213.63, 101.68), 1.4f, 1.4f, 0);
                schrank4.SetData("FUNCTION_MODEL", new FunctionModel("openstaatsbankschrank", 4));
                schrank4.SetData("Schrank", 4);

                ColShape schrank5 = NAPI.ColShape.CreateCylinderColShape(new Vector3(263.65, 212.49, 101.68), 1.4f, 1.4f, 0);
                schrank5.SetData("FUNCTION_MODEL", new FunctionModel("openstaatsbankschrank", 5));
                schrank5.SetData("Schrank", 5);

                Constants.Staatsbank.LastOpened = DateTime.Now;
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            return false;
        }

        [RemoteEvent("openstaatsbankschrank")]
        public static void openstaatsbankschrank(Player player, int schrank)
        {
            try
            {
                DbPlayer dbPlayer = player.GetPlayer();
                if (Constants.Staatsbankschrankopen(schrank) || !Constants.Staatsbank.Opened || Constants.Staatsbankschrankbroken(schrank)) return;

                dbPlayer.disableAllPlayerActions(true);
                dbPlayer.PlayAnimation(49, "amb@code_human_police_investigate@idle_b", "idle_e");
                dbPlayer.SendProgressbar(Constants.Staatsbank.Staatsschranktime);
                Constants.SetStaatsbankschrankbroken(schrank);

                NAPI.Task.Run(() =>
                {
                    if (dbPlayer.DeathData.IsDead)
                    {
                        Constants.SetStaatsbankschrankunbroken(schrank);
                        return;
                    }

                    dbPlayer.disableAllPlayerActions(false);
                    dbPlayer.StopAnimation();
                    Constants.SetStaatsbankschrankopen(schrank);
                    dbPlayer.SendNotification("Fach erfolgreich aufgebrochen", 3000, "yellow", "Staatsbank");
                }, Constants.Staatsbank.Staatsschranktime);

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        [RemoteEvent("leavestaatsbank")]
        public static void leavestaatsbank(Player player, int number, int number2)
        {
            try
            {
                DbPlayer dbPlayer = player.GetPlayer();
                if (Constants.Staatsbank.Opened)
                {
                    player.Position = new Vector3(254, 225.27, 101.88);
                    dbPlayer.SendNotification("Staatsbank Tresorraum verlassen", 3000, "red", "Staatsbank");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        public static void endstaatsbank()
        {
            try
            {
                Constants.Staatsbank.Broken = false;
                Constants.Staatsbank.LastOpened = DateTime.Now.AddHours(2);
                Constants.Staatsbank.Schrank1Open = false;
                Constants.Staatsbank.Schrank1Broken = false;
                Constants.Staatsbank.Schrank2Open = false;
                Constants.Staatsbank.Schrank2Broken = false;
                Constants.Staatsbank.Schrank3Open = false;
                Constants.Staatsbank.Schrank3Broken = false;
                Constants.Staatsbank.Schrank4Open = false;
                Constants.Staatsbank.Schrank4Broken = false;
                Constants.Staatsbank.Schrank5Open = false;
                Constants.Staatsbank.Schrank5Broken = false;
                Constants.Staatsbank.Opened = false;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }


        [RemoteEvent("openstaatsbank")]
        public static void openstaatsbank(Player player)
        {
            try
            {
                DbPlayer dbPlayer = player.GetPlayer();
                if (Constants.Staatsbank.Opened)
                {
                    player.Position = new Vector3(253.26, 223.13, 101.68);
                    dbPlayer.SendNotification("Staatsbank Tresorraum betreten", 3000, "green", "Staatsbank");
                }
                else
                {
                    if (Constants.Staatsbank.Broken) return;
                    if (Constants.Staatsbank.LastOpened > DateTime.Now)
                    {
                        dbPlayer.SendNotification($"Die Staatsbank wurde vor kurzem ausgeraubt! Komme wieder um: {Constants.Staatsbank.LastOpened}", 3000, "red", "Staatsbank");
                        return;
                    }


                    MySqlQuery mySqlQuery = new MySqlQuery("SELECT * FROM inventorys WHERE Id = @userId LIMIT 1");
                    mySqlQuery.AddParameter("@userId", dbPlayer.Id);
                    MySqlResult mySqlResult = MySqlHandler.GetQuery(mySqlQuery);

                    MySqlDataReader reader = mySqlResult.Reader;

                    reader.Read();

                    string items = reader.GetString("Items");

                    if (!items.Contains("Schweissgeraet"))
                    {
                        dbPlayer.SendNotification("Du besitzt nicht das nötige Item um die Staatsbank zu öffnen", 3000, "red", "Staatsbank");
                        return;
                    }

                    Notification.SendGlobalNotification("Es wird versucht die Staatsbank aufzubrechen! Greift sie an, und holt euch das Gold!", 7500, "lightblue", Notification.icon.bullhorn);
                    dbPlayer.disableAllPlayerActions(true);
                    dbPlayer.PlayAnimation(33, "amb@world_human_welding@male@idle_a", "idle_a");
                    dbPlayer.SendProgressbar(Constants.Staatsbank.Staatsbankopentime);
                    Constants.Staatsbank.Broken = true;

                    NAPI.Task.Run(() =>
                    {
                        if (player.Dead)
                        {
                            Constants.Staatsbank.Broken = false;
                            return;
                        }
                        dbPlayer.disableAllPlayerActions(false);
                        dbPlayer.StopAnimation();
                        dbPlayer.SendNotification("Erfolgreich aufgebrochen", 3000, "green", "Staatsbank");
                        Constants.Staatsbank.Opened = true;
                        Constants.Staatsbank.LastOpened = DateTime.Now.AddMinutes(5);

                        MySqlQuery mySqlQuery2 = new MySqlQuery("UPDATE staatsbank SET Storage = @storage WHERE Id = 1");
                        mySqlQuery2.AddParameter("@storage", fillstaatsbank());
                        MySqlHandler.ExecuteSync(mySqlQuery2);

                        MySqlQuery mySqlQuery3 = new MySqlQuery("UPDATE staatsbank SET Storage = @storage WHERE Id = 2");
                        mySqlQuery3.AddParameter("@storage", fillstaatsbank());
                        MySqlHandler.ExecuteSync(mySqlQuery3);

                        MySqlQuery mySqlQuery4 = new MySqlQuery("UPDATE staatsbank SET Storage = @storage WHERE Id = 3");
                        mySqlQuery4.AddParameter("@storage", fillstaatsbank());
                        MySqlHandler.ExecuteSync(mySqlQuery4);

                        MySqlQuery mySqlQuery5 = new MySqlQuery("UPDATE staatsbank SET Storage = @storage WHERE Id = 4");
                        mySqlQuery5.AddParameter("@storage", fillstaatsbank());
                        MySqlHandler.ExecuteSync(mySqlQuery5);

                        MySqlQuery mySqlQuery6 = new MySqlQuery("UPDATE staatsbank SET Storage = @storage WHERE Id = 5");
                        mySqlQuery6.AddParameter("@storage", fillstaatsbank());
                        MySqlHandler.ExecuteSync(mySqlQuery6);

                        NAPI.Task.Run(() =>
                        {
                            endstaatsbank();
                        }, Constants.Staatsbank.Staatsbankclosetime);
                    }, Constants.Staatsbank.Staatsbankopentime);
                    reader.Dispose();

                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }
    }
}
