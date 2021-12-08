using GTANetworkAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GVMP;
using Crimelife.Module;


namespace Crimelife
{
	public sealed class ReversePhoneModule : Module<ReversePhoneModule>
	{
		public Dictionary<int, List<PhoneHistoryEntry>> phoneHistory = new Dictionary<int, List<PhoneHistoryEntry>>();

		public override bool Load(bool reload = false)
		{
			phoneHistory = new Dictionary<int, List<PhoneHistoryEntry>>();
			return true;
		}

		public void AddPhoneHistory(DbPlayer iPlayer, int number, int dauer)
		{
			//DbPlayer dbPlayer = c.GetPlayer();
			if (!phoneHistory.ContainsKey(iPlayer.Id))
			{
				phoneHistory.Add(iPlayer.Id, new List<PhoneHistoryEntry>());
			}
			phoneHistory[iPlayer.Id].Add(new PhoneHistoryEntry(number, dauer));
			iPlayer.Client.TriggerEvent("componentServerEvent", "TelefonCalls", "responsePhoneCalls",
			NAPI.Util.ToJson(phoneHistory));
		}
	}
}
;
/*using GVMP.Module;
using GTANetworkAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace GVMP
{
	public sealed class ReversePhoneModule : Module<ReversePhoneModule>
	{
		public Dictionary<int, List<PhoneHistoryEntry>> phoneHistory = new Dictionary<int, List<PhoneHistoryEntry>>();

		public override bool Load(bool reload = false)
		{
			phoneHistory = new Dictionary<int, List<PhoneHistoryEntry>>();
			return true;
		}

		public void AddPhoneHistory(DbPlayer iPlayer)
		{
			//DbPlayer dbPlayer = c.GetPlayer();
			if (!phoneHistory.ContainsKey(iPlayer.Id))
			{
			//phoneHistory.Add(iPlayer.Id, new PhoneHistoryEntry());
			phoneHistory.Add(iPlayer.Id, new List<PhoneHistoryEntry>());
			}
			//phoneHistory[iPlayer.Id].Add(new PhoneHistoryEntry());
			iPlayer.Client.TriggerEvent("componentServerEvent", "TelefonCalls", "responsePhoneCalls",
			NAPI.Util.ToJson(phoneHistory));
		}
	}
}
;*/