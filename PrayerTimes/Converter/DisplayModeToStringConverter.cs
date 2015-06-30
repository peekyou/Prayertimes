using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PrayerTimes.Common;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;

namespace PrayerTimes.Converter
{
    class DisplayModeToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var loader = new Windows.ApplicationModel.Resources.ResourceLoader();
            DisplayModes displayMode = (DisplayModes)value;
            switch (displayMode)
            {
                case DisplayModes.Today:
                    return loader.GetString("DisplayModesToday");
                case DisplayModes.NextSevenDays:
                    return loader.GetString("DisplayModesNextSevenDays");
                case DisplayModes.CurrentMonth:
                    return loader.GetString("DisplayModesCurrentMonth");
            }
            return "";
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return null;
        }
    }
}
