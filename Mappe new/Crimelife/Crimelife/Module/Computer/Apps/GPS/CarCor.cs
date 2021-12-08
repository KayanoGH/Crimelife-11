using Newtonsoft.Json;

namespace Crimelife
{
	public class CarCoorinate
	{
		[JsonProperty(PropertyName = "x")]
		public float position_x { get; set; }

		[JsonProperty(PropertyName = "y")]
		public float position_y { get; set; }

		[JsonProperty(PropertyName = "z")]
		public float position_z { get; set; }
	}
}
