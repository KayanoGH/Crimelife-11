using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Crimelife
{
    public class Advertise
    {
        [JsonProperty("id")]
        public int Id { get; set; }
        [JsonProperty("title")]
        public string Title { get; set; }
        [JsonProperty("content")]
        public string Content { get; set; }
        [JsonProperty("expires")]
        public DateTime Expires { get; set; }

        public Advertise() { }
    }
}
