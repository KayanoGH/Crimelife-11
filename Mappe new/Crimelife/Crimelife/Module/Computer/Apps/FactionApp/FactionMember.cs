using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Crimelife
{
    public class FactionMember
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("rang")]
        public int Rang { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("payday")]
        public string Payday { get; set; }

        public FactionMember() { }
    }
}
