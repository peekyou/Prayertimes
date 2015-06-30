using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PrayerTimes.Models;

namespace PrayerTimes
{
    public class Group : INotifyPropertyChanged
    {
        private string title;
        public string Title
        {
            get { return title; }
            set { title = value; OnPropertyChanged("Title"); }
        }

        private ObservableCollection<Prayer> items;
        public ObservableCollection<Prayer> Items
        {
            get { return items; }
            set { items = value; OnPropertyChanged("Items"); }
        }

        public Group()
        {
            if (Items == null)
                Items = new ObservableCollection<Prayer>();
        }

        public void Add(Prayer item)
        {
            if (Items == null)
                Items = new ObservableCollection<Prayer>();
            Items.Add(item);
        }

        public void Clear()
        {
            if (Items != null)
                Items.Clear();
        }

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
    }
}
