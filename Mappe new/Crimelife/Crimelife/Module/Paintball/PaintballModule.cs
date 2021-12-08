using GTANetworkAPI;
using System;
using System.Collections.Generic;
using GVMP;

namespace Crimelife
{
    class PaintballModule : Crimelife.Module.Module<PaintballModule>
    {
        public static List<PaintballModel> Zones = new List<PaintballModel>();

        protected override bool OnLoad()
        {
            Zones.Add(new PaintballModel
            {
                Id = Zones.Count,
                Name = "Würfelpark",
                Spawns = new List<Vector3>
                {
                    new Vector3(170.8255, -915.5659, 30.69199),
                    new Vector3(211.5563, -944.5418, 30.68113),
                    new Vector3(241.9889, -886.0068, 30.48896),
                    new Vector3(159.5039, -969.2851, 30.09191)
                },
                MaxPlayer = 10,
                Weapons = new List<WeaponHash>()
                {
                    WeaponHash.Advancedrifle,
                    WeaponHash.Gusenberg,
                    WeaponHash.Heavypistol,
                    WeaponHash.Assaultrifle,
                    WeaponHash.Bullpuprifle
                }
            });

            Zones.Add(new PaintballModel
            {
                Id = Zones.Count,
                Name = "U.S. Army Ford Zancudo",
                Spawns = new List<Vector3>
                {
                    new Vector3(-2083.38, 3023.34, 32.81),
                    new Vector3(-2014.22, 3053.7, 32.81),
                    new Vector3(-2241.51, 3040.71, 32.82),
                    new Vector3(-2013.77, 2915.01, 32.17)
                },
                MaxPlayer = 10,
                Weapons = new List<WeaponHash>()
                {
                    WeaponHash.Marksmanrifle
                }
            });

            Zones.Add(new PaintballModel
            {
                Id = Zones.Count,
                Name = "Lost Camp",
                Spawns = new List<Vector3>
                {
                    new Vector3(78.37, 3706.25, 41.08),
                    new Vector3(52.22, 3682.64, 39.75),
                    new Vector3(43.88, 3723.44, 39.58),
                    new Vector3(89.15, 3728.03, 39.54)
                },
                MaxPlayer = 10,
                Weapons = new List<WeaponHash>()
                {
                    WeaponHash.Heavyshotgun
                }
            });

            Zones.Add(new PaintballModel
            {
                Id = Zones.Count,
                Name = "Burrito Camp",
                Spawns = new List<Vector3>
                {
                    new Vector3(2316.73, 2523.17, 46.67),
                    new Vector3(2323.24, 2588.42, 46.65),
                    new Vector3(2371.18, 2623.24, 46.66),
                    new Vector3(2350.78, 2550.69, 46.67),
                    new Vector3(2352.86, 2523.49, 47.69)
                },
                MaxPlayer = 10,
                Weapons = new List<WeaponHash>()
                {
                    WeaponHash.Advancedrifle,
                    WeaponHash.Gusenberg,
                    WeaponHash.Heavypistol,
                    WeaponHash.Assaultrifle,
                    WeaponHash.Bullpuprifle
                }
            });

            Zones.Add(new PaintballModel
            {
                Id = Zones.Count,
                Name = "Bratwa Dorf",
                Spawns = new List<Vector3>
                {
                    new Vector3(-1124.67, 4947.55, 220.1),
                    new Vector3(-1158.27, 4923.96, 222.46),
                    new Vector3(-1106.51, 4891.97, 215.48),
                    new Vector3(-1081.32, 4913.33, 214.15)
                },
                MaxPlayer = 10,
                Weapons = new List<WeaponHash>()
                {
                    WeaponHash.Revolver
                }
            });

            Zones.Add(new PaintballModel
            {
                Id = Zones.Count,
                Name = "LS Supply",
                Spawns = new List<Vector3>
                {
                    new Vector3(1216.89, -1270.16, 35.37),
                    new Vector3(1188.77, -1296.84, 34.92),
                    new Vector3(1214.44, -1364.21, 35.23),
                    new Vector3(1180.58, -1412.93, 34.86),
                    new Vector3(1137.9, -1358.51, 34.59),
                    new Vector3(1151.74, -1326.77, 34.69)
                },
                MaxPlayer = 10,
                Weapons = new List<WeaponHash>()
                {
                    WeaponHash.Advancedrifle,
                    WeaponHash.Gusenberg,
                    WeaponHash.Heavypistol,
                    WeaponHash.Assaultrifle,
                    WeaponHash.Bullpuprifle
                }
            }); 

            Zones.Add(new PaintballModel
            {
                Id = Zones.Count,
                Name = "Windräder",
                Spawns = new List<Vector3>
                {
                    new Vector3(2189.88, 1809.76, 107.14),
                    new Vector3(2084.8, 1825.02, 99.11),
                    new Vector3(2181.8, 1938.61, 98.81),
                    new Vector3(2095.29, 1957.99, 90.41)       
                },
                MaxPlayer = 10,
                Weapons = new List<WeaponHash>()
                {
                    WeaponHash.Advancedrifle,
                    WeaponHash.Gusenberg,
                    WeaponHash.Heavypistol,
                    WeaponHash.Assaultrifle,
                    WeaponHash.Bullpuprifle
                }
            });

            NAPI.Blip.CreateBlip(432, new Vector3(73.08, -1027.24, 27.48), 1.0f, 0, "Paintball", 255, 0, true, 0, 0);
            NAPI.Marker.CreateMarker(1, new Vector3(73.08, -1027.24, 27.48), new Vector3(), new Vector3(), 1.0f, new Color(255, 140, 0), false, 0);

            ColShape cb = NAPI.ColShape.CreateCylinderColShape(new Vector3(73.08, -1027.24, 29), 1.4f, 1.4f, 0);
            cb.SetData("FUNCTION_MODEL", new FunctionModel("Paintball-Menu"));
            cb.SetData("MESSAGE", new Message("Benutze E um Painball zu spielen.", "PAINTBALL", "orange", 3000));

            return true;
        }

        [RemoteEvent("Paintball-Menu")]
        public void PaintballMenu(Player c)
        {
            try
            {
                if (c == null) return;
                DbPlayer dbPlayer = c.GetPlayer();
                if (dbPlayer == null || !dbPlayer.IsValid(true) || dbPlayer.Client == null) return;

                List<NativeItem> nativeItems = new List<NativeItem>();
                foreach (var t in Zones)
                {
                    nativeItems.Add(new NativeItem(t.Name + " (" + t.Players().Count + " / 10)", t.Name));
                }

                NativeMenu nativeMenu = new NativeMenu("Paintball", "", nativeItems);
                dbPlayer.ShowNativeMenu(nativeMenu);
            }
            catch (Exception ex)
            {
                Logger.Print("[EXCEPTION Paintball-Menu] " + ex.Message);
                Logger.Print("[EXCEPTION Paintball-Menu] " + ex.StackTrace);
            }
        }

        [RemoteEvent("nM-Paintball")]
        public void PaintballEnter(Player c, string value)
        {
            try
            {
                if (c == null) return;
                DbPlayer dbPlayer = c.GetPlayer();
                if (dbPlayer == null || !dbPlayer.IsValid(true) || dbPlayer.Client == null) return;

                /*   foreach (var t in Zones)
                   {
                   if (t.Name == "Würfelpark" && t.Players().Count >= 10)
                   {

                           dbPlayer.SendNotification("Die Lobby Würfelpark ist bereits voll!", 3000, "red");
                           return;
                   }
                   }*/
                dbPlayer.CloseNativeMenu();

                PaintballModel zone = null;
                foreach (var t in Zones)
                    if (t.Name == value)
                        zone = t;
                if (zone != null)
                {
                    if (zone.Players().Count >= zone.MaxPlayer)
                    {

                        dbPlayer.SendNotification("Die Lobby ist bereits voll!", 3000, "red");
                        return;
                    }
                    dbPlayer.RemoveAllWeapons();
                    Random r = new Random();
                    c.Dimension = Convert.ToUInt32(22750 + zone.Id);
                    c.Position = zone.Spawns[r.Next(0, zone.Spawns.Count)];
                    dbPlayer.SetData("PBZone", zone);
                    dbPlayer.SetData("PBKills", 0);
                    dbPlayer.SetData("PBDeaths", 0);
                    dbPlayer.SetData("PBZoneplayer", 0);

                    dbPlayer.SetArmor(100);

                    dbPlayer.initializePaintball();

                    foreach (WeaponHash weaponHash in zone.Weapons)
                    {
                        dbPlayer.GiveWeapon(weaponHash, 9999);
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Print("[EXCEPTION nM-Paintball] " + ex.Message);
                Logger.Print("[EXCEPTION nM-Paintball] " + ex.StackTrace);
            }
        }

        public static void leavePaintball(Player c)
        {
            try
            {
                if (c == null || !c.Exists) return;
                DbPlayer dbPlayer = c.GetPlayer();
                if (dbPlayer == null || !dbPlayer.IsValid(true) || dbPlayer.Client == null)
                    return;

                PaintballModel paintballModel = dbPlayer.GetPBData("PBZone");
                if (paintballModel == null) return;

                dbPlayer.ACWait();
                dbPlayer.SetPosition(new Vector3(73.08, -1027.24, 29));

                dbPlayer.SetData("PBZone", null);
                dbPlayer.SetData("PBKills", 0);
                dbPlayer.SetData("PBDeaths", 0);

                dbPlayer.SetArmor(0);

                dbPlayer.finishPaintball();

                dbPlayer.Client.RemoveAllWeapons();

                NAPI.Task.Run(() =>
                {
                    WeaponManager.loadWeapons(c);
                    dbPlayer.SetDimension(0);
                }, 5000);
            }
            catch (Exception ex)
            {
                Logger.Print("[EXCEPTION leavePaintball] " + ex.Message);
                Logger.Print("[EXCEPTION leavePaintball] " + ex.StackTrace);
            }
        }

        public static void PaintballDeath(DbPlayer dbPlayer, DbPlayer dbPlayer2)
        {
            try
            {
                if (dbPlayer == null || dbPlayer2 == null) return;

                Random r = new Random();

                if (!dbPlayer.HasData("PBZone") || !dbPlayer.HasData("PBDeaths") || !dbPlayer.HasData("PBKills") || dbPlayer.GetPBData("PBZone") == null && dbPlayer.GetIntData("PBDeaths") == null || dbPlayer.GetIntData("PBKills") == null) return;

                PaintballModel paintballModel = dbPlayer.GetPBData("PBZone");
                int newdeaths = 1;
                if (!dbPlayer.HasData("PBDeaths") || dbPlayer.GetIntData("PBDeaths") == null)
                {
                    dbPlayer.SetData("PBDeaths", 1);
                }
                else
                {
                    newdeaths = dbPlayer.GetIntData("PBDeaths");
                    newdeaths += 1;
                }

                dbPlayer.SetData("PBKillstreak", 0);
                dbPlayer.SetData("PBDeaths", newdeaths);
                dbPlayer.SpawnPlayer(paintballModel.Spawns[r.Next(0, paintballModel.Spawns.Count)]);

                int newkills = dbPlayer2.GetIntData("PBKills");
                int killstreak = 0;

                if (dbPlayer2.HasData("PBKillstreak") && dbPlayer2.GetIntData("PBKillstreak") != null && dbPlayer2.GetIntData("PBKillstreak") is int)
                    killstreak = dbPlayer2.GetIntData("PBKillstreak");

                killstreak += 1;
                newkills += 1;

                if (killstreak == 3)
                {
                    paintballModel.Players().ForEach(p => Notification.SendGlobalNotification2(p, "Bei " + dbPlayer2.Name + " läuft!", 5000, "white", Notification.icon.bullhorn));
                }

                else if (killstreak == 5)
                {
                    paintballModel.Players().ForEach(p => Notification.SendGlobalNotification2(p, dbPlayer2.Name + " scheppert richtig!", 5000, "white", Notification.icon.bullhorn));
                }

                else if (killstreak == 10)
                {
                    paintballModel.Players().ForEach(p => Notification.SendGlobalNotification2(p, dbPlayer2.Name + " ist GODLIKE!", 5000, "white", Notification.icon.bullhorn));
                }

                dbPlayer2.SetData("PBKills", newkills);
                dbPlayer2.SetData("PBKillstreak", killstreak);

                foreach (WeaponHash weaponHash in paintballModel.Weapons)
                {
                    dbPlayer.GiveWeapon(weaponHash, 9999);
                }

                dbPlayer.updatePaintballScore((int)dbPlayer.GetIntData("PBKills"), (int)dbPlayer.GetIntData("PBDeaths"));
                dbPlayer2.updatePaintballScore((int)dbPlayer2.GetIntData("PBKills"), (int)dbPlayer2.GetIntData("PBDeaths"));

                dbPlayer.SendNotification("Du hast noch " + (10 - (int)dbPlayer2.GetIntData("PBDeaths")) + " Leben!", 3000, "#2f2f30");

                if (dbPlayer.GetIntData("PBDeaths") >= 10) leavePaintball(dbPlayer.Client);

                dbPlayer.StopAnimation();
                dbPlayer.SetInvincible(false);
                dbPlayer.SetArmor(100);
                dbPlayer.disableAllPlayerActions(false);
            }
            catch (Exception ex)
            {
                Logger.Print("[EXCEPTION PaintballDeath] " + ex.Message);
                Logger.Print("[EXCEPTION PaintballDeath] " + ex.StackTrace);
            }
        }

        public static void PaintballDeath2(DbPlayer dbPlayer)
        {
            try
            {
                if (dbPlayer == null) return;

                Random r = new Random();

                if (!dbPlayer.HasData("PBZone") || !dbPlayer.HasData("PBDeaths") || !dbPlayer.HasData("PBKills") || dbPlayer.GetPBData("PBZone") == null && dbPlayer.GetIntData("PBDeaths") == null || dbPlayer.GetIntData("PBKills") == null) return;

                PaintballModel paintballModel = dbPlayer.GetPBData("PBZone");
                int newdeaths = 1;
                if (!dbPlayer.HasData("PBDeaths") || dbPlayer.GetIntData("PBDeaths") == null)
                {
                    dbPlayer.SetData("PBDeaths", 1);
                }
                else
                {
                    newdeaths = dbPlayer.GetIntData("PBDeaths");
                    newdeaths += 1;
                }

                dbPlayer.SetData("PBKillstreak", 0);
                dbPlayer.SetData("PBDeaths", newdeaths);
                dbPlayer.SpawnPlayer(paintballModel.Spawns[r.Next(0, paintballModel.Spawns.Count)]);


                foreach (WeaponHash weaponHash in paintballModel.Weapons)
                {
                    dbPlayer.GiveWeapon(weaponHash, 9999);
                }

                dbPlayer.updatePaintballScore((int)dbPlayer.GetIntData("PBKills"), (int)dbPlayer.GetIntData("PBDeaths"));

                dbPlayer.SendNotification("Du hast noch " + (10 - (int)dbPlayer.GetIntData("PBDeaths")) + " Leben!", 3000, "#2f2f30");

                if (dbPlayer.GetIntData("PBDeaths") >= 10) leavePaintball(dbPlayer.Client);

                dbPlayer.StopAnimation();
                dbPlayer.SetInvincible(false);
                dbPlayer.SetArmor(100);
                dbPlayer.disableAllPlayerActions(false);
            }
            catch (Exception ex)
            {
                Logger.Print("[EXCEPTION PaintballDeath] " + ex.Message);
                Logger.Print("[EXCEPTION PaintballDeath] " + ex.StackTrace);
            }
        }
    }
}



/*using GTANetworkAPI;
using System;
using System.Collections.Generic;
using System.Text;

namespace GVMP
{
    class PaintballModule : GVMP.Module.Module<PaintballModule>
    {
        public static List<PaintballModel> Zones = new List<PaintballModel>();
        public static List<WeaponHash> PaintballWeapons = new List<WeaponHash>();
        public static List<WeaponHash> PaintballWeapons2 = new List<WeaponHash>();
        public static List<WeaponHash> PaintballWeapons3 = new List<WeaponHash>();

        protected override bool OnLoad()
        {
            PaintballWeapons.Add(WeaponHash.AdvancedRifle);
            PaintballWeapons.Add(WeaponHash.Gusenberg);
            PaintballWeapons.Add(WeaponHash.HeavyPistol);
            PaintballWeapons.Add(WeaponHash.AssaultRifle);
            PaintballWeapons.Add(WeaponHash.BullpupRifle);

            PaintballWeapons2.Add(WeaponHash.MarksmanRifle);

            PaintballWeapons3.Add(WeaponHash.SpecialCarbine);

            Zones.Add(new PaintballModel
            {
                Id = Zones.Count,
                Name = "Würfelpark",
                Spawns = new List<Vector3>
                {
                    new Vector3(170.8255, -915.5659, 30.69199),
                    new Vector3(211.5563, -944.5418, 30.68113),
                    new Vector3(241.9889, -886.0068, 30.48896),
                    new Vector3(159.5039, -969.2851, 30.09191)
                },
                MaxPlayer = 10
            });

            Zones.Add(new PaintballModel
            {
                Id = Zones.Count,
                Name = "Bratwa Dorf",
                Spawns = new List<Vector3>
                {
                    new Vector3(-1124.67, 4947.55, 220.1),
                    new Vector3(-1158.27, 4923.96, 222.46),
                    new Vector3(-1106.51, 4891.97, 215.48),
                    new Vector3(-1081.32, 4913.33, 214.15)
                },
                MaxPlayer = 10
            });

            Zones.Add(new PaintballModel
            {
                Id = Zones.Count,
                Name = "LS Supply",
                Spawns = new List<Vector3>
                {
                    new Vector3(1216.89, -1270.16, 35.37),
                    new Vector3(1188.77, -1296.84, 34.92),
                    new Vector3(1214.44, -1364.21, 35.23),
                    new Vector3(1180.58, -1412.93, 34.86),
                    new Vector3(1137.9, -1358.51, 34.59),
                    new Vector3(1151.74, -1326.77, 34.69)
                },
                MaxPlayer = 10
            });

            NAPI.Blip.CreateBlip(432, new Vector3(570.46, 2796.68, 41.01), 1.0f, 0, "Paintball", 255, 0, true, 0, 0);
            NAPI.Marker.CreateMarker(1, new Vector3(570.46, 2796.68, 41.01), new Vector3(), new Vector3(), 1.0f, new Color(255, 140, 0), false, 0);

            ColShape cb = NAPI.ColShape.CreateCylinderColShape(new Vector3(570.46, 2796.68, 41.01), 1.4f, 1.4f, 0);
            cb.SetData("FUNCTION_MODEL", new FunctionModel("Paintball-Menu"));
            cb.SetData("MESSAGE", new Message("Benutze E um Painball zu spielen.", "PAINTBALL", "orange", 3000));

            return true;
        }

        [RemoteEvent("Paintball-Menu")]
        public void PaintballMenu(Client c)
        {
            try
            {
                if (c == null) return;
                DbPlayer dbPlayer = c.GetPlayer();
                if (dbPlayer == null || !dbPlayer.IsValid(true) || dbPlayer.Client == null) return;

                List<NativeItem> nativeItems = new List<NativeItem>();
                foreach (var t in Zones)
                {
                    nativeItems.Add(new NativeItem(t.Name, t.Name));
                }

                NativeMenu nativeMenu = new NativeMenu("Paintball", "", nativeItems);
                dbPlayer.ShowNativeMenu(nativeMenu);
            }
            catch (Exception ex)
            {
                Logger.Print("[EXCEPTION Paintball-Menu] " + ex.Message);
                Logger.Print("[EXCEPTION Paintball-Menu] " + ex.StackTrace);
            }
        }

        [RemoteEvent("nM-Paintball")]
        public void PaintballEnter(Client c, string value)
        {
            try
            {
                if (c == null) return;
                DbPlayer dbPlayer = c.GetPlayer();
                if (dbPlayer == null || !dbPlayer.IsValid(true) || dbPlayer.Client == null) return;

                PaintballModel paintballModel = dbPlayer.GetData("PBZone");
                if (paintballModel == null) return;

                dbPlayer.CloseNativeMenu();

                PaintballModel zone = null;
                foreach (var t in Zones)
                    if (t.Name == value)
                        zone = t;
                if (zone != null)
                {
                    Random r = new Random();
                    c.Dimension = Convert.ToUInt32(22750 + zone.Id);
                    c.Position = zone.Spawns[r.Next(0, zone.Spawns.Count)];
                    dbPlayer.SetData("PBZone", zone);
                    dbPlayer.SetData("PBKills", 0);
                    dbPlayer.SetData("PBDeaths", 0);

                    dbPlayer.SetArmor(100);

                    dbPlayer.initializePaintball();

                    if (paintballModel.Name == "Würfelpark")
                        foreach (WeaponHash weaponHash in PaintballWeapons)
                        {
                            dbPlayer.GiveWeapon(weaponHash, 9999);
                        }
                    else if (paintballModel.Name == "Bratwa Dorf")
                        foreach (WeaponHash weaponHash in PaintballWeapons2)
                        {
                            dbPlayer.GiveWeapon(weaponHash, 9999);
                        }
                    else if (paintballModel.Name == "LS Supply")
                        foreach (WeaponHash weaponHash in PaintballWeapons3)
                        {
                            dbPlayer.GiveWeapon(weaponHash, 9999);
                        }
                }
            }
            catch (Exception ex)
            {
                Logger.Print("[EXCEPTION nM-Paintball] " + ex.Message);
                Logger.Print("[EXCEPTION nM-Paintball] " + ex.StackTrace);
            }
        }

        public static void leavePaintball(Client c)
        {
            try
            {
                if (c == null) return;
                DbPlayer dbPlayer = c.GetPlayer();
                if (dbPlayer == null || !dbPlayer.IsValid(true) || dbPlayer.Client == null)
                    return;

                PaintballModel paintballModel = dbPlayer.GetData("PBZone");
                if (paintballModel == null) return;

                dbPlayer.ACWait();
                dbPlayer.SetPosition(new Vector3(570.46, 2796.68, 41.01));

                dbPlayer.SetDimension(0);
                dbPlayer.SetData("PBZone", null);
                dbPlayer.SetData("PBKills", 0);
                dbPlayer.SetData("PBDeaths", 0);

                dbPlayer.SetArmor(0);

                dbPlayer.finishPaintball();
                dbPlayer.RemoveAllWeapons();
                WeaponManager.loadWeapons(c);
            }
            catch (Exception ex)
            {
                Logger.Print("[EXCEPTION leavePaintball] " + ex.Message);
                Logger.Print("[EXCEPTION leavePaintball] " + ex.StackTrace);
            }
        }

        public static void PaintballDeath(DbPlayer dbPlayer, DbPlayer dbPlayer2)
        {
            try
            {
                if (dbPlayer == null || dbPlayer2 == null) return;

                Random r = new Random();

                PaintballModel paintballModel = dbPlayer.GetData("PBZone");
                int newdeaths = dbPlayer.GetData("PBDeaths");
                newdeaths += 1;

                dbPlayer.SetData("PBDeaths", newdeaths);
                dbPlayer.SpawnPlayer(paintballModel.Spawns[r.Next(0, paintballModel.Spawns.Count)]);

                int newkills = dbPlayer2.GetData("PBKills");
                newkills += 1;

                dbPlayer2.SetData("PBKills", newkills);

                foreach (WeaponHash weaponHash in PaintballWeapons)
                {
                    dbPlayer.GiveWeapon(weaponHash, 9999);
                }

                dbPlayer.updatePaintballScore((int)dbPlayer.GetData("PBKills"), (int)dbPlayer.GetData("PBDeaths"));
                dbPlayer2.updatePaintballScore((int)dbPlayer2.GetData("PBKills"), (int)dbPlayer2.GetData("PBDeaths"));

                dbPlayer.SendNotification("Du hast noch " + (10 - (int)dbPlayer2.GetData("PBDeaths")) + " Leben!", 3000, "red");

                if (dbPlayer.GetData("PBDeaths") >= 10) leavePaintball(dbPlayer.Client);

                dbPlayer.StopAnimation();
                dbPlayer.SetInvincible(false);
                dbPlayer.SetArmor(100);
                dbPlayer.disableAllPlayerActions(false);
            }
            catch (Exception ex)
            {
                Logger.Print("[EXCEPTION PaintballDeath] " + ex.Message);
                Logger.Print("[EXCEPTION PaintballDeath] " + ex.StackTrace);
            }
        }
    }
}
*/
