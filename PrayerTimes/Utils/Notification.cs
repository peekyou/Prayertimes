using PrayerCalculation;
using PrayerTimes.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using Windows.Data.Xml.Dom;
using Windows.UI.Notifications;

namespace PrayerTimes.Utils
{
    public class Notification
    {
        private static readonly Windows.ApplicationModel.Resources.ResourceLoader loader = new Windows.ApplicationModel.Resources.ResourceLoader();

        public async static void UpdateTile(Location location, string method, string asrMethod, string midnightMethod)
        {
            string lang = "en";
            CultureInfo currentCulture = CultureInfo.CurrentCulture;
            if (currentCulture.Name == "fr-FR")
                lang = "fr";

            DateTime date = DateTime.Now;
            int dst = LocationService.GetDSTByRegion(location.Country, location.State, location.City, location.TimeZone, date);
            CultureInfo culture = new CultureInfo("en-US");
            DateTime expires = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 23, 59, 59).ToUniversalTime();
            string urlString = string.Format("http://islamine.com/services/prayertimes?date={0}&latitude={1}&longitude={2}&timezone={3}&dst={4}&method={5}&city={6}&timezonename={7}&lang={8}&asr={9}&midnight={10}",
                date.ToString("yyyy-MM-dd HH:mm:ss"),
                location.Latitude.ToString("0.#####", culture),
                location.Longitude.ToString("0.#####", culture),
                location.TimeZone.ToString("0.#", culture),
                dst,
                method,
                location.City,
                location.TimezoneName.Replace('/', '-'),
                lang,
                asrMethod,
                midnightMethod);

            PeriodicUpdateRecurrence recurrence = PeriodicUpdateRecurrence.Hour;
            DateTime tomorrow = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 0, 0, 1).AddDays(1);
            DateTimeOffset startTime = new DateTimeOffset(tomorrow);
            TileUpdateManager.CreateTileUpdaterForApplication().StartPeriodicUpdate(new Uri(urlString), recurrence);
        }

        public async static Task PlanToastNotifications(CancellationToken token, PrayerTimeCalculation prayerCalculation, Location location)
        {
            if (!token.IsCancellationRequested)
            {
                await Task.Factory.StartNew(() =>
                {
                    DoToastNotificationPlanning(ref token, prayerCalculation, location);
                }, token);
            }
        }

        private static void DoToastNotificationPlanning(ref CancellationToken token, PrayerTimeCalculation prayerCalculation, Location location)
        {
            // Plan notifications for 680 days
            int days = 680;
            DateTime now = DateTime.Now;
            DateTime currentDate = new DateTime(now.Year, now.Month, now.Day);
            DateTime date;
            XmlDocument toastXml = ToastNotificationManager.GetTemplateContent(ToastTemplateType.ToastText02);
            ToastNotifier notifier = ToastNotificationManager.CreateToastNotifier();

            // Remove all scheduled notifications
            IEnumerable<ScheduledToastNotification> scheduledNotifications = notifier.GetScheduledToastNotifications();
            foreach (ScheduledToastNotification scheduledNotification in scheduledNotifications)
            {
                if (token.IsCancellationRequested)
                {
                    return;
                }
                notifier.RemoveFromSchedule(scheduledNotification);
            }

            for (int i = 0; i < days; i++)
            {
                date = currentDate.AddDays(i);
                string day = date.ToString("ddd d MMM");

                TimeFormat format = TimeFormat.Format24h;
                int dst = LocationService.GetDSTByRegion(location.Country, location.State, location.City, location.TimeZone, date);
                IDictionary prayers = prayerCalculation.GetTimes(date, location.Latitude, location.Longitude, location.TimeZone, dst, format);

                foreach (DictionaryEntry prayer in prayers)
                {
                    if (token.IsCancellationRequested)
                    {
                        return;
                    }

                    if (prayer.Key != "Imsak" && prayer.Key != "Sunset" && prayer.Key != "Midnight")
                    {
                        string prayerName = prayer.Key.ToString();
                        if (prayer.Key == "Sunrise")
                        {
                            prayerName = "Shuruq";
                        }

                        XmlNodeList stringElements = toastXml.GetElementsByTagName("text");
                        string label = prayerName == "Shuruq" ? loader.GetString("ToastNotificationShuruqText") : loader.GetString("ToastNotificationText");
                        if (!stringElements.Item(0).HasChildNodes())
                        {
                            stringElements.Item(0).AppendChild(toastXml.CreateTextNode(label));
                        }
                        else
                        {
                            stringElements.Item(0).ReplaceChild(toastXml.CreateTextNode(label), stringElements.Item(0).FirstChild);
                        }

                        string prayerText = prayerName + " " + prayer.Value.ToString();
                        if (!stringElements.Item(1).HasChildNodes())
                        {
                            stringElements.Item(1).AppendChild(toastXml.CreateTextNode(prayerText));
                        }
                        else
                        {
                            stringElements.Item(1).ReplaceChild(toastXml.CreateTextNode(prayerText), stringElements.Item(1).FirstChild);
                        }

                        string[] split = prayer.Value.ToString().Split(':');
                        DateTime prayerTime = date.AddHours(Convert.ToDouble(split[0])).AddMinutes(Convert.ToDouble(split[1]));

                        if (prayerTime > DateTime.Now)
                        {
                            DateTimeOffset displayTime = new DateTimeOffset(prayerTime);
                            ScheduledToastNotification scheduledToast = new ScheduledToastNotification(toastXml, displayTime);
                            scheduledToast.Id = prayerName + " " + i;
                            notifier.AddToSchedule(scheduledToast);
                        }
                    }
                }
            }
        }
    }
}
