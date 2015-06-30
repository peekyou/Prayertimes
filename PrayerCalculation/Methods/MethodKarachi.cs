
namespace PrayerCalculation.Methods
{
    public class MethodKarachi : MethodBase
    {
        public MethodKarachi()
            : base(MethodNames.Karachi)
        {
            parameters = new Dictionary(8);
            parameters.Add(TimeNames.Fajr, 18);
            parameters.Add(TimeNames.Isha, 18);
        }

        public override string ToString()
        {
            return "MethodKarachi";
        }
    }
}
