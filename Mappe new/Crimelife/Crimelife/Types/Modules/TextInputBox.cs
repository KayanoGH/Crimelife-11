using GTANetworkAPI;
using System;
using System.Collections.Generic;
using System.Text;
using GVMP;

namespace Crimelife
{
    public static class TextInputBox
    {
        public static void OpenTextInputBox(this DbPlayer dbPlayer, TextInputBoxObject textInputBoxObject)
        {
            object variable = new { textBoxObject = textInputBoxObject };
            Player client = dbPlayer.Client;
            client.TriggerEvent("openWindow", new object[] { "TextInputBox", NAPI.Util.ToJson(variable) });
            client.TriggerEvent("componentReady", new object[] { "TextInputBox" });
        }

        public static void OpenTextInputBox(this Player c, TextInputBoxObject textInputBoxObject)
        {
            object variable = new { textBoxObject = textInputBoxObject };
            c.TriggerEvent("openWindow", new object[] { "TextInputBox", NAPI.Util.ToJson(variable) });
            c.TriggerEvent("componentReady", new object[] { "TextInputBox" });
        }
    }
}
