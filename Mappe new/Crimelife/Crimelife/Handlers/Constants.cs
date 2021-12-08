using GTANetworkAPI;
using System;

namespace GVMP.Handlers
{
    public static class Constants
    {
        public static class MAZ
        {
            public static bool Mazopen = false;
            public static int mazid = 0;
            public static int mazopentime = 300000;
            public static int mazdespawntime = 1200000;
            public static bool MazSpawn = false;
        }

        public static class Timer
        {
            public static int TimerMin = 1;
        }

        public static class Fraktionslager
        {
            public static int Fraktionslagertime = 300000;
            public static int Fraktionslagerclosetime = 1200000;
        }

        public static class Shop
        {
            public static int ShopopenTime = 300000;
            public static bool IsShopRobed = false;
        }

        public static class Staatsbank
        {
            public static DateTime LastOpened = DateTime.Now;
            public static int Staatsbankclosetime = 1200000;
            public static bool Opened = false;
            public static int Staatsbankopentime = 120000;
            public static int Staatsschranktime = 60000;
            public static bool Broken = false;
            public static bool Schrank1Open = false;
            public static bool Schrank2Open = false;
            public static bool Schrank3Open = false;
            public static bool Schrank4Open = false;
            public static bool Schrank5Open = false;

            public static bool Schrank1Broken = false;
            public static bool Schrank2Broken = false;
            public static bool Schrank3Broken = false;
            public static bool Schrank4Broken = false;
            public static bool Schrank5Broken = false;

            public static Vector3 Schrank1Pos = new Vector3(258.36, 214.62, 101.68);
            public static Vector3 Schrank2Pos = new Vector3(259.41, 217.77, 101.68);
            public static Vector3 Schrank3Pos = new Vector3(264.6, 215.85, 101.68);
            public static Vector3 Schrank4Pos = new Vector3(265.98, 213.63, 101.68);
            public static Vector3 Schrank5Pos = new Vector3(263.65, 212.49, 101.68);
        }

        public static bool Staatsbankschrankopen(int Schrank)
        {
            try
            {
                switch (Schrank)
                {
                    case 1:
                        return Constants.Staatsbank.Schrank1Open;
                    case 2:
                        return Constants.Staatsbank.Schrank2Open;
                    case 3:
                        return Constants.Staatsbank.Schrank3Open;
                    case 4:
                        return Constants.Staatsbank.Schrank4Open;
                    case 5:
                        return Constants.Staatsbank.Schrank5Open;
                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            return false;
        }

        public static bool Staatsbankschrankbroken(int Schrank)
        {
            try
            {
                switch (Schrank)
                {
                    case 1:
                        return Constants.Staatsbank.Schrank1Broken;
                    case 2:
                        return Constants.Staatsbank.Schrank2Broken;
                    case 3:
                        return Constants.Staatsbank.Schrank3Broken;
                    case 4:
                        return Constants.Staatsbank.Schrank4Broken;
                    case 5:
                        return Constants.Staatsbank.Schrank5Broken;
                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            return false;
        }

        public static Vector3 GetClosetSchrank(this Player player)
        {
            try
            {
                if (player.Position.DistanceTo2D(Constants.Staatsbank.Schrank1Pos) < 2f)
                {
                    return Constants.Staatsbank.Schrank1Pos;
                }
                else if (player.Position.DistanceTo2D(Constants.Staatsbank.Schrank2Pos) < 2f)
                {
                    return Constants.Staatsbank.Schrank2Pos;
                }
                else if (player.Position.DistanceTo2D(Constants.Staatsbank.Schrank3Pos) < 2f)
                {
                    return Constants.Staatsbank.Schrank3Pos;
                }
                else if (player.Position.DistanceTo2D(Constants.Staatsbank.Schrank4Pos) < 2f)
                {
                    return Constants.Staatsbank.Schrank4Pos;
                }
                else if (player.Position.DistanceTo2D(Constants.Staatsbank.Schrank5Pos) < 2f)
                {
                    return Constants.Staatsbank.Schrank5Pos;
                }
                else
                {
                    return new Vector3(0, 0, 0);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            return new Vector3(0, 0, 0);
        }


        public static int GetClosetSchrankID(this Player player)
        {
            try
            {
                if (player.Position.DistanceTo2D(Constants.Staatsbank.Schrank1Pos) < 2f)
                {
                    return 1;
                }
                else if (player.Position.DistanceTo2D(Constants.Staatsbank.Schrank2Pos) < 2f)
                {
                    return 2;
                }
                else if (player.Position.DistanceTo2D(Constants.Staatsbank.Schrank3Pos) < 2f)
                {
                    return 3;
                }
                else if (player.Position.DistanceTo2D(Constants.Staatsbank.Schrank4Pos) < 2f)
                {
                    return 4;
                }
                else if (player.Position.DistanceTo2D(Constants.Staatsbank.Schrank5Pos) < 2f)
                {
                    return 5;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            return 0;
        }


        public static void SetStaatsbankschrankopen(int Schrank)
        {
            try
            {
                switch (Schrank)
                {
                    case 1:
                        Constants.Staatsbank.Schrank1Open = true;
                        break;
                    case 2:
                        Constants.Staatsbank.Schrank2Open = true;
                        break;
                    case 3:
                        Constants.Staatsbank.Schrank3Open = true;
                        break;
                    case 4:
                        Constants.Staatsbank.Schrank4Open = true;
                        break;
                    case 5:
                        Constants.Staatsbank.Schrank5Open = true;
                        break;
                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        public static void SetStaatsbankschrankbroken(int Schrank)
        {
            try
            {
                switch (Schrank)
                {
                    case 1:
                        Constants.Staatsbank.Schrank1Broken = true;
                        break;
                    case 2:
                        Constants.Staatsbank.Schrank2Broken = true;
                        break;
                    case 3:
                        Constants.Staatsbank.Schrank3Broken = true;
                        break;
                    case 4:
                        Constants.Staatsbank.Schrank4Broken = true;
                        break;
                    case 5:
                        Constants.Staatsbank.Schrank5Broken = true;
                        break;
                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        public static void SetPlayerACFlag(Player player)
        {
            try
            {
                NAPI.Task.Run(() =>
                {
                    player.SetData("DisableAC", true);
                });

                NAPI.Task.Run(() =>
                {
                    player.SetData("DisableAC", false);
                }, 8000);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        public static void SetPlayerACFlagLogin(Player player)
        {
            try
            {
                NAPI.Task.Run(() =>
                {
                    player.SetData("DisableAC", true);
                });

                NAPI.Task.Run(() =>
                {
                    player.SetData("DisableAC", false);
                }, 20000);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        public static void SetPlayerACFlagLoginPerm(Player player)
        {
            try
            {
                NAPI.Task.Run(() =>
                {
                    player.SetData("DisableAC", true);
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        public static void SetStaatsbankschrankunbroken(int Schrank)
        {
            try
            {
                switch (Schrank)
                {
                    case 1:
                        Constants.Staatsbank.Schrank1Broken = false;
                        break;
                    case 2:
                        Constants.Staatsbank.Schrank2Broken = false;
                        break;
                    case 3:
                        Constants.Staatsbank.Schrank3Broken = false;
                        break;
                    case 4:
                        Constants.Staatsbank.Schrank4Broken = false;
                        break;
                    case 5:
                        Constants.Staatsbank.Schrank5Broken = false;
                        break;
                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }
    }
}
