using System;
using System.Collections.Generic;
using System.Text;
using GVMP;

namespace Crimelife
{
    public static class Notifications
    {
        public static void SendNotification(this DbPlayer dbPlayer, string msg, int duration = 3000, string color = "#2f2f30", string title = "")
        {
            dbPlayer.Client.TriggerEvent("sendPlayerNotification", msg, duration, color, title, "");
        }

        public static void SendNotification2(this DbPlayer dbPlayer, string msg, string color = "#2f2f30", int duration = 3000, string title = "")
        {
            dbPlayer.Client.TriggerEvent("sendPlayerNotification", msg, duration, color, title, "");
        }
    }
}
