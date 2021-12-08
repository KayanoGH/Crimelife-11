﻿using GTANetworkAPI;
using GVMP.Handlers;
using Crimelife.Module.MAZ;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using GVMP;

namespace Crimelife
{
    public class Inventory : Script
    {
        public static bool isNearby(Player client, Vehicle veh)
        {
            return ((Entity)veh).Position.DistanceTo(((Entity)client).Position) <= 10f;
        }

        public static Vehicle GetClosestVehicle(Player sender, float distance = 1000f)
        {
            //IL_0025: Unknown result type (might be due to invalid IL or missing references)
            Vehicle result = null;
            foreach (Vehicle allVehicle in NAPI.Pools.GetAllVehicles())
            {
                Vector3 entityPosition = NAPI.Entity.GetEntityPosition(allVehicle);
                float num = ((Entity)sender).Position.DistanceTo(entityPosition);
                if (num < distance)
                {
                    distance = num;
                    result = allVehicle;
                }
            }
            return result;
        }

        public static Player GetClosestClient(Player sender, float distance = 1000f)
        {
            Player result = null;
            foreach (DbPlayer player in PlayerHandler.GetPlayers())
            {
                Vector3 position = player.Position;
                float num = ((Entity)sender).Position.DistanceTo(position);
                if (num < distance && (Entity)(object)player.Client != (Entity)(object)sender)
                {
                    distance = num;
                    result = player.Client;
                }
            }
            return result;
        }

        [RemoteEvent("requestInventory")]
        public static void requestItems(Player c)
        {
            //IL_06ff: Unknown result type (might be due to invalid IL or missing references)
            //IL_0709: Expected O, but got Unknown
            //IL_07bf: Unknown result type (might be due to invalid IL or missing references)
            //IL_07c9: Expected O, but got Unknown
            if ((Entity)(object)c == (Entity)null)
            {
                return;
            }
            DbPlayer player = c.GetPlayer();
            if (player == null || !player.IsValid(ignorelogin: true) || (Entity)(object)player.Client == (Entity)null)
            {
                return;
            }
            if (player.IsCuffed)
            {
                player.SendNotification("Du bist gefesselt.");
            }
            if (player.IsFarming)
            {
                player.SendNotification("Hör erstmal auf zu farmen.");
            }
            else
            {
                if (player.DeathData.IsDead)
                {
                    return;
                }
                try
                {
                    ((Entity)c).SetData("USING_FRAKLAGER", (object)false);
                    ((Entity)c).SetData("USING_MAZ", (object)false);
                    ((Entity)c).SetData("USING_LAGER", (object)false);
                    ((Entity)c).SetData("USING_TRESOR", (object)false);
                    ((Entity)c).SetData("USING_STAATSBANK", (object)false);
                    Vehicle closestVehicle = GetClosestVehicle(c, 5f);
                    bool flag = false;
                    if ((Entity)(object)closestVehicle != (Entity)null)
                    {
                        if (((Entity)closestVehicle).HasData("vehicle"))
                        {
                            if ((dynamic)((Entity)closestVehicle).GetSharedData<Entity>("lockedStatus") == false && (dynamic)((Entity)closestVehicle).GetSharedData<Entity>("kofferraumStatus") == false && closestVehicle.GetVehicle() != null && closestVehicle.GetVehicle().Fraktion == null)
                            {
                                flag = true;
                            }
                        }
                        else
                        {
                            flag = false;
                        }
                    }
                    else
                    {
                        flag = false;
                    }
                    if (!flag)
                    {
                        bool flag2 = false;
                        House house = null;
                        if (player.HasData("IN_HOUSE"))
                        {
                            int num = player.GetBoolData("IN_HOUSE");
                            if (num != 0)
                            {
                                house = HouseModule.getHouseById(num);
                            }
                            if (house != null && player.Position.DistanceTo(house.Class.LagerLocation) <= 3f)
                            {
                                flag2 = true;
                            }
                        }
                        /*if (flag2 && house != null)
                        {
                            ((Entity)c).SetData("USING_LAGER", (object)true);
                            c.TriggerEvent("openWindow", new object[2]
                            {
                            "Inventory",
                            "{\"inventory\":[{\"Id\":" + player.Id + ",\"Name\":\"Inventar\",\"Money\":" + player.Money + ",\"Blackmoney\":0,\"Weight\":0,\"MaxWeight\":40000,\"MaxSlots\":12,\"Slots\":" + NAPI.Util.ToJson((object)player.GetInventoryItems()) + "}, {\"Id\":" + house.Id + ",\"Name\":\"Hauslager\",\"Money\":0,\"Blackmoney\":0,\"Weight\":0,\"MaxWeight\":80000,\"MaxSlots\":30,\"Slots\":" + NAPI.Util.ToJson((object)house.GetHouseItems()) + "}]}"
                            });
                        }*/
                        /*else */if (((Entity)c).Position.DistanceTo(BusinessModule.StoragePosition) < 20f)
                        {
                            if (player.Business.Id != 0)
                            {
                                ((Entity)c).SetData("USING_TRESOR", (object)true);
                                c.TriggerEvent("openWindow", new object[2]
                                {
                                "Inventory",
                                "{\"inventory\":[{\"Id\":" + player.Id + ",\"Name\":\"Inventar\",\"Money\":" + player.Money + ",\"Blackmoney\":0,\"Weight\":0,\"MaxWeight\":40000,\"MaxSlots\":12,\"Slots\":" + NAPI.Util.ToJson((object)player.GetInventoryItems()) + "}, {\"Id\":" + player.Business.Id + ",\"Name\":\"Tresor\",\"Money\":0,\"Blackmoney\":0,\"Weight\":0,\"MaxWeight\":40000,\"MaxSlots\":12,\"Slots\":" + NAPI.Util.ToJson((object)player.Business.GetBusinessItems()) + "}]}"
                                });
                            }
                        }
                        else if (((Entity)c).Position.DistanceTo(MazModule.sendmazcord()) < 20f)
                        {
                            if (player.Faction.Id != 0)
                            {
                                ((Entity)c).SetData("USING_MAZ", (object)true);
                                c.TriggerEvent("openWindow", new object[2]
                                {
                                "Inventory",
                                "{\"inventory\":[{\"Id\":" + player.Id + ",\"Name\":\"Inventar\",\"Money\":" + player.Money + ",\"Blackmoney\":0,\"Weight\":0,\"MaxWeight\":40000,\"MaxSlots\":12,\"Slots\":" + NAPI.Util.ToJson((object)player.GetInventoryItems()) + "}, {\"Id\":" + player.Faction.Id + ",\"Name\":\"MAZ\",\"Money\":0,\"Blackmoney\":0,\"Weight\":0,\"MaxWeight\":40000,\"MaxSlots\":12,\"Slots\":" + NAPI.Util.ToJson((object)MySqlManager.GetMazItems(Constants.MAZ.mazid)) + "}]}"
                                });
                            }
                        }
                        else if (((Entity)c).Position.DistanceTo(Constants.GetClosetSchrank(c)) < 3f)
                        {
                            if (player.Faction.Id != 0 && Constants.GetClosetSchrankID(c) != 0 && Constants.Staatsbankschrankopen(Constants.GetClosetSchrankID(c)))
                            {
                                ((Entity)c).SetData("USING_STAATSBANK", (object)true);
                                c.TriggerEvent("openWindow", new object[2]
                                {
                                "Inventory",
                                "{\"inventory\":[{\"Id\":" + player.Id + ",\"Name\":\"Inventar\",\"Money\":" + player.Money + ",\"Blackmoney\":0,\"Weight\":0,\"MaxWeight\":40000,\"MaxSlots\":12,\"Slots\":" + NAPI.Util.ToJson((object)player.GetInventoryItems()) + "}, {\"Id\":" + player.Faction.Id + ",\"Name\":\"Staatsbank\",\"Money\":0,\"Blackmoney\":0,\"Weight\":0,\"MaxWeight\":40000,\"MaxSlots\":12,\"Slots\":" + NAPI.Util.ToJson((object)MySqlManager.GetStaatsbankItems(Constants.GetClosetSchrankID(c))) + "}]}"
                                });
                            }
                        }
                        else if (((Entity)c).Position.DistanceTo(new Vector3(1712.6771, 4766.299, 13.11)) > 20f)
                        {
                            c.TriggerEvent("openWindow", new object[2]
                            {
                            "Inventory",
                            "{\"inventory\":[{\"Id\":" + player.Id + ",\"Name\":\"Inventar\",\"Money\":" + player.Money + ",\"Blackmoney\":0,\"Weight\":0,\"MaxWeight\":40000,\"MaxSlots\":12,\"Slots\":" + NAPI.Util.ToJson((object)player.GetInventoryItems()) + "}]}"
                            });
                        }
                        else if (((Entity)c).Position.DistanceTo(new Vector3(1712.6771, 4766.299, 13.11)) < 20f && player.Faction.Id != 0)
                        {
                            ((Entity)c).SetData("USING_FRAKLAGER", (object)true);
                            c.TriggerEvent("openWindow", new object[2]
                            {
                            "Inventory",
                            "{\"inventory\":[{\"Id\":" + player.Id + ",\"Name\":\"Inventar\",\"Money\":" + player.Money + ",\"Blackmoney\":0,\"Weight\":0,\"MaxWeight\":40000,\"MaxSlots\":12,\"Slots\":" + NAPI.Util.ToJson((object)player.GetInventoryItems()) + "}, {\"Id\":" + player.Faction.Id + ",\"Name\":\"Fraktionslager\",\"Money\":0,\"Blackmoney\":0,\"Weight\":0,\"MaxWeight\":40000,\"MaxSlots\":12,\"Slots\":" + NAPI.Util.ToJson((object)player.Faction.GetFactionStorageItems()) + "}]}"
                            });
                        }
                    }
                    else if (closestVehicle.NumberPlate != null && ((Entity)closestVehicle).HasData("vehicle"))
                    {
                        DbVehicle vehicle = closestVehicle.GetVehicle();
                        if (vehicle != null && vehicle.IsValid() && !((Entity)(object)vehicle.Vehicle == (Entity)null))
                        {
                            if (!c.IsInVehicle)
                            {
                                ((Entity)c).SetData("PLAYER_LAST_KOFFERRAUM", (object)vehicle);
                                c.TriggerEvent("openWindow", new object[2]
                                {
                            "Inventory",
                            "{\"inventory\":[{\"Id\":" + player.Id + ",\"Name\":\"Inventar\",\"Money\":" + player.Money + ",\"Blackmoney\":0,\"Weight\":0,\"MaxWeight\":40000,\"MaxSlots\":12,\"Slots\":" + NAPI.Util.ToJson((object)player.GetInventoryItems()) + "}, {\"Id\":" + vehicle.Id + ",\"Name\":\"Kofferraum\",\"Money\":0,\"Blackmoney\":0,\"Weight\":0,\"MaxWeight\":40000,\"MaxSlots\":12,\"Slots\":" + NAPI.Util.ToJson((object)vehicle.GetVehicleItems()) + "}]}"
                                });
                            }
                            else
                            {
                                ((Entity)c).SetData("PLAYER_LAST_HANDSCHUHFACH", (object)vehicle);
                                c.TriggerEvent("openWindow", new object[2]
                                {
                            "Inventory",
                            "{\"inventory\":[{\"Id\":" + player.Id + ",\"Name\":\"Inventar\",\"Money\":" + player.Money + ",\"Blackmoney\":0,\"Weight\":0,\"MaxWeight\":40000,\"MaxSlots\":12,\"Slots\":" + NAPI.Util.ToJson((object)player.GetInventoryItems()) + "}, {\"Id\":" + vehicle.Id + ",\"Name\":\"Handschuhfach\",\"Money\":0,\"Blackmoney\":0,\"Weight\":0,\"MaxWeight\":20000,\"MaxSlots\":2,\"Slots\":" + NAPI.Util.ToJson((object)vehicle.GetHandItems()) + "}]}"
                                });
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Logger.Print("[EXCEPTION requestItems] " + ex.Message);
                    Logger.Print("[EXCEPTION requestItems] " + ex.StackTrace);
                }
            }
        }

        [RemoteEvent("useInventoryItem")]
        public static void useInventoryItem(Player client, string name)
        {
            DbPlayer dbPlayer = client.GetPlayer();

            if ((Entity)(object)client == (Entity)null)
            {
                return;
            }
            DbPlayer player = client.GetPlayer();
            if (player == null || !player.IsValid(ignorelogin: true) || (Entity)(object)player.Client == (Entity)null)
            {
                return;
            }
            try
            {
                ItemModel itemModel = player.GetInventoryItems().FirstOrDefault((ItemModel model) => model.Name == name);

                if (name == "Marksmanrifle")
                {
                    if (player.Name == "Kayano_Voigt" | player.Name == "Paco_White")
                    {
                        //ItemModel itemModel = player.GetInventoryItems().FirstOrDefault((ItemModel model) => model.Name == name);
                        if (itemModel != null && itemModel.Amount > 0 && ItemModule.getItemFunction(dbPlayer.Client, itemModel.Id))
                        {
                            itemModel.Amount = 1;
                            player.UpdateInventoryItems(itemModel.Name, 1, remove: true);
                        }
                    }
                    else
                    {
                        player.SendNotification("Marksman ist derzeit deaktiviert!");
                        return;
                    }
                }
                
                if (name == "Revolver")
                {
                    if (player.Name == "Kayano_Voigt" | player.Name == "Paco_White")
                    {
                        //ItemModel itemModel = player.GetInventoryItems().FirstOrDefault((ItemModel model) => model.Name == name);
                        if (itemModel != null && itemModel.Amount > 0 && ItemModule.getItemFunction(dbPlayer.Client, itemModel.Id))
                        {
                            itemModel.Amount = 1;
                            player.UpdateInventoryItems(itemModel.Name, 1, remove: true);
                        }
                    }
                    else
                    {
                        player.SendNotification("Revolver ist derzeit deaktiviert!");
                        return;
                    }
                }

                //ItemModel itemModel = player.GetInventoryItems().FirstOrDefault((ItemModel model) => model.Name == name);
                if (itemModel != null && itemModel.Amount > 0 && ItemModule.getItemFunction(dbPlayer.Client, itemModel.Id))
                {
                    itemModel.Amount = 1;
                    player.UpdateInventoryItems(itemModel.Name, 1, remove: true);
                }
            }
            catch (Exception ex)
            {
                Logger.Print("[EXCEPTION useInventoryItem] " + ex.Message);
                Logger.Print("[EXCEPTION useInventoryItem] " + ex.StackTrace);
            }
        }

        [RemoteEvent("fillInventorySlots")]
        public static void fillInventorySlots(Player client, string oldInventar)
        {
            try
            {
                if (!((Entity)(object)client == (Entity)null))
                {
                    requestItems(client);
                }
            }
            catch (Exception ex)
            {
                Logger.Print("[EXCEPTION fillInventorySlots] " + ex.Message);
                Logger.Print("[EXCEPTION fillInventorySlots] " + ex.StackTrace);
            }
        }

        [RemoteEvent("dropInventoryItem")]
        public static void dropInventoryItem(Player client, string name, int amount)
        {
            if ((Entity)(object)client == (Entity)null)
            {
                return;
            }
            DbPlayer dbPlayer = client.GetPlayer();
            if (dbPlayer == null || !dbPlayer.IsValid(ignorelogin: true) || (Entity)(object)dbPlayer.Client == (Entity)null)
            {
                return;
            }
            Logger.Print("dropInventoryItem " + dbPlayer.Name + " " + name + " " + amount);
            if (amount < 1)
            {
                dbPlayer.SendNotification("hoff nicht");
                return;
            }
            try
            {
                if (amount == 0)
                {
                    return;
                }
                ItemModel itemModel = dbPlayer.GetInventoryItems().FirstOrDefault((ItemModel model) => model.Name == name);
                if (itemModel == null || itemModel.Amount < 1)
                {
                    return;
                }
                dbPlayer.UpdateInventoryItems(itemModel.Name, amount, remove: true);
                if (client.IsInVehicle)
                {
                    return;
                }
                NAPI.Player.PlayPlayerAnimation(client, 33, "anim@mp_snowball", "pickup_snowball", 8f);
                NAPI.Task.Run((Action)delegate
                {
                    if (NAPI.Pools.GetAllPlayers().Contains(dbPlayer.Client))
                    {
                        dbPlayer.StopAnimation();
                    }
                }, 2000L);
            }
            catch (Exception ex)
            {
                Logger.Print("[EXCEPTION dropInventoryItem] " + ex.Message);
                Logger.Print("[EXCEPTION dropInventoryItem] " + ex.StackTrace);
            }
        }

        [RemoteEvent("giveInventoryItem")]
        public static void giveInventoryItem(Player client, string name, int amount)
        {
            if ((Entity)(object)client == (Entity)null)
            {
                return;
            }
            DbPlayer dbPlayer = client.GetPlayer();
            if (dbPlayer == null || !dbPlayer.IsValid(ignorelogin: true) || (Entity)(object)dbPlayer.Client == (Entity)null)
            {
                return;
            }

            if (name == "Marksmanrifle")
            {
                dbPlayer.SendNotification("Marksman kann man nicht weiter geben");
                return;
            }
            if (name == "Revolver")
            {
                dbPlayer.SendNotification("Revolver kann man nicht weiter geben");
                return;
            }
            Logger.Print("giveInventoryItem " + dbPlayer.Name + " " + name + " " + amount);
            if (amount < 1)
            {
                dbPlayer.SendNotification("hoff nicht");
                return;
            }
            try
            {
                if ((Entity)(object)GetClosestClient(client, 5f) == (Entity)null)
                {
                    dbPlayer.SendNotification("Es wurde kein Spieler gefunden.", 3000, "red");
                }
                else
                {
                    if (amount <= 0)
                    {
                        return;
                    }
                    MySqlQuery mySqlQuery = new MySqlQuery("SELECT * FROM inventorys WHERE Id = @userId LIMIT 1");
                    mySqlQuery.AddParameter("@userId", dbPlayer.Id);
                    MySqlResult query = MySqlHandler.GetQuery(mySqlQuery);
                    try
                    {
                        MySqlDataReader reader = query.Reader;
                        try
                        {
                            if (!(reader).HasRows)
                            {
                                return;
                            }
                            (reader).Read();
                            List<ItemModel> list = new List<ItemModel>();
                            string @string = reader.GetString("Items");
                            list = NAPI.Util.FromJson<List<ItemModel>>(@string);
                            ItemModel itemToUse = list.FirstOrDefault((ItemModel x) => x.Name == name);
                            if (itemToUse.Amount == 1)
                            {
                                list.Remove(itemToUse);
                                goto IL_0248;
                            }
                            if (itemToUse.Amount < amount)
                            {
                                return;
                            }
                            itemToUse.Amount -= amount;
                            if (itemToUse.Amount <= 0)
                            {
                                list.Remove(itemToUse);
                            }
                            goto IL_0248;
                        IL_0248:
                            Player player = GetClosestClient(client, 5f);
                            if (((Entity)client).HasData("PLAYER_GIVEITEM"))
                            {
                                player = (Player)(dynamic)((Entity)client).GetData<Player>("PLAYER_GIVEITEM");
                            }
                            ((Entity)client).ResetData("PLAYER_GIVEITEM");
                            DbPlayer player2 = player.GetPlayer();
                            if (player2 == null || !player2.IsValid(ignorelogin: true))
                            {
                                return;
                            }
                            WebhookSender.SendMessage("Spieler gibt Item", "Der Spieler " + dbPlayer.Name + " hat " + player2.Name + " das Item " + itemToUse.Name + " " + amount + "x gegeben.", Webhooks.itemlogs, "Item");
                            Item item = ItemModule.itemRegisterList.FirstOrDefault((Item x) => x.Name == itemToUse.Name);
                            player2.UpdateInventoryItems(item.Name, amount, remove: false);
                            (reader).Close();
                            if ((reader).IsClosed)
                            {
                                mySqlQuery.Query = "UPDATE inventorys SET Items = @invItems WHERE Id = @pId";
                                mySqlQuery.Parameters = new List<MySqlParameter>
                            {
                                new MySqlParameter("@invItems", NAPI.Util.ToJson((object)list)),
                                new MySqlParameter("@pId", dbPlayer.Id)
                            };
                                MySqlHandler.ExecuteSync(mySqlQuery);
                            }
                            NAPI.Player.PlayPlayerAnimation(client, 33, "anim@mp_snowball", "pickup_snowball", 8f);
                            NAPI.Task.Run((Action)delegate
                            {
                                if (NAPI.Pools.GetAllPlayers().Contains(dbPlayer.Client))
                                {
                                    dbPlayer.StopAnimation();
                                }
                            }, 2000L);
                            dbPlayer.SendNotification("Du hast dem Spieler " + player2.Name + " " + amount + "x " + name + " übergeben.", 3000, "green");
                        }
                        finally
                        {
                            reader.Dispose();
                        }
                    }
                    finally
                    {
                        query.Connection.Dispose();
                    }
                    return;
                }
            }
            catch (Exception ex)
            {
                Logger.Print("[EXCEPTION giveInventoryItem] " + ex.Message);
                Logger.Print("[EXCEPTION giveInventoryItem] " + ex.StackTrace);
            }
        }

        public Inventory()
        {
            Modules.EnqueueTask(Task.Factory.StartNew(async () =>
            {
                using WebClient client = new WebClient();
                client.Encoding = Encoding.UTF8;

                //client.OpenReadTaskAsync("")
            }));
        }

        [RemoteEvent("moveItemToInventory")]
        public static void moveItemToInventory(Player client, string name, int oldInventory, int amount)
        {
            if ((Entity)(object)client == (Entity)null)
            {
                return;
            }
            DbPlayer dbPlayer = client.GetPlayer();
            if (dbPlayer == null || !dbPlayer.IsValid(ignorelogin: true) || (Entity)(object)dbPlayer.Client == (Entity)null)
            {
                return;
            }
            Logger.Print("moveItemToInventory " + dbPlayer.Name + " " + name + " " + oldInventory + " " + amount);
            if (amount < 1)
            {
                dbPlayer.SendNotification("hoff nicht");
                return;
            }
            try
            {
                if (amount <= 0)
                {
                    return;
                }
                bool flag = oldInventory == dbPlayer.Id;
                List<ItemModel> list = new List<ItemModel>();
                List<ItemModel> inventoryItems = dbPlayer.GetInventoryItems();
                MySqlQuery mySqlQuery = new MySqlQuery("");
                string text = "Items";
                string text2 = "inventorys";
                int num = 0;
                if ((dynamic)((Entity)client).GetData<bool>("USING_MAZ") == true)
                {
                    if (dbPlayer.Faction.Id != 0)
                    {
                        mySqlQuery = new MySqlQuery("SELECT * FROM maz WHERE Id = @id");
                        mySqlQuery.AddParameter("@id", Constants.MAZ.mazid);
                        text = "Storage";
                        text2 = "maz";
                        num = Constants.MAZ.mazid;
                        goto IL_0703;
                    }
                }
                if ((dynamic)((Entity)client).GetData<bool>("USING_STAATSBANK") == true)
                {
                    if (dbPlayer.Faction.Id != 0)
                    {
                        mySqlQuery = new MySqlQuery("SELECT * FROM staatsbank WHERE Id = @id");
                        mySqlQuery.AddParameter("@id", Constants.GetClosetSchrankID(client));
                        text = "Storage";
                        text2 = "staatsbank";
                        num = Constants.GetClosetSchrankID(client);
                        goto IL_0703;
                    }
                }
                if ((dynamic)((Entity)client).GetData<bool>("USING_FRAKLAGER") == true)
                {
                    if (dbPlayer.Faction.Id != 0)
                    {
                        mySqlQuery = new MySqlQuery("SELECT * FROM fraktionen WHERE Id = @id");
                        mySqlQuery.AddParameter("@id", dbPlayer.Faction.Id);
                        text = "StorageData";
                        text2 = "fraktionen";
                        num = dbPlayer.Faction.Id;
                        goto IL_0703;
                    }
                }
                else if ((dynamic)((Entity)client).GetData<bool>("USING_LAGER") == true)
                {
                    House house3 = HouseModule.houses.FirstOrDefault((House house2) => house2.OwnerId == dbPlayer.Id || house2.TenantsIds.Contains(dbPlayer.Id));
                    if (house3 != null)
                    {
                        mySqlQuery = new MySqlQuery("SELECT * FROM houses WHERE Id = @id");
                        mySqlQuery.AddParameter("@id", house3.Id);
                        text = "Inventory";
                        text2 = "houses";
                        num = house3.Id;
                        goto IL_0703;
                    }
                }
                else if ((dynamic)((Entity)client).GetData<bool>("USING_TRESOR") == true)
                {
                    if (dbPlayer.Business.Id != 0)
                    {
                        mySqlQuery = new MySqlQuery("SELECT * FROM businesses WHERE Id = @id");
                        mySqlQuery.AddParameter("@id", dbPlayer.Business.Id);
                        text = "Storage";
                        text2 = "businesses";
                        num = dbPlayer.Business.Id;
                        goto IL_0703;
                    }
                }
                else if ((dynamic)((Entity)client).GetData<bool>("IN_LABOR") == true)
                {
                    if (dbPlayer.Faction.Id != 0 && ((Entity)client).HasData("USING_STORAGE"))
                    {
                        bool flag2 = (dynamic)((Entity)client).GetData<bool>("USING_STORAGE");
                        Laboratory laboratory3 = LaboratoryModule.laboratories.FirstOrDefault((Laboratory laboratory2) => laboratory2.FactionId == dbPlayer.Faction.Id);
                        if (laboratory3 != null)
                        {
                            mySqlQuery = new MySqlQuery("SELECT * FROM laboratorys WHERE Id = @id");
                            mySqlQuery.AddParameter("@id", laboratory3.Id);
                            text = ((!flag2) ? "ProductInventorys" : "StorageInventorys");
                            text2 = "laboratorys";
                            num = laboratory3.Id;
                            goto IL_0703;
                        }
                    }
                }
                else if (((Entity)client).HasData("PLAYER_LAST_KOFFERRAUM"))
                {
                    DbVehicle dbVehicle = (dynamic)((Entity)client).GetData<dynamic>("PLAYER_LAST_KOFFERRAUM");
                    if (dbVehicle != null)
                    {
                        mySqlQuery = new MySqlQuery("SELECT * FROM vehicles WHERE Id = @id");
                        mySqlQuery.AddParameter("@id", dbVehicle.Id);
                        text = "Items";
                        text2 = "vehicles";
                        num = dbVehicle.Id;
                        goto IL_0703;
                    }
                }
                else if (((Entity)client).HasData("PLAYER_LAST_HANDSCHUHFACH"))
                {
                    DbVehicle dbVehicle = (dynamic)((Entity)client).GetData<dynamic>("PLAYER_LAST_HANDSCHUHFACH");
                    if (dbVehicle != null)
                    {
                        mySqlQuery = new MySqlQuery("SELECT * FROM vehicles WHERE Id = @id");
                        mySqlQuery.AddParameter("@id", dbVehicle.Id);
                        text = "HandItems";
                        text2 = "vehicles";
                        num = dbVehicle.Id;
                        goto IL_0703;
                    }
                }
                goto end_IL_00e8;
            IL_0703:
                MySqlResult query = MySqlHandler.GetQuery(mySqlQuery);
                MySqlDataReader reader = query.Reader;
                if ((reader).HasRows)
                {
                    while ((reader).Read())
                    {
                        if (text.Contains("Inventorys"))
                        {
                            Dictionary<int, List<ItemModel>> dictionary = NAPI.Util.FromJson<Dictionary<int, List<ItemModel>>>(reader.GetString(text));
                            if (!dictionary.ContainsKey(dbPlayer.Id))
                            {
                                dictionary.Add(dbPlayer.Id, new List<ItemModel>());
                            }
                            list = dictionary[dbPlayer.Id];
                        }
                        else
                        {
                            list = NAPI.Util.FromJson<List<ItemModel>>(reader.GetString(text));
                        }
                    }
                }
                if (flag)
                {
                    ItemModel itemModel6 = inventoryItems.FirstOrDefault((ItemModel itemModel2) => itemModel2.Name == name);
                    if (itemModel6 == null)
                    {
                        return;
                    }
                    if (amount > itemModel6.Amount)
                    {
                        dbPlayer.SendNotification("hoff nicht");
                        return;
                    }
                    if (itemModel6.Amount >= amount)
                    {
                        inventoryItems.Remove(itemModel6);
                        itemModel6.Amount -= amount;
                        if (itemModel6.Amount > 0)
                        {
                            inventoryItems.Add(itemModel6);
                        }
                    }
                    mySqlQuery.Parameters.Clear();
                    mySqlQuery = new MySqlQuery("UPDATE inventorys SET Items = @items WHERE Id = @id");
                    mySqlQuery.AddParameter("@id", dbPlayer.Id);
                    mySqlQuery.AddParameter("@items", NAPI.Util.ToJson((object)inventoryItems));
                    MySqlHandler.ExecuteSync(mySqlQuery);
                    ItemModel itemModel7 = list.FirstOrDefault((ItemModel itemModel4) => itemModel4.Name == itemModel6.Name);
                    if (itemModel7 != null && itemModel7.Amount > 0)
                    {
                        itemModel7.Amount += amount;
                    }
                    else
                    {
                        itemModel7 = new ItemModel
                        {
                            Id = itemModel6.Id,
                            Amount = amount,
                            ImagePath = itemModel6.ImagePath,
                            Slot = list.Count + 1,
                            Weight = 0,
                            Name = itemModel6.Name
                        };
                        list.Add(itemModel7);
                    }
                    if (text2 == "laboratorys")
                    {
                        Dictionary<int, List<ItemModel>> dictionary2 = NAPI.Util.FromJson<Dictionary<int, List<ItemModel>>>(reader.GetString(text));
                        if (!dictionary2.ContainsKey(dbPlayer.Id))
                        {
                            dictionary2.Add(dbPlayer.Id, list);
                        }
                        dictionary2[dbPlayer.Id] = list;
                        mySqlQuery.Parameters.Clear();
                        mySqlQuery = new MySqlQuery("UPDATE " + text2 + " SET " + text + " = @items WHERE Id = @id");
                        mySqlQuery.AddParameter("@id", num);
                        mySqlQuery.AddParameter("@items", NAPI.Util.ToJson((object)dictionary2));
                        MySqlHandler.ExecuteSync(mySqlQuery);
                        goto IL_0b6e;
                    }
                    mySqlQuery.Parameters.Clear();
                    mySqlQuery = new MySqlQuery("UPDATE " + text2 + " SET " + text + " = @items WHERE Id = @id");
                    mySqlQuery.AddParameter("@id", num);
                    mySqlQuery.AddParameter("@items", NAPI.Util.ToJson((object)list));
                    MySqlHandler.ExecuteSync(mySqlQuery);
                    if (!(text2 == "houses"))
                    {
                        goto IL_0b6e;
                    }
                    House house4 = HouseModule.houses.FirstOrDefault((House house2) => house2.OwnerId == dbPlayer.Id || house2.TenantsIds.Contains(dbPlayer.Id));
                    if (house4 == null)
                    {
                        return;
                    }
                    HouseModule.houses.Remove(house4);
                    house4.Inventory = list;
                    HouseModule.houses.Add(house4);
                    goto IL_0b6e;
                }
                if (((dynamic)((Entity)client).GetData<dynamic>("USING_FRAKLAGER") == true) && dbPlayer.Factionrank < 10)
                {
                    dbPlayer.SendNotification("Du brauchst einen höheren Rang um etwas aus dem Fraktionslager rauszunehmen!", 3000, "white", "FRAKLAGER");
                    return;
                }
                if (((dynamic)((Entity)client).GetData<dynamic>("USING_TRESOR") == true) && dbPlayer.Businessrank < 1)
                {
                    dbPlayer.SendNotification("Du brauchst einen höheren Rang um etwas aus dem Tresor rauszunehmen!", 3000, "orange", "BUSINESS");
                    return;
                }
                if (name.Length <= 0)
                {
                    dbPlayer.SendNotification("FEHLER 1231");
                    return;
                }
                ItemModel itemModel5 = list.FirstOrDefault((ItemModel itemModel2) => itemModel2.Name == name);
                if (itemModel5 == null || amount == null)
                {
                    dbPlayer.SendNotification("FEHLER 3102");
                    return;
                }
                if (amount > itemModel5.Amount)
                {
                    dbPlayer.SendNotification("hoff nicht");
                    return;
                }
                if (itemModel5.Amount >= amount)
                {
                    list.Remove(itemModel5);
                    itemModel5.Amount -= amount;
                    if (itemModel5.Amount > 0)
                    {
                        list.Add(itemModel5);
                    }
                }
                if (text2 == "laboratorys")
                {
                    Dictionary<int, List<ItemModel>> dictionary3 = NAPI.Util.FromJson<Dictionary<int, List<ItemModel>>>(reader.GetString(text));
                    if (dictionary3 == null)
                    {
                        return;
                    }
                    if (!dictionary3.ContainsKey(dbPlayer.Id))
                    {
                        dictionary3.Add(dbPlayer.Id, list);
                    }
                    dictionary3[dbPlayer.Id] = list;
                    mySqlQuery.Parameters.Clear();
                    mySqlQuery = new MySqlQuery("UPDATE " + text2 + " SET " + text + " = @items WHERE Id = @id");
                    mySqlQuery.AddParameter("@id", num);
                    mySqlQuery.AddParameter("@items", NAPI.Util.ToJson((object)dictionary3));
                    MySqlHandler.ExecuteSync(mySqlQuery);
                    goto IL_0ff9;
                }
                mySqlQuery.Parameters.Clear();
                mySqlQuery = new MySqlQuery("UPDATE " + text2 + " SET " + text + " = @items WHERE Id = @id");
                mySqlQuery.AddParameter("@id", num);
                mySqlQuery.AddParameter("@items", NAPI.Util.ToJson((object)list));
                MySqlHandler.ExecuteSync(mySqlQuery);
                goto IL_0ff9;
            IL_0b6e:
                //WebhookSender.SendMessage("Item wird verschoben", dbPlayer.Name + " hat das Item " + name + " " + amount + "x in sein eigenes Inventar verschoben. Von: Table " + text2 + " | ItemList " + text + " | Id " + num, Webhooks.inventarlogs, "Inventar-Log");
                goto IL_11de;
            IL_0ff9:
                //WebhookSender.SendMessage("Item wird verschoben", dbPlayer.Name + " hat das Item " + name + " " + amount + "x verschoben. In: Table " + text2 + " | ItemList " + text + " | Id " + num, Webhooks.inventarlogs, "Inventar-Log");
                ItemModel itemModel8 = inventoryItems.FirstOrDefault((ItemModel itemModel4) => itemModel4.Name == itemModel5.Name);
                if (itemModel8 != null && itemModel8.Amount > 0)
                {
                    itemModel8.Amount += amount;
                }
                else
                {
                    itemModel8 = new ItemModel
                    {
                        Id = itemModel5.Id,
                        Amount = amount,
                        ImagePath = itemModel5.ImagePath,
                        Slot = inventoryItems.Count + 1,
                        Weight = 0,
                        Name = itemModel5.Name
                    };
                    inventoryItems.Add(itemModel8);
                }
                mySqlQuery.Parameters.Clear();
                mySqlQuery = new MySqlQuery("UPDATE inventorys SET Items = @items WHERE Id = @id");
                mySqlQuery.AddParameter("@id", dbPlayer.Id);
                mySqlQuery.AddParameter("@items", NAPI.Util.ToJson((object)inventoryItems));
                MySqlHandler.ExecuteSync(mySqlQuery);
                if (!(text2 == "houses"))
                {
                    goto IL_11de;
                }
                House house5 = HouseModule.houses.FirstOrDefault((House house2) => house2.OwnerId == dbPlayer.Id || house2.TenantsIds.Contains(dbPlayer.Id));
                if (house5 == null)
                {
                    return;
                }
                HouseModule.houses.Remove(house5);
                house5.Inventory = list;
                HouseModule.houses.Add(house5);
                goto IL_11de;
            IL_11de:
                reader.Dispose();
                query.Connection.Dispose();
                if (!client.IsInVehicle)
                {
                    NAPI.Player.PlayPlayerAnimation(client, 33, "anim@mp_snowball", "pickup_snowball", 8f);
                    NAPI.Task.Run((Action)delegate
                    {
                        if (NAPI.Pools.GetAllPlayers().Contains(dbPlayer.Client))
                        {
                            dbPlayer.StopAnimation();
                        }
                    }, 2000L);
                }
                client.TriggerEvent("closeWindow", new object[1] { "Inventory" });
                requestItems(client);
            end_IL_00e8:;
            }
            catch (Exception ex)
            {
                Logger.Print("[EXCEPTION moveItemToInventory] " + ex.Message);
                Logger.Print("[EXCEPTION moveItemToInventory] " + ex.StackTrace);
            }
        }

    }
}
