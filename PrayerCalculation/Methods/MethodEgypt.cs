using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace PrayerCalculation.Methods
{
    public class MethodEgypt : MethodBase
    {
        public MethodEgypt()
            : base(MethodNames.Egypt)
        {
            parameters = new Dictionary(8);
            parameters.Add(TimeNames.Fajr, 19.5);
            parameters.Add(TimeNames.Isha, 17.5);
        }

        public override string ToString()
        {
            return "MethodEgypt";
        }
    }
}
