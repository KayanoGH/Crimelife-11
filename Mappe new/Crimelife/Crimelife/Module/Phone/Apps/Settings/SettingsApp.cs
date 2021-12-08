using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GTANetworkAPI;
using MySql.Data.MySqlClient;
using GVMP;

namespace Crimelife
{
    class SettingsApp : Crimelife.Module.Module<SettingsApp>
    {
        List<Wallpaper> wallpapers = new List<Wallpaper>()
        {
            new Wallpaper(1, "Park", "https://i.imgur.com/Kw2VuCY.jpg"),
            new Wallpaper(2, "LCN", "https://i.imgur.com/FvsOEm2.png"),
            new Wallpaper(3, "LOST", "https://i.imgur.com/JbY482X.png"),
            new Wallpaper(4, "LSPD", "https://i.imgur.com/TgQwuzE.png"),
            new Wallpaper(5, "Marabunta", "https://i.imgur.com/belPu9t.png"),
            new Wallpaper(6, "Midnight", "https://i.imgur.com/JVV9wMS.png"),
            new Wallpaper(7, "Pier", "https://i.imgur.com/GQQ40BV.jpg"),
            new Wallpaper(8, "Triaden", "https://i.imgur.com/kMU9B90.png"),
            new Wallpaper(9, "Vagos", "https://i.imgur.com/TYZgwX7.png"),
            new Wallpaper(10, "YakuZa", "https://i.imgur.com/5hoqvjH.png"),
            new Wallpaper(11, "Feuerlord", "https://i.ibb.co/g6Vfh3w/milakunis.gif"),
            new Wallpaper(12, "", "https://i.ibb.co/SJmX6tR/tenor2.gif"),
            new Wallpaper(13, "Bubblebutt", "https://i.ibb.co/8PrTgHf/bubblebutt.gif")
        };

        [RemoteEvent("requestWallpaperList")]
        public void requestWallpaperList(Player c)
        {
            try
            {
                if (c == null) return;
                c.TriggerEvent("componentServerEvent", "SettingsEditWallpaperApp", "responseWallpaperList",
                    NAPI.Util.ToJson(wallpapers));
            }
            catch (Exception ex)
            {
                Logger.Print("[EXCEPTION requestWallpaperList] " + ex.Message);
                Logger.Print("[EXCEPTION requestWallpaperList] " + ex.StackTrace);
            }
        }

        public static void checkUserSettingsTable(Player c)
        {
            try
            {
                if (c == null) return;
                DbPlayer dbPlayer = c.GetPlayer();
                if (dbPlayer == null || !dbPlayer.IsValid(true) || dbPlayer.Client == null)
                    return;

                MySqlQuery mySqlQuery = new MySqlQuery("SELECT * FROM phone_settings WHERE Id = @userid LIMIT 1");
                mySqlQuery.Parameters = new List<MySqlParameter>()
                {
                    new MySqlParameter("@userid", dbPlayer.Id)
                };
                MySqlResult mySqlReaderCon = MySqlHandler.GetQuery(mySqlQuery);
                try
                {
                    MySqlDataReader reader = mySqlReaderCon.Reader;
                    if (!reader.HasRows)
                    {
                        reader.Dispose();
                        mySqlQuery.Query = "INSERT INTO phone_settings (Id) VALUES (@userid)";
                        mySqlQuery.Parameters = new List<MySqlParameter>()
                        {
                            new MySqlParameter("@userid", dbPlayer.Id)
                        };
                        MySqlHandler.ExecuteSync(mySqlQuery);
                    }
                }
                catch (Exception ex)
                {
                    Logger.Print("[EXCEPTION checkUserSettingsTable] " + ex.Message);
                    Logger.Print("[EXCEPTION checkUserSettingsTable] " + ex.StackTrace);
                }
                finally
                {
                    mySqlReaderCon.Connection.Dispose();
                }
            }
            catch (Exception ex)
            {
                Logger.Print("[EXCEPTION checkUserSettingsTable] " + ex.Message);
                Logger.Print("[EXCEPTION checkUserSettingsTable] " + ex.StackTrace);
            }
        }

        public static bool isFlugmodus(Player c)
        {
            if (c == null) return false;
            DbPlayer dbPlayer = c.GetPlayer();
            if (dbPlayer == null || !dbPlayer.IsValid(true) || dbPlayer.Client == null)
                return false;

            checkUserSettingsTable(c);
            MySqlQuery mySqlQuery = new MySqlQuery("SELECT * FROM phone_settings WHERE Id = @userid LIMIT 1");
            mySqlQuery.Parameters = new List<MySqlParameter>()
            {
                new MySqlParameter("@userid", dbPlayer.Id)
            };
            MySqlResult mySqlReaderCon = MySqlHandler.GetQuery(mySqlQuery);
            try
            {
                mySqlQuery.Query = "SELECT * FROM phone_settings WHERE Id = @userid LIMIT 1";
                mySqlQuery.Parameters = new List<MySqlParameter>()
                {
                    new MySqlParameter("@userid", dbPlayer.Id)
                };
                MySqlHandler.ExecuteSync(mySqlQuery);
                MySqlDataReader reader = mySqlReaderCon.Reader;
                try
                {
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            return reader.GetInt32("Flugmodus") == 1;
                        }
                    }
                }
                finally
                {
                    mySqlReaderCon.Reader.Dispose();
                }
            }
            catch (Exception ex)
            {
                Logger.Print("[EXCEPTION isFlugmodus] " + ex.Message);
                Logger.Print("[EXCEPTION isFlugmodus] " + ex.StackTrace);
            }
            finally
            {
                mySqlReaderCon.Connection.Dispose();
            }
            return false;
        }

        public static bool blockCalls(Player c)
        {
            if (c == null) return false;
            DbPlayer dbPlayer = c.GetPlayer();
            if (dbPlayer == null || !dbPlayer.IsValid(true) || dbPlayer.Client == null)
                return false;

            checkUserSettingsTable(c);
            MySqlQuery mySqlQuery = new MySqlQuery("SELECT * FROM phone_settings WHERE Id = @userid LIMIT 1");
            mySqlQuery.Parameters = new List<MySqlParameter>()
            {
                new MySqlParameter("@userid", dbPlayer.Id)
            };
            MySqlResult mySqlReaderCon = MySqlHandler.GetQuery(mySqlQuery);
            try
            {
                mySqlQuery.Query = "SELECT * FROM phone_settings WHERE Id = @userid LIMIT 1";
                mySqlQuery.Parameters = new List<MySqlParameter>()
                {
                    new MySqlParameter("@userid", dbPlayer.Id)
                };
                MySqlHandler.ExecuteSync(mySqlQuery);
                MySqlDataReader reader = mySqlReaderCon.Reader;
                try
                {
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            return reader.GetInt32("blockCalls") == 1;
                        }
                    }
                }
                finally
                {
                    mySqlReaderCon.Reader.Dispose();
                }
            }
            catch (Exception ex)
            {
                Logger.Print("[EXCEPTION blockCalls] " + ex.Message);
                Logger.Print("[EXCEPTION blockCalls] " + ex.StackTrace);
            }
            finally
            {
                mySqlReaderCon.Connection.Dispose();
            }
            return false;
        }

        [RemoteEvent("requestPhoneWallpaper")]
        public void requestPhoneWallpaper(Player c)
        {
            try
            {
                if (c == null) return;
                DbPlayer dbPlayer = c.GetPlayer();
                if (dbPlayer == null || !dbPlayer.IsValid(true) || dbPlayer.Client == null)
                    return;

                checkUserSettingsTable(c);
                MySqlQuery mySqlQuery = new MySqlQuery("SELECT * FROM phone_settings WHERE Id = @userid LIMIT 1");
                mySqlQuery.AddParameter("@userid", dbPlayer.Id);
                MySqlResult mySqlReaderCon = MySqlHandler.GetQuery(mySqlQuery);
                try
                {
                    MySqlDataReader reader = mySqlReaderCon.Reader;
                    try
                    {
                        if (!reader.HasRows)
                        {
                            mySqlQuery.Parameters.Clear();
                            mySqlQuery.Query = "INSERT INTO phone_settings (Id) VALUES (@userid)";
                            mySqlQuery.AddParameter("@userid", dbPlayer.Id);
                            MySqlHandler.ExecuteSync(mySqlQuery);
                        }
                        else
                        {
                            reader.Read();
                            Wallpaper wallpaper =
                                wallpapers.FirstOrDefault((Wallpaper wall) => wall.Id == reader.GetInt32("Wallpaper"));

                            if (wallpaper != null)
                                c.TriggerEvent("componentServerEvent", "HomeApp", "responsePhoneWallpaper",
                                    wallpaper.File);
                        }
                    }
                    finally
                    {
                        reader.Dispose();
                    }
                }
                catch (Exception ex)
                {
                    Logger.Print("[EXCEPTION requestPhoneWallpaper] " + ex.Message);
                    Logger.Print("[EXCEPTION requestPhoneWallpaper] " + ex.StackTrace);
                }
                finally
                {
                    mySqlReaderCon.Connection.Dispose();
                }
            }
            catch (Exception ex)
            {
                Logger.Print("[EXCEPTION requestPhoneWallpaper] " + ex.Message);
                Logger.Print("[EXCEPTION requestPhoneWallpaper] " + ex.StackTrace);
            }

        }

        [RemoteEvent("saveWallpaper")]
        public void saveWallpaper(Player c, int id)
        {
            if (c == null) return;
            DbPlayer dbPlayer = c.GetPlayer();
            if (dbPlayer == null || !dbPlayer.IsValid(true) || dbPlayer.Client == null)
                return;

            checkUserSettingsTable(c);
            try
            {
                MySqlQuery mySqlQuery = new MySqlQuery("UPDATE phone_settings SET Wallpaper = @val WHERE Id = @userid");
                mySqlQuery.AddParameter("@userid", dbPlayer.Id);
                mySqlQuery.AddParameter("@val", id);
                MySqlHandler.ExecuteSync(mySqlQuery);
            }
            catch (Exception ex)
            {
                Logger.Print("[EXCEPTION saveWallpaper] " + ex.Message);
                Logger.Print("[EXCEPTION saveWallpaper] " + ex.StackTrace);
            }
        }

        [RemoteEvent("requestPhoneSettings")]
        public void requestPhoneSettings(Player c)
        {
            if (c == null) return;
            DbPlayer dbPlayer = c.GetPlayer();
            if (dbPlayer == null || !dbPlayer.IsValid(true) || dbPlayer.Client == null)
                return;

            MySqlQuery mySqlQuery = new MySqlQuery("SELECT * FROM phone_settings WHERE Id = @userid LIMIT 1");
            mySqlQuery.AddParameter("@userid", dbPlayer.Id);
            MySqlResult mySqlReaderCon = MySqlHandler.GetQuery(mySqlQuery);
            try
            {
                MySqlDataReader reader = mySqlReaderCon.Reader;
                try
                {
                    if (!reader.HasRows)
                    {
                        mySqlQuery.Query = "INSERT INTO phone_settings (Id) VALUES (@userid)";
                        mySqlQuery.Parameters = new List<MySqlParameter>()
                        {
                            new MySqlParameter("@userid", dbPlayer.Id)
                        };
                        MySqlHandler.ExecuteSync(mySqlQuery);
                    }
                    else
                    {
                        reader.Read();
                        string boolstring = "true";
                        if (reader.GetInt32("Flugmodus") == 0)
                            boolstring = "false";

                        string boolstring2 = "true";
                        if (reader.GetInt32("blockCalls") == 0)
                            boolstring2 = "false";


                        c.TriggerEvent("componentServerEvent", "SettingsApp", "responsePhoneSettings", boolstring, boolstring2, boolstring2);
                    }
                }
                finally
                {
                    reader.Dispose();
                }
            }
            catch (Exception ex)
            {
                Logger.Print("[EXCEPTION requestPhoneSettings] " + ex.Message);
                Logger.Print("[EXCEPTION requestPhoneSettings] " + ex.StackTrace);
            }
            finally
            {
                mySqlReaderCon.Connection.Dispose();
            }
        }

        [RemoteEvent("savePhoneSettings")]
        public void savePhoneSettings(Player c, bool flugmodusStatus, bool lautlosStatus, bool anrufAblehnen)
        {
            if (c == null) return;
            DbPlayer dbPlayer = c.GetPlayer();
            if (dbPlayer == null || !dbPlayer.IsValid(true) || dbPlayer.Client == null)
                return;

            checkUserSettingsTable(c);
            try
            {
                if (Convert.ToInt32(flugmodusStatus) == 1)
                {
                    dbPlayer.SetSharedData("FUNK_CHANNEL", 0);
                    dbPlayer.SetSharedData("FUNK_TALKING", false);
                    //Logger.Print("Funk Reset");
                }
                MySqlQuery mySqlQuery = new MySqlQuery("UPDATE phone_settings SET Flugmodus = @val, blockCalls = @val2 WHERE Id = @userid");
                mySqlQuery.Parameters = new List<MySqlParameter>()
                {
                    new MySqlParameter("@userid", dbPlayer.Id),
                    new MySqlParameter("@val", Convert.ToInt32(flugmodusStatus)),
                    new MySqlParameter("@val2", Convert.ToInt32(anrufAblehnen))
                };
                MySqlHandler.ExecuteSync(mySqlQuery);
            }
            catch (Exception ex)
            {
                Logger.Print("[EXCEPTION savePhoneSettings] " + ex.Message);
                Logger.Print("[EXCEPTION savePhoneSettings] " + ex.StackTrace);
            }
        }
    }
}
