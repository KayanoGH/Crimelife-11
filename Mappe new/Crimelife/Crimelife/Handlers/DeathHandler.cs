using Crimelife;
using GTANetworkAPI;
using GVMP;
using System;

namespace Crimelife
{
    internal class DeathHandler : Script
    {
        private void handleDeath(DbPlayer dbPlayer)
        {
            dbPlayer.StopScreenEffect("DeathFailOut");
            dbPlayer.SpawnPlayer(dbPlayer.Client.Position);
            dbPlayer.disableAllPlayerActions(true);
            dbPlayer.StartScreenEffect("DeathFailOut", 0, true);
            dbPlayer.PlayAnimation(33, "combat@damage@rb_writhe", "rb_writhe_loop", 8f);
            dbPlayer.SetInvincible(true);
            dbPlayer.SetSharedData("FUNK_CHANNEL", 0);
            dbPlayer.SetSharedData("FUNK_TALKING", false);
        }

        [ServerEvent(Event.PlayerDeath)]
        public void OnPlayerDeath(Player c, Player k, uint reason)
        {
            try
            {
                if (c == null) return;
                //DbPlayer dbPlayer = c.GetPlayer();
                DbPlayer dbPlayer = PlayerHandler.GetPlayer(c.Name);
                if (dbPlayer == null || !dbPlayer.IsValid(true) || dbPlayer.Client == null)
                    return;

                PaintballModel paintballModel = dbPlayer.GetPBData("PBZone");

                if (k != null && k.Exists)
                {
                    DbPlayer dbKiller = PlayerHandler.GetPlayer(k.Name);
                    WebhookSender.SendMessage("Spieler wird getötet", "Der Spieler " + dbPlayer.Name + " wurde von " + dbKiller.Name + " getötet." + "Waffe: " + dbKiller.Client.CurrentWeapon, Webhooks.killWebhook, "Kill");
                }


                if (paintballModel != null)
                {
                    if (k != null && k.Exists)
                    {
                        DbPlayer dbKiller = PlayerHandler.GetPlayer(k.Name);
                        if (dbKiller == null || !dbKiller.IsValid(true))
                            return;
                        //Paintball
                        dbPlayer.SendNotification("Du wurdest von " + dbKiller.Name + " getötet!", 5000, "black");
                        dbKiller.SendNotification("Du hast " + dbPlayer.Name + " getötet! +60.000$", 5000, "black");
                        dbKiller.addMoney(60000);

                        dbKiller.SetHealth(200);
                        dbKiller.SetArmor(100);

                        dbPlayer.disableAllPlayerActions(true);
                        dbPlayer.SpawnPlayer(dbPlayer.Client.Position);
                        dbPlayer.PlayAnimation(33, "combat@damage@rb_writhe", "rb_writhe_loop", 8f);
                        dbPlayer.SetInvincible(true);



                        NAPI.Task.Run(() => { PaintballModule.PaintballDeath(dbPlayer, dbKiller); }, 3000);

                        return;
                    }
                    else
                    {
                        //Paintball
                        dbPlayer.SendNotification("Du bist gestorben!", 5000, "black");

                        dbPlayer.disableAllPlayerActions(true);
                        dbPlayer.SpawnPlayer(dbPlayer.Client.Position);
                        dbPlayer.PlayAnimation(33, "combat@damage@rb_writhe", "rb_writhe_loop", 8f);
                        dbPlayer.SetInvincible(true);



                        NAPI.Task.Run(() => { PaintballModule.PaintballDeath2(dbPlayer); }, 3000);

                        return;
                        
                    }

                }

                if (dbPlayer.HasData("IN_GANGWAR"))
                {
                    dbPlayer.StartScreenEffect("DeathFailOut", 0, true);
                    dbPlayer.DeathData = new DeathData
                    {
                        IsDead = true,
                        DeathTime = new DateTime(0)
                    };
                    if (k != null && k.Exists)
                    {
                        DbPlayer dbKiller = PlayerHandler.GetPlayer(k.Name);
                        if (dbKiller == null || !dbKiller.IsValid(true))
                            return;

                        if (dbKiller.Faction.Id != dbPlayer.Faction.Id)
                            GangwarModule.handleKill(dbKiller);
                        //Gangwar
                        dbPlayer.SendNotification("Du wurdest von " + dbKiller.Name + " getötet!", 5000, "black");
                        dbKiller.SendNotification("Du hast " + dbPlayer.Name + " getötet! +60.000$", 5000, "black");
                        dbKiller.addMoney(60000);
                    }
                    else
                    {
                        dbPlayer.SendNotification("Du bist gestorben!", 3000, "black");
                    }

                    NAPI.Task.Run(() => handleDeath(dbPlayer), 2000);
                }
                else if (paintballModel == null)
                {
                    dbPlayer.StartScreenEffect("DeathFailOut", 0, true);
                    dbPlayer.DeathData = new DeathData { IsDead = true, DeathTime = DateTime.Now };
                    if (k != null && k.Exists)
                    {
                        DbPlayer dbKiller = PlayerHandler.GetPlayer(k.Name);
                        if (dbKiller == null || !dbKiller.IsValid(true))
                            return;
                        //Normal
                        dbPlayer.SendNotification("Du wurdest von " + dbKiller.Name + " getötet!", 5000, "black");
                        dbKiller.SendNotification("Du hast " + dbPlayer.Name + " getötet! +50.000$", 5000, "black");
                        dbKiller.addMoney(50000);
                    }
                    else
                    {
                        dbPlayer.SendNotification("Du bist gestorben!", 3000, "black");
                    }

                    dbPlayer.SetSharedData("FUNK_CHANNEL", 0);
                    dbPlayer.SetSharedData("FUNK_TALKING", false);
                    dbPlayer.SetAttribute("Death", 1);

                    NAPI.Task.Run(() => handleDeath(dbPlayer), 3000);
                }
            }
            catch (Exception ex)
            {
                Logger.Print("[EXCEPTION PlayerDeath] " + ex.Message);
                Logger.Print("[EXCEPTION PlayerDeath] " + ex.StackTrace);
            }
        }
    }
}