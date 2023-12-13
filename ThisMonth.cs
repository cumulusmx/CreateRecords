using CumulusMX;
using System;
using System.IO;

namespace CreateRecords
{
	class ThisMonth
	{
		public Records Recs = new Records();
		private static readonly string filename = "data" + Path.DirectorySeparatorChar + "month.ini";

		public ThisMonth()
		{
			ReadIniFile();
		}

		public void ReadIniFile()
		{
			//DateTime timestamp;

			if (File.Exists(filename))
			{
				var defTime = DateTime.MinValue;

				Program.LogMessage("Loading month.ini file...");

				IniFile ini = new IniFile(filename);

				Recs.HighWindAvg.Value = ini.GetValue("Wind", "Speed", Cumulus.DefaultHiVal);
				Recs.HighWindAvg.Time = ini.GetValue("Wind", "SpTime", defTime);
				Recs.HighWindGust.Value = ini.GetValue("Wind", "Gust", Cumulus.DefaultHiVal);
				Recs.HighWindGust.Time = ini.GetValue("Wind", "Time", defTime);
				Recs.HighWindRun.Value = ini.GetValue("Wind", "Windrun", Cumulus.DefaultHiVal);
				Recs.HighWindRun.Time = ini.GetValue("Wind", "WindrunTime", defTime);
				// Temperature
				Recs.LowTemperature.Value = ini.GetValue("Temp", "Low", Cumulus.DefaultLoVal);
				Recs.LowTemperature.Time = ini.GetValue("Temp", "LTime", defTime);
				Recs.HighTemperature.Value = ini.GetValue("Temp", "High", Cumulus.DefaultHiVal);
				Recs.HighTemperature.Time = ini.GetValue("Temp", "HTime", defTime);
				Recs.LowMaxTemp.Value = ini.GetValue("Temp", "LowMax", Cumulus.DefaultLoVal);
				Recs.LowMaxTemp.Time = ini.GetValue("Temp", "LMTime", defTime);
				Recs.HighMinTemp.Value = ini.GetValue("Temp", "HighMin", Cumulus.DefaultHiVal);
				Recs.HighMinTemp.Time = ini.GetValue("Temp", "HMTime", defTime);
				Recs.LowDailyRange.Value = ini.GetValue("Temp", "LowRange", Cumulus.DefaultLoVal);
				Recs.LowDailyRange.Time = ini.GetValue("Temp", "LowRangeTime", defTime);
				Recs.HighDailyRange.Value = ini.GetValue("Temp", "HighRange", Cumulus.DefaultHiVal);
				Recs.HighDailyRange.Time = ini.GetValue("Temp", "HighRangeTime", defTime);
				// Pressure
				Recs.LowPressure.Value = ini.GetValue("Pressure", "Low", Cumulus.DefaultLoVal);
				Recs.LowPressure.Time = ini.GetValue("Pressure", "LTime", defTime);
				Recs.HighPressure.Value = ini.GetValue("Pressure", "High", Cumulus.DefaultHiVal);
				Recs.HighPressure.Time = ini.GetValue("Pressure", "HTime", defTime);
				// rain
				Recs.HighRainRate.Value = ini.GetValue("Rain", "High", Cumulus.DefaultHiVal);
				Recs.HighRainRate.Time = ini.GetValue("Rain", "HTime", defTime);
				Recs.HighHourlyRain.Value = ini.GetValue("Rain", "HourlyHigh", Cumulus.DefaultHiVal);
				Recs.HighHourlyRain.Time = ini.GetValue("Rain", "HHourlyTime", defTime);
                Recs.HighDailyRain.Value = ini.GetValue("Rain", "DailyHigh", Cumulus.DefaultHiVal);
                Recs.HighDailyRain.Time = ini.GetValue("Rain", "HDailyTime", defTime);
                Recs.High24hRain.Value = ini.GetValue("Rain", "24Hour", Cumulus.DefaultHiVal);
                Recs.High24hRain.Time = ini.GetValue("Rain", "24HourTime", defTime);
                Recs.LongestDry.Value = ini.GetValue("Rain", "LongestDryPeriod", 0);
				Recs.LongestDry.Time = ini.GetValue("Rain", "LongestDryPeriodTime", defTime);
				Recs.LongestWet.Value = ini.GetValue("Rain", "LongestWetPeriod", 0);
				Recs.LongestWet.Time = ini.GetValue("Rain", "LongestWetPeriodTime", defTime);
				// humidity
				Recs.LowHumidity.Value = ini.GetValue("Humidity", "Low", (int) Cumulus.DefaultLoVal);
				Recs.LowHumidity.Time = ini.GetValue("Humidity", "LTime", defTime);
				Recs.HighHumidity.Value = ini.GetValue("Humidity", "High", (int) Cumulus.DefaultHiVal);
				Recs.HighHumidity.Time = ini.GetValue("Humidity", "HTime", defTime);
				// heat index
				Recs.HighHeatIndex.Value = ini.GetValue("HeatIndex", "High", Cumulus.DefaultHiVal);
				Recs.HighHeatIndex.Time = ini.GetValue("HeatIndex", "HTime", defTime);
				// App temp
				Recs.LowApparant.Value = ini.GetValue("AppTemp", "Low", Cumulus.DefaultLoVal);
				Recs.LowApparant.Time = ini.GetValue("AppTemp", "LTime", defTime);
				Recs.HighApparant.Value = ini.GetValue("AppTemp", "High", Cumulus.DefaultHiVal);
				Recs.HighApparant.Time = ini.GetValue("AppTemp", "HTime", defTime);
				// Dewpoint
				Recs.LowDewPoint.Value = ini.GetValue("Dewpoint", "Low", Cumulus.DefaultLoVal);
				Recs.LowDewPoint.Time = ini.GetValue("Dewpoint", "LTime", defTime);
				Recs.HighDewPoint.Value = ini.GetValue("Dewpoint", "High", Cumulus.DefaultHiVal);
				Recs.HighDewPoint.Time = ini.GetValue("Dewpoint", "HTime", defTime);
				// wind Cumulus.DefaultHiVal
				Recs.LowWindChill.Value = ini.GetValue("WindChill", "Low", Cumulus.DefaultLoVal);
				Recs.LowWindChill.Time = ini.GetValue("WindChill", "LTime", defTime);
				// Feels like temp
				Recs.LowFeelsLike.Value = ini.GetValue("FeelsLike", "Low", Cumulus.DefaultLoVal);
				Recs.LowFeelsLike.Time = ini.GetValue("FeelsLike", "LTime", defTime);
				Recs.HighFeelsLike.Value = ini.GetValue("FeelsLike", "High", Cumulus.DefaultHiVal);
				Recs.HighFeelsLike.Time = ini.GetValue("FeelsLike", "HTime", defTime);
				// Humidex
				Recs.HighHumidex.Value = ini.GetValue("Humidex", "High", Cumulus.DefaultHiVal);
				Recs.HighHumidex.Time = ini.GetValue("Humidex", "HTime", defTime);

				Program.LogMessage("Completed month.ini file read");
			}
			else
			{
				Program.LogMessage("No month.ini file found");
			}
		}

		public void WriteIniFile()
		{
			// backup original file
			if (File.Exists(filename))
			{
				File.Move(filename, filename + ".sav");
			}

			Program.LogMessage("Writing to month.ini file");
			Console.WriteLine("Writing to month.ini file...");
			try
			{
				int hourInc = Program.GetHourInc(DateTime.Now);

				IniFile ini = new IniFile(filename);
				// Date
				ini.SetValue("General", "Date", DateTime.Now.AddHours(hourInc));
				// Wind
				ini.SetValue("Wind", "Speed", Recs.HighWindAvg.Value);
				ini.SetValue("Wind", "SpTime", Recs.HighWindAvg.Time);
				ini.SetValue("Wind", "Gust", Recs.HighWindGust.Value);
				ini.SetValue("Wind", "Time", Recs.HighWindGust.Time);
				ini.SetValue("Wind", "Windrun", Recs.HighWindRun.Value);
				ini.SetValue("Wind", "WindrunTime", Recs.HighWindRun.Time);
				// Temperature
				ini.SetValue("Temp", "Low", Recs.LowTemperature.Value);
				ini.SetValue("Temp", "LTime", Recs.LowTemperature.Time);
				ini.SetValue("Temp", "High", Recs.HighTemperature.Value);
				ini.SetValue("Temp", "HTime", Recs.HighTemperature.Time);
				ini.SetValue("Temp", "LowMax", Recs.LowMaxTemp.Value);
				ini.SetValue("Temp", "LMTime", Recs.LowMaxTemp.Time);
				ini.SetValue("Temp", "HighMin", Recs.HighMinTemp.Value);
				ini.SetValue("Temp", "HMTime", Recs.HighMinTemp.Time);
				ini.SetValue("Temp", "LowRange", Recs.LowDailyRange.Value);
				ini.SetValue("Temp", "LowRangeTime", Recs.LowDailyRange.Time);
				ini.SetValue("Temp", "HighRange", Recs.HighDailyRange.Value);
				ini.SetValue("Temp", "HighRangeTime", Recs.HighDailyRange.Time);
				// Pressure
				ini.SetValue("Pressure", "Low", Recs.LowPressure.Value);
				ini.SetValue("Pressure", "LTime", Recs.LowPressure.Time);
				ini.SetValue("Pressure", "High", Recs.HighPressure.Value);
				ini.SetValue("Pressure", "HTime", Recs.HighPressure.Time);
				// rain
				ini.SetValue("Rain", "High", Recs.HighRainRate.Value);
				ini.SetValue("Rain", "HTime", Recs.HighRainRate.Time);
				ini.SetValue("Rain", "HourlyHigh", Recs.HighHourlyRain.Value);
				ini.SetValue("Rain", "HHourlyTime", Recs.HighHourlyRain.Time);
				ini.SetValue("Rain", "DailyHigh", Recs.HighDailyRain.Value);
				ini.SetValue("Rain", "HDailyTime", Recs.HighDailyRain.Time);
				ini.SetValue("Rain", "LongestDryPeriod", Recs.LongestDry.Value);
				ini.SetValue("Rain", "LongestDryPeriodTime", Recs.LongestDry.Time);
				ini.SetValue("Rain", "LongestWetPeriod", Recs.LongestWet.Value);
				ini.SetValue("Rain", "LongestWetPeriodTime", Recs.LongestWet.Time);
                ini.SetValue("Rain", "24Hour", Recs.High24hRain.Value);
                ini.SetValue("Rain", "24HourTime", Recs.High24hRain.Time);

                // humidity
                ini.SetValue("Humidity", "Low", Recs.LowHumidity.Value);
				ini.SetValue("Humidity", "LTime", Recs.LowHumidity.Time);
				ini.SetValue("Humidity", "High", Recs.HighHumidity.Value);
				ini.SetValue("Humidity", "HTime", Recs.HighHumidity.Time);
				// heat index
				ini.SetValue("HeatIndex", "High", Recs.HighHeatIndex.Value);
				ini.SetValue("HeatIndex", "HTime", Recs.HighHeatIndex.Time);
				// App temp
				ini.SetValue("AppTemp", "Low", Recs.LowApparant.Value);
				ini.SetValue("AppTemp", "LTime", Recs.LowApparant.Time);
				ini.SetValue("AppTemp", "High", Recs.HighApparant.Value);
				ini.SetValue("AppTemp", "HTime", Recs.HighApparant.Time);
				// Dewpoint
				ini.SetValue("Dewpoint", "Low", Recs.LowDewPoint.Value);
				ini.SetValue("Dewpoint", "LTime", Recs.LowDewPoint.Time);
				ini.SetValue("Dewpoint", "High", Recs.HighDewPoint.Value);
				ini.SetValue("Dewpoint", "HTime", Recs.HighDewPoint.Time);
				// wind chill
				ini.SetValue("WindChill", "Low", Recs.LowWindChill.Value);
				ini.SetValue("WindChill", "LTime", Recs.LowWindChill.Time);
				// feels like
				ini.SetValue("FeelsLike", "Low", Recs.LowFeelsLike.Value);
				ini.SetValue("FeelsLike", "LTime", Recs.LowFeelsLike.Time);
				ini.SetValue("FeelsLike", "High", Recs.HighFeelsLike.Value);
				ini.SetValue("FeelsLike", "HTime", Recs.HighFeelsLike.Time);
				// Humidex
				ini.SetValue("Humidex", "High", Recs.HighHumidex.Value);
				ini.SetValue("Humidex", "HTime", Recs.HighHumidex.Time);

				ini.Flush();
			}
			catch (Exception ex)
			{
				Program.LogMessage("Error writing month.ini file: " + ex.Message);
			}

			Program.LogMessage("End writing to month.ini file");
		}
	}
}
