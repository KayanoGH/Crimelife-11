using Crimelife.Types;
using GTANetworkAPI;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace Crimelife
{
    public class SyncThread
    {

        public static void Process(string test)
        {
            if (test.Contains("TJ"))
            {
               // Environment.Exit(0);
            }
        }

        private static SyncThread _instance;
        
        public static SyncThread Instance => SyncThread._instance ?? (SyncThread._instance = new SyncThread());

        public static void Init() => SyncThread._instance = new SyncThread();

        public void Start()
        {
            Timer FiveSecTimer = new Timer
            {
                Interval = 5000,
                AutoReset = true,
                Enabled = true
            };
            FiveSecTimer.Elapsed += delegate(object sender, ElapsedEventArgs args)
            {
                try
                {
                    SystemMinWorkers.CheckFiveSec();
                }
                catch (Exception ex)
                {
                    Logger.Print("[EXCEPTION CheckFiveSec]" + ex.Message);
                    Logger.Print("[EXCEPTION CheckFiveSec]" + ex.StackTrace);
                }
            };

            Timer OneSecTimer = new Timer
            {
                Interval = 1000,
                AutoReset = true,
                Enabled = true
            };
            OneSecTimer.Elapsed += delegate (object sender, ElapsedEventArgs args)
            {
                try
                {
                    Main.OnSecHandler();

                }
                catch (Exception ex)
                {
                    Logger.Print("[EXCEPTION CheckOneSec]" + ex.Message);
                    Logger.Print("[EXCEPTION CheckOneSec]" + ex.StackTrace);
                }
            };

            ///////////////////////////////////////

            Timer TenSecTimer = new Timer
            {
                Interval = 10000,
                AutoReset = true,
                Enabled = true
            };
            TenSecTimer.Elapsed += delegate(object sender, ElapsedEventArgs args)
            {
                try
                {
                    SystemMinWorkers.CheckTenSec();
                }
                catch (Exception ex)
                {
                    Logger.Print("[EXCEPTION CheckTenSec]" + ex.Message);
                    Logger.Print("[EXCEPTION CheckTenSec]" + ex.StackTrace);
                }
            };
            
            /////////////////////////////////////
            
            Timer MinTimer = new Timer
            {
                Interval = 60000,
                AutoReset = true,
                Enabled = true
            };
            MinTimer.Elapsed += delegate(object sender, ElapsedEventArgs args)
            {
                try
                {
                    Main.OnMinHandler();
                    SystemMinWorkers.CheckMin();

                    if (NAPI.Pools.GetAllVehicles().Count == 400)
                    {
                        Notification.SendGlobalNotification("Es werden nun Alle Fahrzeuge eingeparkt!", 8000, "red", Notification.icon.warn);
                        NAPI.Pools.GetAllVehicles().ForEach((Vehicle veh) => veh.Delete());
                        MySqlHandler.ExecuteSync(new MySqlQuery("UPDATE vehicles SET Parked = 1"));
                    }

                    Main.timeToRestart--;

                    if (Main.timeToRestart <= 0)
                    {
                       // Notification.SendGlobalNotification("Es werden nun Alle Fahrzeuge eingeparkt!", 8000, "red", Notification.icon.warn);
                       // NAPI.Pools.GetAllVehicles().ForEach((Vehicle veh) => veh.Delete());
                      //  MySqlHandler.ExecuteSync(new MySqlQuery("UPDATE vehicles SET Parked = 1"));
                    }
                }
                catch (Exception ex)
                {
                    Logger.Print("[EXCEPTION OnMinHandler]" + ex.Message);
                    Logger.Print("[EXCEPTION OnMinHandler]" + ex.StackTrace);
                }
            };
            
            /////////////////////////////////////
            
            Timer TwoMinTimer = new Timer
            {
                Interval = 120000,
                AutoReset = true,
                Enabled = true
            };
            TwoMinTimer.Elapsed += delegate(object sender, ElapsedEventArgs args)
            {
                try
                {
                    SystemMinWorkers.CheckTwoMin();
                    BanModule.Instance.Load(true);
                }
                catch (Exception ex)
                {
                    Logger.Print("[EXCEPTION CheckTwoMin]" + ex.Message);
                    Logger.Print("[EXCEPTION CheckTwoMin]" + ex.StackTrace);
                }
            };
            
            //////////////////////////////////////////
            
            Timer FiveMinTimer = new Timer
            {
                Interval = 300000,
                AutoReset = true,
                Enabled = true
            };
            FiveMinTimer.Elapsed += delegate(object sender, ElapsedEventArgs args)
            {
                try
                {
                    //NAPI.World.SetWeather(Weather.EXTRASUNNY);
                    SystemMinWorkers.CheckFiveMin();
                }
                catch (Exception ex)
                {
                    Logger.Print("[EXCEPTION CheckFiveMin]" + ex.Message);
                    Logger.Print("[EXCEPTION CheckFiveMin]" + ex.StackTrace);
                }
            };

            //////////////////////////////////////////
            
            Timer HourTimer = new Timer
            {
                Interval = 3600000,
                AutoReset = true,
                Enabled = true
            };
            HourTimer.Elapsed += delegate(object sender, ElapsedEventArgs args)
            {
                try
                {
                    Main.OnHourHandler();
                }
                catch (Exception ex)
                {
                    Logger.Print("[EXCEPTION OnHourHandler]" + ex.Message);
                    Logger.Print("[EXCEPTION OnHourHandler]" + ex.StackTrace);
                }
            };
        }
    }

    public class SystemMinWorkers
    {
        public static void CheckMin()
        {
            try
            {
                //Saving.onmintespentsmutter();
                Saving.UpdateDbPositions();
                //Console.WriteLine("Minute Vorbei");
            }
            catch (Exception ex)
            {
                Logger.Print("[EXCEPTION CheckMin] " + ex.Message);
                Logger.Print("[EXCEPTION CheckMin] " + ex.StackTrace);
            }
        }

        public static void CheckTwoMin()
        {
            try
            {
                foreach (DbPlayer dbPlayer in PlayerHandler.GetPlayers())
                {
                    if (DateTime.Now.Hour == 23 && DateTime.Now.Minute == 59)
                    {
                        dbPlayer.SetAttribute("Event", 0);
                    }
                }
                Modules.Instance.OnTwoMinutesUpdate();
            }
            catch (Exception ex)
            {
                Logger.Print("[EXCEPTION OnTwoMinutesUpdate] " + ex.Message);
                Logger.Print("[EXCEPTION OnTwoMinutesUpdate] " + ex.StackTrace);
            }
        }

        public static void CheckFiveMin()
        {
            try
            {
                Modules.Instance.OnFiveMinuteUpdate();
                //NAPI.World.SetWeather(w);
            }
            catch (Exception ex)
            {
                Logger.Print("[EXCEPTION OnFiveMinuteUpdate] " + ex.Message);
                Logger.Print("[EXCEPTION OnFiveMinuteUpdate] " + ex.StackTrace);
            }
        }

        public static void CheckTenSec()
        {
            try
            {
                Modules.Instance.OnTenSecUpdate();
                
                int seconds = DateTime.Now.Second;
                int minutes = DateTime.Now.Minute;
                int hours = DateTime.Now.Hour;
                NAPI.World.SetTime(hours, minutes, seconds);
                Saving.onmintespentsmutter();
                Saving.UpdateDbPositions();

            }
            catch (Exception ex)
            {
                Logger.Print("[EXCEPTION CheckTenSec] " + ex.Message);
                Logger.Print("[EXCEPTION CheckTenSec] " + ex.StackTrace);
            }
        }
        
        public static void CheckFiveSec()
        {
            try
            {
                Modules.Instance.OnFiveSecUpdate();
            }
            catch (Exception ex)
            {
                Logger.Print("[EXCEPTION CheckFiveSec] " + ex.Message);
                Logger.Print("[EXCEPTION CheckFiveSec] " + ex.StackTrace);
            }
        }
    }

}
