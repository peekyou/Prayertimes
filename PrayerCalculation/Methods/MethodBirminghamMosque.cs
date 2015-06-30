
namespace PrayerCalculation.Methods
{
    public class MethodBirminghamMosque : MethodBase
    {
        public MethodBirminghamMosque()
            : base(MethodNames.BirminghamMosque)
        {
            parameters = new Dictionary(8);
            parameters.Add(TimeNames.Fajr, 13);
            parameters.Add(TimeNames.Isha, 15);
        }

        public override string ToString()
        {
            return "MethodBirminghamMosque";
        }
    }
}
