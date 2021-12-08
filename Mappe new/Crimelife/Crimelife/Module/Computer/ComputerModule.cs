using GTANetworkAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GVMP;

namespace Crimelife
{
    class ComputerModule : Crimelife.Module.Module<ComputerModule>
    {
        [RemoteEvent("computerCheck")]
        public void openComputer(Player c)
        {
            try
            {
                if (c == null) return;
                DbPlayer dbPlayer = PlayerHandler.GetPlayer(c.Name);
                if (dbPlayer == null || !dbPlayer.IsValid(true) || dbPlayer.Client == null || dbPlayer.DeathData.IsDead)
                    return;

                dbPlayer.OpenComputer();
            }
            catch (Exception ex)
            {
                Logger.Print("[EXCEPTION computerCheck] " + ex.Message);
                Logger.Print("[EXCEPTION computerCheck] " + ex.StackTrace);
            }
        }

        [RemoteEvent("requestComputerApps")]
        public void requestComputerApps(Player c)
        {
            try
            {
                if (c == null) return;
                DbPlayer dbPlayer = c.GetPlayer();
                if (dbPlayer == null || !dbPlayer.IsValid(true) || dbPlayer.Client == null)
                    return;

                string HouseApp = "";
                House house = HouseModule.houses.FirstOrDefault((House house2) => house2.OwnerId == dbPlayer.Id);
                if (house != null)
                    HouseApp = "{\"id\":9,\"appName\":\"HouseApp\", \"name\":\"HausApp\", \"icon\":\"1055644.svg\"}, ";

                string FraktionsApp = "";
                if (dbPlayer.Faction.Id != 0)
                    FraktionsApp =
                        "{\"id\":10,\"appName\":\"FraktionListApp\", \"name\":\"Fraktionsapp\", \"icon\":\"1055644.svg\"}, ";

                dbPlayer.responseComputerApps("[" + HouseApp + FraktionsApp +
                                              "{\"id\":8,\"appName\":\"FahrzeugUebersichtApp\", \"name\":\"Schluessel\", \"icon\":\"189088.svg\"}]");
            }
            catch (Exception ex)//MAZ ERROR BEI HANDSCHUHFACH + HANDSCHUFACH NUR ÖFFNEN WENN AUTO OFFEN IST UND Nd KOFFERAUm
            {
                Logger.Print("[EXCEPTION requestComputerApps] " + ex.Message);
                Logger.Print("[EXCEPTION requestComputerApps] " + ex.StackTrace);
            }
        }

        [RemoteEvent("closeComputer")]
        public void closeComputer(Player c)
        {
            try
            {
                DbPlayer dbPlayer = c.GetPlayer();
                if (dbPlayer == null || !dbPlayer.IsValid(true) || dbPlayer.Client == null)
                    return;

                dbPlayer.CloseComputer();
            }
            catch (Exception ex)
            {
                Logger.Print("[EXCEPTION closeComputer] " + ex.Message);
                Logger.Print("[EXCEPTION closeComputer] " + ex.StackTrace);
            }
        }
    }
}
