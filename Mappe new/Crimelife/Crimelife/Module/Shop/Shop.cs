using GTANetworkAPI;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Crimelife
{
    public class Shop
    {
        [JsonProperty("id")]
        public int Id { get; set; }
        [JsonProperty("title")]
        public string Title { get; set; }
        [JsonProperty("customBlip")]
        public int Blip { get; set; } = 0;
        [JsonProperty("customBlipColor")]
        public int BlipColor { get; set; }
        [JsonProperty("position")]
        public Vector3 Position { get; set; }
        [JsonProperty("items")]
        public List<BuyItem> Items { get; set; }
        [JsonProperty("LastRob")]
        public DateTime LastRob { get; set; }

        public Shop() { }
    }
}
