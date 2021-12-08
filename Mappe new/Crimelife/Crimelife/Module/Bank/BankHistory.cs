using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using GVMP;

namespace Crimelife
{
    public class BankHistory
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("value")]
        public string Value { get; set; }

        public BankHistory() { }
    }
}
