using GTANetworkAPI;
using System;
using GVMP;

namespace Crimelife
{
    public class NativeModule : Crimelife.Module.Module<NativeModule>
    {
        [RemoteEvent("m")]
        public static void nativeMenu(Player client, string id)
        {
            try
            {
                if (string.IsNullOrEmpty(id) || id.Length <= 0) return;
                if (client == null || !client.Exists) return;
                DbPlayer dbPlayer = client.GetPlayer();
                if (dbPlayer == null || !dbPlayer.IsValid(true) || dbPlayer.Client == null) return;

                if (id != "NaN")
                {
                    NativeMenu nativeMenu = (NativeMenu)dbPlayer.GetNMData("PLAYER_CURRENT_NATIVEMENU");
                    if (nativeMenu != null && nativeMenu.Items.Count >= Convert.ToInt32(id) && nativeMenu.Items[Convert.ToInt32(id)] != null)
                    {
                        client.Eval("mp.events.callRemote('nM-" + nativeMenu.Title + "', '" +
                                    nativeMenu.Items[Convert.ToInt32(id)].selectionName + "');");
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Print("[EXCEPTION m] " + ex.Message);
                Logger.Print("[EXCEPTION m] " + ex.StackTrace);
            }
        }
    }
}
