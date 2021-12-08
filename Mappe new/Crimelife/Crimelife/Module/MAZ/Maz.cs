using GTANetworkAPI;
using System;

namespace Crimelife.Module.MAZ
{
    public class Maz
    {
        public int Id { get; set; }
        public Vector3 Spawn { get; set; }
        public string Storage { get; set; }
        public DateTime LastSpawn { get; set; }
        public int Open { get; set; }
        public int Broke { get; set; }
        public Maz() { }
    }
}
