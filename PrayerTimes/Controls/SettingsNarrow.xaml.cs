using PrayerTimes.Common;
using PrayerTimes.ViewModel;
using System.Threading.Tasks;
using Windows.System;
using Windows.UI.ApplicationSettings;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;

// Pour en savoir plus sur le modèle d'élément Contrôle utilisateur, consultez la page http://go.microsoft.com/fwlink/?LinkId=234236

namespace PrayerTimes.Controls
{
    public sealed partial class SettingsNarrow : UserControl
    {
        Windows.Storage.ApplicationDataContainer localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
        PrayerViewModel prayerViewModel;

        public SettingsNarrow(PrayerViewModel context)
        {
            this.InitializeComponent();
            DataContext = prayerViewModel = context;

            Windows.ApplicationModel.Resources.ResourceLoader loader = new Windows.ApplicationModel.Resources.ResourceLoader();

            if (prayerViewModel == null)
                prayerViewModel = new PrayerViewModel();

            CalculationMethodSettingsString.Text = loader.GetString("CalculationMethodSettingsString");
            AsrMethodSettingsString.Text = loader.GetString("AsrMethodSettingsString");
            MidnightMethodSettingsString.Text = loader.GetString("MidnightMethodSettingsString");
            MaghribAdjustementSettingsString.Text = loader.GetString("MaghribAdjustementSettingsString");
        }

        private async void MySettingsBackClicked(object sender, RoutedEventArgs e)
        {
            await SaveSettings();

            if (this.Parent.GetType() == typeof(Popup))
            {
                ((Popup)this.Parent).IsOpen = false;
            }
            SettingsPane.Show();
        }

        private void MaghribAdjustmentTextBox_KeyDown(object sender, Windows.UI.Xaml.Input.KeyRoutedEventArgs e)
        {
            if (e.Key != VirtualKey.Number0 && e.Key != VirtualKey.Number1 && e.Key != VirtualKey.Number2 && e.Key != VirtualKey.Number3 &&
               e.Key != VirtualKey.Number4 && e.Key != VirtualKey.Number5 && e.Key != VirtualKey.Number6 &&
               e.Key != VirtualKey.Number7 && e.Key != VirtualKey.Number8 && e.Key != VirtualKey.Number9 &&
               e.Key != VirtualKey.NumberPad0 && e.Key != VirtualKey.NumberPad1 && e.Key != VirtualKey.NumberPad2 &&
               e.Key != VirtualKey.NumberPad3 && e.Key != VirtualKey.NumberPad4 && e.Key != VirtualKey.NumberPad5 &&
               e.Key != VirtualKey.NumberPad6 && e.Key != VirtualKey.NumberPad7 && e.Key != VirtualKey.NumberPad8 &&
               e.Key != VirtualKey.NumberPad9 && e.Key != VirtualKey.Back && e.Key != VirtualKey.Enter)
            {
                e.Handled = true;
            }
        }

        public async Task SaveSettings()
        {
            localSettings.Values["method"] = prayerViewModel.MethodName = MethodComboBox.SelectedItem.ToString();
            localSettings.Values["asrMethod"] = prayerViewModel.AsrMethod = AsrMethodComboBox.SelectedItem.ToString();
            localSettings.Values["midnightMethod"] = prayerViewModel.MidnightMethod = MidnightMethodComboBox.SelectedItem.ToString();

            int maghribAdj;
            if (int.TryParse(MaghribAdjustmentTextBox.Text, out maghribAdj))
                prayerViewModel.MaghribAdjustement = maghribAdj;
            else
                prayerViewModel.MaghribAdjustement = DefaultValues.MaghribAdjustment;
            localSettings.Values["maghribAdjustement"] = MaghribAdjustmentTextBox.Text;

            await prayerViewModel.LoadPrayers(false);
            prayerViewModel.UpdateTile();
        }
    }
}
