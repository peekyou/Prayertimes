
namespace PrayerCalculation.Methods
{
    public class MethodLondonMosque : MethodBase
    {
        public MethodLondonMosque()
            : base(MethodNames.LondonMosque)
        {
            parameters = new Dictionary(8);
            parameters.Add(TimeNames.Fajr, 13);
            parameters.Add(TimeNames.Isha, 11);
        }

        public override string ToString()
        {
            return "MethodLondonMosque";
        }
    }
}
