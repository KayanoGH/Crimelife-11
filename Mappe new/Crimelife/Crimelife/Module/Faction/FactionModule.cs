using GTANetworkAPI;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using GVMP;

namespace Crimelife
{
    class FactionModule : Crimelife.Module.Module<FactionModule>
    {
        public static List<Faction> factionList = new List<Faction>();
        public static Dictionary<int, List<GarageVehicle>> VehicleList = new Dictionary<int, List<GarageVehicle>>();
        public static Vector3 StoragePosition = new Vector3(-72.34, -804.34, 243.39);
        public static int GangwarDimension = 8888;


        public static List<NativeItem> Weapons = new List<NativeItem>()            {
new NativeItem("5x Schutzweste - 3500$", "Schutzweste-3500-5"),
new NativeItem("5x Underarmor - 2500$", "Underarmor-2500-5"),
new NativeItem("5x Beamten-Schutzweste - 1000$", "BeamtenSchutzweste-1000-5"),
new NativeItem("5x Verbandskasten - 3500$", "Verbandskasten-3500-5"),
new NativeItem("1x Advancedrifle - 80000$", "Advancedrifle-80000-1"),
new NativeItem("1x Bullpuprifle - 75000$", "Bullpuprifle-75000-1"),
new NativeItem("1x Compactrifle - 50000$", "Compactrifle-50000-1"),
new NativeItem("1x Gusenberg - 90000$", "Gusenberg-90000-1"),
new NativeItem("1x Assaultrifle - 45000$", "Assaultrifle-45000-1"),
new NativeItem("1x Heavypistol - 10000$", "HeavyPistol-10000-1"),
new NativeItem("1x Pistol50 - 15000$", "Pistol50-15000-1"),
new NativeItem("1x Machete - 5000$", "Machete-5000-1"),
new NativeItem("1x Axt - 5000$", "Axt-5000-1"),
new NativeItem("1x Schlagring - 5000$", "Schlagring-5000-1"),//
new NativeItem("1x LSPDKnuebel - 5000$", "LSPDKnuebel-5000-1"),
new NativeItem("1x BaseballSchlaeger - 5000$", "BaseballSchlaeger-5000-1"),
new NativeItem("1x Klappmesser - 150000$", "Klappmesser-150000-1"),
new NativeItem("1x Schweissgeraet - 15000$", "Schweissgeraet-5000-1"),
new NativeItem("1x FastEquipAG - 210000$", "FastEquipAG-210000-1"),
new NativeItem("1x FastEquipAC - 210000$", "FastEquipAC-210000-1"),
new NativeItem("1x FastEquipBG - 210000$", "FastEquipBG-210000-1"),
new NativeItem("1x FastEquipBC - 210000$", "FastEquipBC-210000-1")
        };




        protected override bool OnLoad()
        {
            using MySqlConnection con = new MySqlConnection(Configuration.connectionString);
            try
            {
                con.Open();
                MySqlCommand cmd = con.CreateCommand();
                cmd.CommandText = "SELECT * FROM fraktionen";
                MySqlDataReader reader = cmd.ExecuteReader();
                try
                {
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            Faction fraktion = new Faction
                            {
                                Name = reader.GetString("Name"),
                                Short = reader.GetString("Short"),
                                Id = reader.GetInt32("Id"),
                                Blip = reader.GetInt32("Blip"),
                                RGB = NAPI.Util.FromJson<Color>(reader.GetString("RGB")),
                                BadFraktion = reader.GetInt32("BadFraktion") == 1,
                                Dimension = reader.GetInt32("Dimension"),
                                Storage = NAPI.Util.FromJson<Vector3>(reader.GetString("Storage")),
                                Garage = NAPI.Util.FromJson<Vector3>(reader.GetString("Garage")),
                                GarageSpawn = NAPI.Util.FromJson<Vector3>(reader.GetString("GarageSpawn")),
                                GarageSpawnRotation = reader.GetFloat("GarageSpawnRotation"),
                                Spawn = NAPI.Util.FromJson<Vector3>(reader.GetString("Spawn")),
                                ClothesFemale = NAPI.Util.FromJson<List<ClothingModel>>(reader.GetString("ClothesFemale")),
                                ClothesMale = NAPI.Util.FromJson<List<ClothingModel>>(reader.GetString("ClothesMale")),
                                Money = reader.GetInt32("Money"),
                                Logo = reader.GetString("Logo"),
                                CustomCarColor = reader.GetInt32("CustomCarColor")
                            };

                            factionList.Add(fraktion);

                            FraktionsGarage fraktionsGarage = new FraktionsGarage
                            {
                                Id = reader.GetInt32("Id"),
                                CarPoint = NAPI.Util.FromJson<Vector3>(reader.GetString("GarageSpawn")),
                                Rotation = reader.GetFloat("GarageSpawnRotation"),
                                Name = reader.GetString("Name"),
                                Position = NAPI.Util.FromJson<Vector3>(reader.GetString("Garage"))
                            };

                            GarageModule.fraktionsGarages.Add(fraktionsGarage);

                            VehicleList.Add(fraktion.Id, new List<GarageVehicle>());
                        }
                    }
                }
                finally
                {
                    con.Dispose();
                }
            }
            catch (Exception ex)
            {
                Logger.Print("[EXCEPTION loadFraktionen] " + ex.Message);
                Logger.Print("[EXCEPTION loadFraktionen] " + ex.StackTrace);
            }
            finally
            {
                con.Dispose();
            }

            initializeFraktionen();

            return true;
        }

        public static void initializeFraktionen()
        {

            using MySqlConnection con = new MySqlConnection(Configuration.connectionString);
            try
            {
                con.Open();
                MySqlCommand cmd = con.CreateCommand();
                cmd.CommandText = "SELECT * FROM fraktion_vehicles";
                MySqlDataReader reader = cmd.ExecuteReader();
                try
                {
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            if (VehicleList.ContainsKey(reader.GetInt32("FactionId")))
                            {
                                VehicleList[reader.GetInt32("FactionId")].Add(new GarageVehicle
                                {
                                    Id = reader.GetInt32("Id"),
                                    OwnerID = reader.GetInt32("FactionId"),
                                    Name = reader.GetString("Model"),
                                    Plate = "",
                                    Keys = new List<int>() { }
                                });
                            }
                        }
                    }


                }
                finally
                {
                    con.Dispose();
                }
            }
            catch (Exception ex)
            {
                Logger.Print("[EXCEPTION initializeFraktionen] " + ex.Message);
                Logger.Print("[EXCEPTION initializeFraktionen] " + ex.StackTrace);
            }
            finally
            {
                con.Dispose();
            }

            try
            {
                NAPI.Marker.CreateMarker(1, new Vector3(-82.98, -802.79, 243.39), new Vector3(), new Vector3(), 1.0f, new Color(255, 140, 0));
                NAPI.Marker.CreateMarker(1, new Vector3(-80.43, -801.76, 243.39), new Vector3(), new Vector3(), 1.0f, new Color(255, 140, 0));
                NAPI.Marker.CreateMarker(1, new Vector3(-77.73, -810.84, 243.39), new Vector3(), new Vector3(), 1.0f, new Color(255, 140, 0));
                NAPI.Marker.CreateMarker(1, new Vector3(-81.72, -799.36, 243.39), new Vector3(), new Vector3(), 1.0f, new Color(255, 140, 0));
                NAPI.Marker.CreateMarker(1, StoragePosition, new Vector3(), new Vector3(), 1.0f, new Color(255, 140, 0));

                ColShape val3 = NAPI.ColShape.CreateCylinderColShape(new Vector3(-80.43, -801.76, 243.39), 1.4f, 1.4f, uint.MaxValue);
                val3.SetData("FUNCTION_MODEL", new FunctionModel("openFrak"));
                val3.SetData("MESSAGE", new Message("Benutze E um das Fraktionsmenü zu öffnen.", "FRAKTIONSSCHRANK", "#2f2f30", 3000));

                ColShape val4 = NAPI.ColShape.CreateCylinderColShape(new Vector3(-77.71, -810.71, 243.39), 1.4f, 1.4f, uint.MaxValue);
                val4.SetData("FUNCTION_MODEL", new FunctionModel("openFraktionskleiderschrank"));
                val4.SetData("MESSAGE", new Message("Benutze E um den Fraktionskleiderschrank zu öffnen.", "KLEIDERSCHRANK", "white", 3000));

                ColShape val2 = NAPI.ColShape.CreateCylinderColShape(new Vector3(-82.98, -802.79, 243.39), 1.4f, 1.4f, uint.MaxValue);
                val2.SetData("FUNCTION_MODEL", new FunctionModel("enterGangwarDimension"));
                val2.SetData("MESSAGE", new Message("Benutze E um die Gangwar Dimension zu betreten / verlassen.", "GANGWAR", "orange", 3000));

                ColShape val5 = NAPI.ColShape.CreateCylinderColShape(new Vector3(-81.72, -799.36, 243.39), 1.4f, 1.4f, uint.MaxValue);
                val5.SetData("FUNCTION_MODEL", new FunctionModel("enterKriegsDimension"));
                val5.SetData("MESSAGE", new Message("Benutze E um die Kriegs Dimension zu betreten / verlassen.", "KRIEG", "orange", 3000));

                ColShape val = NAPI.ColShape.CreateCylinderColShape(StoragePosition, 2.4f, 2.4f, uint.MaxValue);
                val.SetData("FUNCTION_MODEL", new FunctionModel("exitFraklager"));
                val.SetData("MESSAGE", new Message("Benutze E um das Fraklager zu verlassen.", "", "#2f2f30", 3000));

                ColShape colShape3 = NAPI.ColShape.CreateCylinderColShape(new Vector3(935.89, 47.26, 80.2), 1.4f, 1.4f, uint.MaxValue);
                colShape3.SetData("FUNCTION_MODEL", new FunctionModel("enterCasino"));
                NAPI.Marker.CreateMarker(1, new Vector3(935.89, 47.26, 80.2), new Vector3(), new Vector3(), 1.0f, new Color(173, 216, 230), false, 0);
                colShape3.SetData("MESSAGE", new Message("Benutze E um das Casino zu betreten.", "CASINO", "lightblue", 4000));



                ColShape colShape4 = NAPI.ColShape.CreateCylinderColShape(new Vector3(1090.00, 207.00, -49.9), 1.4f, 1.4f, uint.MaxValue);
                colShape4.SetData("FUNCTION_MODEL", new FunctionModel("leaveCasino"));//
                NAPI.Marker.CreateMarker(1, new Vector3(1090.00, 207.00, -49.9), new Vector3(), new Vector3(), 1.0f, new Color(173, 216, 230), false, 0);
                colShape4.SetData("MESSAGE", new Message("Benutze E um das Casino zu verlassen.", "CASINO", "lightblue", 4000));

                ColShape colShape5 = NAPI.ColShape.CreateCylinderColShape(new Vector3(13.98, -1106.34, 28.8), 1.4f, 1.4f, uint.MaxValue);
                colShape5.SetData("FUNCTION_MODEL", new FunctionModel("Waffendings"));//
                NAPI.Marker.CreateMarker(1, new Vector3(13.98, -1106.34, 28.8), new Vector3(), new Vector3(), 1.0f, new Color(255, 140, 0), false, 0);
                colShape5.SetData("MESSAGE", new Message("Benutze E um den Aufsätzeshop zu öffnen.", "SHOP", "#2f2f30", 4000));

                //dbPlayer.ShowNativeMenu(new NativeMenu("Waffenaufsätze", dbPlayer.Faction.Name, nativeItems));
                foreach (Faction fraktion in factionList)
                {

                    //if (fraktion.BadFraktion)
                    {
                        NAPI.Blip.CreateBlip(436, fraktion.Spawn, 1f, (byte)fraktion.Blip, fraktion.Name, 255, 0, true, 0);
                        NAPI.Marker.CreateMarker(1, fraktion.Storage, new Vector3(), new Vector3(), 1f, new Color(0, 0, 0));
                        NAPI.Marker.CreateMarker(1, fraktion.Garage, new Vector3(), new Vector3(), 1f, new Color(255, 140, 0));

                        ColShape colShape = NAPI.ColShape.CreateCylinderColShape(fraktion.Garage, 1.4f, 1.4f, uint.MaxValue);
                        colShape.SetData("FUNCTION_MODEL", new FunctionModel("openFraktionsGarage", fraktion.Id, fraktion.Name));
                        colShape.SetData("MESSAGE", new Message("Benutze E um die Fraktionsgarage zu öffnen.", fraktion.Name, fraktion.GetRGBStr()));


                        ColShape colShape2 = NAPI.ColShape.CreateCylinderColShape(fraktion.Storage, 1.4f, 1.4f, uint.MaxValue);
                        colShape2.SetData("FUNCTION_MODEL", new FunctionModel("enterFraklager", fraktion.Id.ToString()));
                        colShape2.SetData("MESSAGE", new Message("Benutze E um das Fraklager zu betreten.", fraktion.Name, fraktion.GetRGBStr()));

                        //FraktionsVehicles.list.Add(fraktion.fraktionName, Database.getFraktionVehicles2(fraktion.fraktionName));

                    }
                }

            }
            catch (Exception ex)
            {
                Logger.Print("[EXCEPTION initializeFraktionen] " + ex.Message);
                Logger.Print("[EXCEPTION initializeFraktionen] " + ex.StackTrace);
            }
        }

        [RemoteEvent("openFrak")]
        public static void openFrak(Player c)
        {
            try
            {
                DbPlayer dbPlayer = c.GetPlayer();
                if (dbPlayer == null || !dbPlayer.IsValid(true) || dbPlayer.Client == null)
                    return;

                dbPlayer.ShowNativeMenu(new NativeMenu("Fraktionswaffenschrank", "Menu", new List<NativeItem>()
                {
                    new NativeItem("Waffe Herstellen", "workonweapon"),//openitem
                    new NativeItem("Nahkampfwaffe Herstellen", "openNahkampf"),
                    new NativeItem("Item Herstellen", "openitem"),
                    new NativeItem("Waffenaufsätze", "weaponcomponents"),
                    new NativeItem("Fast Equip", "fastequip"),
                    new NativeItem("Fraktionsbank", "openfrakbank")
            }));
            }
            catch (Exception ex) { }
        }

        [RemoteEvent("nM-Fraktionswaffenschrank")]
        public void Fraktionswaffenschranks(Player c, string arg)
        {
            try
            {
                DbPlayer dbPlayer = c.GetPlayer();
                if (dbPlayer == null || !dbPlayer.IsValid(true) || dbPlayer.Client == null)
                    return;


                if (arg == "weaponcomponents")
                {
                    dbPlayer.CloseNativeMenu();

                    List<NativeItem> nativeItems = new List<NativeItem>();

                    List<string> strings = new List<string>();

                    WeaponComponentModule.nXWeaponComponents.ForEach(comp =>
                    {
                        if (!strings.Contains(comp.Name))
                        {
                            nativeItems.Add(new NativeItem("" + comp.Name + " - " + comp.Price + "$", comp.Name));
                            strings.Add(comp.Name);
                        }
                    });

                    dbPlayer.ShowNativeMenu(new NativeMenu("Waffenaufsätze", dbPlayer.Faction.Name, nativeItems));

                    return;
                }
                if (arg == "openNahkampf")
                {
                    if (dbPlayer.Faction.Id == 0)
                        return;

                    if (dbPlayer.Faction.Name == "FIB" || dbPlayer.Faction.Name == "PoliceDepartment" || dbPlayer.Faction.Id == 31)
                    {
                        NativeMenu nativeMenu = new NativeMenu("Waffenschrank", dbPlayer.Faction.Name, new List<NativeItem>()
                {
new NativeItem("1x Machete - 5000$", "Machete-5000-1"),
new NativeItem("1x Axt - 5000$", "Axt-5000-1"),
new NativeItem("1x LSPDKnuebel - 5000$", "LSPDKnuebel-5000-1"),
new NativeItem("1x BaseballSchlaeger - 5000$", "BaseballSchlaeger-5000-1")
                });
                        //    nativeMenu.Items.Add(new NativeItem("Waffenaufsätze", "weaponcomponents"));
                        dbPlayer.ShowNativeMenu(nativeMenu);
                    }
                    else
                    {
                        NativeMenu nativeMenu2 = new NativeMenu("Waffenschrank", dbPlayer.Faction.Name, new List<NativeItem>()
                {
new NativeItem("1x Machete - 5000$", "Machete-5000-1"),
new NativeItem("1x Axt - 5000$", "Axt-5000-1"),//
new NativeItem("1x Schlagring - 5000$", "Schlagring-5000-1"),//
new NativeItem("1x BaseballSchlaeger - 5000$", "BaseballSchlaeger-5000-1"),
new NativeItem("1x Klappmesser - 150000$", "Klappmesser-150000-1")
                });
                        //   nativeMenu2.Items.Add(new NativeItem("Waffenaufsätze", "weaponcomponents"));
                        dbPlayer.ShowNativeMenu(nativeMenu2);
                    }
                }
                if (arg == "openitem")
                {
                    if (dbPlayer.Faction.Id == 0)
                        return;

                    if (dbPlayer.Faction.Name == "FIB" || dbPlayer.Faction.Name == "PoliceDepartment" || dbPlayer.Faction.Id == 31)
                    {
                        NativeMenu nativeMenu = new NativeMenu("Waffenschrank", dbPlayer.Faction.Name, new List<NativeItem>()
                {
new NativeItem("5x Beamten-Schutzweste - 1000$", "BeamtenSchutzweste-1000-5"),
new NativeItem("5x Verbandskasten - 3500$", "Verbandskasten-3500-5"),
new NativeItem("1x Schweissgeraet - 15000$", "Schweissgeraet-5000-1")
                });
                        //    nativeMenu.Items.Add(new NativeItem("Waffenaufsätze", "weaponcomponents"));
                        dbPlayer.ShowNativeMenu(nativeMenu);
                    }
                    else
                    {
                        NativeMenu nativeMenu2 = new NativeMenu("Waffenschrank", dbPlayer.Faction.Name, new List<NativeItem>()
                {
new NativeItem("5x Schutzweste - 3500$", "Schutzweste-3500-5"),
new NativeItem("5x Underarmor - 2500$", "Underarmor-2500-5"),
new NativeItem("5x Verbandskasten - 3500$", "Verbandskasten-3500-5"),
new NativeItem("1x Schweissgeraet - 15000$", "Schweissgeraet-5000-1")
                });
                        //   nativeMenu2.Items.Add(new NativeItem("Waffenaufsätze", "weaponcomponents"));
                        dbPlayer.ShowNativeMenu(nativeMenu2);
                    }
                }
                if (arg == "workonweapon")
                {
                    if (dbPlayer.Faction.Id == 0)
                        return;

                    if (dbPlayer.Faction.Name == "FIB" || dbPlayer.Faction.Name == "PoliceDepartment" || dbPlayer.Faction.Id == 31)
                    {
                        NativeMenu nativeMenu = new NativeMenu("Waffenschrank", dbPlayer.Faction.Name, new List<NativeItem>()
                {
new NativeItem("1x Advancedrifle - 80000$", "Advancedrifle-80000-1"),
new NativeItem("1x Bullpuprifle - 75000$", "Bullpuprifle-75000-1"),
new NativeItem("1x Compactrifle - 50000$", "Compactrifle-50000-1"),
new NativeItem("1x Gusenberg - 90000$", "Gusenberg-90000-1"),
new NativeItem("1x Assaultrifle - 45000$", "Assaultrifle-45000-1"),
new NativeItem("1x Heavypistol - 10000$", "HeavyPistol-10000-1"),
new NativeItem("1x Pistol50 - 15000$", "Pistol50-15000-1")
                });
                        //    nativeMenu.Items.Add(new NativeItem("Waffenaufsätze", "weaponcomponents"));
                        dbPlayer.ShowNativeMenu(nativeMenu);
                    }
                    else
                    {
                        NativeMenu nativeMenu2 = new NativeMenu("Waffenschrank", dbPlayer.Faction.Name, new List<NativeItem>()
                {
new NativeItem("1x Advancedrifle - 80000$", "Advancedrifle-80000-1"),
new NativeItem("1x Bullpuprifle - 75000$", "Bullpuprifle-75000-1"),
new NativeItem("1x Compactrifle - 50000$", "Compactrifle-50000-1"),
new NativeItem("1x Gusenberg - 90000$", "Gusenberg-90000-1"),
new NativeItem("1x Assaultrifle - 45000$", "Assaultrifle-45000-1"),
new NativeItem("1x Heavypistol - 10000$", "HeavyPistol-10000-1"),
new NativeItem("1x Pistol50 - 15000$", "Pistol50-15000-1")
                });
                        //   nativeMenu2.Items.Add(new NativeItem("Waffenaufsätze", "weaponcomponents"));
                        dbPlayer.ShowNativeMenu(nativeMenu2);
                    }
                }
                if (arg == "fastequip")
                {
                    if (dbPlayer.Faction.Id == 0)
                        return;

                    if (dbPlayer.Faction.Name == "FIB" || dbPlayer.Faction.Name == "PoliceDepartment" || dbPlayer.Faction.Id == 31)
                    {
                        NativeMenu nativeMenu = new NativeMenu("Waffenschrank", dbPlayer.Faction.Name, new List<NativeItem>()
                {
new NativeItem("1x FastEquipAG - 210000$", "FastEquipAG-210000-1"),
new NativeItem("1x FastEquipAC - 210000$", "FastEquipAC-210000-1"),
new NativeItem("1x FastEquipBG - 210000$", "FastEquipBG-210000-1"),
new NativeItem("1x FastEquipBC - 210000$", "FastEquipBC-210000-1")
                });
                        //    nativeMenu.Items.Add(new NativeItem("Waffenaufsätze", "weaponcomponents"));
                        dbPlayer.ShowNativeMenu(nativeMenu);
                    }
                    else
                    {
                        NativeMenu nativeMenu2 = new NativeMenu("Waffenschrank", dbPlayer.Faction.Name, new List<NativeItem>()
                {
new NativeItem("1x FastEquipAG - 210000$", "FastEquipAG-210000-1"),
new NativeItem("1x FastEquipAC - 210000$", "FastEquipAC-210000-1"),
new NativeItem("1x FastEquipBG - 210000$", "FastEquipBG-210000-1"),
new NativeItem("1x FastEquipBC - 210000$", "FastEquipBC-210000-1")
                });
                        //   nativeMenu2.Items.Add(new NativeItem("Waffenaufsätze", "weaponcomponents"));
                        dbPlayer.ShowNativeMenu(nativeMenu2);
                    }
                }
                if (arg == "openfrakbank")
                {
                    dbPlayer.CloseNativeMenu();
                    FactionBank.OpenFactionBank(dbPlayer);
                    return;
                }
            }
            catch (Exception ex) { }
        }




        [RemoteEvent("openfrakbank")]
        public void openfrakbank(Player c)
        {
            try
            {
                if (c == null) return;
                DbPlayer dbPlayer = c.GetPlayer();
                if (dbPlayer == null || !dbPlayer.IsValid(true) || dbPlayer.Client == null)
                    return;

                if (dbPlayer.Faction.Id == 0)
                    return;
                //nM-Waffenaufsätze
                FactionBank.OpenFactionBank(dbPlayer);

            }
            catch (Exception ex)
            {
                Logger.Print("[EXCEPTION openFraktionskleiderschrank] " + ex.Message);
                Logger.Print("[EXCEPTION openFraktionskleiderschrank] " + ex.StackTrace);
            }
        }


        [RemoteEvent("leaveCasino")]
        public void leaveCasino(Player c)
        {
            try
            {
                DbPlayer dbPlayer = PlayerHandler.GetPlayer(c.Name);

                WeaponManager.loadWeapons(dbPlayer.Client);
                c.Position = new Vector3(935.89, 47.26, 80.9);

            }
            catch (Exception ex)
            {
                Logger.Print("[EXCEPTION Pressed_KOMMA] " + ex.Message);
                Logger.Print("[EXCEPTION Pressed_KOMMA] " + ex.StackTrace);
            }
        }

        [RemoteEvent("Waffendings")]
        public void Waffendings(Player c)
        {
            DbPlayer dbPlayer = c.GetPlayer();

            List<NativeItem> nativeItems = new List<NativeItem>();

                    List<string> strings = new List<string>();

                    WeaponComponentModule.nXWeaponComponents.ForEach(comp =>
                    {
                        if (!strings.Contains(comp.Name))
                        {
                            nativeItems.Add(new NativeItem("" + comp.Name + " - " + comp.Price + "$", comp.Name));
                            strings.Add(comp.Name);
                        }
                    });

                    dbPlayer.ShowNativeMenu(new NativeMenu("Waffenaufsätze", "Menu", nativeItems));

                    return;
        } 


        [RemoteEvent("enterGangwarDimension")]
        public void enterGangwarDimension(Player c)
        {
            try
            {
                if (c == null) return;
                DbPlayer dbPlayer = c.GetPlayer();
                if (dbPlayer == null || !dbPlayer.IsValid(true) || dbPlayer.Client == null)
                    return;
                if (c.Dimension == dbPlayer.Faction.Dimension)
                {
                    dbPlayer.SendNotification("Du hast die Gangwar Dimension betreten.", 3000, "orange", "GANGWAR");
                    c.Dimension = Convert.ToUInt32(GangwarDimension);
                    c.Position = FactionModule.StoragePosition;
                }
                else
                {
                    dbPlayer.SendNotification("Du hast die Gangwar Dimension verlassen.", 3000, "orange", "GANGWAR");
                    c.RemoveAllWeapons();
                    c.Dimension = 9999;

                    NAPI.Task.Run(() =>
                    {
                        WeaponManager.loadWeapons(dbPlayer.Client);
                        c.Dimension = Convert.ToUInt32(dbPlayer.Faction.Dimension);
                    }, 3000);
                }
            }
            catch (Exception ex)
            {
                Logger.Print("[EXCEPTION enterGangwarDimension] " + ex.Message);
                Logger.Print("[EXCEPTION enterGangwarDimension] " + ex.StackTrace);
            }
        }

        [RemoteEvent("enterKriegsDimension")]
        public void enterKriegsDimension(Player c)
        {
            try
            {
                if (c == null) return;
                DbPlayer dbPlayer = c.GetPlayer();
                if (dbPlayer == null || !dbPlayer.IsValid(true) || dbPlayer.Client == null)
                    return;

                if (c.Dimension == dbPlayer.Faction.Dimension)
                {
                    dbPlayer.SendNotification("Du hast die Kriegs Dimension betreten.", 3000, "orange", "KRIEG");
                    c.Dimension = Convert.ToUInt32(1338);
                }
                else
                {
                    dbPlayer.SendNotification("Du hast die Kriegs Dimension verlassen.", 3000, "orange", "KRIEG");
                    c.Dimension = Convert.ToUInt32(dbPlayer.Faction.Dimension);
                }
            }
            catch (Exception ex)
            {
                Logger.Print("[EXCEPTION enterGangwarDimension] " + ex.Message);
                Logger.Print("[EXCEPTION enterGangwarDimension] " + ex.StackTrace);
            }
        }

        [RemoteEvent("exitFraklager")]
        public void exitFraklager(Player c)
        {
            try
            {
                if (c == null) return;
                DbPlayer dbPlayer = c.GetPlayer();
                if (dbPlayer == null || !dbPlayer.IsValid(true) || dbPlayer.Client == null)
                    return;

                try
                {
                    if (dbPlayer.Faction == null || dbPlayer.Faction.Id == 0)
                        return;

                    c.Position = dbPlayer.Faction.Storage.Add(new Vector3(0, 0, 1.5));
                    if (c.Dimension == dbPlayer.Faction.Dimension)
                    {
                        c.Dimension = 0;
                        dbPlayer.SendNotification("Fraktions Lager Verlassen Dimension " + dbPlayer.Client.Dimension);
                    }
                    else
                    {
                        dbPlayer.SendNotification("Fraktions Lager Verlassen Dimension " + dbPlayer.Client.Dimension);
                    }

                }
                catch (Exception ex)
                {
                    Logger.Print("[EXCEPTION exitFraklager] " + ex.Message);
                    Logger.Print("[EXCEPTION exitFraklager] " + ex.StackTrace);
                }
            }
            catch (Exception ex)
            {
                Logger.Print("[EXCEPTION exitFraklager] " + ex.Message);
                Logger.Print("[EXCEPTION exitFraklager] " + ex.StackTrace);
            }
        }

        [RemoteEvent("enterFraklager")]
        public void enterFraklager(Player c, string fraktionsId)
        {
            try
            {
                if (c == null) return;
                if (fraktionsId == null)
                    return;

                DbPlayer dbPlayer = c.GetPlayer();
                if (dbPlayer == null || !dbPlayer.IsValid(true) || dbPlayer.Client == null)
                    return;

                try
                {
                    int id = 0;
                    bool id2 = int.TryParse(fraktionsId, out id);
                    if (!id2) return;

                    if (dbPlayer.Faction == null || dbPlayer.Faction.Id == 0)
                        return;

                    if (id == dbPlayer.Faction.Id)
                    {
                        if (c.Position.DistanceTo(dbPlayer.Faction.Storage) < 2.0f)
                        {
                            if (c.Dimension == Convert.ToUInt32(GangwarDimension))
                            {
                                WeaponManager.loadWeapons(dbPlayer.Client);
                            }
                            c.Position = StoragePosition;
                            c.Dimension = Convert.ToUInt32(dbPlayer.Faction.Dimension);
                            return;
                        }
                    }
                    else
                    {
                        dbPlayer.SendNotification("Du bist nicht in der Fraktion.", 3000, dbPlayer.Faction.GetRGBStr(),
                            dbPlayer.Faction.Name);
                    }
                }
                catch (Exception ex)
                {
                    Logger.Print("[EXCEPTION enterFraklager] " + ex.Message);
                    Logger.Print("[EXCEPTION enterFraklager] " + ex.StackTrace);
                }
            }
            catch (Exception ex)
            {
                Logger.Print("[EXCEPTION enterFraklager] " + ex.Message);
                Logger.Print("[EXCEPTION enterFraklager] " + ex.StackTrace);
            }
        }

        [RemoteEvent("enterCasino")]
        public void enterCasino(Player c)
        {
            try
            {
                if (c == null) return;
                DbPlayer dbPlayer = c.GetPlayer();
                if (dbPlayer == null || !dbPlayer.IsValid(true) || dbPlayer.Client == null)
                {
                    return;
                }
                //Logger.Print("1");

                if (dbPlayer.DeathData.IsDead)
                {
                    return;
                }
                //  Logger.Print("2");

                if (dbPlayer.IsFarming)
                {
                    return;
                }
                // Logger.Print("3"); // Exeption liet hier!



                MySqlQuery mySqlQuery = new MySqlQuery("SELECT * FROM inventorys WHERE Id = @userId LIMIT 1");
                mySqlQuery.AddParameter("@userId", dbPlayer.Id);
                MySqlResult mySqlReaderCon = MySqlHandler.GetQuery(mySqlQuery);
                try
                {
                    MySqlDataReader reader = mySqlReaderCon.Reader;
                    try
                    {
                        if (!reader.HasRows)
                        {
                            return;
                        }
                        reader.Read();
                        List<ItemModel> list = new List<ItemModel>();
                        string @string = reader.GetString("Items");
                        list = NAPI.Util.FromJson<List<ItemModel>>(@string);
                        ItemModel itemToUse = list.FirstOrDefault((ItemModel x) => x.Name == "CaillouCard");
                        if (list == null) return;
                        if (itemToUse == null)
                        {
                            //   dbPlayer.SendNotification("Du hast kein Gültigen Ticket!", 3000, "red");
                            return;
                        }

                        int index = list.IndexOf(itemToUse);
                        if (index == null)
                        {
                            return;
                        }
                        if (itemToUse.Amount == 1)
                        {
                            list.Remove(itemToUse);
                        }
                        else
                        {
                            //  dbPlayer.SendNotification("Du hast kein Gültigen Ticket!", 3000, "red");
                        }

                        // Logger.Print("7");
                        reader.Close();
                        if (reader.IsClosed)
                        {
                            mySqlQuery.Query = "UPDATE inventorys SET Items = @invItems WHERE Id = @pId";
                            mySqlQuery.Parameters = new List<MySqlParameter>()
                            {
                                new MySqlParameter("@invItems", NAPI.Util.ToJson(list)),
                                new MySqlParameter("@pId", dbPlayer.Id)
                            };
                            MySqlHandler.ExecuteSync(mySqlQuery);
                            dbPlayer.RemoveAllWeapons();
                            c.Position = new Vector3(1090.00, 207.00, -48.9);
                            object JSONobject = new
                            {
                                /* Willkommen = "    " + dbPlayer.Name */
                            };

                            dbPlayer.TriggerEvent("sendInfocard", "Casino", "lightblue", "nightclubs.jpg", 8500, NAPI.Util.ToJson(JSONobject));

                        }
                    }
                    finally
                    {
                        reader.Dispose();
                    }
                }
                finally
                {
                    mySqlReaderCon.Connection.Dispose();
                }
            }
            catch (Exception ex)
            {
                Logger.Print("[EXCEPTION Pressed_KOMMA] " + ex.Message);
                Logger.Print("[EXCEPTION Pressed_KOMMA] " + ex.StackTrace);
            }
        }
        // openNahkampf

        [RemoteEvent("openitem")]
        public void openitem(Player c)
        {
            try
            {
                if (c == null) return;
                DbPlayer dbPlayer = c.GetPlayer();
                if (dbPlayer == null || !dbPlayer.IsValid(true) || dbPlayer.Client == null)
                    return;

                try
                {
                    //  NativeMenu nativeMenu = new NativeMenu("Fraktionswaffenschrank", dbPlayer.Faction.Name, new List<NativeItem>());

                    //NativeMenu nativeMenu2 = new NativeMenu("Waffenkammer", dbPlayer.Faction.Name, new List<NativeItem>());
                    if (dbPlayer.Faction.Id == 0)
                        return;

                    if (dbPlayer.Faction.Name == "FIB" || dbPlayer.Faction.Name == "PoliceDepartment" || dbPlayer.Faction.Id == 31)
                    {
                        NativeMenu nativeMenu = new NativeMenu("Fraktionswaffenschrank", dbPlayer.Faction.Name, new List<NativeItem>()
                {
new NativeItem("5x Beamten-Schutzweste - 1000$", "BeamtenSchutzweste-1000-5"),
new NativeItem("5x Verbandskasten - 3500$", "Verbandskasten-3500-5"),
new NativeItem("1x Schweissgeraet - 15000$", "Schweissgeraet-5000-1")
                });
                        //    nativeMenu.Items.Add(new NativeItem("Waffenaufsätze", "weaponcomponents"));
                        dbPlayer.ShowNativeMenu(nativeMenu);
                    }
                    else
                    {
                        NativeMenu nativeMenu2 = new NativeMenu("Fraktionswaffenschrank", dbPlayer.Faction.Name, new List<NativeItem>()
                {
new NativeItem("5x Schutzweste - 3500$", "Schutzweste-3500-5"),
new NativeItem("5x Underarmor - 2500$", "Underarmor-2500-5"),
new NativeItem("5x Verbandskasten - 3500$", "Verbandskasten-3500-5"),
new NativeItem("1x Schweissgeraet - 15000$", "Schweissgeraet-5000-1")
                });
                        //   nativeMenu2.Items.Add(new NativeItem("Waffenaufsätze", "weaponcomponents"));
                        dbPlayer.ShowNativeMenu(nativeMenu2);
                    }

                    //Weapons.ForEach(f => nativeMenu.Items.Add(f));

                    //  nativeMenu.Items.Add(new NativeItem("Waffenaufsätze", "weaponcomponents"));

                    //  dbPlayer.ShowNativeMenu(nativeMenu);
                }
                catch (Exception ex)
                {
                    Logger.Print("[EXCEPTION openWaffenschrank] " + ex.Message);
                    Logger.Print("[EXCEPTION openWaffenschrank] " + ex.StackTrace);
                }
            }
            catch (Exception ex)
            {
                Logger.Print("[EXCEPTION openWaffenschrank] " + ex.Message);
                Logger.Print("[EXCEPTION openWaffenschrank] " + ex.StackTrace);
            }
        }

        [RemoteEvent("openNahkampf")]
        public void openNahkampf(Player c)
        {
            try
            {
                if (c == null) return;
                DbPlayer dbPlayer = c.GetPlayer();
                if (dbPlayer == null || !dbPlayer.IsValid(true) || dbPlayer.Client == null)
                    return;

                try
                {
                    //  NativeMenu nativeMenu = new NativeMenu("Fraktionswaffenschrank", dbPlayer.Faction.Name, new List<NativeItem>());

                    //NativeMenu nativeMenu2 = new NativeMenu("Waffenkammer", dbPlayer.Faction.Name, new List<NativeItem>());
                    if (dbPlayer.Faction.Id == 0)
                        return;

                    if (dbPlayer.Faction.Name == "FIB" || dbPlayer.Faction.Name == "PoliceDepartment" || dbPlayer.Faction.Id == 31)
                    {
                        NativeMenu nativeMenu = new NativeMenu("Fraktionswaffenschrank", dbPlayer.Faction.Name, new List<NativeItem>()
                {
new NativeItem("1x Machete - 5000$", "Machete-5000-1"),
new NativeItem("1x Axt - 5000$", "Axt-5000-1"),
new NativeItem("1x LSPDKnuebel - 5000$", "LSPDKnuebel-5000-1"),
new NativeItem("1x BaseballSchlaeger - 5000$", "BaseballSchlaeger-5000-1")
                });
                        //    nativeMenu.Items.Add(new NativeItem("Waffenaufsätze", "weaponcomponents"));
                        dbPlayer.ShowNativeMenu(nativeMenu);
                    }
                    else
                    {
                        NativeMenu nativeMenu2 = new NativeMenu("Fraktionswaffenschrank", dbPlayer.Faction.Name, new List<NativeItem>()
                {
new NativeItem("1x Machete - 5000$", "Machete-5000-1"),
new NativeItem("1x Axt - 5000$", "Axt-5000-1"),//
new NativeItem("1x Schlagring - 5000$", "Schlagring-5000-1"),//
new NativeItem("1x BaseballSchlaeger - 5000$", "BaseballSchlaeger-5000-1"),
new NativeItem("1x Klappmesser - 150000$", "Klappmesser-150000-1")
                });
                        //   nativeMenu2.Items.Add(new NativeItem("Waffenaufsätze", "weaponcomponents"));
                        dbPlayer.ShowNativeMenu(nativeMenu2);
                    }

                    //Weapons.ForEach(f => nativeMenu.Items.Add(f));

                    //  nativeMenu.Items.Add(new NativeItem("Waffenaufsätze", "weaponcomponents"));

                    //  dbPlayer.ShowNativeMenu(nativeMenu);
                }
                catch (Exception ex)
                {
                    Logger.Print("[EXCEPTION openWaffenschrank] " + ex.Message);
                    Logger.Print("[EXCEPTION openWaffenschrank] " + ex.StackTrace);
                }
            }
            catch (Exception ex)
            {
                Logger.Print("[EXCEPTION openWaffenschrank] " + ex.Message);
                Logger.Print("[EXCEPTION openWaffenschrank] " + ex.StackTrace);
            }
        }

        [RemoteEvent("openWaffenschrank")]
        public void openWaffenschrank(Player c)
        {
            try
            {
                if (c == null) return;
                DbPlayer dbPlayer = c.GetPlayer();
                if (dbPlayer == null || !dbPlayer.IsValid(true) || dbPlayer.Client == null)
                    return;

                try
                {
                    //  NativeMenu nativeMenu = new NativeMenu("Fraktionswaffenschrank", dbPlayer.Faction.Name, new List<NativeItem>());

                    //NativeMenu nativeMenu2 = new NativeMenu("Waffenkammer", dbPlayer.Faction.Name, new List<NativeItem>());
                    if (dbPlayer.Faction.Id == 0)
                        return;

                    if (dbPlayer.Faction.Name == "FIB" || dbPlayer.Faction.Name == "PoliceDepartment" || dbPlayer.Faction.Id == 31)
                    {
                        NativeMenu nativeMenu = new NativeMenu("Waffenschrank", dbPlayer.Faction.Name, new List<NativeItem>()
                {
new NativeItem("1x Advancedrifle - 80000$", "Advancedrifle-80000-1"),
new NativeItem("1x Bullpuprifle - 75000$", "Bullpuprifle-75000-1"),
new NativeItem("1x Compactrifle - 50000$", "Compactrifle-50000-1"),
new NativeItem("1x Gusenberg - 90000$", "Gusenberg-90000-1"),
new NativeItem("1x Assaultrifle - 45000$", "Assaultrifle-45000-1"),
new NativeItem("1x Heavypistol - 10000$", "HeavyPistol-10000-1"),
new NativeItem("1x Pistol50 - 15000$", "Pistol50-15000-1")
                });
                        //    nativeMenu.Items.Add(new NativeItem("Waffenaufsätze", "weaponcomponents"));
                        dbPlayer.ShowNativeMenu(nativeMenu);
                    }
                    else
                    {
                        NativeMenu nativeMenu2 = new NativeMenu("Waffenschrank", dbPlayer.Faction.Name, new List<NativeItem>()
                {
new NativeItem("1x Advancedrifle - 80000$", "Advancedrifle-80000-1"),
new NativeItem("1x Bullpuprifle - 75000$", "Bullpuprifle-75000-1"),
new NativeItem("1x Compactrifle - 50000$", "Compactrifle-50000-1"),
new NativeItem("1x Gusenberg - 90000$", "Gusenberg-90000-1"),
new NativeItem("1x Assaultrifle - 45000$", "Assaultrifle-45000-1"),
new NativeItem("1x Heavypistol - 10000$", "HeavyPistol-10000-1"),
new NativeItem("1x Pistol50 - 15000$", "Pistol50-15000-1")
                });
                        //   nativeMenu2.Items.Add(new NativeItem("Waffenaufsätze", "weaponcomponents"));
                        dbPlayer.ShowNativeMenu(nativeMenu2);
                    }

                    //Weapons.ForEach(f => nativeMenu.Items.Add(f));

                    //  nativeMenu.Items.Add(new NativeItem("Waffenaufsätze", "weaponcomponents"));

                    //  dbPlayer.ShowNativeMenu(nativeMenu);
                }
                catch (Exception ex)
                {
                    Logger.Print("[EXCEPTION openWaffenschrank] " + ex.Message);
                    Logger.Print("[EXCEPTION openWaffenschrank] " + ex.StackTrace);
                }
            }
            catch (Exception ex)
            {
                Logger.Print("[EXCEPTION openWaffenschrank] " + ex.Message);
                Logger.Print("[EXCEPTION openWaffenschrank] " + ex.StackTrace);
            }
        }

        [RemoteEvent("nM-Waffenaufsätze")]
        public void Waffenaufsaetze(Player c, string selection)
        {
            if (selection == null)
                return;

            DbPlayer dbPlayer = c.GetPlayer();
            if (dbPlayer == null || !dbPlayer.IsValid(true) || dbPlayer.Client == null)
                return;

            var weapon = NAPI.Player.GetPlayerCurrentWeapon(c);
            if (weapon == null || weapon == WeaponHash.Unarmed)
            {
                dbPlayer.SendNotification("Du musst eine Waffe in die Hand nehmen!", 3500, "red", "WAFFENAUFSÄTZE");
                return;
            }

            NXWeaponComponent component = WeaponComponentModule.nXWeaponComponents.FirstOrDefault(c => c.WeaponHash == weapon && c.Name == selection);
            if (component == null)
            {
                dbPlayer.SendNotification("Für diese Waffe gibt es diesen Waffenaufsatz nicht!", 3500, "red", "WAFFENAUFSÄTZE");
                return;
            }

            if (dbPlayer.Money >= component.Price)
            {
                dbPlayer.SendNotification(component.Name + " an deine Waffe angebracht!");
                WeaponManager.addWeaponComponent(dbPlayer.Client, weapon, component.WeaponComponent);
            }
            else
            {
                dbPlayer.SendNotification("Du hast nicht genug Geld!", 3500, "red", "WAFFENAUFSÄTZE");
            }

        }

        [RemoteEvent("nM-Waffenschrank")]
        public void Fraktionswaffenschrank(Player c, string selection)
        {
            try
            {
                if (c == null) return;

                if (selection == null)
                    return;

                DbPlayer dbPlayer = c.GetPlayer();
                if (dbPlayer == null || !dbPlayer.IsValid(true) || dbPlayer.Client == null)
                    return;

                try
                {
                    if (selection == "weaponcomponents")
                    {
                        dbPlayer.CloseNativeMenu();

                        List<NativeItem> nativeItems = new List<NativeItem>();

                        List<string> strings = new List<string>();

                        WeaponComponentModule.nXWeaponComponents.ForEach(comp =>
                        {
                            if (!strings.Contains(comp.Name))
                            {
                                nativeItems.Add(new NativeItem("1x " + comp.Name + " - " + comp.Price + "$", comp.Name));
                                strings.Add(comp.Name);
                            }
                        });

                        dbPlayer.ShowNativeMenu(new NativeMenu("Waffenaufsätze", dbPlayer.Faction.Name, nativeItems));

                        return;
                    }

                    NativeItem nativeItem = Weapons.FirstOrDefault((NativeItem item) => item.selectionName == selection);
                    if (nativeItem == null) return;
                    Item itemObj = ItemModule.itemRegisterList.FirstOrDefault((Item x) => x.Name == selection.Split("-")[0]);
                    if (itemObj == null) return;

                    string[] strArray = selection.Split("-");

                    string item = strArray[0];
                    int price = 0;
                    int count = 0;
                    bool price2 = int.TryParse(strArray[1], out price);
                    bool count2 = int.TryParse(strArray[2], out count);
                    if (!price2) return;
                    if (!count2) return;
                    if (dbPlayer.Money >= price)
                    {

                        dbPlayer.UpdateInventoryItems(itemObj.Name, count, false);
                        dbPlayer.removeMoney(Convert.ToInt32(price));
                        dbPlayer.SendNotification("Du hast " + count + "x " + item + " hergestellt!");
                    }
                    else
                    {
                        dbPlayer.SendNotification("Du hast zu wenig Geld.", 3000, "red", "waffenschrank");
                    }
                }
                catch (Exception ex)
                {
                    Logger.Print("[EXCEPTION Fraktionswaffenschrank] " + ex.Message);
                    Logger.Print("[EXCEPTION Fraktionswaffenschrank] " + ex.StackTrace);
                }
            }
            catch (Exception ex)
            {
                Logger.Print("[EXCEPTION Fraktionswaffenschrank] " + ex.Message);
                Logger.Print("[EXCEPTION Fraktionswaffenschrank] " + ex.StackTrace);
            }
        }



        [RemoteEvent("openFraktionskleiderschrank")]
        public void openFraktionskleiderschrank(Player c)
        {
            try
            {
                if (c == null) return;
                DbPlayer dbPlayer = c.GetPlayer();
                if (dbPlayer == null || !dbPlayer.IsValid(true) || dbPlayer.Client == null)
                    return;

                if (dbPlayer.Faction.Id == 0)
                    return;

                dbPlayer.ShowNativeMenu(new NativeMenu("Fraktionskleiderschrank", dbPlayer.Faction.Short, new List<NativeItem>()
                {
                    new NativeItem("Maske", "Maske"),
                    new NativeItem("Oberteil", "Oberteil"),
                    new NativeItem("Unterteil", "Unterteil"),
                    new NativeItem("Körper", "Koerper"),
                    new NativeItem("Hose", "Hose"),
                    new NativeItem("Schuhe", "Schuhe")
                }));

            }
            catch (Exception ex)
            {
                Logger.Print("[EXCEPTION openFraktionskleiderschrank] " + ex.Message);
                Logger.Print("[EXCEPTION openFraktionskleiderschrank] " + ex.StackTrace);
            }
        }

        [RemoteEvent("nM-Fraktionskleiderschrank")]
        public void Fraktionskleiderschrank(Player c, string selection)
        {
            if (c == null) return;
            if (selection == null)
                return;

            DbPlayer dbPlayer = c.GetPlayer();
            if (dbPlayer == null || !dbPlayer.IsValid(true) || dbPlayer.Client == null)
                return;

            try
            {
                if (dbPlayer.Faction.Id == 0)
                    return;

                List<NativeItem> Items = new List<NativeItem>();
                List<ClothingModel> clothingList = new List<ClothingModel>();

                if (ClothingManager.isMale(c))
                {
                    clothingList = dbPlayer.Faction.ClothesMale;
                }
                else
                {
                    clothingList = dbPlayer.Faction.ClothesFemale;
                }

                foreach (ClothingModel fraktionsClothe in clothingList)
                {
                    if (fraktionsClothe.category == selection)
                    {
                        Items.Add(new NativeItem(fraktionsClothe.name, selection + "-" + fraktionsClothe.component.ToString() + "-" + fraktionsClothe.drawable.ToString() + "-" + fraktionsClothe.texture.ToString()));
                    }
                    dbPlayer.CloseNativeMenu();
                    dbPlayer.ShowNativeMenu(new NativeMenu("Kleidungsauswahl", dbPlayer.Faction.Name + " | " + selection, Items));
                }

            }
            catch (Exception ex)
            {
                Logger.Print("[EXCEPTION nM-Fraktionskleiderschrank] " + ex.Message);
                Logger.Print("[EXCEPTION nM-Fraktionskleiderschrank] " + ex.StackTrace);
            }
        }

        public static Faction getFactionById(int id)
        {
            Faction fraktion = factionList.FirstOrDefault((Faction frak) => frak.Id == id);
            if (fraktion == null)
                return new Faction
                {
                    Name = "Zivilist",
                    Id = 0,
                    Storage = new Vector3(),
                    Garage = new Vector3(),
                    ClothesFemale = new List<ClothingModel>(),
                    ClothesMale = new List<ClothingModel>(),
                    Blip = 0,
                    GarageSpawn = new Vector3(),
                    Dimension = 0,
                    GarageSpawnRotation = 0,
                    BadFraktion = false,
                    RGB = new Color(0, 0, 0),
                    Short = "Zivilist",
                    Spawn = new Vector3(),
                    Money = 0,
                    Logo = ""
                };
            else
                return fraktion;
        }

        public static Faction getFactionByName(string faction)
        {
            Faction fraktion = factionList.FirstOrDefault((Faction frak) => frak.Name == faction);
            if (fraktion == null)
                return new Faction
                {
                    Name = "Zivilist",
                    Id = 0,
                    Storage = new Vector3(),
                    Garage = new Vector3(),
                    ClothesFemale = new List<ClothingModel>(),
                    ClothesMale = new List<ClothingModel>(),
                    Blip = 0,
                    GarageSpawn = new Vector3(),
                    Dimension = 0,
                    GarageSpawnRotation = 0,
                    BadFraktion = false,
                    RGB = new Color(0, 0, 0),
                    Short = "Zivilist",
                    Spawn = new Vector3(),
                    Money = 0,
                    Logo = ""
                };
            else
                return fraktion;
        }

        [RemoteEvent("nM-Leadershop")]
        public void buyFraktionsCar(Player c, string value)
        {
            try
            {
                if (c == null) return;
                DbPlayer dbPlayer = c.GetPlayer();
                if (dbPlayer == null || !dbPlayer.IsValid(true) || dbPlayer.Client == null)
                    return;

                if (value == null)
                    return;

                try
                {
                    string[] splitted = value.Split("-");
                    string name = splitted[0];
                    int price = 0;
                    bool price2 = int.TryParse(splitted[1], out price);
                    if (!price2) return;

                    if (dbPlayer.Faction.Money >= price)
                    {
                        MySqlQuery query5 = new MySqlQuery("SELECT MAX(Id) as maxId FROM fraktion_vehicles");
                        MySqlResult mySqlReaderCon5 = MySqlHandler.GetQuery(query5);
                        if (!mySqlReaderCon5.Reader.HasRows) return;
                        mySqlReaderCon5.Reader.Read();
                        int id = mySqlReaderCon5.Reader.GetInt32("maxId") + 1;
                        dbPlayer.CloseNativeMenu();
                        dbPlayer.Faction.removeMoney(price);

                        VehicleList[dbPlayer.Faction.Id].Add(new GarageVehicle
                        {
                            Id = id,
                            OwnerID = dbPlayer.Faction.Id,
                            Name = name.ToLower(),
                            Plate = "",
                            Keys = new List<int>() { }
                        });

                        MySqlQuery mySqlQuery =
                            new MySqlQuery("INSERT INTO fraktion_vehicles (FactionId, Model) VALUES (@id, @model)");
                        mySqlQuery.AddParameter("@id", dbPlayer.Faction.Id);
                        mySqlQuery.AddParameter("@model", name);
                        MySqlHandler.ExecuteSync(mySqlQuery);

                        dbPlayer.SendNotification(
                            "Du hast das Fahrzeug " + name + " erfolgreich für deine Fraktion gekauft.", 3000,
                            dbPlayer.Faction.GetRGBStr(), dbPlayer.Faction.Name);

                        mySqlReaderCon5.Reader.Dispose();
                        mySqlReaderCon5.Connection.Dispose();
                    }
                    else
                    {
                        dbPlayer.SendNotification("Es ist zu wenig Geld auf der Fraktionsbank.", 3000,
                            dbPlayer.Faction.GetRGBStr(), dbPlayer.Faction.Name);
                    }
                }
                catch (Exception ex)
                {
                    Logger.Print("[EXCEPTION Leadershop] " + ex.Message);
                    Logger.Print("[EXCEPTION Leadershop] " + ex.StackTrace);
                }
            }
            catch (Exception ex)
            {
                Logger.Print("[EXCEPTION Leadershop] " + ex.Message);
                Logger.Print("[EXCEPTION Leadershop] " + ex.StackTrace);
            }
        }

    }
}