using GTANetworkAPI;
using GVMP.Handlers;
using Crimelife.Types;
using System.Collections.Generic;
using System.Linq;
using GVMP;

namespace Crimelife
{
    class WeaponManager : Script
    {
        public static void removeWeapon(Player client, WeaponHash weaponHash)
        {
            DbPlayer dbPlayer = client.GetPlayer();
            if (dbPlayer == null || !dbPlayer.IsValid(true) || dbPlayer.Client == null)
                return;

            List<NXWeapon> playerWeapons = NAPI.Util.FromJson<List<NXWeapon>>(dbPlayer.GetAttributeString("Loadout"));

            playerWeapons.RemoveAll(w => w.Weapon == weaponHash);

            dbPlayer.Loadout = playerWeapons;
            dbPlayer.RefreshData(dbPlayer);

            dbPlayer.Client.RemoveAllOwnWeaponComponent(weaponHash);
            NAPI.Player.RemovePlayerWeapon(client, weaponHash);
            dbPlayer.TriggerEvent("client:weaponSwap");
            dbPlayer.SetAttribute("Loadout", NAPI.Util.ToJson(playerWeapons));
        }

        public static void loadWeapons(Player c)
        {
            DbPlayer dbPlayer = c.GetPlayer();
            if (dbPlayer == null || !dbPlayer.IsValid(true) || dbPlayer.Client == null)
                return;

            Constants.SetPlayerACFlag(c);
            List<NXWeapon> playerWeapons = NAPI.Util.FromJson<List<NXWeapon>>(dbPlayer.GetAttributeString("Loadout"));

            dbPlayer.Loadout = playerWeapons;
            dbPlayer.RefreshData(dbPlayer);

            c.RemoveAllWeapons();

            foreach (NXWeapon weapon in playerWeapons)
            {
                c.GiveWeapon(weapon.Weapon, 9999);
                foreach (WeaponComponent weaponComponent in weapon.Components)
                {
                    dbPlayer.Client.SetOwnWeaponComponent(weapon.Weapon, weaponComponent);
                }
            }
            NAPI.Player.SetPlayerCurrentWeapon(c, WeaponHash.Unarmed);
        }

        public static List<WeaponHash> LoadWeaponModels(Player c)
        {
            DbPlayer dbPlayer = c.GetPlayer();
            if (dbPlayer == null || !dbPlayer.IsValid(true) || dbPlayer.Client == null)
                return new List<WeaponHash>();

            List<NXWeapon> playerWeapons = dbPlayer.Loadout;

            return playerWeapons.Select(x => x.Weapon).ToList();
        }

        public static void addWeapon(Player client, WeaponHash weaponHash)
        {
            DbPlayer dbPlayer = client.GetPlayer();
            if (dbPlayer == null || !dbPlayer.IsValid(true) || dbPlayer.Client == null)
                return;

            dbPlayer.Client.RemoveAllOwnWeaponComponent(weaponHash);
            client.GiveWeapon(weaponHash, 9999);

            List<NXWeapon> playerWeapons = NAPI.Util.FromJson<List<NXWeapon>>(dbPlayer.GetAttributeString("Loadout"));
            playerWeapons.Add(new NXWeapon
            {
                Weapon = weaponHash,
                Components = new List<WeaponComponent>()
            });

            dbPlayer.Loadout = playerWeapons;
            dbPlayer.RefreshData(dbPlayer);

            dbPlayer.SetAttribute("Loadout", NAPI.Util.ToJson(playerWeapons));
        }

        public static void addWeaponComponent(Player client, WeaponHash weaponHash, WeaponComponent weaponComponent)
        {
            DbPlayer dbPlayer = client.GetPlayer();
            if (dbPlayer == null || !dbPlayer.IsValid(true) || dbPlayer.Client == null)
                return;

            dbPlayer.Client.SetOwnWeaponComponent(weaponHash, weaponComponent);

            List<NXWeapon> playerWeapons = NAPI.Util.FromJson<List<NXWeapon>>(dbPlayer.GetAttributeString("Loadout"));

            var weapon = playerWeapons.FirstOrDefault(w => w.Weapon == weaponHash);
            if (weapon == null) return;

            if (!weapon.Components.Contains(weaponComponent))
                weapon.Components.Add(weaponComponent);

            dbPlayer.Loadout = playerWeapons;
            dbPlayer.RefreshData(dbPlayer);

            dbPlayer.SetAttribute("Loadout", NAPI.Util.ToJson(playerWeapons));
        }

        public static void removeWeaponComponent(Player client, WeaponHash weaponHash, WeaponComponent weaponComponent)
        {
            DbPlayer dbPlayer = client.GetPlayer();
            if (dbPlayer == null || !dbPlayer.IsValid(true) || dbPlayer.Client == null)
                return;

            dbPlayer.Client.RemoveOwnWeaponComponent(weaponHash, weaponComponent);

            List<NXWeapon> playerWeapons = NAPI.Util.FromJson<List<NXWeapon>>(dbPlayer.GetAttributeString("Loadout"));

            var weapon = playerWeapons.FirstOrDefault(w => w.Weapon == weaponHash);
            if (weapon == null) return;

            if (weapon.Components.Contains(weaponComponent))
                weapon.Components.Remove(weaponComponent);

            dbPlayer.Loadout = playerWeapons;
            dbPlayer.RefreshData(dbPlayer);

            dbPlayer.SetAttribute("Loadout", NAPI.Util.ToJson(playerWeapons));
        }

        public static void removeAllWeapons(Player client)
        {
            DbPlayer dbPlayer = client.GetPlayer();
            if (dbPlayer == null || !dbPlayer.IsValid(true) || dbPlayer.Client == null)
                return;

            dbPlayer.Client.ResetAllOwnWeaponComponents();
            client.RemoveAllWeapons();

            dbPlayer.Loadout.Clear();
            dbPlayer.RefreshData(dbPlayer);

            dbPlayer.SetAttribute("Loadout", NAPI.Util.ToJson(new List<WeaponHash>()));
        }
    }
}