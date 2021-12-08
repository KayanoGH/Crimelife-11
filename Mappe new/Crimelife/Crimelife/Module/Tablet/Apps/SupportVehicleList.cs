/*using System.Collections.Generic;
using GTANetworkAPI;
using VMP_CNR.Module.ClientUI.Apps;
using VMP_CNR.Module.Players;
using VMP_CNR.Module.Players.Db;

namespace VMP_CNR.Module.Computer.Apps.SupportVehicleApp.Apps
{
	internal class SupportVehicleList : SimpleApp
	{
		public SupportVehicleList()
			: base("SupportVehicleList")
		{
		}

		[RemoteEvent]
		public async void requestSupportVehicleList(Client client, int owner)
		{
			DbPlayer player = client.GetPlayer();
			if (player != null && player.IsValid())
			{
				List<VehicleData> vehicleData = SupportVehicleFunctions.GetVehicleData(SupportVehicleFunctions.VehicleCategory.ALL, owner);
				string text = NAPI.Util.ToJson((object)vehicleData);
				TriggerEvent(client, "responseVehicleList", text);
			}
		}
	}
}*/
