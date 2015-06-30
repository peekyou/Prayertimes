
namespace PrayerCalculation.Methods
{
    public class MethodMorocco : MethodBase
    {
        public MethodMorocco()
            : base(MethodNames.Morocco)
        {
            parameters = new Dictionary(8);
            parameters.Add(TimeNames.Fajr, 19);
            parameters.Add(TimeNames.Dhuhr, "6 min");
            parameters.Add(TimeNames.Maghrib, "5 min");
            parameters.Add(TimeNames.Isha, 17);
        }

        public override string ToString()
        {
            return "MethodMorocco";
        }
    }
}
