﻿using GTANetworkAPI;
using System;
using System.Collections.Generic;
using System.Text;

namespace Crimelife
{
    public class FahrzeugShop
    {
        public int Id
        {
            get;
            set;
        }

        public string Name
        {
            get;
            set;
        }

        public Vector3 Position
        {
            get;
            set;
        }

        public Vector3 CarSpawn
        {
            get;
            set;
        }

        public float CarSpawnRotation
        {
            get;
            set;
        }

        public List<BuyCar> BuyItems
        {
            get;
            set;
        }

        public FahrzeugShop() { }
    }
}
