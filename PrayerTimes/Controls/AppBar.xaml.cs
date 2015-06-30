using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using PrayerTimes.View;
using PrayerTimes.ViewModel;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace PrayerTimes
{
    public sealed partial class AppBar : UserControl
    {
        private Page _rootPage;
        FavoritesViewModel favoritesViewModel;

        public AppBar(Page rootPage)
        {
            this.InitializeComponent();
            _rootPage = rootPage;
        }

        Rect GetElementRect(FrameworkElement element)
        {
            GeneralTransform buttonTransform = element.TransformToVisual(null);
            Point point = buttonTransform.TransformPoint(new Point());
            return new Rect(point, new Size(element.ActualWidth, element.ActualHeight));
        }

        private async void ButtonHelp_Click(object sender, RoutedEventArgs e)
        {
            // Create a menu and add commands specifying an id value for each instead of a delegate.
            var menu = new PopupMenu();
            menu.Commands.Add(new UICommand("About", null, 1));
            //menu.Commands.Add(new UICommandSeparator());
            menu.Commands.Add(new UICommand("Add to favorites", null, 2));

            // We don't want to obscure content, so pass in a rectangle representing the sender of the context menu event.
            // We registered command callbacks; no need to await and handle context menu completion
            var chosenCommand = await menu.ShowForSelectionAsync(GetElementRect((FrameworkElement)sender));
            if (chosenCommand != null)
            {
                switch ((int)chosenCommand.Id)
                {
                    case 1:
                        MessageDialog md = new MessageDialog("Prayer Times Copyright 2012");
                        await md.ShowAsync();
                        break;

                    case 2:
                        break;
                }
            }
        }

        private async void ButtonAddToFavorites_Click(object sender, RoutedEventArgs e)
        {
            favoritesViewModel = DataContext as FavoritesViewModel;
            if(!string.IsNullOrEmpty(favoritesViewModel.location.City))
                await favoritesViewModel.AddLocationToFavorites();
        }

        private async void ButtonRemoveFromFavorites_Click(object sender, RoutedEventArgs e)
        {
            favoritesViewModel = DataContext as FavoritesViewModel;
            if (!string.IsNullOrEmpty(favoritesViewModel.location.City))
                await favoritesViewModel.RemoveLocationFromFavorites();
        }

        private void ButtonFavorites_Click(object sender, RoutedEventArgs e)
        {
            //prayerViewModel = DataContext as PrayerViewModel;
            _rootPage.Frame.Navigate(typeof(FavoritesPage));
        }

        private async void ButtonGeolocation_Click(object sender, RoutedEventArgs e)
        {
            PrayerViewModel prayerViewModel = _rootPage.DataContext as PrayerViewModel;
            await prayerViewModel.LoadPrayersWithGeolocation();
        }

        private void GoBack(object sender, RoutedEventArgs e)
        {
            if (_rootPage.Frame != null && _rootPage.Frame.CanGoBack)
                _rootPage.Frame.GoBack();
        }
    }
}
