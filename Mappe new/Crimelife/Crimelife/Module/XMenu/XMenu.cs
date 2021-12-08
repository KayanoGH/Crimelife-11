﻿using GTANetworkAPI;
using Crimelife.Handlers;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using GVMP;

namespace Crimelife
{
    class XMenu : Script
    {
        public static void PressedL(DbPlayer dbPlayer)
        {
            try
            {
                Vehicle vehicle = dbPlayer.GetClosestVehicle(20f);

                if (vehicle == null) return;

                DbVehicle dbVehicle = vehicle.GetVehicle();
                if (dbVehicle == null || !dbVehicle.IsValid() || dbVehicle.Vehicle == null)
                    return;

                if (!dbVehicle.Vehicle.HasSharedData("lockedStatus")) return;

                if (dbVehicle.Keys.Contains(dbPlayer.Id) || dbVehicle.OwnerId == dbPlayer.Id || dbPlayer.VehicleKeys.ContainsKey(dbVehicle.Id))
                {
                    dbVehicle.Vehicle.SetSharedData("lockedStatus", !dbVehicle.Vehicle.GetSharedData<bool>("lockedStatus"));
                    dbVehicle.Vehicle.Locked = dbVehicle.Vehicle.GetSharedData<bool>("lockedStatus");
                    dbVehicle.RefreshData(dbVehicle);

                    dbPlayer.SendNotification(
                        "Fahrzeug" + (dbVehicle.Vehicle.GetSharedData<bool>("lockedStatus") == true
                            ? " zugeschlossen!"
                            : " aufgeschlossen!"), 3000,
                        dbVehicle.Vehicle.GetSharedData<bool>("lockedStatus") == true ? "red" : "green");
                }
            }
            catch (Exception ex)
            {
                Logger.Print("[EXCEPTION PRESSEDL VEHICLE] " + ex.Message);
                Logger.Print("[EXCEPTION PRESSEDL VEHICLE] " + ex.StackTrace);
            }
        }

        [RemoteEvent]
        public void REQUEST_PEDS_PLAYER_GIVEKEY(Player client, Player destinationPlayer)
        {
            DbPlayer player = client.GetPlayer();
            DbPlayer player2 = destinationPlayer.GetPlayer();
            if (player != null && player2 != null && player2.IsValid() && !(((Entity)player2.Client).Position.DistanceTo(((Entity)player.Client).Position) > 2.5f))
            {
                Dictionary<string, List<VHKey>> dictionary = new Dictionary<string, List<VHKey>>();
                List<VHKey> ownHouseKey = HouseKeyHandler.Instance.GetOwnHouseKey(player);
                List<VHKey> ownVehicleKeys = VehicleKeyHandler.Instance.GetOwnVehicleKeys(player);
                dictionary.Add("Häuser", ownHouseKey);
                dictionary.Add("Fahrzeuge", ownVehicleKeys);
                new KeyWindow().Show(player, player2.Name, dictionary);
            }
        }

        [RemoteEvent("REQUEST_VEHICLE_INFORMATION")]
        public static void REQUEST_VEHICLE_INFORMATION(Player c, Vehicle veh = null)
        {
            try
            {
                DbPlayer dbPlayer = c.GetPlayer();
                if (dbPlayer == null || !dbPlayer.IsValid(true) || dbPlayer.Client == null)
                    return;

                if (!dbPlayer.CanInteractAntiFlood(2)) return;

                if (veh == null) return;

                DbVehicle dbVehicle = veh.GetVehicle();
                if (dbVehicle == null || !dbVehicle.IsValid() || dbVehicle.Vehicle == null)
                    return;

                string Owner = "";

                if (dbVehicle.OwnerId != 0)
                {
                    MySqlQuery mySqlQuery = new MySqlQuery("SELECT * FROM accounts WHERE Id = @id");
                    mySqlQuery.AddParameter("@id", dbVehicle.OwnerId);
                    MySqlResult mySqlResult = MySqlHandler.GetQuery(mySqlQuery);

                    MySqlDataReader mySqlDataReader = mySqlResult.Reader;

                    while (mySqlDataReader.Read())
                    {
                        Owner = mySqlDataReader.GetString("Username");
                    }

                    mySqlResult.Reader.Dispose();
                    mySqlResult.Connection.Dispose();
                }

                dbPlayer.SendNotification(
                    "Auto-Model: " + dbVehicle.Model + " - ID: " + dbVehicle.Id + " - Besitzer: " + Owner, 7000,
                    "lightblue", "Information");
            }
            catch (Exception ex)
            {
                Logger.Print("[EXCEPTION REQUEST_VEHICLE_INFORMATION] " + ex.Message);
                Logger.Print("[EXCEPTION REQUEST_VEHICLE_INFORMATION] " + ex.StackTrace);
            }
        }

        [RemoteEvent("REQUEST_VEHICLE_TOGGLE_LOCK")]
        public void REQUEST_VEHICLE_TOGGLE_LOCK(Player c)
        {
            try
            {
                DbPlayer dbPlayer = c.GetPlayer();
                if (dbPlayer == null || !dbPlayer.IsValid(true) || dbPlayer.Client == null)
                    return;

                Vehicle veh = c.Vehicle;

                if (veh == null) return;

                DbVehicle dbVehicle = veh.GetVehicle();
                if (dbVehicle == null || !dbVehicle.IsValid() || dbVehicle.Vehicle == null)
                    return;

                if (!dbVehicle.Vehicle.HasSharedData("lockedStatus")) return;

                if (dbVehicle.Vehicle.GetSharedData<bool>("lockedStatus") == null) return;

                if (dbVehicle.Vehicle.GetSharedData<bool>("lockedStatus") is bool)
                {

                }
                else
                {
                    dbVehicle.Vehicle.SetSharedData("lockedStatus", false);
                }

                if (dbVehicle.Keys.Contains(dbPlayer.Id) || dbVehicle.OwnerId == dbPlayer.Id ||
                    dbPlayer.VehicleKeys.ContainsKey(dbVehicle.Id) ||
                    (dbVehicle.Fraktion != null && dbPlayer.Faction.Id == dbVehicle.Fraktion.Id))
                {
                    dbVehicle.Vehicle.SetSharedData("lockedStatus", !dbVehicle.Vehicle.GetSharedData<bool>("lockedStatus"));
                    dbVehicle.Vehicle.Locked = dbVehicle.Vehicle.GetSharedData<bool>("lockedStatus");
                    dbVehicle.RefreshData(dbVehicle);

                    dbPlayer.SendNotification(
                        "Fahrzeug" + (dbVehicle.Vehicle.GetSharedData<bool>("lockedStatus") == true
                            ? " zugeschlossen!"
                            : " aufgeschlossen!"), 3000,
                        dbVehicle.Vehicle.GetSharedData<bool>("lockedStatus") == true ? "red" : "green");
                }
            }
            catch (Exception ex)
            {
                Logger.Print("[EXCEPTION REQUEST_VEHICLE_LOCK] " + ex.Message);
                Logger.Print("[EXCEPTION REQUEST_VEHICLE_LOCK] " + ex.StackTrace);
            }
        }

        [RemoteEvent("REQUEST_VEHICLE_TOGGLE_LOCK_OUTSIDE")]
        public void REQUEST_VEHICLE_TOGGLE_LOCK_OUTSIDE(Player c, Vehicle veh = null)
        {
            try
            {
                DbPlayer dbPlayer = c.GetPlayer();
                if (dbPlayer == null || !dbPlayer.IsValid(true) || dbPlayer.Client == null)
                    return;

                if (veh == null) return;

                DbVehicle dbVehicle = veh.GetVehicle();
                if (dbVehicle == null || !dbVehicle.IsValid() || dbVehicle.Vehicle == null)
                    return;

                if (!dbVehicle.Vehicle.HasSharedData("lockedStatus")) return;

                if (dbVehicle.Vehicle.GetSharedData<bool>("lockedStatus") == null) return;

                if (dbVehicle.Vehicle.GetSharedData<bool>("lockedStatus") is bool)
                {

                }
                else
                {
                    dbVehicle.Vehicle.SetSharedData("lockedStatus", false);
                }

                if (dbVehicle.Keys.Contains(dbPlayer.Id) || dbVehicle.OwnerId == dbPlayer.Id ||
                    dbPlayer.VehicleKeys.ContainsKey(dbVehicle.Id) ||
                    (dbVehicle.Fraktion != null && dbPlayer.Faction.Id == dbVehicle.Fraktion.Id))
                {
                    dbVehicle.Vehicle.SetSharedData("lockedStatus", !dbVehicle.Vehicle.GetSharedData<bool>("lockedStatus"));
                    dbVehicle.Vehicle.Locked = dbVehicle.Vehicle.GetSharedData<bool>("lockedStatus");
                    dbVehicle.RefreshData(dbVehicle);

                    
                    if (dbVehicle.Vehicle.GetSharedData<bool>("lockedStatus") == false)
                    {
                        dbPlayer.SendNotification("Fahrzeug aufgeschlossen", 3000, "green", "");
                    }
                    else
                    {
                        dbPlayer.SendNotification("Fahrzeug zugeschlossen", 3000, "red", "");
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Print("[EXCEPTION REQUEST_VEHICLE_OUTSIDE_LOCK] " + ex.Message);
                Logger.Print("[EXCEPTION REQUEST_VEHICLE_OUTSIDE_LOCK] " + ex.StackTrace);
            }
        }

        [RemoteEvent("REQUEST_VEHICLE_TOGGLE_DOOR")]
        public void REQUEST_VEHICLE_TOGGLE_DOOR(Player c)
        {
            try
            {
                DbPlayer dbPlayer = c.GetPlayer();
                if (dbPlayer == null || !dbPlayer.IsValid(true) || dbPlayer.Client == null)
                    return;

                Vehicle veh = c.Vehicle;

                if (veh == null) return;

                DbVehicle dbVehicle = veh.GetVehicle();
                if (dbVehicle == null || !dbVehicle.IsValid() || dbVehicle.Vehicle == null)
                    return;

                if (!dbVehicle.Vehicle.HasSharedData("lockedStatus")) return;

                if (dbVehicle.Vehicle.GetSharedData<bool>("lockedStatus") is bool)
                {

                }
                else
                {
                    dbVehicle.Vehicle.SetSharedData("lockedStatus", false);
                }

                if (dbVehicle.Vehicle.GetSharedData<bool>("lockedStatus") == true)
                {
                    dbPlayer.SendNotification("Das Fahrzeug ist abgeschlossen.", 3000, "red");
                    return;
                }

                if (!dbVehicle.Vehicle.HasSharedData("kofferraumStatus")) return;

                if (dbVehicle.Vehicle.GetSharedData<bool>("kofferraumStatus") == null) return;

                if (dbVehicle.Vehicle.GetSharedData<bool>("kofferraumStatus") is bool)
                {

                }
                else
                {
                    dbVehicle.Vehicle.SetSharedData("kofferraumStatus", false);
                }

                dbVehicle.Vehicle.SetSharedData("kofferraumStatus", !dbVehicle.Vehicle.GetSharedData<bool>("kofferraumStatus"));
                dbVehicle.RefreshData(dbVehicle);

                dbPlayer.SendNotification("Kofferaum" + (dbVehicle.Vehicle.GetSharedData<bool>("kofferraumStatus") == true ? " zugeschlossen!" : " aufgeschlossen!"), 3000, dbVehicle.Vehicle.GetSharedData<bool>("kofferraumStatus") == true ? "red" : "green");
            }
            catch (Exception ex)
            {
                Logger.Print("[EXCEPTION REQUEST_VEHICLE_DOOR_LOCK] " + ex.Message);
                Logger.Print("[EXCEPTION REQUEST_VEHICLE_DOOR_LOCK] " + ex.StackTrace);
            }
        }

        [RemoteEvent("REQUEST_VEHICLE_TOGGLE_DOOR_OUTSIDE")]
        public void REQUEST_VEHICLE_TOGGLE_DOOR_OUTSIDE(Player c, Vehicle veh = null)
        {
            try
            {
                DbPlayer dbPlayer = c.GetPlayer();
                if (dbPlayer == null || !dbPlayer.IsValid(true) || dbPlayer.Client == null)
                    return;

                if (veh == null) return;

                DbVehicle dbVehicle = veh.GetVehicle();
                if (dbVehicle == null || !dbVehicle.IsValid() || dbVehicle.Vehicle == null)
                    return;

                if (!dbVehicle.Vehicle.HasSharedData("lockedStatus")) return;

                if (dbVehicle.Vehicle.GetSharedData<string>("lockedStatus") is bool)
                {

                }
                else
                {
                    dbVehicle.Vehicle.SetSharedData("lockedStatus", false);
                }

                if (dbVehicle.Vehicle.GetSharedData<bool>("lockedStatus") == true)
                {
                    dbPlayer.SendNotification("Das Fahrzeug ist abgeschlossen.", 3000, "red");
                    return;
                }

                if (!dbVehicle.Vehicle.HasSharedData("kofferraumStatus")) return;

                if (dbVehicle.Vehicle.GetSharedData<bool>("kofferraumStatus") == null) return;

                if (dbVehicle.Vehicle.GetSharedData<bool>("kofferraumStatus") is bool)
                {

                }
                else
                {
                    dbVehicle.Vehicle.SetSharedData("kofferraumStatus", false);
                }

                dbVehicle.Vehicle.SetSharedData("kofferraumStatus", !dbVehicle.Vehicle.GetSharedData<bool>("kofferraumStatus"));
                dbVehicle.RefreshData(dbVehicle);

                dbPlayer.SendNotification("Kofferaum" + (dbVehicle.Vehicle.GetSharedData<bool>("kofferraumStatus") == true ? " zugeschlossen!" : " aufgeschlossen!"), 3000, dbVehicle.Vehicle.GetSharedData<bool>("kofferraumStatus") == true ? "red" : "green");
            }
            catch (Exception ex)
            {
                Logger.Print("[EXCEPTION REQUEST_VEHICLE_OUTSIDE_DOOR_LOCK] " + ex.Message);
                Logger.Print("[EXCEPTION REQUEST_VEHICLE_OUTSIDE_DOOR_LOCK] " + ex.StackTrace);
            }
        }

        [RemoteEvent("REQUEST_VEHICLE_REPAIR")]
        public void REQUEST_VEHICLE_REPAIR(Player c, Vehicle veh = null)
        {
            try
            {
                if (c == null || !c.Exists) return;

                DbPlayer dbPlayer = c.GetPlayer();
                if (dbPlayer == null || !dbPlayer.IsValid(true) || dbPlayer.Client == null)
                    return;

                if (veh == null || dbPlayer.DeathData.IsDead) return;

                DbVehicle dbVehicle = veh.GetVehicle();
                if (dbVehicle == null || !dbVehicle.IsValid() || dbVehicle.Vehicle == null)
                    return;

                dbPlayer.disableAllPlayerActions(true);
                dbPlayer.SendProgressbar(5000);
                dbPlayer.PlayAnimation(33, "mini@repair", "fixing_a_player", 8f);
                NAPI.Task.Run(() =>
                {
                    dbPlayer.StopAnimation();
                    dbPlayer.disableAllPlayerActions(false);
                    dbPlayer.StopProgressbar();
                    veh.Repair();
                    dbPlayer.SendNotification("Fahrzeug repariert!", 3000, "green");
                }, 5000);
            }
            catch (Exception ex)
            {
                Logger.Print("[EXCEPTION REQUEST_VEHICLE_REPAIR] " + ex.Message);
                Logger.Print("[EXCEPTION REQUEST_VEHICLE_REPAIR] " + ex.StackTrace);
            }
        }

        [RemoteEvent("REQUEST_VEHICLE_TOGGLE_ENGINE")]
        public void REQUEST_VEHICLE_TOGGLE_ENGINE(Player c)
        {
            try
            {
                if (c == null) return;

                DbPlayer dbPlayer = c.GetPlayer();
                if (dbPlayer == null || !dbPlayer.IsValid(true) || dbPlayer.Client == null)
                    return;

                Vehicle veh = c.Vehicle;
                if (veh == null || c.VehicleSeat != 0) return;
                DbVehicle dbVehicle = veh.GetVehicle();
                if (dbVehicle == null || !dbVehicle.IsValid() || dbVehicle.Vehicle == null)
                    return;
                if (!dbVehicle.Vehicle.HasSharedData("engineStatus")) return;
                if (dbVehicle.Vehicle.GetSharedData<bool>("engineStatus") == null) return;
                if (dbVehicle.Vehicle.GetSharedData<bool>("engineStatus") is bool)
                {
                    //Console.WriteLine("CRASH");
                    //return;
                }
                else
                {
                    dbVehicle.Vehicle.SetSharedData("engineStatus", false);
                }
                Console.WriteLine("6");
                if (dbVehicle.Keys.Contains(dbPlayer.Id) || dbVehicle.OwnerId == dbPlayer.Id ||
                    dbPlayer.VehicleKeys.ContainsKey(dbVehicle.Id) ||
                    (dbVehicle.Fraktion != null && dbPlayer.Faction.Id == dbVehicle.Fraktion.Id))
                {
                    dbVehicle.Vehicle.SetSharedData("engineStatus", !dbVehicle.Vehicle.GetSharedData<bool>("engineStatus"));
                    dbVehicle.Vehicle.EngineStatus = dbVehicle.Vehicle.GetSharedData<bool>("engineStatus");
                    dbVehicle.RefreshData(dbVehicle);
                    Console.WriteLine("7");
                    dbPlayer.SendNotification(
                        "Motor" + (dbVehicle.Vehicle.GetSharedData<bool>("engineStatus")
                            ? " angeschaltet!"
                            : " ausgeschaltet!"), 3000,
                        dbVehicle.Vehicle.GetSharedData<bool>("engineStatus") == true ? "green" : "red");
                    Console.WriteLine("8");
                }
            }
            catch (Exception ex)
            {
                Logger.Print("[EXCEPTION REQUEST_VEHICLE_ENGINE] " + ex.Message);
                Logger.Print("[EXCEPTION REQUEST_VEHICLE_ENGINE] " + ex.StackTrace);
            }
        }
    }
}
