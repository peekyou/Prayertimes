using PrayerTimes.Common;
using PrayerTimes.Utils;
using PrayerTimes.ViewModel;
using System;
using System.Collections.Generic;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// The Basic Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234237

namespace PrayerTimes.View
{
    /// <summary>
    /// A basic page that provides characteristics common to most applications.
    /// </summary>
    public sealed partial class PreferencesSettings : PrayerTimes.Common.LayoutAwarePage
    {
        Windows.Storage.ApplicationDataContainer localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
        PrayerViewModel prayerViewModel;

        public PreferencesSettings()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Populates the page with content passed during navigation.  Any saved state is also
        /// provided when recreating a page from a prior session.
        /// </summary>
        /// <param name="navigationParameter">The parameter value passed to
        /// <see cref="Frame.Navigate(Type, Object)"/> when this page was initially requested.
        /// </param>
        /// <param name="pageState">A dictionary of state preserved by this page during an earlier
        /// session.  This will be null the first time a page is visited.</param>
        protected override void LoadState(Object navigationParameter, Dictionary<String, Object> pageState)
        {
            Windows.ApplicationModel.Resources.ResourceLoader loader = new Windows.ApplicationModel.Resources.ResourceLoader();

            prayerViewModel = navigationParameter as PrayerViewModel;
            if (prayerViewModel == null)
                prayerViewModel = new PrayerViewModel();

            DefaultViewModel["Countries"] = LocationService.GetCountryList();
            DefaultViewModel["DefaultLocation"] = localSettings.Values["location"];
            DefaultViewModel["MethodName"] = prayerViewModel.MethodName;
            DefaultViewModel["AsrMethod"] = prayerViewModel.AsrMethod;
            DefaultViewModel["MidnightMethod"] = prayerViewModel.MidnightMethod;
            DefaultViewModel["MaghribAdjustement"] = prayerViewModel.MaghribAdjustement;

            DefaultViewModel["CalculationMethodSettingsString"] = loader.GetString("CalculationMethodSettingsString");
            DefaultViewModel["AsrMethodSettingsString"] = loader.GetString("AsrMethodSettingsString");
            DefaultViewModel["MidnightMethodSettingsString"] = loader.GetString("MidnightMethodSettingsString");
            DefaultViewModel["MaghribAdjustementSettingsString"] = loader.GetString("MaghribAdjustementSettingsString");
        }

        /// <summary>
        /// Preserves state associated with this page in case the application is suspended or the
        /// page is discarded from the navigation cache.  Values must conform to the serialization
        /// requirements of <see cref="SuspensionManager.SessionState"/>.
        /// </summary>
        /// <param name="pageState">An empty dictionary to be populated with serializable state.</param>
        protected override void SaveState(Dictionary<String, Object> pageState)
        {
        }

        protected async override void GoBack(object sender, RoutedEventArgs e)
        {
            // Return the application to the state it was in before search results were requested
            if (this.Frame != null && this.Frame.CanGoBack)
            {
                localSettings.Values["method"] = prayerViewModel.MethodName = MethodComboBox.SelectedItem.ToString();
                localSettings.Values["asrMethod"] = AsrMethodComboBox.SelectedItem.ToString();
                localSettings.Values["midnightMethod"] = MidnightMethodComboBox.SelectedItem.ToString();
                localSettings.Values["maghribAdjustement"] = MaghribAdjustmentTextBox.Text;

                await prayerViewModel.LoadPrayers(false);
                this.Frame.GoBack();
            }
        }

        private void MaghribAdjustmentTextBox_KeyUp(object sender, Windows.UI.Xaml.Input.KeyRoutedEventArgs e)
        {
        }
    }
}
