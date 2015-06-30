
namespace PrayerTimes.Common
{
    public enum DisplayModes
    {
        Today,
        NextSevenDays,
        CurrentMonth
    }

    public class Filenames
    {
        public const string Locations = "locations";
        public const string Favorites = "favorites";
    }

    public class ErrorMessages
    {
        public const string GeolocationError = "Could not find your position. Try to search a city";
        public const string InternetConnectionError = "Your are not connected to the Internet. Please check your connection and try again";
    }

    public class DefaultValues
    {
        public const int MaghribAdjustment = 0;
    }
}
