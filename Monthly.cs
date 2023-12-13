using CumulusMX;
using System;
using System.Collections.Generic;
using System.IO;

namespace CreateRecords
{
	class Monthly
	{
		public List<Records> Recs = new List<Records>(13);
		private static readonly string filename = "data" + Path.DirectorySeparatorChar + "monthlyalltime.ini";

		public Monthly()
		{
			for (var i = 0; i < 13; i++)
				Recs.Add(new Records());

			ReadIniFile();
		}


		public void ReadIniFile()
		{
			if (File.Exists(filename))
			{
				var defTime = DateTime.MinValue;

				IniFile ini = new IniFile(filename);
				for (int month = 1; month <= 12; month++)
				{
					string monthstr = month.ToString("D2");

					Recs[month].HighTemperature.Value = ini.GetValue("Temperature" + monthstr, "hightempvalue", Cumulus.DefaultHiVal);
					Recs[month].HighTemperature.Time = ini.GetValue("Temperature" + monthstr, "hightemptime", defTime);

					Recs[month].LowTemperature.Value = ini.GetValue("Temperature" + monthstr, "lowtempvalue", Cumulus.DefaultLoVal);
					Recs[month].LowTemperature.Time = ini.GetValue("Temperature" + monthstr, "lowtemptime", defTime);

					Recs[month].LowWindChill.Value = ini.GetValue("Temperature" + monthstr, "lowchillvalue", Cumulus.DefaultLoVal);
					Recs[month].LowWindChill.Time = ini.GetValue("Temperature" + monthstr, "lowchilltime", defTime);

					Recs[month].HighMinTemp.Value = ini.GetValue("Temperature" + monthstr, "highmintempvalue", Cumulus.DefaultHiVal);
					Recs[month].HighMinTemp.Time = ini.GetValue("Temperature" + monthstr, "highmintemptime", defTime);

					Recs[month].LowMaxTemp.Value = ini.GetValue("Temperature" + monthstr, "lowmaxtempvalue", Cumulus.DefaultLoVal);
					Recs[month].LowMaxTemp.Time = ini.GetValue("Temperature" + monthstr, "lowmaxtemptime", defTime);

					Recs[month].HighApparant.Value = ini.GetValue("Temperature" + monthstr, "highapptempvalue", Cumulus.DefaultHiVal);
					Recs[month].HighApparant.Time = ini.GetValue("Temperature" + monthstr, "highapptemptime", defTime);

					Recs[month].LowApparant.Value = ini.GetValue("Temperature" + monthstr, "lowapptempvalue", Cumulus.DefaultLoVal);
					Recs[month].LowApparant.Time = ini.GetValue("Temperature" + monthstr, "lowapptemptime", defTime);

					Recs[month].HighFeelsLike.Value = ini.GetValue("Temperature" + monthstr, "highfeelslikevalue", Cumulus.DefaultHiVal);
					Recs[month].HighFeelsLike.Time = ini.GetValue("Temperature" + monthstr, "highfeelsliketime", defTime);

					Recs[month].LowFeelsLike.Value = ini.GetValue("Temperature" + monthstr, "lowfeelslikevalue", Cumulus.DefaultLoVal);
					Recs[month].LowFeelsLike.Time = ini.GetValue("Temperature" + monthstr, "lowfeelsliketime", defTime);

					Recs[month].HighHumidex.Value = ini.GetValue("Temperature" + monthstr, "highhumidexvalue", Cumulus.DefaultHiVal);
					Recs[month].HighHumidex.Time = ini.GetValue("Temperature" + monthstr, "highhumidextime", defTime);

					Recs[month].HighHeatIndex.Value = ini.GetValue("Temperature" + monthstr, "highheatindexvalue", Cumulus.DefaultHiVal);
					Recs[month].HighHeatIndex.Time = ini.GetValue("Temperature" + monthstr, "highheatindextime", defTime);

					Recs[month].HighDewPoint.Value = ini.GetValue("Temperature" + monthstr, "highdewpointvalue", Cumulus.DefaultHiVal);
					Recs[month].HighDewPoint.Time = ini.GetValue("Temperature" + monthstr, "highdewpointtime", defTime);

					Recs[month].LowDewPoint.Value = ini.GetValue("Temperature" + monthstr, "lowdewpointvalue", Cumulus.DefaultLoVal);
					Recs[month].LowDewPoint.Time = ini.GetValue("Temperature" + monthstr, "lowdewpointtime", defTime);

					Recs[month].HighDailyRange.Value = ini.GetValue("Temperature" + monthstr, "hightemprangevalue", Cumulus.DefaultHiVal);
					Recs[month].HighDailyRange.Time = ini.GetValue("Temperature" + monthstr, "hightemprangetime", defTime);

					Recs[month].LowDailyRange.Value = ini.GetValue("Temperature" + monthstr, "lowtemprangevalue", Cumulus.DefaultLoVal);
					Recs[month].LowDailyRange.Time = ini.GetValue("Temperature" + monthstr, "lowtemprangetime", defTime);

					Recs[month].HighWindAvg.Value = ini.GetValue("Wind" + monthstr, "highwindvalue", Cumulus.DefaultHiVal);
					Recs[month].HighWindAvg.Time = ini.GetValue("Wind" + monthstr, "highwindtime", defTime);

					Recs[month].HighWindGust.Value = ini.GetValue("Wind" + monthstr, "highgustvalue", Cumulus.DefaultHiVal);
					Recs[month].HighWindGust.Time = ini.GetValue("Wind" + monthstr, "highgusttime", defTime);

					Recs[month].HighWindRun.Value = ini.GetValue("Wind" + monthstr, "highdailywindrunvalue", Cumulus.DefaultHiVal);
					Recs[month].HighWindRun.Time = ini.GetValue("Wind" + monthstr, "highdailywindruntime", defTime);

					Recs[month].HighRainRate.Value = ini.GetValue("Rain" + monthstr, "highrainratevalue", Cumulus.DefaultHiVal);
					Recs[month].HighRainRate.Time = ini.GetValue("Rain" + monthstr, "highrainratetime", defTime);

					Recs[month].HighDailyRain.Value = ini.GetValue("Rain" + monthstr, "highdailyrainvalue", Cumulus.DefaultHiVal);
					Recs[month].HighDailyRain.Time = ini.GetValue("Rain" + monthstr, "highdailyraintime", defTime);

					Recs[month].High24hRain.Value = ini.GetValue("Rain" + monthstr, "high24hourrainvalue", Cumulus.DefaultHiVal);
					Recs[month].High24hRain.Time = ini.GetValue("Rain" + monthstr, "high24hourraintime", defTime);

					Recs[month].HighHourlyRain.Value = ini.GetValue("Rain" + monthstr, "highhourlyrainvalue", Cumulus.DefaultHiVal);
					Recs[month].HighHourlyRain.Time = ini.GetValue("Rain" + monthstr, "highhourlyraintime", defTime);

					Recs[month].HighMonthlyRain.Value = ini.GetValue("Rain" + monthstr, "highmonthlyrainvalue", Cumulus.DefaultHiVal);
					Recs[month].HighMonthlyRain.Time = ini.GetValue("Rain" + monthstr, "highmonthlyraintime", defTime);

					Recs[month].LongestDry.Value = ini.GetValue("Rain" + monthstr, "longestdryperiodvalue", 0);
					Recs[month].LongestDry.Time = ini.GetValue("Rain" + monthstr, "longestdryperiodtime", defTime);

					Recs[month].LongestWet.Value = ini.GetValue("Rain" + monthstr, "longestwetperiodvalue", 0);
					Recs[month].LongestWet.Time = ini.GetValue("Rain" + monthstr, "longestwetperiodtime", defTime);

					Recs[month].HighPressure.Value = ini.GetValue("Pressure" + monthstr, "highpressurevalue", Cumulus.DefaultHiVal);
					Recs[month].HighPressure.Time = ini.GetValue("Pressure" + monthstr, "highpressuretime", defTime);

					Recs[month].LowPressure.Value = ini.GetValue("Pressure" + monthstr, "lowpressurevalue", Cumulus.DefaultLoVal);
					Recs[month].LowPressure.Time = ini.GetValue("Pressure" + monthstr, "lowpressuretime", defTime);

					Recs[month].HighHumidity.Value = ini.GetValue("Humidity" + monthstr, "highhumidityvalue", (int) Cumulus.DefaultHiVal);
					Recs[month].HighHumidity.Time = ini.GetValue("Humidity" + monthstr, "highhumiditytime", defTime);

					Recs[month].LowHumidity.Value = ini.GetValue("Humidity" + monthstr, "lowhumidityvalue", (int) Cumulus.DefaultLoVal);
					Recs[month].LowHumidity.Time = ini.GetValue("Humidity" + monthstr, "lowhumiditytime", defTime);
				}

				Program.LogMessage("Completed monthlyAlltime.ini file read");
			}
			else
			{
				Program.LogMessage("No monthlyAlltime.ini file found");
			}

		}

		public void WriteIniFile()
		{
			// backup original file
			if (File.Exists(filename))
			{
				File.Move(filename, filename + ".sav");
			}

			Console.WriteLine("Writing to monthly.ini file...");

			try
			{
				IniFile ini = new IniFile(filename);
				for (int month = 1; month <= 12; month++)
				{
					string monthstr = month.ToString("D2");

					ini.SetValue("Temperature" + monthstr, "hightempvalue", Recs[month].HighTemperature.Value);
					ini.SetValue("Temperature" + monthstr, "hightemptime", Recs[month].HighTemperature.Time);
					ini.SetValue("Temperature" + monthstr, "lowtempvalue", Recs[month].LowTemperature.Value);
					ini.SetValue("Temperature" + monthstr, "lowtemptime", Recs[month].LowTemperature.Time);
					ini.SetValue("Temperature" + monthstr, "lowchillvalue", Recs[month].LowWindChill.Value);
					ini.SetValue("Temperature" + monthstr, "lowchilltime", Recs[month].LowWindChill.Time);
					ini.SetValue("Temperature" + monthstr, "highmintempvalue", Recs[month].HighMinTemp.Value);
					ini.SetValue("Temperature" + monthstr, "highmintemptime", Recs[month].HighMinTemp.Time);
					ini.SetValue("Temperature" + monthstr, "lowmaxtempvalue", Recs[month].LowMaxTemp.Value);
					ini.SetValue("Temperature" + monthstr, "lowmaxtemptime", Recs[month].LowMaxTemp.Time);
					ini.SetValue("Temperature" + monthstr, "highapptempvalue", Recs[month].HighApparant.Value);
					ini.SetValue("Temperature" + monthstr, "highapptemptime", Recs[month].HighApparant.Time);
					ini.SetValue("Temperature" + monthstr, "lowapptempvalue", Recs[month].LowApparant.Value);
					ini.SetValue("Temperature" + monthstr, "lowapptemptime", Recs[month].LowApparant.Time);
					ini.SetValue("Temperature" + monthstr, "highfeelslikevalue", Recs[month].HighFeelsLike.Value);
					ini.SetValue("Temperature" + monthstr, "highfeelsliketime", Recs[month].HighFeelsLike.Time);
					ini.SetValue("Temperature" + monthstr, "lowfeelslikevalue", Recs[month].LowFeelsLike.Value);
					ini.SetValue("Temperature" + monthstr, "lowfeelsliketime", Recs[month].LowFeelsLike.Time);
					ini.SetValue("Temperature" + monthstr, "highhumidexvalue", Recs[month].HighHumidex.Value);
					ini.SetValue("Temperature" + monthstr, "highhumidextime", Recs[month].HighHumidex.Time);
					ini.SetValue("Temperature" + monthstr, "highheatindexvalue", Recs[month].HighHeatIndex.Value);
					ini.SetValue("Temperature" + monthstr, "highheatindextime", Recs[month].HighHeatIndex.Time);
					ini.SetValue("Temperature" + monthstr, "highdewpointvalue", Recs[month].HighDewPoint.Value);
					ini.SetValue("Temperature" + monthstr, "highdewpointtime", Recs[month].HighDewPoint.Time);
					ini.SetValue("Temperature" + monthstr, "lowdewpointvalue", Recs[month].LowDewPoint.Value);
					ini.SetValue("Temperature" + monthstr, "lowdewpointtime", Recs[month].LowDewPoint.Time);
					ini.SetValue("Temperature" + monthstr, "hightemprangevalue", Recs[month].HighDailyRange.Value);
					ini.SetValue("Temperature" + monthstr, "hightemprangetime", Recs[month].HighDailyRange.Time);
					ini.SetValue("Temperature" + monthstr, "lowtemprangevalue", Recs[month].LowDailyRange.Value);
					ini.SetValue("Temperature" + monthstr, "lowtemprangetime", Recs[month].LowDailyRange.Time);
					ini.SetValue("Wind" + monthstr, "highwindvalue", Recs[month].HighWindAvg.Value);
					ini.SetValue("Wind" + monthstr, "highwindtime", Recs[month].HighWindAvg.Time);
					ini.SetValue("Wind" + monthstr, "highgustvalue", Recs[month].HighWindGust.Value);
					ini.SetValue("Wind" + monthstr, "highgusttime", Recs[month].HighWindGust.Time);
					ini.SetValue("Wind" + monthstr, "highdailywindrunvalue", Recs[month].HighWindRun.Value);
					ini.SetValue("Wind" + monthstr, "highdailywindruntime", Recs[month].HighWindRun.Time);
					ini.SetValue("Rain" + monthstr, "highrainratevalue", Recs[month].HighRainRate.Value);
					ini.SetValue("Rain" + monthstr, "highrainratetime", Recs[month].HighRainRate.Time);
					ini.SetValue("Rain" + monthstr, "highdailyrainvalue", Recs[month].HighDailyRain.Value);
					ini.SetValue("Rain" + monthstr, "highdailyraintime", Recs[month].HighDailyRain.Time);
					ini.SetValue("Rain" + monthstr, "highhourlyrainvalue", Recs[month].HighHourlyRain.Value);
					ini.SetValue("Rain" + monthstr, "highhourlyraintime", Recs[month].HighHourlyRain.Time);
					ini.SetValue("Rain" + monthstr, "highmonthlyrainvalue", Recs[month].HighMonthlyRain.Value);
					ini.SetValue("Rain" + monthstr, "highmonthlyraintime", Recs[month].HighMonthlyRain.Time);
					ini.SetValue("Rain" + monthstr, "longestdryperiodvalue", Recs[month].LongestDry.Value);
					ini.SetValue("Rain" + monthstr, "longestdryperiodtime", Recs[month].LongestDry.Time);
					ini.SetValue("Rain" + monthstr, "longestwetperiodvalue", Recs[month].LongestWet.Value);
					ini.SetValue("Rain" + monthstr, "longestwetperiodtime", Recs[month].LongestWet.Time);
					ini.SetValue("Rain" + monthstr, "high24hourrainvalue", Recs[month].High24hRain.Value);
					ini.SetValue("Rain" + monthstr, "high24hourraintime", Recs[month].High24hRain.Time);
					ini.SetValue("Pressure" + monthstr, "highpressurevalue", Recs[month].HighPressure.Value);
					ini.SetValue("Pressure" + monthstr, "highpressuretime", Recs[month].HighPressure.Time);
					ini.SetValue("Pressure" + monthstr, "lowpressurevalue", Recs[month].LowPressure.Value);
					ini.SetValue("Pressure" + monthstr, "lowpressuretime", Recs[month].LowPressure.Time);
					ini.SetValue("Humidity" + monthstr, "highhumidityvalue", Recs[month].HighHumidity.Value);
					ini.SetValue("Humidity" + monthstr, "highhumiditytime", Recs[month].HighHumidity.Time);
					ini.SetValue("Humidity" + monthstr, "lowhumidityvalue", Recs[month].LowHumidity.Value);
					ini.SetValue("Humidity" + monthstr, "lowhumiditytime", Recs[month].LowHumidity.Time);
				}
				ini.Flush();
			}
			catch (Exception ex)
			{
				Program.LogMessage("Error writing MonthlyAlltime.ini file: " + ex.Message);
			}
		}
	}
}
