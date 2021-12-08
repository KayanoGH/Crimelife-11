﻿using GTANetworkAPI;
using System.Collections.Generic;
using GVMP;

namespace Crimelife
{
    class FactionBank : Script
    {
        public static void OpenFactionBank(DbPlayer dbPlayer)
        {
            if (dbPlayer.Faction.Id == 0) return;

            Bank bank = new Bank
            {
                Playername = dbPlayer.Name,
                Money = dbPlayer.Money,
                Balance = dbPlayer.Faction.Money,
                History = new List<BankHistory>(),
                Bankaccount = dbPlayer.Faction.Name
            };

            dbPlayer.SetData("USING_FACTIONBANK", true);
            dbPlayer.OpenBank(bank);
            //dbPlayer.SendNotification("Frakbank aktuell deaktiviert");
        }

        [RemoteEvent("bankDeposit")]
        public void bankDeposit(Player c, int Amount)
        {
            DbPlayer dbPlayer = c.GetPlayer();
            if (dbPlayer == null || !dbPlayer.IsValid(true) || dbPlayer.Client == null)
                return;

            if (dbPlayer.Faction.Id == 0) return;

            if (!dbPlayer.HasData("USING_FACTIONBANK")) return;
            if (!dbPlayer.GetBoolData("USING_FACTIONBANK")) return;

            if (dbPlayer.Money >= Amount)
            {
                dbPlayer.removeMoney(Amount);
                dbPlayer.Faction.addMoney(Amount);
                dbPlayer.SendNotification(Amount.ToDots() + "$ erfolgreich eingezahlt!", 3000, "green");
            }
            else
            {
                dbPlayer.SendNotification("Zu wenig Geld!", 3000, "red");
            }
        }

        [RemoteEvent("bankPayout")]
        public void bankPayout(Player c, int Amount)
        {
            DbPlayer dbPlayer = c.GetPlayer();
            if (dbPlayer == null || !dbPlayer.IsValid(true) || dbPlayer.Client == null)
                return;

            bool deakt = true;
            if (deakt)
            {
                dbPlayer.SendNotification("Fehler Derzeit deaktiviert", 3000, "red");
            }

            if (dbPlayer.Faction.Id == 0) return;
            if (dbPlayer.Factionrank < 10) return;
            if (!dbPlayer.HasData("USING_FACTIONBANK")) return;
            if (!dbPlayer.GetBoolData("USING_FACTIONBANK")) return;

            if (dbPlayer.Faction.Money >= Amount)
            {
                dbPlayer.addMoney(Amount);
                dbPlayer.Faction.removeMoney(Amount);
                dbPlayer.SendNotification(Amount.ToDots() + "$ erfolgreich ausgezahlt!", 3000, "green");
            }
            else
            {
                dbPlayer.SendNotification("Zu wenig Geld auf dem Fraktionskonto!", 3000, "red");
            }
        }
    }
}
