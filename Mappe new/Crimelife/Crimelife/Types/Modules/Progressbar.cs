using GTANetworkAPI;
using System;
using System.Collections.Generic;
using System.Text;
using GVMP;

namespace Crimelife
{
    public static class Progressbar
    {
        public static void SendProgressbar(this DbPlayer dbPlayer, int duration)
        {
            Player c = dbPlayer.Client;

            c.TriggerEvent("sendProgressbar", duration);
        }

        public static void StopProgressbar(this DbPlayer dbPlayer)
        {
            Player c = dbPlayer.Client;

            c.TriggerEvent("componentServerEvent", "Progressbar", "StopProgressbar");
        }
    }
}
