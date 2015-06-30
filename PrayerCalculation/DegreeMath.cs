using System;

namespace PrayerCalculation
{
    public class DegreeMath
    {
        // degree sin
        public static double Sin(double d)
        {
            return Math.Sin(DegreeToRadian(d));
        }

        // degree cos
        public static double Cos(double d)
        {
            return Math.Cos(DegreeToRadian(d));
        }

        // degree tan
        public static double Tan(double d)
        {
            return Math.Tan(DegreeToRadian(d));
        }

        // degree arcsin
        public static double Asin(double x)
        {
            return RadianToDegree(Math.Asin(x));
        }

        // degree arccos
        public static double Acos(double x)
        {
            return RadianToDegree(Math.Acos(x));
        }

        // degree arctan
        public static double Atan(double x)
        {
            return RadianToDegree(Math.Atan(x));
        }

        // degree arctan2
        public static double Atan2(double y, double x)
        {
            return RadianToDegree(Math.Atan2(y, x));
        }

        // degree arccot
        public static double Acot(double x)
        {
            return RadianToDegree(Math.Atan(1 / x));
        }

        // Radian to Degree
        public static double RadianToDegree(double radian)
        {
            return (radian * 180.0) / Math.PI;
        }

        // degree to radian
        public static double DegreeToRadian(double degree)
        {
            return (degree * Math.PI) / 180.0;
        }
    }
}
