﻿using System;
using System.Globalization;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace PrayerTimes.Converter
{
    class StringFormatConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            string formatString = parameter as string;
            if (!string.IsNullOrEmpty(formatString))
            {
                if (!string.IsNullOrEmpty(language))
                    return string.Format(new CultureInfo(language), formatString, value);
                else
                    return string.Format(formatString, value);
            }
            return value.ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return DependencyProperty.UnsetValue;
        }
    }
}
