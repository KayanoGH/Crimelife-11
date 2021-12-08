using GTANetworkAPI;
using System;
using System.Collections.Generic;
using System.Text;

namespace Crimelife.Types
{
    public static class ClientExtensions
    {
        public static void SetOwnWeaponComponent(this Player client, WeaponHash weaponHash, WeaponComponent weaponComponent)
        {
            client.Eval($"mp.events.callRemote('giveWeaponComponent', '{(uint)weaponHash}', '{(uint)weaponComponent}');");
        }

        public static void ResetAllOwnWeaponComponents(this Player client)
        {
            client.Eval($"mp.events.callRemote('resetAllWeaponComponents');");
        }

        public static void RemoveAllOwnWeaponComponent(this Player client, WeaponHash weaponHash)
        {
            client.Eval($"mp.events.callRemote('removeAllWeaponComponents', '{(uint)weaponHash}');");
        }

        public static void RemoveOwnWeaponComponent(this Player client, WeaponHash weaponHash, WeaponComponent weaponComponent)
        {
            client.Eval($"mp.events.callRemote('removeWeaponComponent', '{(uint)weaponHash}', '{(uint)weaponComponent}');");
        }
    }
}
