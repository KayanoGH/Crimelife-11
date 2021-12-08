﻿using GTANetworkAPI;
using System;
using System.Collections.Generic;
using System.Text;
using GVMP;

namespace Crimelife
{
    class PaintballModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<Vector3> Spawns { get; set; }
        public int MaxPlayer { get; set; }
        public List<DbPlayer> Players()
        {
            return PlayerHandler.GetPlayers().FindAll(p => p.HasData("PBZone") && p.GetPBData("PBZone") != null && ((PaintballModel)p.GetPBData("PBZone")).Id == this.Id);
        }
        public List<WeaponHash> Weapons { get; set; } = new List<WeaponHash>();

        public PaintballModel() { }
    }
}
