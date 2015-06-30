
namespace PrayerCalculation.Methods
{
    public class MethodJafari : MethodBase
    {
        public MethodJafari()
            : base(MethodNames.Jafari)
        {
            parameters = new Dictionary(8);
            parameters.Add(TimeNames.Fajr, 16);
            parameters.Add(TimeNames.Isha, 14);
            parameters.Add(TimeNames.Maghrib, 4);
            parameters.Add(TimeNames.Midnight, MidnightMethod.Jafari);
        }

        public override string ToString()
        {
            return "MethodJafari";
        }
    }
}
