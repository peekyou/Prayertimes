
namespace PrayerCalculation.Methods
{
    public class MethodMakkah : MethodBase
    {
        public MethodMakkah()
            : base(MethodNames.Makkah)
        {
            parameters = new Dictionary(8);
            parameters.Add(TimeNames.Fajr, 18.5);
            parameters.Add(TimeNames.Isha, "90 min");
        }

        public override string ToString()
        {
            return "MethodMakkah";
        }
    }
}
