using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Crimelife
{
    public class VHKey
    {
		[JsonProperty(PropertyName = "name")]
		public string Name { get; set; }

		[JsonProperty(PropertyName = "id")]
		public int id { get; set; }

		public VHKey(string name, int id)
		{
			Name = name;
			this.id = id;
		}
	}
}
