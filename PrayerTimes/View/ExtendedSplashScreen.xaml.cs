using System;
using Windows.ApplicationModel.Activation;
using Windows.Foundation;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace PrayerTimes.View
{

    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ExtendedSplashScreen : Page
    {
        private Rect splashImageCoordinates; // Rect to store splash screen image coordinates.
        private SplashScreen splash; // Variable to hold the splash screen object.
        private bool dismissed = false; // Variable to track splash screen dismissal status.

        public ExtendedSplashScreen(SplashScreen splashScreen, bool dismissed)
        {
            this.InitializeComponent();
            this.splashImageCoordinates = splashScreen.ImageLocation;
            this.splash = splashScreen;
            this.dismissed = dismissed;

            // Position the extended splash screen image in the same location as the splash screen image.
            PositionImage();

            Window.Current.SizeChanged += new WindowSizeChangedEventHandler(ExtendedSplash_OnResize);

        }

        void PositionImage()
        {
            extendedSplashImage.SetValue(Canvas.LeftProperty, splashImageCoordinates.X);
            extendedSplashImage.SetValue(Canvas.TopProperty, splashImageCoordinates.Y);
            extendedSplashImage.Height = splashImageCoordinates.Height;
            extendedSplashImage.Width = splashImageCoordinates.Width;
        }

        void ExtendedSplash_OnResize(Object sender, WindowSizeChangedEventArgs e)
        {
            // Safely update the extended splash screen image coordinates. This function will be fired in response to snapping, unsnapping, rotation, etc...
            if (splash != null)
            {
                // Update the coordinates of the splash screen image.
                splashImageCoordinates = splash.ImageLocation;
                PositionImage();
            }
        }

        /// <summary>
        /// Event handler for dismissed event to know when the Splash screen is dismissed
        /// </summary>
        /// <param name="sender">SplashScreen</param>
        internal void DismissedEventHandler(Windows.ApplicationModel.Activation.SplashScreen sender, object e)
        {
            this.dismissed = true;
        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.  The Parameter
        /// property is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
        }
    }
}
