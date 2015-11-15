using PrayerCalculation;
using PrayerCalculation.Methods;
using PrayerTimes.Common;
using PrayerTimes.Model;
using PrayerTimes.Models;
using PrayerTimes.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.ApplicationModel.Search;
using Windows.Devices.Geolocation;
using Windows.UI.Xaml;

/**
 * TODO : 
 * vérifier les noms des pays par rapport au web service de google
 * check East Timor, Congo ? South Sudan...
 * Les états du Brésil, du Canada, du Mexique, Australie
 **/

namespace PrayerTimes.ViewModel
{
    public class PrayerViewModel : BindableBase
    {
        #region Fields & Properties
        Windows.ApplicationModel.Resources.ResourceLoader loader = new Windows.ApplicationModel.Resources.ResourceLoader();
        private PrayerTimeCalculation prayerCalculation;
        Windows.Storage.ApplicationDataContainer localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
        private DispatcherTimer timer = new DispatcherTimer();

        private bool isSearchEnabled;
        public bool IsSearchEnabled
        {
            get { return isSearchEnabled; }
            set { isSearchEnabled = value; OnPropertyChanged("IsSearchEnabled"); }
        }


        private bool isCoordsComplete;
        public bool IsCoordsComplete
        {
            get { return isCoordsComplete; }
            set { isCoordsComplete = value; }
        }

        private DateTime currentDate;
        public DateTime CurrentDate
        {
            get { return currentDate; }
            set { currentDate = value; OnPropertyChanged("CurrentDate"); }
        }

        public string MonthName
        {
            get { return currentDate.ToString("MMMM"); }
        }

        private Location location;
        public Location Location
        {
            get { return location; }
            set { location = value; OnPropertyChanged("Location"); }
        }

        public ObservableCollection<Group> Groups { get; set; }

        private DisplayModes displayMode;
        public DisplayModes DisplayMode
        {
            get { return displayMode; }
            set { displayMode = value; OnPropertyChanged("DisplayMode"); }
        }

        private bool isSearching;
        public bool IsSearching
        {
            get { return isSearching; }
            set
            {
                isSearching = value;
                OnPropertyChanged("IsSearching");
                if (value)
                    IsSearchingOrError = true;
                else
                {
                    if (string.IsNullOrEmpty(ErrorMessage))
                        IsSearchingOrError = false;
                }
            }
        }

        private ObservableCollection<Location> userFavorites;
        public ObservableCollection<Location> UserFavorites
        {
            get { return userFavorites; }
            set { userFavorites = value; OnPropertyChanged("UserFavorites"); }
        }

        private string errorMessage;
        public string ErrorMessage
        {
            get { return errorMessage; }
            set
            {
                errorMessage = value;
                OnPropertyChanged("ErrorMessage");
                if (!string.IsNullOrEmpty(value))
                    IsSearchingOrError = true;
                else
                {
                    if (!IsSearching)
                        IsSearchingOrError = false;
                }
            }
        }

        private bool isSearchingOrError;
        public bool IsSearchingOrError
        {
            get { return isSearchingOrError; }
            set { isSearchingOrError = value; OnPropertyChanged("IsSearchingOrError"); }
        }


        private string methodName;
        public string MethodName
        {
            get { return methodName; }
            set { methodName = value; OnPropertyChanged("MethodName"); }
        }

        private string asrMethod;
        public string AsrMethod
        {
            get { return asrMethod; }
            set { asrMethod = value; OnPropertyChanged("AsrMethod"); }
        }

        private string midnightMethod;
        public string MidnightMethod
        {
            get { return midnightMethod; }
            set { midnightMethod = value; OnPropertyChanged("MidnightMethod"); }
        }

        private int maghribAdjustement;
        public int MaghribAdjustement
        {
            get { return maghribAdjustement; }
            set { maghribAdjustement = value; OnPropertyChanged("MaghribAdjustement"); }
        }


        public string MethodUsedString { get; set; }

        private MethodBase method;
        public MethodBase Method
        {
            get { return method; }
            set { method = value; OnPropertyChanged("Method"); }
        }

        public SearchPane SearchPane { get; set; }

        private ObservableCollection<string> methodsList = new ObservableCollection<string>()
        {
            "Auto",
            MethodNames.MWL,
            MethodNames.Makkah,
            MethodNames.UOIF,
            MethodNames.ParisMosque,
            MethodNames.ISNA,
            //MethodNames.LondonMosque,
            //MethodNames.BirminghamMosque,
            MethodNames.Morocco,
            MethodNames.Egypt,
            MethodNames.Jafari,
            MethodNames.Karachi,
            MethodNames.Tehran
        };
        public ObservableCollection<string> MethodsList
        {
            get { return methodsList; }
        }

        private ObservableCollection<string> asrMethodsList = new ObservableCollection<string>()
        {
            "Standard",
            "Hanafi"
        };
        public ObservableCollection<string> AsrMethodsList
        {
            get { return asrMethodsList; }
        }

        private ObservableCollection<string> midnightMethodsList = new ObservableCollection<string>()
        {
            "Standard",
            "Jafari"
        };
        public ObservableCollection<string> MidnightMethodsList
        {
            get { return midnightMethodsList; }
        }

        #endregion Fields & Properties

        #region Commands
        private ICommand searchCommand;
        public ICommand SearchCommand
        {
            get
            {
                if (searchCommand == null)
                    searchCommand = new DelegateCommand(Search, null);
                return searchCommand;
            }
        }

        private void Search(object obj)
        {
            if (SearchPane != null)
            {
                SearchPane.PlaceholderText = loader.GetString("SearchPanePlaceholderText");
                SearchPane.Show();
            }
        }
        #endregion

        public PrayerViewModel()
        {
            timer.Tick += TimerTick;
            timer.Interval = new TimeSpan(0, 0, 1);
            timer.Start();

            currentDate = DateTime.Now;

            MethodUsedString = loader.GetString("CurrentMethod");

            DisplayMode = DisplayModes.NextSevenDays;
            MethodName = (string)localSettings.Values["method"];
            if (string.IsNullOrWhiteSpace(MethodName))
                MethodName = MethodsList[0];

            AsrMethod = (string)localSettings.Values["asrMethod"];
            if (string.IsNullOrWhiteSpace(AsrMethod))
                AsrMethod = AsrMethodsList[0];

            MidnightMethod = (string)localSettings.Values["midnightMethod"];
            if (string.IsNullOrWhiteSpace(MidnightMethod))
                MidnightMethod = MidnightMethodsList[0];

            int maghribAdj;
            if (int.TryParse((string)localSettings.Values["maghribAdjustement"], out maghribAdj))
                MaghribAdjustement = maghribAdj;
            else
                MaghribAdjustement = DefaultValues.MaghribAdjustment;

            Location = new Location();

            try
            {
                SearchPane = SearchPane.GetForCurrentView();
                if (SearchPane != null)
                {
                    this.IsSearchEnabled = true;
                }
            }
            catch { }
        }

        private void TimerTick(object sender, object e)
        {
            CurrentDate = DateTime.Now;
            if (Groups != null)
            {
                Group first = Groups.Where(g => g.Title == DateTime.Now.ToString("ddd d MMM")).FirstOrDefault();
                if (first != null && first.Items != null)
                {
                    List<Prayer> todayPrayers = first.Items.ToList();

                    foreach (Prayer prayer in todayPrayers)
                    {
                        prayer.TimeChanged();
                    }
                }
            }
        }

        public async Task LoadPrayers(bool locationChanged = true)
        {
            if (locationChanged)
            {
                if (!string.IsNullOrEmpty((string)localSettings.Values["city"]))
                {
                    Location.Latitude = Convert.ToDouble(localSettings.Values["latitude"]);
                    Location.Longitude = Convert.ToDouble(localSettings.Values["longitude"]);
                    Location.City = (string)localSettings.Values["city"];
                    Location.State = (string)localSettings.Values["state"];
                    Location.Country = (string)localSettings.Values["country"];
                    Location.TimeZone = Convert.ToDouble(localSettings.Values["timezone"]);
                    Location.TimezoneName = (string)localSettings.Values["timezoneName"];
                    //Location.Dst = (int)localSettings.Values["dst"];

                    OnPropertyChanged("Location");
                    IsCoordsComplete = true;
                    IsSearching = false;
                    ComputePrayers(TimeZoneInfo.Local);
                }
                else
                {
                    await LoadPrayersWithGeolocation();
                }
            }
            else
            {
                if (string.IsNullOrEmpty(errorMessage))
                {
                    ComputePrayers();
                }
            }
        }

        public async Task LoadPrayersWithGeolocation()
        {
            IsSearching = true;
            Location = new Location();
            bool success = await GetGeoposition();
            if (success)
            {
                SaveHomeSettings();
                ComputePrayers(TimeZoneInfo.Local);
                UpdateNotifications();
            }
            else
            {
                if (Groups != null && Groups.Count > 0)
                    Groups.Clear();
            }
        }

        public void LoadPrayersByLocation(Location location)
        {
            Location = location;
            ComputePrayers();
        }

        private AsrMethod ConvertStringToAsrMethod(string asrMethod)
        {
            if (asrMethod == AsrMethodsList[1])
                return PrayerCalculation.AsrMethod.Hanafi;
            else
                return PrayerCalculation.AsrMethod.Standard;
        }

        private MidnightMethod ConvertStringToMidnightMethod(string midnightMethod)
        {
            if (midnightMethod == MidnightMethodsList[1])
                return PrayerCalculation.MidnightMethod.Jafari;
            else
                return PrayerCalculation.MidnightMethod.Standard;
        }

        private void SaveHomeSettings()
        {
            localSettings.Values["latitude"] = Location.Latitude;
            localSettings.Values["longitude"] = Location.Longitude;
            localSettings.Values["city"] = Location.City;
            localSettings.Values["state"] = Location.State;
            localSettings.Values["country"] = Location.Country;
            localSettings.Values["timezone"] = Location.TimeZone;
            localSettings.Values["timezoneName"] = Location.TimezoneName;
        }

        private MethodBase GetMethod()
        {
            Method = null;
            switch (MethodName)
            {
                case "Auto": Method = GetMethodByCountry();
                    break;
                case MethodNames.Egypt: Method = new MethodEgypt();
                    break;
                case MethodNames.ISNA: Method = new MethodISNA();
                    break;
                case MethodNames.Jafari: Method = new MethodJafari();
                    break;
                case MethodNames.Karachi: Method = new MethodKarachi();
                    break;
                case MethodNames.Makkah: Method = new MethodMakkah();
                    break;
                case MethodNames.MWL: Method = new MethodMWL();
                    break;
                case MethodNames.Tehran: Method = new MethodTehran();
                    break;
                case MethodNames.UOIF: Method = new MethodUOIF();
                    break;
                case MethodNames.ParisMosque: Method = new MethodParisMosque();
                    break;
                //case MethodNames.LondonMosque: Method = new MethodLondonMosque();
                //    break;
                //case MethodNames.BirminghamMosque: Method = new MethodBirminghamMosque();
                //    break;
                case MethodNames.Morocco: Method = new MethodMorocco();
                    break;
                default:
                    throw new NotSupportedException("Method unknown");
            }
            return Method;
        }

        private MethodBase GetMethodByCountry(string country = null, string city = null)
        {
            if (string.IsNullOrEmpty(country))
                country = Location.Country;

            if (string.IsNullOrEmpty(city))
                city = Location.City;

            MethodBase method = null;
            switch (country)
            {
                case "France": method = new MethodUOIF();
                    break;

                case "Morocco": method = new MethodMorocco();
                    break;

                case "Pakistan":
                case "India":
                case "Bangladesh":
                case "Afghanistan":
                    method = new MethodKarachi();
                    break;

                case "Bahrain":
                case "United Arab Emirates":
                case "Saudi Arabia":
                case "Kuwait":
                case "Oman":
                case "Qatar":
                case "Yemen":
                case "Syria":
                    method = new MethodMakkah();
                    break;

                case "Egypt":
                case "Algeria":
                case "Libya":
                    method = new MethodEgypt();
                    break;

                default:
                    switch (LocationService.GetContinent(country))
                    {
                        case "Antarctica":
                        case "Europe": method = new MethodMWL();
                            break;
                        case "North America": method = new MethodISNA();
                            break;
                        case "Africa": method = new MethodMWL();
                            break;
                        case "South America": method = new MethodMWL();
                            break;
                        default: method = new MethodMWL();
                            break;
                    }
                    break;

            }
            return method;
        }

        private void ComputePrayers(TimeZoneInfo timeZoneInfo = null)
        {
            prayerCalculation = new PrayerTimeCalculation(
                GetMethod(),
                ConvertStringToAsrMethod(this.asrMethod),
                ConvertStringToMidnightMethod(this.midnightMethod),
                this.MaghribAdjustement);

            Groups = new ObservableCollection<Group>();
            int length = 0;
            DateTime dateUsed = currentDate, date;
            switch (displayMode)
            {
                case DisplayModes.Today:
                    length = 1;
                    break;
                case DisplayModes.NextSevenDays:
                    length = 7;
                    break;
                case DisplayModes.CurrentMonth:
                    length = DateTime.DaysInMonth(currentDate.Year, currentDate.Month);
                    dateUsed = new DateTime(currentDate.Year, currentDate.Month, 1);
                    break;
            }

            for (int i = 0; i < length; i++)
            {
                date = dateUsed.AddDays(i);
                string day = date.ToString("ddd d MMM");

                TimeFormat format = TimeFormat.Format24h;
                if (timeZoneInfo != null)
                    Location.Dst = LocationService.GetTimezoneDST(timeZoneInfo, date);
                else
                    Location.Dst = LocationService.GetDSTByRegion(Location.Country, Location.State, Location.City, Location.TimeZone, date);

                ObservableCollection<Prayer> prayers = DictionaryPrayersToCollection(prayerCalculation.GetTimes(date, Location.Latitude, Location.Longitude, Location.TimeZone, Location.Dst, format), date, i - 1);
                Group group = new Group();
                group.Items = prayers;
                group.Title = day;
                Groups.Add(group);
            }

            // Assign the next prayer for each prayer...
            for (int i = 0; i < Groups.Count; i++)
            {
                for (int j = 0; j < Groups[i].Items.Count; j++)
                {
                    if (j + 1 < Groups[i].Items.Count)
                        Groups[i].Items[j].NextPrayer = Groups[i].Items[j + 1];
                    else if (i + 1 < Groups.Count)
                        Groups[i].Items[j].NextPrayer = Groups[i + 1].Items[0];
                    else
                        Groups[i].Items[j].NextPrayer = null;
                }
            }

            OnPropertyChanged("Groups");
        }

        private async Task<bool> GetGeoposition()
        {
            try
            {
                ErrorMessage = "";
                Geolocator geo = new Geolocator();
                Geoposition pos = await geo.GetGeopositionAsync();
                Geocoordinate coords = pos.Coordinate;
                Location.Latitude = coords.Latitude;
                Location.Longitude = coords.Longitude;

                Tuple<double, int, string> zoneInfos = await LocationService.GetTimeInfos(Location.Latitude, Location.Longitude);
                Location.TimeZone = zoneInfos.Item1;
                Location.TimezoneName = zoneInfos.Item3;

                IsCoordsComplete = true;
                Tuple<string, string, string> location = await LocationService.ReverseGeolocation(coords.Longitude, coords.Latitude);
                Location.City = location.Item1;
                Location.State = location.Item2;
                Location.Country = location.Item3;

                Location.Dst = LocationService.GetDSTByRegion(Location.Country, Location.State, Location.City, Location.TimeZone, DateTime.Now);
                OnPropertyChanged("Location");

                return true;
            }
            catch (WebException)
            {
                ErrorMessage = loader.GetString("ErrorMessagesInternetConnectionError");
                return false;
            }
            catch (Exception)
            {
                ErrorMessage = loader.GetString("ErrorMessagesGeolocationError");
                return false;
            }
            finally
            {
                IsSearching = false;
            }
        }

        private ObservableCollection<Prayer> DictionaryPrayersToCollection(IDictionary prayers, DateTime prayerDate, int previousGroupIndex)
        {
            prayerDate = new DateTime(prayerDate.Year, prayerDate.Month, prayerDate.Day);
            ObservableCollection<Prayer> prayerList = new ObservableCollection<Prayer>();
            Prayer previousFajr = null;
            if (previousGroupIndex >= 0)
            {
                previousFajr = Groups[previousGroupIndex].Items.LastOrDefault();
            }

            prayerList.Add(new Prayer() { DateTime = prayerDate, Hour = prayers["Fajr"].ToString(), Name = "Fajr", NextPrayer = previousFajr });
            prayerList.Add(new Prayer() { DateTime = prayerDate, Hour = prayers["Sunrise"].ToString(), Name = "Shuruq", NextPrayer = prayerList[0] });
            prayerList.Add(new Prayer() { DateTime = prayerDate, Hour = prayers["Dhuhr"].ToString(), Name = "Dhur", NextPrayer = prayerList[1] });
            prayerList.Add(new Prayer() { DateTime = prayerDate, Hour = prayers["Asr"].ToString(), Name = "Asr", NextPrayer = prayerList[2] });
            prayerList.Add(new Prayer() { DateTime = prayerDate, Hour = prayers["Maghrib"].ToString(), Name = "Maghrib", NextPrayer = prayerList[3] });
            prayerList.Add(new Prayer() { DateTime = prayerDate, Hour = prayers["Isha"].ToString(), Name = "Isha", NextPrayer = prayerList[4] });

            return prayerList;
        }

        public async void SearchCity(string city)
        {
            try
            {
                ErrorMessage = "";
                IsSearching = true;
                bool found = false;
                Location = new Location();

                // Try in cache first
                List<Location> cacheList = await LocationService.GetLocationsFromCache();
                foreach (Location item in cacheList)
                {
                    if (item.City.ToLower().Contains(city.ToLower()))
                    {
                        Location = item;
                        found = true;
                        IsCoordsComplete = true;
                        IsSearching = false;
                        break;
                    }
                }

                //if (found && LocationService.IsCachedLocationExpired(Location))
                //{
                //    LocationService.DeleteFile(Filenames.Locations);
                //    found = false;
                //}

                if (!found)
                {
                    Tuple<double, double, string, string, string> position = await LocationService.GetPosition(city);
                    Location.Latitude = position.Item1;
                    Location.Longitude = position.Item2;
                    Location.City = position.Item3;
                    Location.State = position.Item4;
                    Location.Country = position.Item5;

                    Tuple<double, int, string> zoneInfos = await LocationService.GetTimeInfos(Location.Latitude, Location.Longitude);
                    Location.TimeZone = zoneInfos.Item1;
                    Location.TimezoneName = zoneInfos.Item3;
                    Location.Dst = LocationService.GetDSTByRegion(Location.Country, Location.State, Location.City, Location.TimeZone, DateTime.Now);

                    OnPropertyChanged("Location");

                    IsCoordsComplete = true;

                    // Mise en cache
                    await LocationService.CacheLocation(Location.City, Location.State, Location.Country, Location.Latitude, Location.Longitude, Location.TimeZone, Location.Dst, Location.TimezoneName);
                }
                ComputePrayers();

                if (string.IsNullOrEmpty((string)localSettings.Values["city"]))
                {
                    SaveHomeSettings();
                    UpdateNotifications();
                }
            }
            catch (FileNotFoundException)
            {
                ErrorMessage = loader.GetString("ErrorMessagesLocationNotFoundError");
            }
            catch (NotSupportedException e)
            {
                ErrorMessage = e.Message;
            }
            catch (Exception)
            {
                ErrorMessage = loader.GetString("ErrorMessagesInternetConnectionError");
            }
            finally
            {
                IsSearching = false;
            }
        }

        public void UpdateNotifications()
        {
            // Cancel previous toast notification planning
            this.CancelPlanningToastNotifications();

            Location homeLocation = new Location();
            homeLocation.Latitude = Convert.ToDouble(localSettings.Values["latitude"]);
            homeLocation.Longitude = Convert.ToDouble(localSettings.Values["longitude"]);
            homeLocation.City = (string)localSettings.Values["city"];
            homeLocation.State = (string)localSettings.Values["state"];
            homeLocation.Country = (string)localSettings.Values["country"];
            homeLocation.TimeZone = Convert.ToDouble(localSettings.Values["timezone"]);
            homeLocation.TimezoneName = (string)localSettings.Values["timezoneName"];

            MethodBase homeMethod = null;
            if (MethodName == "Auto")
                homeMethod = GetMethodByCountry(location.Country, location.City);
            else
                homeMethod = Method;

            this.UpdateTile(homeLocation, homeMethod, AsrMethod, MidnightMethod);
            toastNotificationsTask = this.UpdateToastNotifications(homeLocation, homeMethod, AsrMethod, MidnightMethod);
        }

        private CancellationTokenSource source;
        private Task toastNotificationsTask;
        private async Task UpdateToastNotifications(Location location, MethodBase method, string asrMethod, string midnightMethod)
        {

            this.source = new CancellationTokenSource();
            PrayerTimeCalculation prayerCalculation = new PrayerTimeCalculation(
                method,
                ConvertStringToAsrMethod(asrMethod),
                ConvertStringToMidnightMethod(midnightMethod),
                this.MaghribAdjustement);

            await Notification.PlanToastNotifications(source.Token, prayerCalculation, location);
        }

        private void UpdateTile(Location location, MethodBase method, string asrMethod, string midnightMethod)
        {
            if (!string.IsNullOrEmpty(location.City) && method != null)
                Notification.UpdateTile(location, method.ToString(), asrMethod, midnightMethod);
        }

        public void WaitForTask()
        {
            if (this.toastNotificationsTask != null && !this.toastNotificationsTask.IsCanceled
                && !this.toastNotificationsTask.IsCompleted && !this.toastNotificationsTask.IsFaulted)
            {
                Task.WaitAll(this.toastNotificationsTask);
            }
        }

        public void CancelPlanningToastNotifications()
        {
            if (this.source != null)
            {
                this.source.Cancel();
            }
        }
    }
}
