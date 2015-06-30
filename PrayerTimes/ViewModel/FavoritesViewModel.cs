using PrayerTimes.Common;
using PrayerTimes.Model;
using PrayerTimes.Utils;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace PrayerTimes.ViewModel
{
    class FavoritesViewModel : BindableBase
    {
        Windows.ApplicationModel.Resources.ResourceLoader loader = new Windows.ApplicationModel.Resources.ResourceLoader();
        public Location location { get; private set; }

        private bool success;
        public bool Success
        {
            get { return success; }
            set { success = value; OnPropertyChanged("Success"); }
        }

        private ObservableCollection<Location> userFavorites;
        public ObservableCollection<Location> UserFavorites
        {
            get { return userFavorites; }
            set { userFavorites = value; OnPropertyChanged("UserFavorites"); }
        }

        public string FavoritesTitle { get; set; }

        public FavoritesViewModel(Location location = null)
        {
            this.location = location;
            FavoritesTitle = loader.GetString("FavoritesTitle");
        }

        public async Task AddLocationToFavorites()
        {
            try
            {
                await LocationService.AddLocationToFavorites(location.City, location.State, location.Country, location.Latitude, location.Longitude, location.TimeZone, location.Dst, location.TimezoneName);
                Success = true;
            }
            catch (Exception)
            {
                // TODO
                // Check la meilleure méthode pour afficher un message d'erreur
                Success = false;
            }
        }

        public async Task RemoveLocationFromFavorites()
        {
            try
            {
                await LocationService.RemoveLocationFromFile(location.City, location.State, location.Country, Filenames.Favorites);
                Success = true;
            }
            catch (Exception)
            {
                // TODO
                // Check la meilleure méthode pour afficher un message d'erreur
                Success = false;
            }
        }

        public async Task<List<Location>> GetFavoriteLocations()
        {
            return await LocationService.GetFavoriteLocations();
        }
    }
}
