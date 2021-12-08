using GTANetworkAPI;
using GVMP.Handlers;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using GVMP;

namespace Crimelife.Module.MAZ
{
    class MazModule : Crimelife.Module.Module<MazModule>
    {
        public static List<Maz> mazs = new List<Maz>();

        public static string fillmaz()
        {
            try
            {
                int random = new Random().Next(30, 60);
                JObject admin = new JObject(
                    new JProperty("Id", (510)),
                     new JProperty("Slot", (1)),
                      new JProperty("Weight", (0)),
                       new JProperty("Name", ("Waffenkiste")),
                        new JProperty("ImagePath", ("Waffenkiste.png")),
                         new JProperty("Amount", (random))
                    );
                return "[" + JsonConvert.SerializeObject(admin) + "]";
            }
            catch (Exception)
            {

                throw;
            }
        }

        public static void SpawnMAZ()
        {
            try
            {
                int randomint = new Random().Next(1, 4);
                MySqlQuery mySqlQuery = new MySqlQuery("SELECT * FROM maz WHERE id = @id");
                mySqlQuery.AddParameter("@id", randomint);
                MySqlResult mySqlResult = MySqlHandler.GetQuery(mySqlQuery);
                MySqlQuery mySqlQuery2 = new MySqlQuery("UPDATE maz SET Open = 0, Broke = 0, Storage = @storage, LastSpawn = @spawndate WHERE id = @id");
                mySqlQuery2.AddParameter("@id", randomint);
                mySqlQuery2.AddParameter("@storage", fillmaz());
                mySqlQuery2.AddParameter("@spawndate", DateTime.Now);
                MySqlHandler.ExecuteSync(mySqlQuery2);

                MySqlHandler.ExecuteSync(mySqlQuery);
                try
                {
                    MySqlDataReader reader = mySqlResult.Reader;
                    try
                    {
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                loadmaz(reader);
                            }
                        }
                    }
                    finally
                    {
                        reader.Dispose();
                    }
                }
                catch (Exception ex)
                {
                    Logger.Print("[EXCEPTION loadGarages] " + ex.Message);
                    Logger.Print("[EXCEPTION loadGarages] " + ex.StackTrace);
                }
                finally
                {
                    mySqlResult.Connection.Dispose();
                }

                return;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public static void loadmaz(MySqlDataReader reader)
        {
            try
            {
                Constants.MAZ.MazSpawn = true;
                Maz maz = new Maz
                {
                    Id = reader.GetInt32("id"),
                    Spawn = NAPI.Util.FromJson<Vector3>(reader.GetString("Spawn")),
                    Storage = reader.GetString("Storage"),
                    LastSpawn = (DateTime)reader.GetMySqlDateTime("LastSpawn"),
                    Open = reader.GetInt32("Open")
                };
                mazs.Add(maz);

                NAPI.Task.Run(() =>
                {
                    ColShape val = NAPI.ColShape.CreateCylinderColShape(maz.Spawn, 1.4f, 1.4f, 0);
                    val.SetData("FUNCTION_MODEL", new FunctionModel("openmaz", maz.Id, maz.Id));
                    val.SetData("MESSAGE", new Message("Benutze E um das MAZ aufzubrechen", "MAZ", "orange", 5000));
                    val.SetData("mazcol", true);
                });

                Vector3 flugzeug = NAPI.Util.FromJson<Vector3>(reader.GetString("Spawn2"));
                Vector3 box = NAPI.Util.FromJson<Vector3>(reader.GetString("Spawn"));

                NAPI.Task.Run(() =>
                {
                    GTANetworkAPI.Object flugzeugprop = NAPI.Object.CreateObject(249853152, flugzeug, new Vector3(0, 0, 180.2123), 255, 0);
                    GTANetworkAPI.Object boxprop = NAPI.Object.CreateObject(897366637, box, new Vector3(0, 0, 180.2123), 255, 0);
                    flugzeugprop.SetData("mazprop", true);
                    boxprop.SetData("mazprop", true);
                });

                DbPlayer dbPlayer = null;
                Notification.SendGlobalNotification("Zentrales Flugabwehrsystem: Es wurde der Absturz einer feindlichen Militärmaschine im Hoheitsgebiet Los Santos gemeldet!", 7500, "lightblue", Notification.icon.bullhorn);

                NAPI.Task.Run(() =>
                {
                    Marker mark = NAPI.Marker.CreateMarker(1, maz.Spawn, new Vector3(), new Vector3(), 1.0f, new Color(255, 140, 0), false, 0);
                    mark.SetData("mazmark", true);
                });
            }
            catch (Exception ex)
            {
                Logger.Print("[EXCEPTION openGarage] " + ex.Message);
                Logger.Print("[EXCEPTION openGarage] " + ex.StackTrace);
            }
        }

        public static Vector3 sendmazcord()
        {
            try
            {
                MySqlQuery mySqlQuery2 = new MySqlQuery("SELECT * FROM maz WHERE Open = 1");
                if (mySqlQuery2.ToString() == null || mySqlQuery2.ToString().Length < 5) return new Vector3(0, 0, 0);
                MySqlResult mySqlReaderCon = MySqlHandler.GetQuery(mySqlQuery2);
                if (mySqlReaderCon.ToString() == null) return new Vector3(0, 0, 0);
                MySqlDataReader reader = mySqlReaderCon.Reader;
                if (!reader.HasRows) return new Vector3(0, 0, 0);

                reader.Read();

                Vector3 vector = NAPI.Util.FromJson<Vector3>(reader.GetString("Spawn"));

                reader.Dispose();
                return vector;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            return new Vector3(0, 0, 0);
        }

        [RemoteEvent("openmaz")]
        public void openmaz(Player c, int id, int mazid)
        {
            try
            {
                if (c == null) return;
                Constants.MAZ.mazid = mazid;
                DbPlayer dbplayer = c.GetPlayer();

                MySqlQuery mySqlQuery2 = new MySqlQuery("SELECT * FROM inventorys WHERE Id = @userId LIMIT 1");
                mySqlQuery2.AddParameter("@userId", dbplayer.Id);
                MySqlResult mySqlReaderCon = MySqlHandler.GetQuery(mySqlQuery2);
                MySqlDataReader reader = mySqlReaderCon.Reader;

                reader.Read();
                string items = reader.GetString("Items");

                if (!items.Contains("Schweissgeraet"))
                {
                    dbplayer.SendNotification("Du besitzt nicht das nötige Item um das MAZ zu öffnen", 3000, "orange", "MAZ");
                    return;
                }

                MySqlQuery mySqlQuery = new MySqlQuery("SELECT * FROM maz WHERE id = @id");
                mySqlQuery.AddParameter("@id", mazid);
                MySqlResult mySqlResult = MySqlHandler.GetQuery(mySqlQuery);
                Console.WriteLine(mySqlResult.Reader.Read().ToString()); // Muss stehen bleiben sonst Fehlermeldung ... Frag nicht
                DbPlayer dbPlayer = c.GetPlayer();
                if (dbPlayer == null || !dbPlayer.IsValid(true) || dbPlayer.Client == null) return;

                if (mySqlResult.Reader.GetInt32("Open") == 1) return;
                if (mySqlResult.Reader.GetInt32("Broke") == 1) return;

                Notification.SendGlobalNotification($"Army Nachricht: Das abgestürzte Militärflugzeug auf dem {mySqlResult.Reader.GetString("Ort")} (+500m) ist ab sofort eine absolute Sperrzone! Betreten des Bereiches wird mit Beschuss geahndet!", 7500, "lightblue", Notification.icon.bullhorn);

                MySqlQuery mySqlQuery4 = new MySqlQuery("UPDATE maz SET Broke = 1 WHERE id = @id");
                mySqlQuery4.AddParameter("@id", mazid);
                MySqlHandler.ExecuteSync(mySqlQuery4);

                Vector3 mazspawn = NAPI.Util.FromJson<Vector3>(mySqlResult.Reader.GetString("Spawn"));

                if (id == 0 || mazid == 0) return;
                dbPlayer.disableAllPlayerActions(true);
                dbPlayer.PlayAnimation(33, "amb@world_human_welding@male@idle_a", "idle_a");

                dbPlayer.SendProgressbar(Constants.MAZ.mazopentime);
                NAPI.Task.Run(() =>
                {
                    if (c.Dead || c.Position.DistanceTo2D(mazspawn) > 10)
                    {
                        MySqlQuery mySqlQuery3 = new MySqlQuery("UPDATE maz SET Broke = 0 WHERE id = @id");
                        mySqlQuery3.AddParameter("@id", mazid);
                        MySqlHandler.ExecuteSync(mySqlQuery3);
                        return;
                    }

                    dbPlayer.SendNotification("Kiste erfolgreich geöffnet", 3000, "orange", "MAZ");
                    MySqlQuery mySqlQuery = new MySqlQuery("UPDATE maz SET Open = 1 WHERE id = @id");
                    mySqlQuery.AddParameter("@id", mazid);
                    MySqlHandler.ExecuteSync(mySqlQuery);

                    dbplayer.StopAnimation();
                    dbPlayer.disableAllPlayerActions(false);

                    NAPI.Task.Run(() =>
                    {
                        MySqlQuery mySqlQuery = new MySqlQuery("UPDATE maz SET Open = 0 WHERE id = @id");
                        mySqlQuery.AddParameter("@id", mazid);
                        MySqlHandler.ExecuteSync(mySqlQuery);

                        foreach (var x in NAPI.Pools.GetAllMarkers().ToList().Where(x => x.HasData("mazmark")))
                        {
                            x.Delete();
                        }

                        foreach (var x in NAPI.Pools.GetAllColShapes().ToList().Where(x => x.HasData("mazcol")))
                        {
                            x.Delete();
                        }

                        foreach (var x in NAPI.Pools.GetAllObjects().ToList().Where(x => x.HasData("mazprop")))
                        {
                            x.Delete();
                        }
                        Constants.MAZ.MazSpawn = false;
                    }, Constants.MAZ.mazdespawntime);
                }, Constants.MAZ.mazopentime);

            }
            catch (Exception ex)
            {
                Logger.Print("[EXCEPTION openMAZ] " + ex.Message);
                Logger.Print("[EXCEPTION openMAZ] " + ex.StackTrace);
            }

        }
    }
}
