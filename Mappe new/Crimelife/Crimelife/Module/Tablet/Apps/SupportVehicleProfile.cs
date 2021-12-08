using GTANetworkAPI;
using MySql.Data.MySqlClient;
using System.Linq;
using GVMP;

namespace Crimelife
{
    internal class SupportVehicleProfile : Script
    {

        /*public SupportVehicleProfile() : ("SupportVehicleProfile")
		{
		}*/

        /*[RemoteEvent]
		public async void requestVehicleData(Client client, int id)
		{
			DbPlayer player = client.GetPlayer();
			if (player != null && player.IsValid())
			{
				//List<VehicleData> vehicleData = SupportVehicleFunctions.GetVehicleData(SupportVehicleFunctions.VehicleCategory.ID, id);
				//string text = NAPI.Util.ToJson((object)vehicleData);
				//TriggerEvent(client, "responseVehicleData", text);
			}
		}*/

        /*[RemoteEvent]
		public async void requestSupportVehicleList(Client client, int owner)
		{
			DbPlayer player = client.GetPlayer();
			if (player != null && player.IsValid())
			{
				List<VehicleData> vehicleData = SupportVehicleFunctions.GetVehicleData(SupportVehicleFunctions.VehicleCategory.ALL, owner);
				string text = NAPI.Util.ToJson((object)vehicleData);
				client.TriggerEvent("responseVehicleList", text);
				
			}
		}*/

        [RemoteEvent("requestVehicleData")]
        public void requestVehicleData(Player p, string id)
        {
            DbPlayer dbPlayer = p.GetPlayer();
            if (id == null) return;
            int primary = 0;
            bool primary2 = int.TryParse(id, out primary);
            if (!primary2)
            {
                dbPlayer.SendNotification("Gebe bitte eine gültige Fahrzeug ID an!", 3000, "red", "FEHLER!");
                return;
            }
            MySqlQuery query = new MySqlQuery("SELECT * FROM vehicles WHERE Id = @id LIMIT 1");
            query.AddParameter("@id", id);
            MySqlResult query2 = MySqlHandler.GetQuery(query);
            try
            {
                MySqlDataReader reader = query2.Reader;
                try
                {
                    if ((reader).HasRows)
                    {
                        while ((reader).Read())
                        {


                            string parked = "";
                            //	NAPI.Pools.GetAllVehicles().FindAll((Vehicle veh) => veh.GetVehicle() != null && veh.GetVehicle().Id == id).ForEach(delegate (Vehicle veh)
                            //{

                            foreach (GarageVehicle garageVehicles in MySqlManager.GetAllVehicles())
                            {
                                //Garage garage = GarageModule.garages.FirstOrDefault((Garage garage) => garage.Name == garageVehicles.Garage);
                                if (garageVehicles.Id == primary)
                                {
                                    MySqlQuery query3 = new MySqlQuery("SELECT * FROM accounts WHERE Id = @id LIMIT 1");
                                    query3.AddParameter("@id", garageVehicles.OwnerID);
                                    MySqlResult query4 = MySqlHandler.GetQuery(query3);
                                    MySqlDataReader reader2 = query4.Reader;
                                    reader2.Read();




                                    //	DbVehicle dbVehicle = veh.GetVehicle();

                                    //Vehicle vehicle2 = vehicle.Plate;



                                    if (reader.GetInt32("Parked") == 1)
                                    {
                                        parked = "Eingeparkt";
                                    }
                                    else
                                    {
                                        parked = "Ausgeparkt";
                                    }

                                    if (garageVehicles == null) return;
                                    if (parked == null) return;
                                    if (p == null) return;
                                    if (garageVehicles.Id == null) return;
                                    if (id == null) return;
                                    if (primary == null) return;
                                    if (primary == 1) return;
                                    p.TriggerEvent("componentServerEvent", new object[3]{"SupportVehicleProfile","responseVehicleData","[{\"id\":\"" + "0" +"\",\"vehiclehash\":\"" + "Null" +"\",\"inGarage\":\"" + "false" +"\",\"owner\":\"" + reader2.GetString("Username") +"\",\"garage\":\"" + "Null" +"\"}]"
                                    });
                                    //});



                                }




                            }
                        }

                    }
                }
                finally
                {
                    reader.Dispose();
                }
            }
            finally
            {
                query2.Connection.Dispose();
            }

        }






        [RemoteEvent("SupportGoToVehicle")]
        public void SupportGoToVehicle(Player p, int id)
        {
            try
            {
                DbPlayer player = p.GetPlayer();
                //foreach (Vehicle vehicle in NAPI.Pools.GetAllVehicles())
                //{
                //DbVehicle vehicle2 = vehicle.GetVehicle();
                //DbVehicle vehicle2 = vehicle.GetVehicle();

                //Vehicle vehicle2 = vehicle.Plate;
                //59160
                //if (vehicle == id)
                //{
                MySqlQuery query = new MySqlQuery("SELECT * FROM vehicles WHERE Id = @id LIMIT 1");
                query.AddParameter("@id", id);
                MySqlResult query2 = MySqlHandler.GetQuery(query);
                try
                {
                    MySqlDataReader reader = query2.Reader;
                    try
                    {
                        if ((reader).HasRows)
                        {
                            while ((reader).Read())
                            {
                                if (reader.GetInt32("Parked") == 0)
                                {
                                    foreach (Vehicle vehicle in NAPI.Pools.GetAllVehicles().Where(x => x != null && x.Exists))
                                    {
                                        if (vehicle == null || !vehicle.Exists) continue;
                                        DbVehicle vehicle2 = vehicle.GetVehicle();
                                        if (vehicle2 == null) return;
                                        if (vehicle2.Id == id)
                                        {

                                            p.Position = vehicle.Position.Add(new Vector3(0, 0, 1.5));
                                            player.SendNotification("Du hast dich zu dem Fahrzeug " + vehicle2.Model + " teleportiert!", 5000, "red", "SUPPORT");
                                        }
                                    }
                                }
                                else
                                {
                                    Garage garage = GarageModule.garages.FirstOrDefault((Garage garage) => garage.Name == reader.GetString("Garage"));
                                    p.Position = garage.Position.Add(new Vector3(0, 0, 1.5));
                                    player.SendNotification("Du hast dich zu " + garage.Name + " teleportiert!", 5000, "red", "SUPPORT");
                                }
                            }
                        }

                    }


                    finally
                    {
                        reader.Dispose();
                    }
                }
                finally
                {
                    query2.Connection.Dispose();
                }
            }
            catch (System.Exception ex)
            {
                System.Console.WriteLine(ex);
            }
        }


        [RemoteEvent("SupportSetGarage")]
        public void SupportSetGarage(Player p, int id)
        {
            try
            {
                DbPlayer player = p.GetPlayer();
                //foreach (Vehicle vehicle in NAPI.Pools.GetAllVehicles())
                //{
                //DbVehicle vehicle2 = vehicle.GetVehicle();
                //DbVehicle vehicle2 = vehicle.GetVehicle();

                //Vehicle vehicle2 = vehicle.Plate;
                //59160
                //if (vehicle == id)
                //{
                MySqlQuery query = new MySqlQuery("SELECT * FROM vehicles WHERE Id = @id LIMIT 1");
                query.AddParameter("@id", id);
                MySqlResult query2 = MySqlHandler.GetQuery(query);
                try
                {
                    MySqlDataReader reader = query2.Reader;
                    try
                    {
                        if ((reader).HasRows)
                        {
                            while ((reader).Read())
                            {
                                MySqlQuery mySqlQuery = new MySqlQuery("UPDATE vehicles SET Parked = 1 WHERE Id = @id");
                                mySqlQuery.AddParameter("@id", id);
                                MySqlHandler.ExecuteSync(mySqlQuery);
                                if (reader.GetInt32("Parked") == 0)
                                {
                                    foreach (Vehicle vehicle in NAPI.Pools.GetAllVehicles().Where(x => x != null && x.Exists))
                                    {
                                        if (vehicle == null || !vehicle.Exists) continue;
                                        DbVehicle vehicle2 = vehicle.GetVehicle();
                                        if (vehicle2 == null) return;
                                        if (vehicle2.Id == id)
                                        {
                                            ((Entity)vehicle).Delete();

                                            player.SendNotification("Fahrzeug " + vehicle2.Model + " wurde in die Garage gesetzt", 5000, "red", "SUPPORT");
                                        }
                                    }
                                }
                                else
                                {
                                    player.SendNotification("Fahrzeug Bereits eingeparkt!", 5000, "red", "SUPPORT");
                                }
                            }
                        }

                    }


                    finally
                    {
                        reader.Dispose();
                    }
                }
                finally
                {
                    query2.Connection.Dispose();
                }
            }
            catch (System.Exception ex)
            {
                System.Console.WriteLine(ex);
            }
        }
    }
    /*[RemoteEvent]
        public async void SupportSetGarage(Client client, uint vehicleId)
        {
            DbPlayer player = client.GetPlayer();
            if (player == null || !player.IsValid())
            {
                return;
            }
            foreach (Vehicle vehicle in NAPI.Pools.GetAllVehicles())
            {
                DbVehicle byVehicleDatabaseId = vehicle.GetVehicle();




                if (byVehicleDatabaseId != null)
            {
                if (byVehicleDatabaseId.IsValid() && byVehicleDatabaseId.Id == vehicleId)
                {
                        MySqlQuery mySqlQuery = new MySqlQuery("UPDATE vehicles SET Parked = 1 WHERE Id = @id");
                        mySqlQuery.AddParameter("@id", vehicleId);
                        ((Entity)vehicle).Delete();
                        //byVehicleDatabaseId.SetPrivateCarGarage(1u);
                        player.SendNotification("Fahrzeug " + byVehicleDatabaseId.Model + " wurde in die Garage gesetzt!", 3000, "red", "ADMIN");
                }
                else
                {
                    player.SendNotification("Fahrzeug ist kein privat Fahrzeug!", 3000, "red", "ADMIN");
                }
                }
            }
        }*/
}


