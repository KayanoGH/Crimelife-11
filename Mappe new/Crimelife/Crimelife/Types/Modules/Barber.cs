﻿using GTANetworkAPI;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using GVMP;

namespace Crimelife
{
    public static class Barber
    {
        public static void OpenBarberShop(this DbPlayer dbPlayer, BarberObject barberObject)
        {
            dbPlayer.TriggerEvent("openWindow", nameof(Barber), NAPI.Util.ToJson(barberObject));
        }
    }
}
