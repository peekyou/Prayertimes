
namespace PrayerCalculation.Methods
{
    public class MethodTehran : MethodBase
    {
        public MethodTehran()
            : base(MethodNames.Tehran)
        {
            parameters = new Dictionary(8);
            parameters.Add(TimeNames.Fajr, 17.7);
            parameters.Add(TimeNames.Isha, 14);
            parameters.Add(TimeNames.Maghrib, 4.5);
            parameters.Add(TimeNames.Midnight, MidnightMethod.Jafari);
        }

        public override string ToString()
        {
            return "MethodTehran";
        }
    }
}
