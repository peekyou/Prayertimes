using System;
using System.ComponentModel;
using Windows.UI;
using Windows.UI.Xaml.Media;

namespace PrayerTimes.Models
{
    public class Prayer : INotifyPropertyChanged
    {
        private string name;
        public string Name
        {
            get { return name; }
            set { name = value; OnPropertyChanged("Name"); }
        }

        private DateTime dateTime;
        public DateTime DateTime
        {
            get { return dateTime; }
            set
            {
                dateTime = value;
                OnPropertyChanged("DateTime");
            }
        }

        private string hour;
        public string Hour
        {
            get
            {
                return dateTime.ToString("t");
            }
            set
            {
                hour = value;
                double val;
                double.TryParse(hour.Split(':')[0], out val);
                dateTime = dateTime.AddHours(val);
                double.TryParse(hour.Split(':')[1], out val);
                dateTime = dateTime.AddMinutes(val);
                OnPropertyChanged("Hour");
                OnPropertyChanged("IsPassed");
                OnPropertyChanged("IsCurrent");
                OnPropertyChanged("IsToday");
            }
        }

        public bool IsToday
        {
            get
            {
                return DateTime.Now.Day == dateTime.Day && DateTime.Now.Month == dateTime.Month && DateTime.Now.Year == dateTime.Year;
            }
        }

        public SolidColorBrush ColorState
        {
            get
            {
                if (IsCurrent)
                    return new SolidColorBrush(Colors.Orange);
                else if (IsPassed)
                    return new SolidColorBrush(Colors.Gray);
                else
                    return new SolidColorBrush(Colors.White);
            }
        }

        public bool IsPassed
        {
            get
            {
                return dateTime.CompareTo(DateTime.Now) < 0;
            }
        }

        public bool IsCurrent
        {
            get
            {
                if (NextPrayer == null && IsPassed)
                    return true;
                else
                    return Name.Equals("Shuruq") ? false : IsPassed && NextPrayer.DateTime.CompareTo(DateTime.Now) > 0;
            }
        }

        public Prayer NextPrayer { get; set; }

        public void TimeChanged()
        {
            OnPropertyChanged("IsPassed");
            OnPropertyChanged("IsCurrent");
            OnPropertyChanged("IsToday");
            OnPropertyChanged("ColorState");
        }

        #region INotifyPropertyChanged
        /// <summary>
        /// Raised when a property on this object has a new value.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Raises this object's PropertyChanged event.
        /// </summary>
        /// <param name="propertyName">The name of the property that has a new value.</param>
        protected virtual void OnPropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        #endregion INotifyPropertyChanged
    }
}
