
namespace PrayerCalculation
{
    public class SunPosition
    {
        public double Equation { get; set; }
        public double Declinaison { get; set; }

        // Compute declination angle of sun and equation of time
        // Ref: http://aa.usno.navy.mil/faq/docs/SunApprox.php
        public SunPosition(double julianDate)
        {
            double D = julianDate - 2451545.0;
            double g = Utils.FixAngle(357.529 + 0.98560028 * D);
            double q = Utils.FixAngle(280.459 + 0.98564736 * D);
            double L = Utils.FixAngle(q + 1.915 * DegreeMath.Sin(g) + 0.020 * DegreeMath.Sin(2 * g));

            double R = 1.00014 - 0.01671 * DegreeMath.Cos(g) - 0.00014 * DegreeMath.Cos(2 * g);
            double e = 23.439 - 0.00000036 * D;

            double RA = DegreeMath.Atan2(DegreeMath.Cos(e) * DegreeMath.Sin(L), DegreeMath.Cos(L)) / 15;
            Equation = q / 15 - Utils.FixHour(RA);
            Declinaison = DegreeMath.Asin(DegreeMath.Sin(e) * DegreeMath.Sin(L));
        }
    }
}
