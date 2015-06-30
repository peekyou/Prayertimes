using PrayerTimes.Models;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace PrayerTimes.Common.Selectors
{
    public class PrayerTemplateSelector : DataTemplateSelector
    {
        public DataTemplate PrayerTemplate { get; set; }
        public DataTemplate PassedPrayerTemplate { get; set; }
        public DataTemplate CurrentPrayerTemplate { get; set; }
        public DataTemplate TileTemplate { get; set; }

        protected override DataTemplate SelectTemplateCore(object item, Windows.UI.Xaml.DependencyObject container)
        {
            if (item != null && item.GetType() == typeof(Prayer) && ((Prayer)item).IsCurrent)
                return CurrentPrayerTemplate;
            else if (item != null && item.GetType() == typeof(Prayer) && ((Prayer)item).IsPassed)
                return PassedPrayerTemplate;
            else
                return PrayerTemplate;
        }
    }
}
