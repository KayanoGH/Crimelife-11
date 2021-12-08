using GTANetworkAPI;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using GVMP;

namespace Crimelife
{
public class OwnVehicleModel
	{

        [JsonProperty(PropertyName = "id")]
		public int id { get; set; }


		[JsonProperty(PropertyName = "inGarage")]
		public int inGarage { get; set; }

		[JsonProperty(PropertyName = "garage")]
		public string garage { get; set; }

		[JsonProperty(PropertyName = "vehiclehash")]
		public string vehiclehash { get; set; }

		[JsonProperty(PropertyName = "carCor")]
		public CarCoorinate CarCor { get; set; }

		public OwnVehicleModel(string name, int id, int parked, string garage, CarCoorinate carCor)
		{
			this.vehiclehash = name;
			this.id = id;
			this.inGarage = parked;
			this.garage = garage;
			CarCor = carCor;
		}
	}
}
/*	public string vehiclehash
	{
		get;
		set;
	}

	public int id
	{
		get;
		set;
	}

	public int inGarage
	{
		get;
		set;
	}

	public string garage
	{
		get;
		set;
	}

	[JsonProperty(PropertyName = "carCor")]
	public CarCoorinate CarCor { get; set; }

	public OwnVehicleModel(string model, int id, int inGarage, string garage, Vector3 pos)
	{
		this.vehiclehash = model;
		this.id = id;
		this.inGarage = inGarage;
		this.garage = garage;
		this.CarCor = pos;
		}
}
}*/


