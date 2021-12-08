using GTANetworkAPI;
using GVMP.Handlers;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GVMP;

namespace Crimelife
{
	public class FahrzeugÜbersichtApp : Script
	{
		[RemoteEvent("requestVehicleOverviewByCategory")]
		public void requestVehicleOverviewByCategory(Player p, int id)
		{
			DbPlayer dbPlayer = p.GetPlayer();
			if (dbPlayer == null || !dbPlayer.IsValid(true) || dbPlayer.Client == null)
				return;

			if (!dbPlayer.CanInteractAntiFlood(5))
			//if (dbPlayer.LastInteracted.AddSeconds(5.0) > DateTime.Now)
			{
				dbPlayer.SendNotification("Antispam: Bitte 5 Sekunden warten!");
				return;
			}
			List<OwnVehicleModel> ownedVehicles = new List<OwnVehicleModel>();
			List<OwnVehicleModel> keys = new List<OwnVehicleModel>();


			foreach (GarageVehicle garageVehicles in MySqlManager.GetAllVehicles())
			{
				Garage garage = GarageModule.garages.FirstOrDefault((Garage garage) => garage.Name == garageVehicles.Garage);
				CarCoorinate carCor = new CarCoorinate
				{
					position_x = garage.Position.X,
					position_y = garage.Position.Y,
					position_z = garage.Position.Z
				};
				if (garageVehicles.Keys.Contains(dbPlayer.Id))
					ownedVehicles.Add(new OwnVehicleModel(garageVehicles.Name, garageVehicles.Id, garageVehicles.Parked, garageVehicles.Garage, carCor));
			}


			foreach (GarageVehicle garageVehicles in MySqlManager.GetAllVehicles())
			{
				Garage garage = GarageModule.garages.FirstOrDefault((Garage garage) => garage.Name == garageVehicles.Garage);
				CarCoorinate carCor = new CarCoorinate
				{
					position_x = garage.Position.X,
					position_y = garage.Position.Y,
					position_z = garage.Position.Z
				};
				if (dbPlayer.VehicleKeys.ContainsKey(garageVehicles.Id))
					keys.Add(new OwnVehicleModel(garageVehicles.Name, garageVehicles.Id, garageVehicles.Parked, garageVehicles.Garage, carCor));
			}





			if (id == 0)
			{
				/*if (!dbPlayer.CanInteractAntiFlood(5))
				//if (dbPlayer.LastInteracted.AddSeconds(5.0) > DateTime.Now)
				{
					dbPlayer.SendNotification("Antispam: Bitte 5 Sekunden warten!");
					return;
				}*/
				p.TriggerEvent("componentServerEvent", new object[3]
			{
					"FahrzeugUebersichtApp",
					"responseVehicleOverview",
					NAPI.Util.ToJson(ownedVehicles)
			});

			}

			if (id == 1)
			{
				/*if (!dbPlayer.CanInteractAntiFlood(5))
				//if (dbPlayer.LastInteracted.AddSeconds(5.0) > DateTime.Now)
				{
					dbPlayer.SendNotification("Antispam: Bitte 5 Sekunden warten!");
					return;
				}*/
				p.TriggerEvent("componentServerEvent", new object[3]
				{
					"FahrzeugUebersichtApp",
					"responseVehicleOverview",
					NAPI.Util.ToJson(keys)
				});

			}
		}
	}
}