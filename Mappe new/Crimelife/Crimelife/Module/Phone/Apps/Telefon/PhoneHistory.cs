using System;

namespace Crimelife
{
	public class PhoneHistoryEntry
	{
		public int Number { get; set; }

		public int Dauer { get; set; }

		public DateTime Time { get; set; }

		public PhoneHistoryEntry(int number, int dauer)
		{
			Number = number;
			Dauer = dauer;
			Time = DateTime.Now;
		}
	}
}
