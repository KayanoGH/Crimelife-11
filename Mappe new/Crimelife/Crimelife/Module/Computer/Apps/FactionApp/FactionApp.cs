using GTANetworkAPI;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GVMP;

namespace Crimelife
{
    class FactionApp : Script
    {
        [RemoteEvent("requestFraktionMembers")]
        public void requestFraktionMembers(Player c)
        {
            try
            {
                if (c == null) return;
                DbPlayer dbPlayer = c.GetPlayer();
                if (dbPlayer == null || !dbPlayer.IsValid(true) || dbPlayer.Client == null)
                    return;

                if (dbPlayer.Faction.Id == 0) return;

                List<FactionMember> factionMembers = new List<FactionMember>();

                MySqlQuery mySqlQuery = new MySqlQuery("SELECT * FROM accounts WHERE Fraktion = @fraktion");
                mySqlQuery.AddParameter("@fraktion", dbPlayer.Faction.Id);
                MySqlResult mySqlResult = MySqlHandler.GetQuery(mySqlQuery);
                MySqlDataReader reader = mySqlResult.Reader;

                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        factionMembers.Add(new FactionMember
                        {
                            Id = reader.GetInt32("Id"),
                            Payday = reader.GetString("Beschreibung"),
                            Name = reader.GetString("Username"),
                            Rang = reader.GetInt32("Fraktionrank"),
                            Title = ""
                        });
                    }
                }

                factionMembers = factionMembers.OrderBy(obj => obj.Rang).ToList();
                factionMembers.Reverse();


                object Obj = new
                {
                    manage = false, //test
                    list = factionMembers
                };

                object Obj2 = new
                {
                    manage = true, //test
                    list = factionMembers
                };

                reader.Dispose();
                mySqlResult.Connection.Dispose();
                if (dbPlayer.Factionrank < 10)
                {
                    dbPlayer.TriggerEvent("componentServerEvent", "FraktionListApp", "responseMembers",
                    NAPI.Util.ToJson(Obj), false);
                }
                else
                    {
                    dbPlayer.TriggerEvent("componentServerEvent", "FraktionListApp", "responseMembers",
NAPI.Util.ToJson(Obj2), false);
                }
            }
            catch (Exception ex)
            {
                Logger.Print("[EXCEPTION requestFraktionMembers] " + ex.Message);
                Logger.Print("[EXCEPTION requestFraktionMembers] " + ex.StackTrace);
            }
        }

        [RemoteEvent("editFraktionMember")]
        public void editFraktionMember(Player c, int Id, int Rang, string Payday, string Title)
        {
            try
            {
                if (c == null) return;
                DbPlayer dbPlayer = c.GetPlayer();
                if (dbPlayer == null || !dbPlayer.IsValid(true) || dbPlayer.Client == null)
                    return;

                if (dbPlayer.Faction.Id == 0) return;
                if (dbPlayer.Factionrank < 10) return;

                MySqlQuery mySqlQuery1 = new MySqlQuery("SELECT * FROM accounts WHERE Id = " + Id);
                MySqlResult mySqlResult = MySqlHandler.GetQuery(mySqlQuery1);

                if (mySqlResult.Reader.HasRows)
                {
                    mySqlResult.Reader.Read();

                    string name = mySqlResult.Reader.GetString("Username");
                    int factionId = mySqlResult.Reader.GetInt32("Fraktion");
                    int factionRank = mySqlResult.Reader.GetInt32("Fraktionrank");

                    if (Rang >= dbPlayer.Factionrank)
                    {
                        dbPlayer.SendNotification("Dazu hast du keine Rechte!", 3000, "red");
                        return;
                    }
                    if (factionId == dbPlayer.Faction.Id && dbPlayer.Factionrank > factionRank)
                    {


                        MySqlQuery mySqlQuery =
                        new MySqlQuery("UPDATE accounts SET Fraktionrank = @rang WHERE Id = @id");
                        mySqlQuery.AddParameter("@id", Id);
                        mySqlQuery.AddParameter("@rang", Rang);
                        MySqlHandler.ExecuteSync(mySqlQuery);

                        MySqlQuery mySqlQuery2 =
                        new MySqlQuery("UPDATE accounts SET Beschreibung = @info WHERE Id = @id");
                        mySqlQuery2.AddParameter("@id", Id);
                        mySqlQuery2.AddParameter("@info", Payday);
                        MySqlHandler.ExecuteSync(mySqlQuery2);

                        dbPlayer.Faction.GetFactionPlayers().ForEach(delegate (DbPlayer target)
                        {
                            target.SendNotification(dbPlayer.Name + " hat " + name + " auf den Rang " + Rang + " gestuft. ", 3000, dbPlayer.Faction.GetRGBStr(), dbPlayer.Faction.Name);
                        });
                        DbPlayer dbPlayer2 = PlayerHandler.GetPlayer(Id);
                        if (dbPlayer2 != null)
                        {
                            dbPlayer2.Factionrank = Rang;
                            dbPlayer2.RefreshData(dbPlayer2);
                        }
                    }
                    else
                    {
                        dbPlayer.SendNotification("Der Spieler hat einen höheren Rang oder ist nicht in deiner Fraktion!", 3000, "red");
                    }
                }

                mySqlResult.Reader.Dispose();
                mySqlResult.Connection.Dispose();
            }
            catch (Exception ex)
            {
                Logger.Print("[EXCEPTION editFraktionMember] " + ex.Message);
                Logger.Print("[EXCEPTION editFraktionMember] " + ex.StackTrace);
            }
        }

       /* dbPlayer.Faction.GetFactionPlayers().ForEach(delegate (DbPlayer target)
                {
                    target.SendNotification(dbPlayer.Name + " hat " + dbPlayer2.Name + " auf den Rang " + Rang + " gestuft. ", 3000, dbPlayer.Faction.GetRGBStr(), dbPlayer.Faction.Name);
                });*/
        [RemoteEvent("kickFraktionMember")]
        public void kickFraktionMember(Player c, int Id, int Rang)
        {
            try
            {
                if (c == null) return;
                DbPlayer dbPlayer = c.GetPlayer();
                if (dbPlayer == null || !dbPlayer.IsValid(true) || dbPlayer.Client == null)
                    return;

                if (dbPlayer.Faction.Id == 0) return;
                if (dbPlayer.Factionrank < 10) return;

                MySqlQuery mySqlQuery1 = new MySqlQuery("SELECT * FROM accounts WHERE Id = " + Id);
                MySqlResult mySqlResult = MySqlHandler.GetQuery(mySqlQuery1);

                if (mySqlResult.Reader.HasRows)
                {
                    mySqlResult.Reader.Read();


                    string name = mySqlResult.Reader.GetString("Username");
                    int factionId = mySqlResult.Reader.GetInt32("Fraktion");
                    int factionRank = mySqlResult.Reader.GetInt32("Fraktionrank");

                    if (factionId == dbPlayer.Faction.Id && dbPlayer.Factionrank > factionRank)
                    {


                        MySqlQuery mySqlQuery =
            new MySqlQuery("UPDATE accounts SET Fraktionrank = @rang, Fraktion = @fraktion WHERE Id = @id");
                        mySqlQuery.AddParameter("@id", Id);
                        mySqlQuery.AddParameter("@rang", 0);
                        mySqlQuery.AddParameter("@fraktion", 0);
                        MySqlHandler.ExecuteSync(mySqlQuery);

                        dbPlayer.SendNotification("Du hast " + name + " aus der Fraktion geworfen!", 3000, dbPlayer.Faction.GetRGBStr(), dbPlayer.Faction.Name);

                        DbPlayer dbPlayer2 = PlayerHandler.GetPlayer(Id);
                        if (dbPlayer2 != null)
                        {
                            dbPlayer2.SetData("Fraksperre", true);
                            dbPlayer2.SendNotification("Du wurdest aus deiner Fraktion rausgeworfen!", 3000, "red");
                            dbPlayer2.Faction = FactionModule.getFactionById(0);
                            dbPlayer2.Factionrank = 0;
                            dbPlayer2.RefreshData(dbPlayer2);
                        }
                    }
                    else
                    {
                        dbPlayer.SendNotification("Der Spieler hat einen höheren Rang oder ist nicht in deiner Fraktion!", 3000, "red");
                    }
                }

                mySqlResult.Reader.Dispose();
                mySqlResult.Connection.Dispose();
            }
            catch (Exception ex)
            {
                Logger.Print("[EXCEPTION kickFraktionMember] " + ex.Message);
                Logger.Print("[EXCEPTION kickFraktionMember] " + ex.StackTrace);
            }
        }
    }
}
