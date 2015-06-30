
namespace PrayerCalculation.Methods
{
    public class MethodISNA : MethodBase
    {
        public MethodISNA()
            : base(MethodNames.ISNA)
        {
            parameters = new Dictionary(8);
            parameters.Add(TimeNames.Fajr, 15);
            parameters.Add(TimeNames.Isha, 15);
        }

        public override string ToString()
        {
            return "MethodISNA";
        }
    }
}
