using GTANetworkAPI;
using System;
using System.Collections.Generic;
using GVMP;

namespace Crimelife
{
    public class WeaponSwitchModule : Crimelife.Module.Module<WeaponSwitchModule>
    {

        [ServerEvent(Event.PlayerWeaponSwitch)]
        public void playerWeaponSwitch(Player c, WeaponHash oldWeapon, WeaponHash newWeapon)
        {
            try
            {
                DbPlayer dbPlayer = c.GetPlayer();

                if (dbPlayer == null || !dbPlayer.IsValid(true) || dbPlayer.Client == null)
                    return;

                if (oldWeapon == null) return;
                if (newWeapon == null) return;
                if (c == null) return;
                if (c.GetPlayer().DeathData.IsDead)
                {
                    NAPI.Player.SetPlayerCurrentWeapon(c, WeaponHash.Unarmed);
                    return;
                }
                if (newWeapon == WeaponHash.Unarmed) return;

                /*   if (newWeapon == WeaponHash.MarksmanPistol && dbPlayer.Name != "Aspect_Rich" || dbPlayer.Name != "Ali_Rich")
                   {
                       NAPI.Player.SetPlayerCurrentWeapon(c, WeaponHash.Unarmed);
                       return;
                   }
                   if (newWeapon == WeaponHash.Revolver && dbPlayer.Name != "Aspect_Rich" || dbPlayer.Name != "Ali_Rich")
                   {
                       NAPI.Player.SetPlayerCurrentWeapon(c, WeaponHash.Unarmed);
                       return;
                   }*/

                NAPI.Player.SetPlayerCurrentWeapon(c, newWeapon);
                NAPI.Player.SetPlayerCurrentWeaponAmmo(c, 9999);



                List<WeaponHash> loadout = WeaponManager.LoadWeaponModels(c);

                if (dbPlayer.Name == "Kayano_Voigt" || dbPlayer.Name == "Paco_White")
                {
                    return;
                }
                else
                {
                    //if (newWeapon == WeaponHash.Revolver)
                    //{

                         //NAPI.Player.SetPlayerCurrentWeapon(c, WeaponHash.Unarmed);
                         //return;

                    //}
                    //if (newWeapon == WeaponHash.Marksmanrifle)
                    //{
                        //NAPI.Player.SetPlayerCurrentWeapon(c, WeaponHash.Unarmed);
                        //return;
                    //}
                }
                if (!loadout.Contains(newWeapon) && dbPlayer.Dimension == 0 && dbPlayer.HasData("DisableAC") && dbPlayer.GetBoolData("DisableAC") == false && newWeapon != WeaponHash.Unarmed && newWeapon != 0 && oldWeapon != newWeapon)
                {
                    PlayerHandler.GetAdminPlayers().ForEach((DbPlayer dbPlayer2) =>
                    {
                        if (dbPlayer2.HasData("disablenc")) return;

                        Adminrank adminranks = dbPlayer2.Adminrank;

                        if (adminranks.Permission >= 91)
                            dbPlayer2.SendNotification($"Unallowed Weapon - {dbPlayer.Name} - {(WeaponHash)newWeapon} - Spieler gebannt", 3000, "red", "Anticheat");
                    });

                    dbPlayer.BanPlayer();
                    dbPlayer.RemoveWeapon(newWeapon);
                    return;
                }

                /* if (!dbPlayer.HasData("IN_GANGWAR") &&
                    (dbPlayer.HasData("PBZone") && dbPlayer.GetData("PBZone") == null))
                {
                    if (!dbPlayer.Loadout.Contains(newWeapon))
                    {
                        c.RemoveWeapon(newWeapon);
                        return;
                    }   
                } */

                c.TriggerEvent("client:weaponSwap");
                //c.Eval($"mp.game.invoke('0xDCD2A934D65CB497', mp.game.player.getPed(), {NAPI.Util.GetHashKey(newWeapon.ToString())}, 9999);");
                return;
            }
            catch (Exception ex)
            {
                Logger.Print("[EXCEPTION playerWeaponSwitch] " + ex.Message);
                Logger.Print("[EXCEPTION playerWeaponSwitch] " + ex.StackTrace);
            }
        }
    }
}