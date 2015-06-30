
namespace PrayerCalculation.Methods
{
    public class MethodUOIF : MethodBase
    {
        public MethodUOIF()
            : base(MethodNames.UOIF)
        {
            parameters = new Dictionary(8);
            parameters.Add(TimeNames.Fajr, 13);
            parameters.Add(TimeNames.Dhuhr, "5 min");
            parameters.Add(TimeNames.Maghrib, "4 min");
            parameters.Add(TimeNames.Isha, 13);
        }

        public override string ToString()
        {
            return "MethodUOIF";
        }
    }
}