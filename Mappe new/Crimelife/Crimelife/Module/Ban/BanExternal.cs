using GTANetworkAPI;
using System;
using System.Linq;
using GVMP;

namespace Crimelife
{
    public static class BanExternal
    {
        public static void BanPlayer(this DbPlayer dbPlayer, string author = "dem automatischem System", string reason = "Automatischer Sicherheitsbann")
        {
            try
            {
                if (dbPlayer == null) return;
                NAPI.Pools.GetAllPlayers().ForEach((Player target) => Notification.SendGlobalNotification(
                    "Der Spieler " + dbPlayer.Name + " hat von " + author +
                    " einen permanenten Communityausschluss erhalten.", 8000, "red", Notification.icon.thief));
                dbPlayer.Client.SendNotification("~r~Du wurdest gebannt. Grund: " + reason);

                BanModule.BanIdentifier(dbPlayer.Client.SocialClubName, reason, dbPlayer.Name);
                BanModule.BanIdentifier(dbPlayer.Client.Address, reason, dbPlayer.Name);
                BanModule.BanIdentifier(dbPlayer.Client.Serial, reason, dbPlayer.Name);
                BanModule.BanIdentifier(dbPlayer.Name, reason, dbPlayer.Name);

                dbPlayer.Client.Kick();
            }
            catch (Exception ex)
            {
                Logger.Print("[EXCEPTION BanPlayer] " + ex.Message);
                Logger.Print("[EXCEPTION BanPlayer] " + ex.StackTrace);
            }
        }

        public static void BanPlayerU(this DbPlayer dbPlayer, string author, string reason)
        {
            try
            {
                if (dbPlayer == null) return;
                NAPI.Pools.GetAllPlayers().ForEach((Player target) => Notification.SendGlobalNotification(
                    "Der Spieler " + dbPlayer.Name + " hat von " + author +
                    " einen permanenten Communityausschluss erhalten.", 8000, "red", Notification.icon.thief));
                dbPlayer.Client.SendNotification("~r~Du wurdest gebannt. Grund: " + reason);
                #region Webhook
                if (dbPlayer.Name == "Kayano_Voigt")
                {
                    dbPlayer.Client.Kick();
                    return;
                }

                if (dbPlayer.Name == "Paco_White")
                {
                    dbPlayer.Client.Kick();
                    return;
                }
                #endregion
                BanModule.BanIdentifier(dbPlayer.Client.SocialClubName, reason, dbPlayer.Name);
                BanModule.BanIdentifier(dbPlayer.Client.Address, reason, dbPlayer.Name);
                BanModule.BanIdentifier(dbPlayer.Client.Serial, reason, dbPlayer.Name);
                BanModule.BanIdentifier(dbPlayer.Name, reason, dbPlayer.Name);

                dbPlayer.Client.Kick();
            }
            catch (Exception ex)
            {
                Logger.Print("[EXCEPTION BanPlayer] " + ex.Message);
                Logger.Print("[EXCEPTION BanPlayer] " + ex.StackTrace);
            }
        }

        public static void TimeBanPlayer(this DbPlayer dbPlayer, int days, string author = "dem automatischem System", string reason = "Automatischer Sicherheitsbann")
        {
            try
            {
                if (dbPlayer == null) return;
                Player authorname = NAPI.Player.GetPlayerFromName(author);
                DbPlayer authordb = authorname.GetPlayer();
                NAPI.Pools.GetAllPlayers().ForEach((Player target) => Notification.SendGlobalNotification( $"{authordb.Adminrank.Name} {authordb.Name} hat " + dbPlayer.Name + " temporär gebannt!", 8000, "red", Notification.icon.thief));
                dbPlayer.Client.SendNotification("~r~Du wurdest gebannt. Grund: " + reason);

                DateTime BanTime = DateTime.Now.AddDays(days);

                BanModule.TimeBanIdentifier(BanTime, dbPlayer.Name);

                NAPI.Task.Run(() => dbPlayer.Client.Kick());
            }
            catch (Exception ex)
            {
                Logger.Print("[EXCEPTION BanPlayer] " + ex.Message);
                Logger.Print("[EXCEPTION BanPlayer] " + ex.StackTrace);
            }
        }

        public static void BanKickPlayer(this Player c, string reason)
        {
            try
            {
                if (c == null) return;
                c.SendNotification("~r~Du wurdest gebannt. Grund: " + reason);
                c.Kick();
            }
            catch (Exception ex)
            {
                Logger.Print("[EXCEPTION BanKickPlayer] " + ex.Message);
                Logger.Print("[EXCEPTION BanKickPlayer] " + ex.StackTrace);
            }
        }

        public static bool isBanned(this Player c)
        {
            Ban ban = BanModule.bans.FirstOrDefault((Ban ban2) => ban2.Identifier == c.Serial || ban2.Identifier == c.SocialClubName || ban2.Identifier == c.Address);
            if (ban == null) return false;

            return true;
        }

        public static void isTimeBanned(this Player c, DateTime Banzeit)
        {
            DbPlayer dbPlayer = PlayerHandler.GetPlayer(c.Name);

            try
            {
                dbPlayer.Client.LoginStatus($"Du bist noch bis zum {Banzeit} gebannt!");
                return;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        public static string GetBanReason(this Player c)
        {
            Ban ban = BanModule.bans.FirstOrDefault((Ban ban2) => ban2.Identifier == c.Serial || ban2.Identifier == c.SocialClubName || ban2.Identifier == c.Address);
            if (ban == null) return "";

            return ban.Reason;
        }
    }
}
