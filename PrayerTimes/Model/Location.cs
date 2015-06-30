
namespace PrayerTimes.Model
{
    public class Location
    {
        private string city;

        public string City
        {
            get { return city; }
            set { city = value; }
        }

        private string state;

        public string State
        {
            get { return state; }
            set { state = value; }
        }

        private string country;

        public string Country
        {
            get { return country; }
            set { country = value; }
        }

        private string continent;

        public string Continent
        {
            get { return continent; }
            set { continent = value; }
        }


        private double latitude;

        public double Latitude
        {
            get { return latitude; }
            set { latitude = value; }
        }

        private double longitude;

        public double Longitude
        {
            get { return longitude; }
            set { longitude = value; }
        }

        private double timeZone;

        public double TimeZone
        {
            get { return timeZone; }
            set { timeZone = value; }
        }

        private int dst;

        public int Dst
        {
            get { return dst; }
            set { dst = value; }
        }

        private string timezoneName;

        public string TimezoneName
        {
            get { return timezoneName; }
            set { timezoneName = value; }
        }


        private string fullLocation;

        public string FullLocation
        {
            get
            {
                string cityS = string.IsNullOrWhiteSpace(city) ? "" : city;
                string stateS = string.IsNullOrWhiteSpace(state) ? "" : state;
                string countryS = string.IsNullOrWhiteSpace(country) ? "" : country;
                string separator = string.IsNullOrWhiteSpace(cityS) ? "" : ", ";
                string separatorState = string.IsNullOrWhiteSpace(stateS) ? "" : ", ";
                return cityS + separator + stateS + separatorState + countryS;
            }
            set { fullLocation = value; }
        }

        public string CountryAndState
        {
            get
            {
                string stateS = string.IsNullOrWhiteSpace(state) || state.Equals(country) ? "" : state;
                string countryS = string.IsNullOrWhiteSpace(country) ? "" : country;
                string separatorState = string.IsNullOrWhiteSpace(stateS) ? "" : ", ";
                return stateS + separatorState + countryS;
            }
        }

        public Location(string city = "", string state = "", string country = "", double latitude = 0, double longitude = 0, double timeZone = 0, int dst = 0, string timezoneName = "")
        {
            this.city = city;
            this.state = state;
            this.country = country;
            this.latitude = latitude;
            this.longitude = longitude;
            this.timeZone = timeZone;
            this.dst = dst;
            this.timezoneName = timezoneName;
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;

            Location location = obj as Location;
            if (location == null)
                return false;
            else
            {
                if (this.City == location.City && this.State == location.State && this.Country == location.Country && this.Dst == location.Dst && this.Latitude == location.Latitude && this.Longitude == location.Longitude && this.TimeZone == location.TimeZone && this.TimezoneName == location.TimezoneName)
                    return true;
                else
                    return false;
            }
        }
    }
}
