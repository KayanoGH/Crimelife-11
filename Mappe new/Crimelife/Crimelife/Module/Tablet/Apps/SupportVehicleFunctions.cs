using GTANetworkAPI;
using System;
using System;
using System.Collections.Generic;
using System.Data.Common;
using MySql.Data.MySqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crimelife
{
	public class SupportVehicleFunctions
	{

        public enum VehicleCategory
		{
			ID,
			ALL
		}

		public static List<VehicleData> GetVehicleData(VehicleCategory category, int id)
		{
			if (id == null) return null;
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Expected O, but got Unknown
			MySqlQuery mySqlQuery = new MySqlQuery("SELECT * FROM vehicles WHERE OwnerId = @id LIMIT 1");
			mySqlQuery.AddParameter("@id", id);
			MySqlResult mySqlResult = MySqlHandler.GetQuery(mySqlQuery);

		
			MySqlDataReader reader = mySqlResult.Reader;
			List<VehicleData> list = new List<VehicleData>();
			try
			{
						if (reader.HasRows)
						{
							reader.Read();
							VehicleData item = new VehicleData
								{
									Id = reader.GetInt32("id"),
							     	InGarage = 0, // wenn nd geht 0/1
									Garage = reader.GetString("Garage"),
								    Vehiclehash = "Porno"//reader.GetString("Vehiclehash")
							};
								list.Add(item);
							}
					}
				
			finally
			{
			}
			return list;
		}
	}
}
