using GTANetworkAPI;
using System;
using System.Collections.Generic;
using System.Text;
using GVMP;

namespace Crimelife
{
    class LifeInvaderModule : Crimelife.Module.Module<LifeInvaderModule>
    {
        public static List<Advertise> Advertisements = new List<Advertise>();

        protected override bool OnLoad()
        {
            ColShape c = NAPI.ColShape.CreateCylinderColShape(new Vector3(-1052, -238, 43), 1.4f, 3.4f, 0);
            c.SetData("FUNCTION_MODEL", new FunctionModel("openLifeInvader"));
            c.SetData("MESSAGE", new Message("Benutze E um Werbung zu schalten.", "LIVEINVADER", "red", 3000));

            NAPI.Marker.CreateMarker(1, new Vector3(-1052, -238, 43), new Vector3(), new Vector3(), 1.0f, new Color(255, 140, 0), false, 0);
            NAPI.Blip.CreateBlip(521, new Vector3(-1052, -238, 44), 1f, 0, "Lifeinvader", 255, 0.0f, true, 0, 0);
            return true;
        }

        [RemoteEvent("openLifeInvader")]
        public void openLifeInvader(Player c)
        {
            try
            {
                if (c == null) return;
                DbPlayer dbPlayer = c.GetPlayer();
                if (dbPlayer == null || !dbPlayer.IsValid(true) || dbPlayer.Client == null)
                    return;

                TextInputBoxObject textInputBoxObject = new TextInputBoxObject
                {
                    Callback = "LifeInvaderPurchaseAd"
                };

                dbPlayer.OpenTextInputBox(textInputBoxObject);
            }
            catch (Exception ex)
            {
                Logger.Print("[EXCEPTION openLifeInvader] " + ex.Message);
                Logger.Print("[EXCEPTION openLifeInvader] " + ex.StackTrace);
            }
        }

        [RemoteEvent("requestAd")]
        public void requestAd(Player c)
        {
            try
            {
                if (c == null) return;
                DbPlayer dbPlayer = c.GetPlayer();
                if (dbPlayer == null || !dbPlayer.IsValid(true) || dbPlayer.Client == null)
                    return;

                foreach (Advertise advertise in Advertisements.ToArray())
                {
                    if (advertise.Expires < DateTime.Now)
                        Advertisements.Remove(advertise);
                }

                c.TriggerEvent("componentServerEvent", "LifeInvaderApp", "updateLifeInvaderAds",
                    NAPI.Util.ToJson(Advertisements));
            }
            catch (Exception ex)
            {
                Logger.Print("[EXCEPTION requestAd] " + ex.Message);
                Logger.Print("[EXCEPTION requestAd] " + ex.StackTrace);
            }
        }

        [RemoteEvent("LifeInvaderPurchaseAd")]
        public void LifeInvaderPurchaseAd(Player c, string t)
        {
            try
            {
                if (c == null) return;
                DbPlayer dbPlayer = c.GetPlayer();
                if (dbPlayer == null || !dbPlayer.IsValid(true) || dbPlayer.Client == null)
                    return;

                var t2 = t.Split("$$$$");
                var price = t2[0].Length + t2[1].Length + 4 * 5;

                if (t.Contains("discord"))
                {
                    dbPlayer.SendNotification("Schade, morgen vllt :D");
                    return;
                }
                if (t.Contains("crmnl"))
                {
                    dbPlayer.SendNotification("Schade, morgen vllt :D ");
                    return;
                }
                if (t.Contains("https://"))
                {
                    dbPlayer.SendNotification("Schade, morgen vllt :D");
                    return;
                }
                if (dbPlayer.Money < price)
                {
                    dbPlayer.SendNotification("Du hast nicht genügend Geld!", 3000, "red");
                    return;
                }

                if (!dbPlayer.CanInteractAntiFlood(5))
                //if (dbPlayer.LastInteracted.AddSeconds(5.0) > DateTime.Now)
                {
                    dbPlayer.SendNotification("Antispam: Bitte 5 Sekunden warten!");
                    return;
                }

                dbPlayer.removeMoney(Convert.ToInt32(price));

                var ad = new Advertise
                {
                    Id = DateTime.Now.Millisecond,
                    Title = DateTime.Now.ToString("dd.MM.yyyy HH:mm"),
                    Content = t,
                    Expires = DateTime.Now.AddMinutes(15)
                };

                WebhookSender.SendMessage("TEXTINPUTBOX", "" + dbPlayer.Name + " " + t + " - LIFEINVADER", Webhooks.shoplogs, "Shoplogs");
                dbPlayer.SendNotification("Du hast die Werbung abgesendet!", 3000, "green");
                PlayerHandler.GetPlayers().ForEach((DbPlayer dbTarget) =>
                {
                    dbTarget.SendNotification("Es ist neue Werbung in der Liveinvader App verfügbar!", 3000, "yellow", "LIFEINVADER");
                });

                Advertisements.Add(ad);
            }
            catch(Exception ex)
            {
                Logger.Print("[EXCEPTION LifeInvaderPurchaseAd]" + ex.Message);
                Logger.Print("[EXCEPTION LifeInvaderPurchaseAd]" + ex.StackTrace);
            }
        }
    }
}
