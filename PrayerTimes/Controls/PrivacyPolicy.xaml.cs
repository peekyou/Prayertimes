using Windows.UI.ApplicationSettings;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;

// Pour en savoir plus sur le modèle d'élément Contrôle utilisateur, consultez la page http://go.microsoft.com/fwlink/?LinkId=234236

namespace PrayerTimes.Controls
{
    public sealed partial class PrivacyPolicy : UserControl
    {
        public PrivacyPolicy()
        {
            this.InitializeComponent();
            SetPrivacyPolicy();
        }

        private async void PrivacyPolicyBackClicked(object sender, RoutedEventArgs e)
        {
            if (this.Parent.GetType() == typeof(Popup))
            {
                ((Popup)this.Parent).IsOpen = false;
            }
            SettingsPane.Show();
        }

        private void SetPrivacyPolicy()
        {
            Windows.ApplicationModel.Resources.ResourceLoader loader = new Windows.ApplicationModel.Resources.ResourceLoader();

            privacyPolicyTitle.Text = loader.GetString("PrivacyPolicyTitle");
            privacyPolicy.Text = loader.GetString("PrivacyPolicyText");

            personalDataTitle.Text = loader.GetString("PersonalDataTitle");
            personalData.Text = loader.GetString("PersonalDataText");

            personalInformationTitle.Text = loader.GetString("PersonalInformationTitle");
            personalInformation.Text = loader.GetString("PersonalInformationText");

            policyChangesTitle.Text = loader.GetString("PolicyChangesTitle");
            policyChanges.Text = loader.GetString("PolicyChangesText");
        }
    }
}
