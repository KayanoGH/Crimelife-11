using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using GVMP;

namespace Crimelife
{
    public class BarberObject
    {
        [JsonProperty(PropertyName = "barber")]
        public ListJsonBarberObject Barber { get; set; }

        [JsonProperty(PropertyName = "player")]
        public BarberPlayerObject Player { get; set; }

        public BarberObject() { }
    }
}
