using PrayerCalculation.Methods;
using System;
using System.Collections;

namespace PrayerCalculation
{
    #region Enums
    public enum AsrMethod
    {
        Standard, // Shafi`i, Maliki, Ja`fari, Hanbali
        Hanafi    // Hanafi
    }

    public enum MidnightMethod
    {
        Standard, // Mid Sunset to Sunrise
        Jafari    // Mid Sunset to Fajr
    }

    public enum HighLatitudeMethod
    {
        NightMiddle, // middle of night
        AngleBased,  // angle/60th of night
        OneSeventh,  // 1/7th of night
        None         // No adjustment
    }

    public enum TimeFormat
    {
        Format24h,         // 24-hour format
        Format12h,         // 12-hour format
        Format12hNS,       // 12-hour format with no suffix
        FormatFloat        // floating point number 
    }
    #endregion

    public class PrayerTimeCalculation
    {
        #region Fields
        private TimeFormat timeFormat;
        private double latitude;
        private double longitude;
        private double elv;
        private double timeZone;
        private double julianDate;
        private int numIterations;

        private Dictionary settings = new Dictionary(10);
        private Dictionary offsets = new Dictionary(9);
        private Dictionary defaultParams = new Dictionary(8);
        private MethodBase currentMethod;
        #endregion

        #region Properties
        public TimeFormat TimeFormat
        {
            get { return timeFormat; }
            set { timeFormat = value; }
        }

        public MethodBase CurrentMethod
        {
            get { return currentMethod; }
            set { currentMethod = value; Adjust(currentMethod.Parameters); }
        }

        #endregion

        #region Constructor
        public PrayerTimeCalculation(MethodBase method, AsrMethod asrMethod, MidnightMethod midnightMethod, int maghribAdjustment)
        {
            if (method == null)
                throw new ArgumentNullException("Method must be specified");
            else
                currentMethod = method;

            timeFormat = TimeFormat.Format24h;

            // Do not change anything here; use adjust method instead
            settings.Add(TimeNames.Imsak, "10 min");
            settings.Add(TimeNames.Dhuhr, "0 min");
            settings.Add(TimeNames.Asr, asrMethod);
            settings.Add("HighLats", HighLatitudeMethod.AngleBased);

            //timeSuffixes = ['am', 'pm'],
            //invalidTime =  '-----',

            numIterations = 1;

            // Default Parameters in Calculation Methods
            defaultParams.Add(TimeNames.Maghrib, maghribAdjustment + " min");
            defaultParams.Add(TimeNames.Midnight, midnightMethod);

            foreach (DictionaryEntry item in defaultParams)
            {
                if (!currentMethod.Parameters.Contains(item.Key))
                    currentMethod.Parameters.Add(item.Key, item.Value);
                else if (item.Key == TimeNames.Maghrib && maghribAdjustment > 0 && Utils.ContainsMin(currentMethod.Parameters[item.Key]))
                {
                    var maghribNewValue = Utils.Eval(currentMethod.Parameters[item.Key].ToString()) + maghribAdjustment;
                    currentMethod.Parameters[item.Key] = maghribNewValue + " min";
                }
            }

            // Initialize settings
            foreach (DictionaryEntry item in currentMethod.Parameters)
            {
                settings[item.Key] = item.Value;
            }

            foreach (string timeName in TimeNames.TimeName)
            {
                offsets.Add(timeName, 0);
            }
        }
        #endregion

        #region Public methods
        public void Adjust(Dictionary parameters)
        {
            foreach (DictionaryEntry item in parameters)
            {
                settings[item.Key] = item.Value;
            }
        }

        public void Tune(Dictionary timeOffsets)
        {
            foreach (DictionaryEntry item in timeOffsets)
            {
                offsets[item.Key] = item.Value;
            }
        }

        public string GetFormattedTime(double time, TimeFormat format)
        {
            if (format == TimeFormat.FormatFloat)
                return time.ToString();

            time = Utils.FixHour(time + 0.5 / 60); // add 0.5 minutes to round
            double hours = Math.Floor(time);
            double minutes = Math.Floor((time - hours) * 60);
            string hour = (format == TimeFormat.Format24h) ? Utils.TwoDigitsFormat(hours) : ((hours + 12 - 1) % 12 + 1).ToString();
            return hour.ToString() + ':' + Utils.TwoDigitsFormat(minutes);
        }

        public Dictionary GetTimes(DateTime date, double latitude, double? longitude, double? timeZone, int? dst = null, TimeFormat timeFormat = TimeFormat.Format24h, int elv = 0)
        {
            if (latitude > 66)
                this.latitude = 45;
            else
                this.latitude = latitude;
            if (longitude != null)
                this.longitude = (double)longitude;

            this.elv = elv;

            this.timeFormat = timeFormat;

            if (timeZone == null)
                timeZone = GetTimeZone(date);

            if (dst == null)
                dst = GetDst(date);

            this.timeZone = (double)timeZone + (int)dst;
            julianDate = Utils.GregorianDateToJulianDay(date);

            return ComputeTimes();
        }
        #endregion

        #region Calculation methods
        private double MidDay(double time)
        {
            SunPosition sunPosition = new SunPosition(julianDate + time);
            double eqt = sunPosition.Equation;
            double noon = Utils.FixHour(12 - eqt);
            return noon;
        }

        // Compute the time at which sun reaches a specific angle below horizon
        private double SunAngleTime(double angle, double time, string direction = "")
        {
            SunPosition sunPosition = new SunPosition(julianDate + time);
            double decl = sunPosition.Declinaison;
            double noon = MidDay(time);
            double acos = (-DegreeMath.Sin(angle) - DegreeMath.Sin(decl) * DegreeMath.Sin(latitude)) /
                          (DegreeMath.Cos(decl) * DegreeMath.Cos(latitude));
            if (acos > 1) acos = 1;
            double t = 1 / 15.0 * DegreeMath.Acos(acos);
            return noon + (direction == "ccw" ? -t : t);
        }

        // Compute asr time 
        private double AsrTime(double factor, double time)
        {
            SunPosition sunPosition = new SunPosition(julianDate + time);
            double decl = sunPosition.Declinaison;
            double angle = -DegreeMath.Acot(factor + DegreeMath.Tan(Math.Abs(latitude - decl)));
            return SunAngleTime(angle, time);
        }

        #endregion

        #region Compute prayer times

        // Compute prayer times at given julian date
        private Dictionary ComputePrayerCalculations(Dictionary times)
        {
            Dictionary PrayerCalculations = new Dictionary(8);
            times = DayPortions(times);

            PrayerCalculations[TimeNames.Imsak] = SunAngleTime(Utils.Eval(settings[TimeNames.Imsak].ToString()), (double)times[TimeNames.Imsak], "ccw");
            PrayerCalculations[TimeNames.Fajr] = SunAngleTime(Utils.Eval(settings[TimeNames.Fajr].ToString()), (double)times[TimeNames.Fajr], "ccw");
            PrayerCalculations[TimeNames.Sunrise] = SunAngleTime(RiseSetAngle(), (double)times[TimeNames.Sunrise], "ccw");
            PrayerCalculations[TimeNames.Dhuhr] = MidDay(Utils.Eval(settings[TimeNames.Dhuhr].ToString()));
            PrayerCalculations[TimeNames.Asr] = AsrTime(AsrFactor((AsrMethod)settings[TimeNames.Asr]), (double)times[TimeNames.Asr]);
            PrayerCalculations[TimeNames.Sunset] = SunAngleTime(RiseSetAngle(), (double)times[TimeNames.Sunset]);
            PrayerCalculations[TimeNames.Maghrib] = SunAngleTime(Utils.Eval(settings[TimeNames.Maghrib].ToString()), (double)times[TimeNames.Maghrib]);
            PrayerCalculations[TimeNames.Isha] = SunAngleTime(Utils.Eval(settings[TimeNames.Isha].ToString()), (double)times[TimeNames.Isha]);

            return PrayerCalculations;
        }

        // Compute prayer times
        private Dictionary ComputeTimes()
        {
            // Default times
            Dictionary times = new Dictionary(10);
            times[TimeNames.Imsak] = 5;
            times[TimeNames.Fajr] = 5;
            times[TimeNames.Sunrise] = 6;
            times[TimeNames.Dhuhr] = 12;
            times[TimeNames.Asr] = 13;
            times[TimeNames.Sunset] = 18;
            times[TimeNames.Maghrib] = 18;
            times[TimeNames.Isha] = 18;

            for (int i = 1; i <= numIterations; i++)
            {
                times = ComputePrayerCalculations(times);
            }

            times = AdjustTimes(times);

            // add midnight time
            times[TimeNames.Midnight] = ((MidnightMethod)settings[TimeNames.Midnight] == MidnightMethod.Jafari) ?
                    (double)times[TimeNames.Sunset] + Utils.TimeDiff((double)times[TimeNames.Sunset], (double)times[TimeNames.Fajr]) / 2 :
                    (double)times[TimeNames.Sunset] + Utils.TimeDiff((double)times[TimeNames.Sunset], (double)times[TimeNames.Sunrise]) / 2;

            times = TuneTimes(times);

            return ModifyFormats(times);
        }

        private Dictionary ModifyFormats(Dictionary times)
        {
            Dictionary formattedTimes = new Dictionary(10);
            foreach (DictionaryEntry item in times)
            {
                var value = GetFormattedTime((double)item.Value, timeFormat);
                formattedTimes[item.Key] = value;
            }
            return formattedTimes;
        }

        private Dictionary TuneTimes(Dictionary times)
        {
            Dictionary tmp = new Dictionary(10);
            foreach (DictionaryEntry item in times)
            {
                var value = Convert.ToDouble(item.Value) + Convert.ToDouble(offsets[item.Key]) / 60;
                tmp[item.Key] = value;
            }

            return tmp;
        }

        private Dictionary AdjustTimes(Dictionary times)
        {
            Dictionary tmp = new Dictionary(10);
            foreach (DictionaryEntry item in times)
            {
                var value = Convert.ToDouble(item.Value) + timeZone - longitude / 15;
                tmp[item.Key] = value;
            }

            if ((HighLatitudeMethod)settings["HighLats"] != HighLatitudeMethod.None)
                tmp = AdjustHighLatitudes(tmp);

            if (Utils.ContainsMin(settings[TimeNames.Imsak]))
                tmp[TimeNames.Imsak] = (double)tmp[TimeNames.Fajr] - Utils.Eval(settings[TimeNames.Imsak].ToString()) / 60;
            if (Utils.ContainsMin(settings[TimeNames.Maghrib]))
                tmp[TimeNames.Maghrib] = (double)tmp[TimeNames.Sunset] + Utils.Eval(settings[TimeNames.Maghrib].ToString()) / 60;
            if (Utils.ContainsMin(settings[TimeNames.Isha]))
                tmp[TimeNames.Isha] = (double)tmp[TimeNames.Maghrib] + Utils.Eval(settings[TimeNames.Isha].ToString()) / 60;

            tmp[TimeNames.Dhuhr] = (double)tmp[TimeNames.Dhuhr] + Utils.Eval(settings[TimeNames.Dhuhr].ToString()) / 60;

            return tmp;
        }

        private Dictionary AdjustHighLatitudes(Dictionary times)
        {
            var nightTime = Utils.TimeDiff((double)times[TimeNames.Sunset], (double)times[TimeNames.Sunrise]);

            times[TimeNames.Imsak] = AdjustHLTime((double)times[TimeNames.Imsak], (double)times[TimeNames.Sunrise], Utils.Eval(settings[TimeNames.Imsak].ToString()), nightTime, "ccw");
            times[TimeNames.Fajr] = AdjustHLTime((double)times[TimeNames.Fajr], (double)times[TimeNames.Sunrise], Utils.Eval(settings[TimeNames.Fajr].ToString()), nightTime, "ccw");
            times[TimeNames.Isha] = AdjustHLTime((double)times[TimeNames.Isha], (double)times[TimeNames.Sunset], Utils.Eval(settings[TimeNames.Isha].ToString()), nightTime);
            times[TimeNames.Maghrib] = AdjustHLTime((double)times[TimeNames.Maghrib], (double)times[TimeNames.Sunset], Utils.Eval(settings[TimeNames.Maghrib].ToString()), nightTime);

            return times;
        }

        private double AdjustHLTime(double time, double based, double angle, double nightTime, string direction = "")
        {
            double portion = NightPortion(angle, nightTime);
            double timeDiff = (direction == "ccw") ? Utils.TimeDiff(time, based) : Utils.TimeDiff(based, time);
            if (double.IsNaN(time) || timeDiff > portion)
                time = based + (direction == "ccw" ? -portion : portion);
            return time;
        }

        private double NightPortion(double angle, double nightTime)
        {
            HighLatitudeMethod method = (HighLatitudeMethod)settings["HighLats"];
            double portion = 1 / 2.0; // MidNight
            if (method == HighLatitudeMethod.AngleBased)
                portion = 1 / 60.0 * angle;
            if (method == HighLatitudeMethod.OneSeventh)
                portion = 1 / 7.0;
            return portion * nightTime;
        }

        // Convert hours to day portions 
        private Dictionary DayPortions(Dictionary times)
        {
            Dictionary tmp = new Dictionary(10);
            foreach (DictionaryEntry item in times)
            {
                var value = Convert.ToDouble(item.Value) / 24.0;
                tmp[item.Key] = value;
            }
            return tmp;
        }

        // Get asr shadow factor
        private double AsrFactor(AsrMethod asrParam)
        {
            return asrParam == AsrMethod.Standard ? 1 : 0;
        }

        // Return sun angle for sunset/sunrise
        private double RiseSetAngle()
        {
            //var earthRad = 6371009; // in meters
            //var angle = DegreeMath.Acos(earthRad/(earthRad+ elv));
            var angle = 0.0347 * Math.Sqrt(elv); // an approximation
            return 0.833 + angle;
        }
        #endregion

        #region Time Zone methods

        // Get daylight saving for a given date
        private int GetDst(DateTime date)
        {
            return Convert.ToInt32(GmtOffset(date.Year, date.Month, date.Day) != GetTimeZone(date));
        }

        // GMT offset for a given date
        private double GmtOffset(int year, int month, int day)
        {
            DateTime localDate = new DateTime(year, month, day, 12, 0, 0, 0);
            DateTime GMTDate = new DateTime(localDate.ToUniversalTime().Ticks);
            TimeSpan timeSpan = localDate.Subtract(GMTDate);
            return timeSpan.Hours;
        }

        private double GetTimeZone(DateTime date)
        {
            int year = date.Year;
            double t1 = GmtOffset(year, 1, 1);
            double t2 = GmtOffset(year, 7, 1);
            return Math.Min(t1, t2);
        }

        #endregion
    }
}
