using System;
using System.Text.RegularExpressions;

namespace PrayerCalculation
{
    public class Utils
    {
        private static double Fix(double a, double b)
        {
            a = a - b * (Math.Floor(a / b));
            return (a < 0) ? a + b : a;
        }

        public static double FixHour(double hour)
        {
            return Fix(hour, 24);
        }

        public static double FixAngle(double angle)
        {
            return Fix(angle, 360);
        }

        public static double GregorianDateToJulianDay(DateTime date)
        {
            int year = date.Year;
            int month = date.Month;
            int day = date.Day;
            if (date.Month <= 2)
            {
                year -= 1;
                month += 12;
            };
            double A = Math.Floor(year / 100.0);
            double B = 2 - A + Math.Floor(A / 4);

            double JD = Math.Floor(365.25 * (year + 4716)) + Math.Floor(30.6001 * (month + 1)) + day + B - 1524.5;
            return JD;
        }

        public static double Eval(string str)
        {
            double result;
            bool success = double.TryParse(str, out result);
            if (!success)
            {
                Regex regexObj = new Regex(@"[\D]");
                string resultString = regexObj.Replace(str, "");
                result = double.Parse(resultString);
            }
            return result;
        }

        public static bool ContainsMin(object str)
        {
            return str.ToString().Contains("min");
        }

        public static double TimeDiff(double time1, double time2)
        {
            return FixHour(time2 - time1);
        }

        // Add a leading 0 if necessary
        public static string TwoDigitsFormat(double num)
        {
            return (num < 10) ? "0" + num.ToString() : num.ToString();
        }
    }
}
