using GTANetworkAPI;
using System;
using GVMP;

namespace Crimelife
{
    public class Notification
    {

        public enum icon
        {
            warn,
            bullhorn,
            thief,
            bell,
            diamond
        }

        //Player player3 = PlayerHandler.GetPlayer(player3.Name);

        public static void SendGlobalNotification2(DbPlayer dbPlayer2, string message, int durationInMS, string color, icon ico)
        {
            string text = "";
            if (ico == icon.warn)
            {
                text = "glob";
            }
            if (ico == icon.bullhorn)
            {
                text = "gov";
            }
            if (ico == icon.thief)
            {
                text = "dev";
            }
            if (ico == icon.bell)
            {
                text = "wed";
            }
            if (ico == icon.diamond)
            {
                text = "casino";
            }
            dbPlayer2.TriggerEvent("sendGlobalNotification", message, durationInMS, color, text);
        }

        //Player client = PlayerHandler.GetPlayer();

        public static void SendGlobalNotification(string message, int durationInMS, string color, icon ico)
        {
            //DbPlayer dbPlayers = PlayerHandler.GetPlayer(player.Name);

            string text = "";
            if (ico == icon.warn)
            {
                text = "glob";
            }
            if (ico == icon.bullhorn)
            {
                text = "gov";
            }
            if (ico == icon.thief)
            {
                text = "dev";
            }
            if (ico == icon.bell)
            {
                text = "wed";
            }
            if (ico == icon.diamond)
            {
                text = "casino";
            }

            foreach (Player target in NAPI.Pools.GetAllPlayers())
                target.TriggerEvent("sendGlobalNotification", message, durationInMS, color, text);
        }
    }
}
