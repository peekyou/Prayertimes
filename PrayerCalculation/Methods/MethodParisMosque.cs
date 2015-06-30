namespace PrayerCalculation.Methods
{
    public class MethodParisMosque : MethodBase
    {
        public MethodParisMosque()
            : base(MethodNames.ParisMosque)
        {
            parameters = new Dictionary(8);
            parameters.Add(TimeNames.Fajr, 18); //18
            parameters.Add(TimeNames.Dhuhr, "5 min");
            parameters.Add(TimeNames.Maghrib, "4 min");
            parameters.Add(TimeNames.Isha, 14); // 17
        }

        public override string ToString()
        {
            return "MethodParisMosque";
        }
    }
}