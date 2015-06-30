using PrayerTimes.Common;
using PrayerTimes.Model;
using PrayerTimes.Utils;
using PrayerTimes.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using Windows.ApplicationModel.Search;
using Windows.Devices.Sensors;
using Windows.Foundation;
using Windows.UI.Notifications;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Grouped Items Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234231

namespace PrayerTimes.View
{
    /// <summary>
    /// A page that displays a grouped collection of items.
    /// </summary>
    public sealed partial class CalendarPage : PrayerTimes.Common.LayoutAwarePage
    {
        PrayerViewModel prayerViewModel;
        SearchPane searchPane;
        bool isEventRegistered;

        private Compass _compass; // Our app's compass object

        // This event handler writes the current compass reading to 
        // the textblocks on the app's main page.

        private void ReadingChanged(object sender, CompassReadingChangedEventArgs e)
        {
            //Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            //{
            //    CompassReading reading = e.Reading;
            //    txtMagnetic.Text = String.Format("{0,5:0.00}", reading.HeadingMagneticNorth);
            //    if (reading.HeadingTrueNorth.HasValue)
            //        txtNorth.Text = String.Format("{0,5:0.00}", reading.HeadingTrueNorth);
            //    else
            //        txtNorth.Text = "No reading.";
            //});
        }


        public CalendarPage()
        {
            this.InitializeComponent();

            BadgeUpdateManager.CreateBadgeUpdaterForApplication().Clear();

            this.TopAppBar.Content = new PrayerTimes.AppBar(this);

            searchPane = SearchPane.GetForCurrentView();
            searchPane.QuerySubmitted += new TypedEventHandler<SearchPane, SearchPaneQuerySubmittedEventArgs>(this.OnQuerySubmitted);
            //searchPane.SuggestionsRequested += new TypedEventHandler<SearchPane, SearchPaneSuggestionsRequestedEventArgs>(this.OnSearchPaneSuggestionsRequested);


            //_compass = Compass.GetDefault(); // Get the default compass object
            //// Assign an event handler for the compass reading-changed event
            //if (_compass != null)
            //{
            //    // Establish the report interval for all scenarios
            //    uint minReportInterval = _compass.MinimumReportInterval;
            //    uint reportInterval = minReportInterval > 16 ? minReportInterval : 16;
            //    _compass.ReportInterval = reportInterval;
            //    _compass.ReadingChanged += new TypedEventHandler<Compass, CompassReadingChangedEventArgs>(ReadingChanged);
            //}
        }

        private async void OnQuerySubmitted(SearchPane sender, SearchPaneQuerySubmittedEventArgs args)
        {
            prayerViewModel.SearchCity(args.QueryText);
            //Frame.Navigate(typeof(SearchResultsPage), args.QueryText);
        }

        public async void OnSearchPaneSuggestionsRequested(object sender, SearchPaneSuggestionsRequestedEventArgs args)
        {
            List<Location> cacheList = await LocationService.GetLocationsFromCache();
            List<string> suggestionList = (from location in cacheList
                                           select location.City).ToList();

            foreach (string suggestion in suggestionList)
            {
                if (suggestion.StartsWith(args.QueryText, StringComparison.CurrentCultureIgnoreCase))
                {
                    // Add suggestion to Search Pane
                    args.Request.SearchSuggestionCollection.AppendQuerySuggestion(suggestion);
                }

                // Break since the Search Pane can show at most 5 suggestions
                if (args.Request.SearchSuggestionCollection.Size >= 5)
                {
                    break;
                }
            }
        }

        /// <summary>
        /// Populates the page with content passed during navigation. Any saved state is also
        /// provided when recreating a page from a prior session.
        /// </summary>
        /// <param name="navigationParameter">The parameter value passed to
        /// <see cref="Frame.Navigate(Type, Object)"/> when this page was initially requested.
        /// </param>
        /// <param name="pageState">A dictionary of state preserved by this page during an earlier
        /// session.  This will be null the first time a page is visited.</param>
        protected async override void LoadState(Object navigationParameter, Dictionary<String, Object> pageState)
        {
            //if (!this.isEventRegistered)
            //{
            //    SettingsPane.GetForCurrentView().CommandsRequested += onCommandsRequested;
            //    this.isEventRegistered = true;
            //}

            prayerViewModel = navigationParameter as PrayerViewModel;
            if (prayerViewModel == null)
            {
                prayerViewModel = new PrayerViewModel();
                Location location = navigationParameter as Location;
                prayerViewModel.LoadPrayersByLocation(location);
            }
            DataContext = prayerViewModel;


            //PeriodicUpdateRecurrence recurrence = PeriodicUpdateRecurrence.Daily;
            //string urlString = string.Format("http://prayerservice.net23.net/service/prayerservice.php?date={0}&latitude={1}&longitude={2}&timezone={3}&dst={4}&method={5}",
            //    DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
            //    prayerViewModel.Location.Latitude,
            //    prayerViewModel.Location.Longitude,
            //    prayerViewModel.Location.TimeZone,
            //    prayerViewModel.Location.Dst,
            //    prayerViewModel.Method.ToString());
            //TileUpdateManager.CreateTileUpdaterForApplication().StartPeriodicUpdate(new Uri(urlString), recurrence);

            //StorageFolder folder = KnownFolders.DocumentsLibrary;
            //StorageFile file = await folder.GetFileAsync("worldcitiespop.txt");
            //var stream = await file.OpenAsync(FileAccessMode.Read);
            //StreamReader sr = new StreamReader(stream.AsStream());
            //while (sr.ReadLine() != null)
            //{
            //    string line = sr.ReadLine();
            //}


        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);

            // Added to make sure the event handler for CommandsRequested in cleaned up before other scenarios
            //if (this.isEventRegistered)
            //{
            //    SettingsPane.GetForCurrentView().CommandsRequested -= onCommandsRequested;
            //    this.isEventRegistered = false;
            //}
        }

        //void onSettingsCommand(IUICommand command)
        //{
        //    SettingsCommand settingsCommand = (SettingsCommand)command;
        //    this.Frame.Navigate(typeof(PreferencesSettings), prayerViewModel);
        //}

        //void onCommandsRequested(SettingsPane settingsPane, SettingsPaneCommandsRequestedEventArgs eventArgs)
        //{
        //    UICommandInvokedHandler handler = new UICommandInvokedHandler(onSettingsCommand);
        //    var loader = new Windows.ApplicationModel.Resources.ResourceLoader();
        //    SettingsCommand generalCommand = new SettingsCommand("preferencesSettings", loader.GetString("Preferences"), handler);
        //    eventArgs.Request.ApplicationCommands.Add(generalCommand);
        //}

        private async void Button_Click_1(object sender, RoutedEventArgs e)
        {
            // Create a menu and add commands specifying an id value for each instead of a delegate.
            var menu = new PopupMenu();
            var loader = new Windows.ApplicationModel.Resources.ResourceLoader();
            menu.Commands.Add(new UICommand(loader.GetString("DisplayModesToday"), null, 1));
            menu.Commands.Add(new UICommand(loader.GetString("DisplayModesNextSevenDays"), null, 2));
            menu.Commands.Add(new UICommand(loader.GetString("DisplayModesCurrentMonth"), null, 3));

            // We don't want to obscure content, so pass in a rectangle representing the sender of the context menu event.
            // We registered command callbacks; no need to await and handle context menu completion
            try
            {
                var chosenCommand = await menu.ShowForSelectionAsync(GetElementRect((FrameworkElement)sender));
                if (chosenCommand != null)
                {
                    switch ((int)chosenCommand.Id)
                    {
                        case 1: prayerViewModel.DisplayMode = DisplayModes.Today;
                            break;
                        case 2: prayerViewModel.DisplayMode = DisplayModes.NextSevenDays;
                            break;
                        case 3: prayerViewModel.DisplayMode = DisplayModes.CurrentMonth;
                            break;

                    }
                    await prayerViewModel.LoadPrayers(false);
                }
            }
            catch (Exception)
            {

            }
        }

        private Rect GetElementRect(FrameworkElement element)
        {
            GeneralTransform buttonTransform = element.TransformToVisual(null);
            Point point = buttonTransform.TransformPoint(new Point());
            return new Rect(point, new Size(element.ActualWidth, element.ActualHeight));
        }

        private void TopBar_Opened(object sender, object e)
        {
            FavoritesViewModel favoritesViewModel = new FavoritesViewModel(prayerViewModel.Location);
            this.TopBar.DataContext = favoritesViewModel;
            favoritesViewModel.Success = false;
        }

        private void OpenAppBar(object sender, RoutedEventArgs e)
        {
            TopBar.IsOpen = true;
        }

        private void OpenParameters(object sender, RoutedEventArgs e)
        {
            this.OpenSettings();
        }
    }
}