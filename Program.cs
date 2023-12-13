using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace CreateRecords
{
	class Program
	{
		public static Cumulus cumulus;
		private static DayFile dayfile;

		private static Monthly monthly;
		private static ThisMonth thisMonth;
		private static ThisYear thisYear;
		private static AllTime allTime;

		private static ConsoleColor defConsoleColour;

		static void Main()
		{

			TextWriterTraceListener myTextListener = new TextWriterTraceListener($"MXdiags{Path.DirectorySeparatorChar}CreateRecords-{DateTime.Now:yyyyMMdd-HHmmss}.txt", "CMlog");
			Trace.Listeners.Add(myTextListener);
			Trace.AutoFlush = true;

			defConsoleColour = Console.ForegroundColor;

			var fullVer = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;
			var version = $"{fullVer.Major}.{fullVer.Minor}.{fullVer.Build}";
			LogMessage("CreateRecords v." + version);
			Console.WriteLine("CreateRecords v." + version);

			LogMessage("Processing started");
			Console.WriteLine();
			Console.WriteLine($"Processing started: {DateTime.Now:U}");
			Console.WriteLine();

			cumulus = new Cumulus();

			// load existing day file
			dayfile = new DayFile();

			thisMonth = new ThisMonth();
			thisYear = new ThisYear();
			allTime = new AllTime();
			monthly = new Monthly();

			// for each day since records began date
			var dayfileStart = dayfile.DayfileRecs.Count > 0 ? dayfile.DayfileRecs[0].Date : DateTime.MaxValue;

			if (dayfileStart == DateTime.MaxValue)
			{
				LogMessage("No valid day file records found!");
				Console.WriteLine("No valid day file records found!");
				Console.WriteLine("Exiting...");

				Environment.Exit(1);
			}
			else
			{
				LogMessage($"First dayfile record: {dayfileStart:d}");
				Console.WriteLine($"First dayfile record: {dayfileStart:d}");
			}


			if (!GetUserConfirmation($"This will attempt to create/update your station weather records from {dayfileStart:D}. Continue? [Y/N]: "))
			{
				Console.WriteLine("Exiting...");
				Environment.Exit(1);
			}
			Console.WriteLine();

			// Sanity check #2
			if (dayfileStart >= DateTime.Today)
			{
				LogMessage("Start date is today!???");
				LogConsole("Start date is today!???", ConsoleColor.Cyan);
				LogConsole("Press any key to exit", ConsoleColor.DarkYellow);
				Console.ReadKey(true);
				Console.WriteLine("Exiting...");

				Environment.Exit(1);
			}


			var rainThreshold = 0.0;
			if (cumulus.RainDayThreshold > -1)
				rainThreshold = cumulus.RainDayThreshold;

			var currMonth = DateTime.MinValue;
			var currMonthRain = 0.0;

			var currDryWetDay = dayfile.DayfileRecs[0].Date;
			var wetDays = 0;
			var dryDays = 0;
			var wetRun = false;
			var dryRun = false;

			foreach (var day in dayfile.DayfileRecs)
			{
				LogConsole(new String('\b', Console.BufferWidth), ConsoleColor.Black, false);
				LogConsole($"\bProcessing {day.Date:d} ...", ConsoleColor.Gray, false);
				DoHighTemp(day.HighTemp, day.HighTempTime);
				DoLowTemp(day.LowTemp, day.LowTempTime);

				DoHighDew(day.HighDewPoint, day.HighDewPointTime);
				DoLowDew(day.LowDewPoint, day.LowDewPointTime);

				DoHighApp(day.HighAppTemp, day.HighAppTempTime);
				DoLowApp(day.LowAppTemp, day.LowAppTempTime);

				DoHighFeels(day.HighFeelsLike, day.HighFeelsLikeTime);
				DoLowFeels(day.LowFeelsLike, day.LowFeelsLikeTime);

				DoHighHumidex(day.HighHumidex, day.HighHumidexTime);

				DoLowWindChill(day.LowWindChill, day.LowWindChillTime);

				DoHighHeatIndex(day.HighHeatIndex, day.HighHeatIndexTime);

				DoHighMin(day.LowTemp, day.LowTempTime);
				DoLowMax(day.HighTemp, day.HighTempTime);

				DoHighRange(day.HighTemp - day.LowTemp, day.Date);
				DoLowRange(day.HighTemp - day.LowTemp, day.Date);

				DoHighHumidity(day.HighHumidity, day.HighHumidityTime);
				DoLowHumidity(day.LowHumidity, day.LowHumidityTime);

				DoHighPress(day.HighPress, day.HighPressTime);
				DoLowPress(day.LowPress, day.LowPressTime);

				DoHighGust(day.HighGust, day.HighGustTime);
				DoHighWindAvg(day.HighAvgWind, day.HighAvgWindTime);
				DoHighWindRun(day.WindRun, day.Date);

				DoHighRainRate(day.HighRainRate, day.HighRainRateTime);
				DoHighHourlyRain(day.HighHourlyRain, day.HighHourlyRainTime);
				DoHighDailyRain(day.TotalRain, day.Date);
				DoHigh24hRain(day.HighRain24h, day.HighRain24hTime);

				if (day.Date.Month != currMonth.Month)
				{
					DoHighMonthlyRain(currMonthRain, currMonth.Date);

					currMonthRain = day.TotalRain;
					currMonth = day.Date;
				}
				else
				{
					currMonthRain += day.TotalRain;
				}

				// Is it a wet day?
				if (day.TotalRain > rainThreshold)
				{
					// were we previously in a dry run?
					if (dryRun)
					{
						DoDryDays(dryDays, currDryWetDay);
						dryRun = false;
						wetRun = true;
						wetDays = 1;
						currDryWetDay = day.Date;
					}
					else
					{
						wetRun = true;
						wetDays++;
						currDryWetDay = day.Date;
					}
				}
				// No its a dry day
				else
				{
					// were we previously in a wet run?
					if (wetRun)
					{
						DoWetDays(wetDays, currDryWetDay);
						wetRun = false;
						dryRun = true;
						dryDays = 1;
						currDryWetDay = day.Date;
					}
					else
					{
						dryRun = true;
						dryDays++;
						currDryWetDay = day.Date;
					}
				}
			}

			if (wetRun)
				DoWetDays(wetDays, currDryWetDay);
			else
				DoDryDays(dryDays, currDryWetDay);



			// create the new dayfile.txt with a different name
			LogMessage("Saving new records .ini files");
			Console.WriteLine("\n");

			thisMonth.WriteIniFile();
			thisYear.WriteIniFile();
			allTime.WriteIniFile();
			monthly.WriteIniFile();

			LogMessage("Created new records .ini files, the old versions are saved as xxxx.ini.sav");
			Console.WriteLine("\nCreated new records .ini files, the old versions are saved as xxxx.ini.sav");


			LogMessage("Processing complete.");
			Console.WriteLine();
			Console.WriteLine();
			Console.WriteLine("\nProcessing complete.");
			LogConsole("Press any key to exit", ConsoleColor.DarkYellow);
			Console.ReadKey(true);
		}

		public static void LogMessage(string message)
		{
			Trace.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff ") + message);
		}

		public static void LogConsole(string msg, ConsoleColor colour, bool newLine = true)
		{
			Console.ForegroundColor = colour;

			if (newLine)
			{
				Console.WriteLine(msg);
			}
			else
			{
				Console.Write(msg);
			}

			Console.ForegroundColor = defConsoleColour;
		}

		private static void DoHighTemp(double val, DateTime time)
		{
			if (allTime.Recs.HighTemperature.CheckHighRecord(val, time))
				LogMessage($"Date {time:d} Set new all time high temperature {val.ToString(cumulus.TempFormat)}, time {time:t}");

			if (monthly.Recs[time.Month].HighTemperature.CheckHighRecord(val, time))
				LogMessage($"Date {time:d} Set new monthly high temperature {val.ToString(cumulus.TempFormat)}, time {time:t}");

			if (time.Year == DateTime.Now.Year)
			{
				if (thisYear.Recs.HighTemperature.CheckHighRecord(val, time))
					LogMessage($"Date {time:d} Set new this year high temperature {val.ToString(cumulus.TempFormat)}, time {time:t}");

				if (time.Month == DateTime.Now.Month && thisMonth.Recs.HighTemperature.CheckHighRecord(val, time))
					LogMessage($"Date {time:d} Set new this month high temperature {val.ToString(cumulus.TempFormat)}, time {time:t}");
			}
		}

		private static void DoLowTemp(double val, DateTime time)
		{
			if (allTime.Recs.LowTemperature.CheckLowRecord(val, time))
				LogMessage($"Date {time:d} Set new all time low temperature {val.ToString(cumulus.TempFormat)}, time {time:t}");

			if (monthly.Recs[time.Month].LowTemperature.CheckLowRecord(val, time))
				LogMessage($"Date {time:d} Set new monthly low temperature {val.ToString(cumulus.TempFormat)}, time {time:t}");

			if (time.Year == DateTime.Now.Year)
			{
				if (thisYear.Recs.LowTemperature.CheckLowRecord(val, time))
					LogMessage($"Date {time:d} Set new this year low temperature {val.ToString(cumulus.TempFormat)}, time {time:t}");

				if (time.Month == DateTime.Now.Month && thisMonth.Recs.LowTemperature.CheckLowRecord(val, time))
					LogMessage($"Date {time:d} Set new this month low temperature {val.ToString(cumulus.TempFormat)}, time {time:t}");
			}
		}

		private static void DoHighDew(double val, DateTime time)
		{
			if (val > allTime.Recs.HighDewPoint.Value)
			{
				allTime.Recs.HighDewPoint.Value = val;
				allTime.Recs.HighDewPoint.Time = time;
				LogMessage($"Date {time:d} Set new all time high dew point {val.ToString(cumulus.TempFormat)}, time {time:t}");
			}

			if (val > monthly.Recs[time.Month].HighDewPoint.Value)
			{
				monthly.Recs[time.Month].HighDewPoint.Value = val;
				monthly.Recs[time.Month].HighDewPoint.Time = time;
				LogMessage($"Date {time:d} Set new monthly high dew point {val.ToString(cumulus.TempFormat)}, time {time:t}");
			}

			if (time.Year == DateTime.Now.Year && val > thisYear.Recs.HighDewPoint.Value)
			{
				thisYear.Recs.HighDewPoint.Value = val;
				thisYear.Recs.HighDewPoint.Time = time;
				LogMessage($"Date {time:d} Set new this year high dew point {val.ToString(cumulus.TempFormat)}, time {time:t}");
			}

			if (time.Year == DateTime.Now.Year && time.Month == DateTime.Now.Month && val > thisMonth.Recs.HighDewPoint.Value)
			{
				thisMonth.Recs.HighDewPoint.Value = val;
				thisMonth.Recs.HighDewPoint.Time = time;
				LogMessage($"Date {time:d} Set new this month high dew point {val.ToString(cumulus.TempFormat)}, time {time:t}");
			}
		}

		private static void DoLowDew(double val, DateTime time)
		{
			if (allTime.Recs.LowDewPoint.CheckLowRecord(val, time))
				LogMessage($"Date {time:d} Set new all time low dew point {val.ToString(cumulus.TempFormat)}, time {time:t}");

			if (monthly.Recs[time.Month].LowDewPoint.CheckLowRecord(val, time))
				LogMessage($"Date {time:d} Set new monthly low dew point {val.ToString(cumulus.TempFormat)}, time {time:t}");

			if (time.Year == DateTime.Now.Year)
			{
				if (thisYear.Recs.LowDewPoint.CheckLowRecord(val, time))
					LogMessage($"Date {time:d} Set new this year low dew point {val.ToString(cumulus.TempFormat)}, time {time:t}");

				if (time.Month == DateTime.Now.Month && thisMonth.Recs.LowDewPoint.CheckLowRecord(val, time))
					LogMessage($"Date {time:d} Set new this month low dew point {val.ToString(cumulus.TempFormat)}, time {time:t}");
			}
		}

		private static void DoHighApp(double val, DateTime time)
		{
			if (allTime.Recs.HighApparant.CheckHighRecord(val, time))
				LogMessage($"Date {time:d} Set new all time high apparant {val.ToString(cumulus.TempFormat)}, time {time:t}");

			if (monthly.Recs[time.Month].HighApparant.CheckHighRecord(val, time))
				LogMessage($"Date {time:d} Set new monthly high apparant {val.ToString(cumulus.TempFormat)}, time {time:t}");

			if (time.Year == DateTime.Now.Year)
			{
				if (thisYear.Recs.HighApparant.CheckHighRecord(val, time))
					LogMessage($"Date {time:d} Set new this year high apparant {val.ToString(cumulus.TempFormat)}, time {time:t}");

				if (time.Month == DateTime.Now.Month && thisMonth.Recs.HighApparant.CheckHighRecord(val, time))
					LogMessage($"Date {time:d} Set new this month high apparant {val.ToString(cumulus.TempFormat)}, time {time:t}");
			}
		}

		private static void DoLowApp(double val, DateTime time)
		{
			if (allTime.Recs.LowApparant.CheckLowRecord(val, time))
				LogMessage($"Date {time:d} Set new all time low apparant {val.ToString(cumulus.TempFormat)}, time {time:t}");

			if (monthly.Recs[time.Month].LowApparant.CheckLowRecord(val, time))
				LogMessage($"Date {time:d} Set new monthly low apparant {val.ToString(cumulus.TempFormat)}, time {time:t}");

			if (time.Year == DateTime.Now.Year)
			{
				if (thisYear.Recs.LowApparant.CheckLowRecord(val, time))
					LogMessage($"Date {time:d} Set new this year low apparant {val.ToString(cumulus.TempFormat)}, time {time:t}");

				if (time.Month == DateTime.Now.Month && thisMonth.Recs.LowApparant.CheckLowRecord(val, time))
					LogMessage($"Date {time:d} Set new this month low apparant {val.ToString(cumulus.TempFormat)}, time {time:t}");
			}
		}

		private static void DoHighFeels(double val, DateTime time)
		{
			if (allTime.Recs.HighFeelsLike.CheckHighRecord(val, time))
				LogMessage($"Date {time:d} Set new all time high feels {val.ToString(cumulus.TempFormat)}, time {time:t}");

			if (monthly.Recs[time.Month].HighFeelsLike.CheckHighRecord(val, time))
				LogMessage($"Date {time:d} Set new monthly high feels {val.ToString(cumulus.TempFormat)}, time {time:t}");

			if (time.Year == DateTime.Now.Year)
			{
				if (thisYear.Recs.HighFeelsLike.CheckHighRecord(val, time))
					LogMessage($"Date {time:d} Set new this year high feels {val.ToString(cumulus.TempFormat)}, time {time:t}");

				if (time.Month == DateTime.Now.Month && thisMonth.Recs.HighFeelsLike.CheckHighRecord(val, time))
					LogMessage($"Date {time:d} Set new this month high feels {val.ToString(cumulus.TempFormat)}, time {time:t}");
			}
		}

		private static void DoLowFeels(double val, DateTime time)
		{
			if (allTime.Recs.LowFeelsLike.CheckLowRecord(val, time))
				LogMessage($"Date {time:d} Set new all time low feels {val.ToString(cumulus.TempFormat)}, time {time:t}");

			if (monthly.Recs[time.Month].LowFeelsLike.CheckLowRecord(val, time))
				LogMessage($"Date {time:d} Set new monthly low feels {val.ToString(cumulus.TempFormat)}, time {time:t}");

			if (time.Year == DateTime.Now.Year)
			{
				if (thisYear.Recs.LowFeelsLike.CheckLowRecord(val, time))
					LogMessage($"Date {time:d} Set new this year low feels {val.ToString(cumulus.TempFormat)}, time {time:t}");

				if (time.Month == DateTime.Now.Month && thisMonth.Recs.LowFeelsLike.CheckLowRecord(val, time))
					LogMessage($"Date {time:d} Set new this month low feels {val.ToString(cumulus.TempFormat)}, time {time:t}");
			}
		}

		private static void DoHighHumidex(double val, DateTime time)
		{
			if (allTime.Recs.HighHumidex.CheckHighRecord(val, time))
				LogMessage($"Date {time:d} Set new all time high humidex {val.ToString(cumulus.TempFormat)}, time {time:t}");

			if (monthly.Recs[time.Month].HighHumidex.CheckHighRecord(val, time))
				LogMessage($"Date {time:d} Set new monthly high humidex {val.ToString(cumulus.TempFormat)}, time {time:t}");

			if (time.Year == DateTime.Now.Year)
			{
				if (thisYear.Recs.HighHumidex.CheckHighRecord(val, time))
					LogMessage($"Date {time:d} Set new this year high humidex {val.ToString(cumulus.TempFormat)}, time {time:t}");

				if (time.Month == DateTime.Now.Month && thisMonth.Recs.HighHumidex.CheckHighRecord(val, time))
					LogMessage($"Date {time:d} Set new this month high humidex {val.ToString(cumulus.TempFormat)}, time {time:t}");
			}
		}

		private static void DoLowWindChill(double val, DateTime time)
		{
			if (allTime.Recs.LowWindChill.CheckLowRecord(val, time))
				LogMessage($"Date {time:d} Set new all time low wind chill {val.ToString(cumulus.TempFormat)}, time {time:t}");

			if (monthly.Recs[time.Month].LowWindChill.CheckLowRecord(val, time))
				LogMessage($"Date {time:d} Set new monthly low wind chill {val.ToString(cumulus.TempFormat)}, time {time:t}");

			if (time.Year == DateTime.Now.Year)
			{
				if (thisYear.Recs.LowWindChill.CheckLowRecord(val, time))
					LogMessage($"Date {time:d} Set new this year low wind chill {val.ToString(cumulus.TempFormat)}, time {time:t}");

				if (time.Month == DateTime.Now.Month && thisMonth.Recs.LowWindChill.CheckLowRecord(val, time))
					LogMessage($"Date {time:d} Set new this month low wind chill {val.ToString(cumulus.TempFormat)}, time {time:t}");
			}
		}

		private static void DoHighHeatIndex(double val, DateTime time)
		{
			if (allTime.Recs.HighHeatIndex.CheckHighRecord(val, time))
				LogMessage($"Date {time:d} Set new all time high heat index {val.ToString(cumulus.TempFormat)}, time {time:t}");

			if (monthly.Recs[time.Month].HighHeatIndex.CheckHighRecord(val, time))
				LogMessage($"Date {time:d} Set new monthly high heat index {val.ToString(cumulus.TempFormat)}, time {time:t}");

			if (time.Year == DateTime.Now.Year)
			{
				if (thisYear.Recs.HighHeatIndex.CheckHighRecord(val, time))
					LogMessage($"Date {time:d} Set new this year high heat index {val.ToString(cumulus.TempFormat)}, time {time:t}");

				if (time.Month == DateTime.Now.Month && thisMonth.Recs.HighHeatIndex.CheckHighRecord(val, time))
					LogMessage($"Date {time:d} Set new this month high heat index {val.ToString(cumulus.TempFormat)}, time {time:t}");
			}
		}

		private static void DoHighMin(double val, DateTime time)
		{
			if (allTime.Recs.HighMinTemp.CheckHighRecord(val, time))
				LogMessage($"Date {time:d} Set new all time high min temp {val.ToString(cumulus.TempFormat)}, time {time:t}");

			if (monthly.Recs[time.Month].HighMinTemp.CheckHighRecord(val, time))
				LogMessage($"Date {time:d} Set new monthly high min temp {val.ToString(cumulus.TempFormat)}, time {time:t}");

			if (time.Year == DateTime.Now.Year)
			{
				if (thisYear.Recs.HighMinTemp.CheckHighRecord(val, time))
					LogMessage($"Date {time:d} Set new this year high min temp {val.ToString(cumulus.TempFormat)}, time {time:t}");

				if (time.Month == DateTime.Now.Month && thisMonth.Recs.HighMinTemp.CheckHighRecord(val, time))
					LogMessage($"Date {time:d} Set new this month high min temp {val.ToString(cumulus.TempFormat)}, time {time:t}");
			}
		}

		private static void DoLowMax(double val, DateTime time)
		{
			if (allTime.Recs.LowMaxTemp.CheckLowRecord(val, time))
				LogMessage($"Date {time:d} Set new all time low max temp {val.ToString(cumulus.TempFormat)}, time {time:t}");

			if (monthly.Recs[time.Month].LowMaxTemp.CheckLowRecord(val, time))
				LogMessage($"Date {time:d} Set new monthly low max temp {val.ToString(cumulus.TempFormat)}, time {time:t}");

			if (time.Year == DateTime.Now.Year)
			{
				if (thisYear.Recs.LowMaxTemp.CheckLowRecord(val, time))
					LogMessage($"Date {time:d} Set new this year low max temp {val.ToString(cumulus.TempFormat)}, time {time:t}");

				if (time.Month == DateTime.Now.Month && thisMonth.Recs.LowMaxTemp.CheckLowRecord(val, time))
					LogMessage($"Date {time:d} Set new this month low max temp {val.ToString(cumulus.TempFormat)}, time {time:t}");
			}
		}

		private static void DoHighRange(double val, DateTime time)
		{
			if (allTime.Recs.HighDailyRange.CheckHighRecord(val, time))
				LogMessage($"Date {time:d} Set new all time high daily range {val.ToString(cumulus.TempFormat)}, time {time:t}");

			if (monthly.Recs[time.Month].HighDailyRange.CheckHighRecord(val, time))
				LogMessage($"Date {time:d} Set new monthly high daily range {val.ToString(cumulus.TempFormat)}, time {time:t}");

			if (time.Year == DateTime.Now.Year)
			{
				if (thisYear.Recs.HighDailyRange.CheckHighRecord(val, time))
					LogMessage($"Date {time:d} Set new this year high daily range {val.ToString(cumulus.TempFormat)}, time {time:t}");

				if (time.Month == DateTime.Now.Month && thisMonth.Recs.HighDailyRange.CheckHighRecord(val, time))
					LogMessage($"Date {time:d} Set new this month high daily range {val.ToString(cumulus.TempFormat)}, time {time:t}");
			}
		}

		private static void DoLowRange(double val, DateTime time)
		{
			if (allTime.Recs.LowDailyRange.CheckLowRecord(val, time))
				LogMessage($"Date {time:d} Set new all time low daily range {val.ToString(cumulus.TempFormat)}, time {time:t}");

			if (monthly.Recs[time.Month].LowDailyRange.CheckLowRecord(val, time))
				LogMessage($"Date {time:d} Set new monthly low daily range {val.ToString(cumulus.TempFormat)}, time {time:t}");

			if (time.Year == DateTime.Now.Year)
			{
				if (thisYear.Recs.LowDailyRange.CheckLowRecord(val, time))
					LogMessage($"Date {time:d} Set new this year low daily range {val.ToString(cumulus.TempFormat)}, time {time:t}");

				if (time.Month == DateTime.Now.Month && thisMonth.Recs.LowDailyRange.CheckLowRecord(val, time))
					LogMessage($"Date {time:d} Set new this month low daily range {val.ToString(cumulus.TempFormat)}, time {time:t}");
			}
		}

		private static void DoHighHumidity(int val, DateTime time)
		{
			if (allTime.Recs.HighHumidity.CheckHighRecord(val, time))
				LogMessage($"Date {time:d} Set new all time high humidity {val}, time {time:t}");

			if (monthly.Recs[time.Month].HighHumidity.CheckHighRecord(val, time))
				LogMessage($"Date {time:d} Set new monthly high humidity {val}, time {time:t}");

			if (time.Year == DateTime.Now.Year)
			{
				if (thisYear.Recs.HighHumidity.CheckHighRecord(val, time))
					LogMessage($"Date {time:d} Set new this year high humidity {val}, time {time:t}");

				if (time.Month == DateTime.Now.Month && thisMonth.Recs.HighHumidity.CheckHighRecord(val, time))
					LogMessage($"Date {time:d} Set new this month high humidity {val}, time {time:t}");
			}
		}

		private static void DoLowHumidity(int val, DateTime time)
		{
			if (allTime.Recs.LowHumidity.CheckLowRecord(val, time))
				LogMessage($"Date {time:d} Set new all time low humidty {val:d}, time {time:t}");

			if (monthly.Recs[time.Month].LowHumidity.CheckLowRecord(val, time))
				LogMessage($"Date {time:d} Set new monthly low humidty {val:d}, time {time:t}");

			if (time.Year == DateTime.Now.Year)
			{
				if (thisYear.Recs.LowHumidity.CheckLowRecord(val, time))
					LogMessage($"Date {time:d} Set new this year low humidty {val:d}, time {time:t}");

				if (time.Month == DateTime.Now.Month && thisMonth.Recs.LowHumidity.CheckLowRecord(val, time))
					LogMessage($"Date {time:d} Set new this month low humidty {val:d}, time {time:t}");
			}
		}

		private static void DoHighPress(double val, DateTime time)
		{
			if (allTime.Recs.HighPressure.CheckHighRecord(val, time))
				LogMessage($"Date {time:d} Set new all time high pressure {val.ToString(cumulus.PressFormat)}, time {time:t}");

			if (monthly.Recs[time.Month].HighPressure.CheckHighRecord(val, time))
				LogMessage($"Date {time:d} Set new monthly high pressure {val.ToString(cumulus.PressFormat)}, time {time:t}");

			if (time.Year == DateTime.Now.Year)
			{
				if (thisYear.Recs.HighPressure.CheckHighRecord(val, time))
					LogMessage($"Date {time:d} Set new this year high pressure {val.ToString(cumulus.PressFormat)}, time {time:t}");

				if (time.Month == DateTime.Now.Month && thisMonth.Recs.HighPressure.CheckHighRecord(val, time))
					LogMessage($"Date {time:d} Set new this month high pressure {val.ToString(cumulus.PressFormat)}, time {time:t}");
			}
		}

		private static void DoLowPress(double val, DateTime time)
		{
			if (allTime.Recs.LowPressure.CheckLowRecord(val, time))
				LogMessage($"Date {time:d} Set new all time low pressure {val.ToString(cumulus.PressFormat)}, time {time:t}");

			if (monthly.Recs[time.Month].LowPressure.CheckLowRecord(val, time))
				LogMessage($"Date {time:d} Set new monthly low pressure {val.ToString(cumulus.PressFormat)}, time {time:t}");

			if (time.Year == DateTime.Now.Year)
			{
				if (thisYear.Recs.LowPressure.CheckLowRecord(val, time))
					LogMessage($"Date {time:d} Set new this year low pressure {val.ToString(cumulus.PressFormat)}, time {time:t}");

				if (time.Month == DateTime.Now.Month && thisMonth.Recs.LowPressure.CheckLowRecord(val, time))
					LogMessage($"Date {time:d} Set new this month low pressure {val.ToString(cumulus.PressFormat)}, time {time:t}");
			}
		}

		private static void DoHighGust(double val, DateTime time)
		{
			if (allTime.Recs.HighWindGust.CheckHighRecord(val, time))
				LogMessage($"Date {time:d} Set new all time high gust {val.ToString(cumulus.WindFormat)}, time {time:t}");

			if (monthly.Recs[time.Month].HighWindGust.CheckHighRecord(val, time))
				LogMessage($"Date {time:d} Set new monthly high gust {val.ToString(cumulus.WindFormat)}, time {time:t}");

			if (time.Year == DateTime.Now.Year)
			{
				if (thisYear.Recs.HighWindGust.CheckHighRecord(val, time))
					LogMessage($"Date {time:d} Set new this year high gust {val.ToString(cumulus.WindFormat)}, time {time:t}");

				if (time.Month == DateTime.Now.Month && thisMonth.Recs.HighWindGust.CheckHighRecord(val, time))
					LogMessage($"Date {time:d} Set new this month high gust {val.ToString(cumulus.WindFormat)}, time {time:t}");
			}
		}

		private static void DoHighWindAvg(double val, DateTime time)
		{
			if (allTime.Recs.HighWindAvg.CheckHighRecord(val, time))
				LogMessage($"Date {time:d} Set new all time high wind avg {val.ToString(cumulus.WindAvgFormat)}, time {time:t}");

			if (monthly.Recs[time.Month].HighWindAvg.CheckHighRecord(val, time))
				LogMessage($"Date {time:d} Set new monthly high wind avg {val.ToString(cumulus.WindAvgFormat)}, time {time:t}");

			if (time.Year == DateTime.Now.Year)
			{
				if (thisYear.Recs.HighWindAvg.CheckHighRecord(val, time))
					LogMessage($"Date {time:d} Set new this year high wind avg {val.ToString(cumulus.WindAvgFormat)}, time {time:t}");

				if (time.Month == DateTime.Now.Month && thisMonth.Recs.HighWindAvg.CheckHighRecord(val, time))
					LogMessage($"Date {time:d} Set new this month high wind avg {val.ToString(cumulus.WindAvgFormat)}, time {time:t}");
			}
		}

		private static void DoHighWindRun(double val, DateTime time)
		{
			if (allTime.Recs.HighWindRun.CheckHighRecord(val, time))
				LogMessage($"Date {time:d} Set new all time high wind run {val.ToString(cumulus.WindRunFormat)}, time {time:t}");

			if (monthly.Recs[time.Month].HighWindRun.CheckHighRecord(val, time))
				LogMessage($"Date {time:d} Set new monthly high wind run {val.ToString(cumulus.WindRunFormat)}, time {time:t}");

			if (time.Year == DateTime.Now.Year)
			{
				if (thisYear.Recs.HighWindRun.CheckHighRecord(val, time))
					LogMessage($"Date {time:d} Set new this year high wind run {val.ToString(cumulus.WindRunFormat)}, time {time:t}");

				if (time.Month == DateTime.Now.Month && thisMonth.Recs.HighWindRun.CheckHighRecord(val, time))
					LogMessage($"Date {time:d} Set new this month high wind run {val.ToString(cumulus.WindRunFormat)}, time {time:t}");
			}
		}

		private static void DoHighRainRate(double val, DateTime time)
		{
			if (allTime.Recs.HighRainRate.CheckHighRecord(val, time))
				LogMessage($"Date {time:d} Set new all time high rain rate {val.ToString(cumulus.RainFormat)}, time {time:t}");

			if (monthly.Recs[time.Month].HighRainRate.CheckHighRecord(val, time))
				LogMessage($"Date {time:d} Set new monthly high rain rate {val.ToString(cumulus.RainFormat)}, time {time:t}");

			if (time.Year == DateTime.Now.Year)
			{
				if (thisYear.Recs.HighRainRate.CheckHighRecord(val, time))
					LogMessage($"Date {time:d} Set new this year high rain rate {val.ToString(cumulus.RainFormat)}, time {time:t}");

				if (time.Month == DateTime.Now.Month && thisMonth.Recs.HighRainRate.CheckHighRecord(val, time))
					LogMessage($"Date {time:d} Set new this month high rain rate {val.ToString(cumulus.RainFormat)}, time {time:t}");
			}
		}

		private static void DoHighHourlyRain(double val, DateTime time)
		{
			if (allTime.Recs.HighHourlyRain.CheckHighRecord(val, time))
				LogMessage($"Date {time:d} Set new all time high hourly rain {val.ToString(cumulus.RainFormat)}, time {time:t}");

			if (monthly.Recs[time.Month].HighHourlyRain.CheckHighRecord(val, time))
				LogMessage($"Date {time:d} Set new monthly high hourly rain {val.ToString(cumulus.RainFormat)}, time {time:t}");

			if (time.Year == DateTime.Now.Year)
			{
				if (thisYear.Recs.HighHourlyRain.CheckHighRecord(val, time))
					LogMessage($"Date {time:d} Set new this year high hourly rain {val.ToString(cumulus.RainFormat)}, time {time:t}");

				if (time.Month == DateTime.Now.Month && thisMonth.Recs.HighHourlyRain.CheckHighRecord(val, time))
					LogMessage($"Date {time:d} Set new this month high hourly rain {val.ToString(cumulus.RainFormat)}, time {time:t}");
			}
		}

		private static void DoHighDailyRain(double val, DateTime time)
		{
			if (allTime.Recs.HighDailyRain.CheckHighRecord(val, time))
				LogMessage($"Date {time:d} Set new all time high daily rain {val.ToString(cumulus.RainFormat)}, time {time:t}");

			if (monthly.Recs[time.Month].HighDailyRain.CheckHighRecord(val, time))
				LogMessage($"Date {time:d} Set new monthly high daily rain {val.ToString(cumulus.RainFormat)}, time {time:t}");

			if (time.Year == DateTime.Now.Year)
			{
				if (thisYear.Recs.HighDailyRain.CheckHighRecord(val, time))
					LogMessage($"Date {time:d} Set new this year high daily rain {val.ToString(cumulus.RainFormat)}, time {time:t}");

				if (time.Month == DateTime.Now.Month && thisMonth.Recs.HighDailyRain.CheckHighRecord(val, time))
					LogMessage($"Date {time:d} Set new this month high daily rain {val.ToString(cumulus.RainFormat)}, time {time:t}");
			}
		}

		private static void DoHigh24hRain(double val, DateTime time)
		{
			if (allTime.Recs.High24hRain.CheckHighRecord(val, time))
				LogMessage($"Date {time:d} Set new all time high 24 hr rain {val.ToString(cumulus.RainFormat)}, time {time:t}");

			if (monthly.Recs[time.Month].High24hRain.CheckHighRecord(val, time))
				LogMessage($"Date {time:d} Set new monthly high 24 hr rain {val.ToString(cumulus.RainFormat)}, time {time:t}");

			if (time.Year == DateTime.Now.Year)
			{
				if (thisYear.Recs.High24hRain.CheckHighRecord(val, time))
					LogMessage($"Date {time:d} Set new this year high 24 hr rain {val.ToString(cumulus.RainFormat)}, time {time:t}");

				if (time.Month == DateTime.Now.Month && thisMonth.Recs.High24hRain.CheckHighRecord(val, time))
					LogMessage($"Date {time:d} Set new this month high 24 hr rain {val.ToString(cumulus.RainFormat)}, time {time:t}");
			}
		}

		private static void DoHighMonthlyRain(double val, DateTime time)
		{
			if (allTime.Recs.HighMonthlyRain.CheckHighRecord(val, time))
				LogMessage($"Date {time:d} Set new all time high daily rain {val.ToString(cumulus.RainFormat)}, time {time:t}");

			if (monthly.Recs[time.Month].HighMonthlyRain.CheckHighRecord(val, time))
				LogMessage($"Date {time:d} Set new monthly high daily rain {val.ToString(cumulus.RainFormat)}, time {time:t}");

			if (time.Year == DateTime.Now.Year && thisYear.Recs.HighMonthlyRain.CheckHighRecord(val, time))
				LogMessage($"Date {time:d} Set new this year high daily rain {val.ToString(cumulus.RainFormat)}, time {time:t}");
		}

		private static void DoWetDays(int val, DateTime time)
		{
			if (allTime.Recs.LongestWet.CheckHighRecord(val, time))
				LogMessage($"Date {time:d} Set new all time most wet days {val}, time {time:t}");

			if (monthly.Recs[time.Month].LongestWet.CheckHighRecord(val, time))
				LogMessage($"Date {time:d} Set new monthly most wet days {val}, time {time:t}");

			if (time.Year == DateTime.Now.Year)
			{
				if (thisYear.Recs.LongestWet.CheckHighRecord(val, time))
					LogMessage($"Date {time:d} Set new this year most wet days {val}, time {time:t}");

				if (time.Month == DateTime.Now.Month && thisMonth.Recs.LongestWet.CheckHighRecord(val, time))
					LogMessage($"Date {time:d} Set new this month most wet days {val}, time {time:t}");
			}
		}

		private static void DoDryDays(int val, DateTime time)
		{
			if (allTime.Recs.LongestDry.CheckHighRecord(val, time))
				LogMessage($"Date {time:d} Set new all time most dry days {val}, time {time:t}");

			if (monthly.Recs[time.Month].LongestDry.CheckHighRecord(val, time))
				LogMessage($"Date {time:d} Set new monthly most dry days {val}, time {time:t}");

			if (time.Year == DateTime.Now.Year)
			{
				if (thisYear.Recs.LongestDry.CheckHighRecord(val, time))
					LogMessage($"Date {time:d} Set new this year most dry days {val}, time {time:t}");

				if (time.Month == DateTime.Now.Month && thisMonth.Recs.LongestDry.CheckHighRecord(val, time))
					LogMessage($"Date {time:d} Set new this month most dry days {val}, time {time:t}");
			}
		}


		private static bool GetUserConfirmation(string msg)
		{
			do
			{
				while (Console.KeyAvailable)
					Console.ReadKey();

				Console.Write(msg);
				var resp = Console.ReadKey().Key;
				Console.WriteLine();

				if (resp == ConsoleKey.Y) return true;
				if (resp == ConsoleKey.N) return false;
			} while (true);
		}

		public static int GetHourInc(DateTime timestamp)
		{
			if (cumulus.RolloverHour == 0)
			{
				return 0;
			}
			else
			{
				try
				{
					if (cumulus.Use10amInSummer && TimeZoneInfo.Local.IsDaylightSavingTime(timestamp))
					{
						// Locale is currently on Daylight time
						return -10;
					}
					else
					{
						// Locale is currently on Standard time or unknown
						return -9;
					}
				}
				catch (Exception)
				{
					return -9;
				}
			}
		}
	}
}
