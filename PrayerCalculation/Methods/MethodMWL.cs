
namespace PrayerCalculation.Methods
{
    public class MethodMWL : MethodBase
    {
        public MethodMWL()
            : base(MethodNames.MWL)
        {
            parameters = new Dictionary(8);
            parameters.Add(TimeNames.Fajr, 18);
            parameters.Add(TimeNames.Isha, 17);
        }

        public override string ToString()
        {
            return "MethodMWL";
        }
    }
}
