
namespace PrayerCalculation.Methods
{
    public abstract class MethodBase
    {
        private string name;

        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        protected Dictionary parameters;

        public Dictionary Parameters
        {
            get { return parameters; }
            set { parameters = value; }
        }

        public MethodBase(string name)
        {
            this.name = name;
        }
    }

    public class MethodNames
    {
        public const string MWL = "Muslim World League";
        public const string ISNA = "Islamic Society of North America (ISNA)";
        public const string Egypt = "Egyptian General Authority of Survey";
        public const string Makkah = "Umm Al-Qura University, Makkah";
        public const string Karachi = "University of Islamic Sciences, Karachi";
        public const string Tehran = "Institute of Geophysics, University of Tehran";
        public const string Jafari = "Shia Ithna-Ashari, Leva Institute, Qum (Jafari)";
        public const string UOIF = "Union des organisations islamiques de France";
        public const string ParisMosque = "Grande mosquée de Paris";
        public const string LondonMosque = "London Central Mosque";
        public const string BirminghamMosque = "Birmingham Central Mosque";
        public const string Morocco = "Maroc, Ministère des Habous et des Affaires Islamiques";
    }
}
