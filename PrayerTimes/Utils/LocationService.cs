using PrayerTimes.Common;
using PrayerTimes.Model;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using System.Xml;
using Windows.Storage;

namespace PrayerTimes.Utils
{
    public class LocationService
    {
        // http://code.google.com/apis/maps/documentation/geocoding/#ReverseGeocoding
        public async static Task<Tuple<string, string, string>> ReverseGeolocation(double longitude, double latitude)
        {

            if (await Network.IsInternetConnected())
            {
                CultureInfo culture = new CultureInfo("en-US");
                string url = "http://maps.googleapis.com/maps/api/geocode/xml?latlng=" + latitude.ToString("0.#####", culture) + "," + longitude.ToString("0.#####", culture) + "&sensor=false";

                Tuple<double, double, string, string, string> res = await GoogleApi(url);
                return new Tuple<string, string, string>(res.Item3, res.Item4, res.Item5);
            }
            else
                throw new WebException();
        }

        public async static Task<Tuple<double, double, string, string, string>> GetPosition(string queryCity)
        {
            if (await Network.IsInternetConnected())
            {
                string url = "http://maps.googleapis.com/maps/api/geocode/xml?address=" + queryCity + "&sensor=false";

                Tuple<double, double, string, string, string> res = await GoogleApi(url, true);

                return new Tuple<double, double, string, string, string>(res.Item1, res.Item2, res.Item3, res.Item4, res.Item5);

                #region Old Yahoo API
                //double latitude = 0;
                //double longitude = 0;

                //string city = string.Empty;
                //string state = string.Empty;
                //string country = string.Empty;

                //using (XmlReader reader = XmlReader.Create("http://where.yahooapis.com/geocode?q=" + queryCity + "&flags=T&appid=rfBy5l6m", new XmlReaderSettings() { Async = true }))
                //{
                //    while (await reader.ReadAsync())
                //    {
                //        if (reader.IsStartElement())
                //        {
                //            if (reader.Name == "Found")
                //            {
                //                if (reader.ReadContentAsInt() == 0)
                //                    throw new FileNotFoundException();
                //            }
                //            else if (reader.Name == "Error")
                //            {
                //                if (reader.ReadContentAsInt() != 0)
                //                    throw new Exception();
                //            }
                //            else if (reader.Name == "latitude")
                //            {
                //                latitude = reader.ReadElementContentAsDouble();
                //            }
                //            else if (reader.Name == "longitude")
                //            {
                //                longitude = reader.ReadElementContentAsDouble();
                //            }
                //            else if (reader.Name == "city")
                //            {
                //                city = reader.ReadElementContentAsString();
                //            }
                //            else if (reader.Name == "state")
                //            {
                //                state = reader.ReadElementContentAsString();
                //            }
                //            else if (reader.Name == "country")
                //            {
                //                country = reader.ReadElementContentAsString();
                //            }
                //        }
                //    }
                //}

                //Uri url = new Uri("http://where.yahooapis.com/geocode?q=" + queryCity + "&flags=T&appid=rfBy5l6m");
                //XmlDocument xmlDocument = await XmlDocument.LoadFromUriAsync(url);

                //int found = Convert.ToInt32(xmlDocument.SelectSingleNode("//ResultSet/Found").InnerText);
                //int error = Convert.ToInt32(xmlDocument.SelectSingleNode("//ResultSet/Error").InnerText);
                //if (found == 0)
                //    throw new FileNotFoundException();
                //if (error != 0)
                //    throw new Exception();

                //latitude = Convert.ToDouble(xmlDocument.SelectSingleNode("//ResultSet/Result/latitude").InnerText, CultureInfo.InvariantCulture);
                //longitude = Convert.ToDouble(xmlDocument.SelectSingleNode("//ResultSet/Result/longitude").InnerText, CultureInfo.InvariantCulture);

                //city = xmlDocument.SelectSingleNode("//ResultSet/Result/city").InnerText;
                //state = xmlDocument.SelectSingleNode("//ResultSet/Result/state").InnerText;
                //country = xmlDocument.SelectSingleNode("//ResultSet/Result/country").InnerText;

                //return new Tuple<double, double, string, string, string>(latitude, longitude, city, state, country);
                #endregion Old Yahoo API
            }
            else
                throw new WebException();
        }

        private async static Task<Tuple<double, double, string, string, string>> GoogleApi(string url, bool breakAtFirst = false)
        {
            double latitude = 0;
            double longitude = 0;

            string Address_ShortName = "";
            string Address_country = "";
            string Address_administrative_area_level_1 = "";
            string Address_administrative_area_level_2 = "";
            string Address_administrative_area_level_3 = "";
            string Address_colloquial_area = "";
            string Address_locality = "";
            string Address_sublocality = "";
            string Address_neighborhood = "";

            try
            {
                using (XmlReader reader = XmlReader.Create(url, new XmlReaderSettings() { Async = true }))
                {
                    string longname = string.Empty;
                    string shortname = string.Empty;
                    string typename = string.Empty;

                    while (await reader.ReadAsync())
                    {
                        if (breakAtFirst && !string.IsNullOrEmpty(Address_locality) && !string.IsNullOrEmpty(Address_country) && latitude != 0 && longitude != 0)
                            break;

                        if (reader.IsStartElement())
                        {
                            if (reader.Name == "status")
                            {
                                if (reader.ReadElementContentAsString() == "ZERO_RESULTS")
                                {
                                    throw new FileNotFoundException("No data available for the specified location");
                                }
                            }
                            else if (reader.Name == "long_name")
                            {
                                longname = reader.ReadElementContentAsString();
                            }
                            else if (reader.Name == "short_name")
                            {
                                shortname = reader.ReadElementContentAsString();
                            }
                            else if (reader.Name == "type")
                            {
                                typename = reader.ReadElementContentAsString();
                            }
                            else if (reader.Name == "lat")
                            {
                                latitude = reader.ReadElementContentAsDouble();
                            }
                            else if (reader.Name == "lng")
                            {
                                longitude = reader.ReadElementContentAsDouble();
                            }

                            switch (typename)
                            {
                                //Add whatever you are looking for below
                                case "country":
                                    {
                                        Address_country = longname;
                                        Address_ShortName = shortname;
                                        break;
                                    }
                                case "locality":
                                    {
                                        Address_locality = longname;
                                        break;
                                    }
                                case "sublocality":
                                    {
                                        Address_sublocality = longname;
                                        break;
                                    }
                                case "neighborhood":
                                    {
                                        Address_neighborhood = longname;
                                        break;
                                    }
                                case "colloquial_area":
                                    {
                                        Address_colloquial_area = longname;
                                        break;
                                    }
                                case "administrative_area_level_1":
                                    {
                                        Address_administrative_area_level_1 = longname;
                                        break;
                                    }
                                case "administrative_area_level_2":
                                    {
                                        Address_administrative_area_level_2 = longname;
                                        break;
                                    }
                                case "administrative_area_level_3":
                                    {
                                        Address_administrative_area_level_3 = longname;
                                        break;
                                    }
                                default:
                                    break;
                            }
                        }
                    }
                }

                if (string.IsNullOrEmpty(Address_locality)) Address_locality = Address_administrative_area_level_1;
                return new Tuple<double, double, string, string, string>(latitude, longitude, Address_locality, Address_administrative_area_level_1, Address_country);

                #region Old code
                //Uri url = new Uri("http://maps.googleapis.com/maps/api/geocode/xml?latlng=" + latitude.ToString("0.#####", culture) + "," + longitude.ToString("0.#####", culture) + "&sensor=false");
                //XmlDocument xmlDocument = await XmlDocument.LoadFromUriAsync(url);
                //XmlElement element = (XmlElement)xmlDocument.SelectSingleNode("//GeocodeResponse/status");

                //if (element.InnerText == "ZERO_RESULTS")
                //{
                //    throw new Exception("No data available for the specified location");

                //}
                //else
                //{
                //    string longname = "";
                //    string shortname = "";
                //    string typename = "";
                //    XmlNodeList xnList = xmlDocument.SelectNodes("//GeocodeResponse/result/address_component");
                //    foreach (XmlElement xn in xnList)
                //    {
                //        try
                //        {
                //            longname = xn.SelectSingleNode("long_name").InnerText;
                //            shortname = xn.SelectSingleNode("short_name").InnerText;
                //            typename = xn.SelectSingleNode("type").InnerText;

                //            switch (typename)
                //            {
                //                //Add whatever you are looking for below
                //                case "country":
                //                    {
                //                        Address_country = longname;
                //                        Address_ShortName = shortname;
                //                        break;
                //                    }
                //                case "locality":
                //                    {
                //                        Address_locality = longname;
                //                        break;
                //                    }
                //                case "sublocality":
                //                    {
                //                        Address_sublocality = longname;
                //                        break;
                //                    }
                //                case "neighborhood":
                //                    {
                //                        Address_neighborhood = longname;
                //                        break;
                //                    }
                //                case "colloquial_area":
                //                    {
                //                        Address_colloquial_area = longname;
                //                        break;
                //                    }
                //                case "administrative_area_level_1":
                //                    {
                //                        Address_administrative_area_level_1 = longname;
                //                        break;
                //                    }
                //                case "administrative_area_level_2":
                //                    {
                //                        Address_administrative_area_level_2 = longname;
                //                        break;
                //                    }
                //                case "administrative_area_level_3":
                //                    {
                //                        Address_administrative_area_level_3 = longname;
                //                        break;
                //                    }
                //                default:
                //                    break;
                //            }
                //        }
                //        catch (Exception)
                //        {
                //        }
                //    }
                //    return new Tuple<string, string, string>(Address_locality, Address_administrative_area_level_1, Address_country);
                //}
                #endregion Old code
            }
            catch (FileNotFoundException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new Exception("(Address lookup failed: ) " + ex.Message);
            }
        }

        public async static Task<Tuple<double, int, string>> GetTimeInfos(double latitude, double longitude)
        {
            if (await Network.IsInternetConnected())
            {
                CultureInfo culture = new CultureInfo("en-US");

                //Uri url = new Uri("http://www.earthtools.org/timezone-1.1/"
                //    + latitude.ToString("0.#####", culture)
                //    + "/" + longitude.ToString("0.#####", culture));
                //XmlDocument xmlDocument = await XmlDocument.LoadFromUriAsync(url);
                //double timeZone = Convert.ToDouble(xmlDocument.SelectSingleNode("//timezone/offset").InnerText, CultureInfo.InvariantCulture);

                string timeZoneName = string.Empty;
                string tz = string.Empty;

                using (XmlReader reader = XmlReader.Create(string.Format("http://api.geonames.org/timezone?lat={0}&lng={1}&username=peekyou",
                    latitude.ToString("#.#####", culture),
                    longitude.ToString("#.#####", culture)), new XmlReaderSettings() { Async = true }))
                {
                    while (await reader.ReadAsync())
                    {
                        if (reader.IsStartElement())
                        {
                            if (reader.Name == "rawOffset")
                            {
                                tz = reader.ReadElementContentAsString().Replace(".", CultureInfo.CurrentCulture.NumberFormat.CurrencyDecimalSeparator);
                            }
                            else if (reader.Name == "timezoneId")
                            {
                                timeZoneName = reader.ReadElementContentAsString();
                            }
                        }
                    }
                }

                //Uri url = new Uri(string.Format("http://api.geonames.org/timezone?lat={0}&lng={1}&username=peekyou",
                //    latitude.ToString("#.#####", culture),
                //    longitude.ToString("#.#####", culture)));

                //XmlDocument xmlDocument = await XmlDocument.LoadFromUriAsync(url);
                //tz = xmlDocument.SelectSingleNode("//geonames/timezone/rawOffset").InnerText.Replace(".", CultureInfo.CurrentCulture.NumberFormat.CurrencyDecimalSeparator);
                //timeZoneName = xmlDocument.SelectSingleNode("//geonames/timezone/timezoneId").InnerText;

                double timeZone = double.Parse(tz, NumberStyles.Any);
                int dst = 0;

                return new Tuple<double, int, string>(timeZone, dst, timeZoneName);
            }
            else
                throw new WebException();
        }

        public static List<string> GetCountryList()
        {
            List<string> cultureList = new List<string>();
            cultureList.Add("Aruba");
            cultureList.Add("Antigua and Barbuda");
            cultureList.Add("United Arab Emirates");
            cultureList.Add("Afghanistan");
            cultureList.Add("Algeria");
            cultureList.Add("Azerbaijan");
            cultureList.Add("Albania");
            cultureList.Add("Armenia");
            cultureList.Add("Andorra");
            cultureList.Add("Angola");
            cultureList.Add("American Samoa");
            cultureList.Add("Argentina");
            cultureList.Add("Australia");
            cultureList.Add("Ashmore and Cartier Islands");
            cultureList.Add("Austria");
            cultureList.Add("Anguilla");
            cultureList.Add("Åland Islands");
            cultureList.Add("Antarctica");
            cultureList.Add("Bahrain");
            cultureList.Add("Barbados");
            cultureList.Add("Botswana");
            cultureList.Add("Bermuda");
            cultureList.Add("Belgium");
            cultureList.Add("Bahamas, The");
            cultureList.Add("Bangladesh");
            cultureList.Add("Belize");
            cultureList.Add("Bosnia and Herzegovina");
            cultureList.Add("Bolivia");
            cultureList.Add("Myanmar");
            cultureList.Add("Benin");
            cultureList.Add("Belarus");
            cultureList.Add("Solomon Islands");
            cultureList.Add("Navassa Island");
            cultureList.Add("Brazil");
            cultureList.Add("Bassas da India");
            cultureList.Add("Bhutan");
            cultureList.Add("Bulgaria");
            cultureList.Add("Bouvet Island");
            cultureList.Add("Brunei");
            cultureList.Add("Burundi");
            cultureList.Add("Canada");
            cultureList.Add("Cambodia");
            cultureList.Add("Chad");
            cultureList.Add("Sri Lanka");
            cultureList.Add("Congo, Republic of the");
            cultureList.Add("Congo, Democratic Republic of the");
            cultureList.Add("China");
            cultureList.Add("Chile");
            cultureList.Add("Cayman Islands");
            cultureList.Add("Cocos (Keeling) Islands");
            cultureList.Add("Cameroon");
            cultureList.Add("Comoros");
            cultureList.Add("Colombia");
            cultureList.Add("Northern Mariana Islands");
            cultureList.Add("Coral Sea Islands");
            cultureList.Add("Costa Rica");
            cultureList.Add("Central African Republic");
            cultureList.Add("Cuba");
            cultureList.Add("Cape Verde");
            cultureList.Add("Cook Islands");
            cultureList.Add("Cyprus");
            cultureList.Add("Denmark");
            cultureList.Add("Djibouti");
            cultureList.Add("Dominica");
            cultureList.Add("Jarvis Island");
            cultureList.Add("Dominican Republic");
            cultureList.Add("Dhekelia Sovereign Base Area");
            cultureList.Add("Ecuador");
            cultureList.Add("Egypt");
            cultureList.Add("Ireland");
            cultureList.Add("Equatorial Guinea");
            cultureList.Add("Estonia");
            cultureList.Add("Eritrea");
            cultureList.Add("El Salvador");
            cultureList.Add("Ethiopia");
            cultureList.Add("Europa Island");
            cultureList.Add("Czech Republic");
            cultureList.Add("French Guiana");
            cultureList.Add("Finland");
            cultureList.Add("Fiji");
            cultureList.Add("Falkland Islands (Islas Malvinas)");
            cultureList.Add("Micronesia, Federated States of");
            cultureList.Add("Faroe Islands");
            cultureList.Add("French Polynesia");
            cultureList.Add("Baker Island");
            cultureList.Add("France");
            cultureList.Add("French Southern and Antarctic Lands");
            cultureList.Add("Gambia, The");
            cultureList.Add("Gabon");
            cultureList.Add("Georgia");
            cultureList.Add("Ghana");
            cultureList.Add("Gibraltar");
            cultureList.Add("Grenada");
            cultureList.Add("Guernsey");
            cultureList.Add("Greenland");
            cultureList.Add("Germany");
            cultureList.Add("Glorioso Islands");
            cultureList.Add("Guadeloupe");
            cultureList.Add("Guam");
            cultureList.Add("Greece");
            cultureList.Add("Guatemala");
            cultureList.Add("Guinea");
            cultureList.Add("Guyana");
            cultureList.Add("Gaza Strip");
            cultureList.Add("Haiti");
            cultureList.Add("Hong Kong");
            cultureList.Add("Heard Island and McDonald Islands");
            cultureList.Add("Honduras");
            cultureList.Add("Howland Island");
            cultureList.Add("Croatia");
            cultureList.Add("Hungary");
            cultureList.Add("Iceland");
            cultureList.Add("Indonesia");
            cultureList.Add("Isle of Man");
            cultureList.Add("India");
            cultureList.Add("British Indian Ocean Territory");
            cultureList.Add("Clipperton Island");
            cultureList.Add("Iran");
            cultureList.Add("Israel");
            cultureList.Add("Italy");
            cultureList.Add("Cote d'Ivoire");
            cultureList.Add("Iraq");
            cultureList.Add("Japan");
            cultureList.Add("Jersey");
            cultureList.Add("Jamaica");
            cultureList.Add("Jan Mayen");
            cultureList.Add("Jordan");
            cultureList.Add("Johnston Atoll");
            cultureList.Add("Juan de Nova Island");
            cultureList.Add("Kenya");
            cultureList.Add("Kyrgyzstan");
            cultureList.Add("Korea, North");
            cultureList.Add("Kingman Reef");
            cultureList.Add("Kiribati");
            cultureList.Add("Korea, South");
            cultureList.Add("Christmas Island");
            cultureList.Add("Kuwait");
            cultureList.Add("Kosovo");
            cultureList.Add("Kazakhstan");
            cultureList.Add("Laos");
            cultureList.Add("Lebanon");
            cultureList.Add("Latvia");
            cultureList.Add("Lithuania");
            cultureList.Add("Liberia");
            cultureList.Add("Slovakia");
            cultureList.Add("Palmyra Atoll");
            cultureList.Add("Liechtenstein");
            cultureList.Add("Lesotho");
            cultureList.Add("Luxembourg");
            cultureList.Add("Libyan Arab");
            cultureList.Add("Madagascar");
            cultureList.Add("Martinique");
            cultureList.Add("Macau");
            cultureList.Add("Moldova, Republic of");
            cultureList.Add("Mayotte");
            cultureList.Add("Mongolia");
            cultureList.Add("Montserrat");
            cultureList.Add("Malawi");
            cultureList.Add("Montenegro");
            cultureList.Add("The Former Yugoslav Republic of Macedonia");
            cultureList.Add("Mali");
            cultureList.Add("Monaco");
            cultureList.Add("Morocco");
            cultureList.Add("Mauritius");
            cultureList.Add("Midway Islands");
            cultureList.Add("Mauritania");
            cultureList.Add("Malta");
            cultureList.Add("Oman");
            cultureList.Add("Maldives");
            cultureList.Add("Mexico");
            cultureList.Add("Malaysia");
            cultureList.Add("Mozambique");
            cultureList.Add("New Caledonia");
            cultureList.Add("Niue");
            cultureList.Add("Norfolk Island");
            cultureList.Add("Niger");
            cultureList.Add("Vanuatu");
            cultureList.Add("Nigeria");
            cultureList.Add("Netherlands");
            cultureList.Add("No Man's Land");
            cultureList.Add("Norway");
            cultureList.Add("Nepal");
            cultureList.Add("Nauru");
            cultureList.Add("Suriname");
            cultureList.Add("Netherlands Antilles");
            cultureList.Add("Nicaragua");
            cultureList.Add("New Zealand");
            cultureList.Add("Paraguay");
            cultureList.Add("Pitcairn Islands");
            cultureList.Add("Peru");
            cultureList.Add("Paracel Islands");
            cultureList.Add("Spratly Islands");
            cultureList.Add("Pakistan");
            cultureList.Add("Poland");
            cultureList.Add("Panama");
            cultureList.Add("Portugal");
            cultureList.Add("Papua New Guinea");
            cultureList.Add("Palau");
            cultureList.Add("Guinea-Bissau");
            cultureList.Add("Qatar");
            cultureList.Add("Reunion");
            cultureList.Add("Serbia");
            cultureList.Add("Marshall Islands");
            cultureList.Add("Saint Martin");
            cultureList.Add("Romania");
            cultureList.Add("Philippines");
            cultureList.Add("Puerto Rico");
            cultureList.Add("Russia");
            cultureList.Add("Rwanda");
            cultureList.Add("Saudi Arabia");
            cultureList.Add("Saint Pierre and Miquelon");
            cultureList.Add("Saint Kitts and Nevis");
            cultureList.Add("Seychelles");
            cultureList.Add("South Africa");
            cultureList.Add("Senegal");
            cultureList.Add("Saint Helena");
            cultureList.Add("Slovenia");
            cultureList.Add("Sierra Leone");
            cultureList.Add("San Marino");
            cultureList.Add("Singapore");
            cultureList.Add("Somalia");
            cultureList.Add("Spain");
            cultureList.Add("Saint Lucia");
            cultureList.Add("Sudan");
            cultureList.Add("Svalbard");
            cultureList.Add("Sweden");
            cultureList.Add("South Georgia and the Islands");
            cultureList.Add("Syrian Arab Republic");
            cultureList.Add("Switzerland");
            cultureList.Add("Trinidad and Tobago");
            cultureList.Add("Tromelin Island");
            cultureList.Add("Thailand");
            cultureList.Add("Tajikistan");
            cultureList.Add("Turks and Caicos Islands");
            cultureList.Add("Tokelau");
            cultureList.Add("Tonga");
            cultureList.Add("Togo");
            cultureList.Add("Sao Tome and Principe");
            cultureList.Add("Tunisia");
            cultureList.Add("East Timor");
            cultureList.Add("Turkey");
            cultureList.Add("Tuvalu");
            cultureList.Add("Taiwan");
            cultureList.Add("Turkmenistan");
            cultureList.Add("Tanzania, United Republic of");
            cultureList.Add("Uganda");
            cultureList.Add("United Kingdom");
            cultureList.Add("Ukraine");
            cultureList.Add("United States");
            cultureList.Add("Burkina Faso");
            cultureList.Add("Uruguay");
            cultureList.Add("Uzbekistan");
            cultureList.Add("Saint Vincent and the Grenadines");
            cultureList.Add("Venezuela");
            cultureList.Add("British Virgin Islands");
            cultureList.Add("Vietnam");
            cultureList.Add("Virgin Islands (US)");
            cultureList.Add("Holy See (Vatican City)");
            cultureList.Add("Namibia");
            cultureList.Add("West Bank");
            cultureList.Add("Wallis and Futuna");
            cultureList.Add("Western Sahara");
            cultureList.Add("Wake Island");
            cultureList.Add("Samoa");
            cultureList.Add("Swaziland");
            cultureList.Add("Serbia and Montenegro");
            cultureList.Add("Yemen");
            cultureList.Add("Zambia");
            cultureList.Add("Zimbabwe");
            return cultureList;
        }

        public static string GetContinent(string country)
        {
            string continent = "";
            switch (country)
            {
                case "Afghanistan":
                case "Bangladesh":
                case "British Indian Ocean Territory":
                case "Myanmar":
                case "Bhutan":
                case "Brunei":
                case "Cambodia":
                case "Sri Lanka":
                case "China":
                case "East Timor":
                case "Hong Kong":
                case "Indonesia":
                case "India":
                case "Japan":
                case "North Korea":
                case "South Korea":
                case "Kazakhstan":
                case "Kyrgyzstan":
                case "Laos":
                case "Macau":
                case "Maldives":
                case "Mongolia":
                case "Malaysia":
                case "Nepal":
                case "Pakistan":
                case "Paracel Islands":
                case "Philippines":
                case "Singapore":
                case "Spratly Islands":
                case "Thailand":
                case "Tajikistan":
                case "Turkmenistan":
                case "Taiwan":
                case "Uzbekistan":
                case "Vietnam":
                    continent = "Asia";
                    break;

                case "Bahrain":
                case "United Arab Emirates":
                case "Saudi Arabia":
                case "Egypt":
                case "Gaza Strip":
                case "Iran":
                case "Israel":
                case "Iraq":
                case "Jordan":
                case "Kuwait":
                case "Lebanon":
                case "Oman":
                case "Turkey":
                case "Qatar":
                case "Syria":
                case "Yemen":
                case "West Bank":
                case "Palestinian Territories":
                case "Palestine":
                    continent = "Middle East";
                    break;

                case "Åland Islands":
                case "Albania":
                case "Armenia":
                case "Andorra":
                case "Austria":
                case "Azerbaijan":
                case "Belgium":
                case "Bosnia and Herzegovina":
                case "Belarus":
                case "Bulgaria":
                case "Cyprus":
                case "Denmark":
                case "Ireland":
                case "Estonia":
                case "Czech Republic":
                case "Faroe Islands":
                case "Finland":
                case "France":
                case "Georgia":
                case "Germany":
                case "Gibraltar":
                case "Croatia":
                case "Greece":
                case "Guernsey":
                case "Hungary":
                case "Iceland":
                case "Italy":
                case "Isle of Man":
                case "Jersey":
                case "Jan Mayen":
                case "Kosovo":
                case "Latvia":
                case "Lithuania":
                case "Slovakia":
                case "Liechtenstein":
                case "Luxembourg":
                case "Montenegro":
                case "The Former Yugoslav Republic of Macedonia":
                case "Macedonia":
                case "Malta":
                case "Monaco":
                case "Netherlands":
                case "The Netherlands":
                case "Norway":
                case "Poland":
                case "Portugal":
                case "Serbia":
                case "Romania":
                case "San Marino":
                case "Slovenia":
                case "Spain":
                case "Svalbard":
                case "Sweden":
                case "Switzerland":
                case "United Kingdom":
                case "Ukraine":
                case "Vatican City":
                case "Serbia and Montenegro":
                case "Moldova":
                case "Russia":
                    continent = "Europe";
                    break;

                case "Algeria":
                case "Angola":
                case "Bassas da India":
                case "Botswana":
                case "Benin":
                case "Burundi":
                case "Chad":
                case "Congo, Republic of the":
                case "Congo":
                case "Congo, Democratic Republic of the":
                case "Cameroon":
                case "Central African Republic":
                case "Cape Verde":
                case "Comoros":
                case "Eritrea":
                case "Djibouti":
                case "Ethiopia":
                case "The Gambia":
                case "Gambia":
                case "Gabon":
                case "Ghana":
                case "Glorioso Islands":
                case "Guinea":
                case "Guinea-Bissau":
                case "Equatorial Guinea":
                case "Europa Island":
                case "Cote d'Ivoire":
                case "Ivory Coast":
                case "Juan de Nova Island":
                case "Kenya":
                case "Liberia":
                case "Lesotho":
                case "Libya":
                case "Madagascar":
                case "Malawi":
                case "Mayotte":
                case "Morocco":
                case "Mozambique":
                case "Mauritania":
                case "Mauritius":
                case "Niger":
                case "Nigeria":
                case "Rwanda":
                case "Reunion":
                case "Saint Helena":
                case "Saint Helena Ascension and Tristan da Cunha":
                case "St Ascension and Tristan da Cunha":
                case "Sao Tome and Principe":
                case "South Africa":
                case "Senegal":
                case "Seychelles":
                case "Sierra Leone":
                case "Somalia":
                case "Sudan":
                case "Tanzania":
                case "Tunisia":
                case "Togo":
                case "Tromelin Island":
                case "Uganda":
                case "Burkina Faso":
                case "Namibia":
                case "Swaziland":
                case "Mali":
                case "Western Sahara":
                case "Zambia":
                case "Zimbabwe":
                    continent = "Africa";
                    break;

                case "Argentina":
                case "Bolivia":
                case "Brazil":
                case "Chile":
                case "Colombia":
                case "Ecuador":
                case "Falkland Islands":
                case "Falkland Islands (Islas Malvinas)":
                case "French Guiana":
                case "Guyana":
                case "Paraguay":
                case "Peru":
                case "South Georgia and the South Sandwich Islands":
                case "Suriname":
                case "Trinidad and Tobago":
                case "Uruguay":
                case "Venezuela":
                    continent = "South America";
                    break;

                case "Anguilla":
                case "Aruba":
                case "Antigua and Barbuda":
                case "The Bahamas":
                case "Bahamas":
                case "Barbados":
                case "Belize":
                case "British Virgin Islands":
                case "Cayman Islands":
                case "Clipperton Island":
                case "Costa Rica":
                case "Cuba":
                case "Dominica":
                case "Dominican Republic":
                case "El Salvador":
                case "Grenada":
                case "Guadeloupe":
                case "Guatemala":
                case "Haiti":
                case "Honduras":
                case "Jamaica":
                case "Martinique":
                case "Montserrat":
                case "Navassa Island":
                case "Netherlands Antilles":
                case "Nicaragua":
                case "Panama":
                case "Puerto Rico":
                case "Saint Kitts and Nevis":
                case "Saint Lucia":
                case "Saint Martin":
                case "Saint Vincent and the Grenadines":
                case "Turks and Caicos Islands":
                    continent = "Central America";
                    break;

                case "Bermuda":
                case "Canada":
                case "Greenland":
                case "Mexico":
                case "Midway Islands":
                case "Saint Pierre and Miquelon":
                case "United States":
                case "US Virgin Islands":
                    continent = "North America";
                    break;

                case "Australia":
                case "Fiji":
                case "Guam":
                case "New Caledonia":
                case "New Zealand":
                case "Papua New Guinea":
                case "Tuvalu":
                case "Samoa":
                case "American Samoa":
                case "Ashmore and Cartier Islands":
                case "Solomon Islands":
                case "Cocos (Keeling) Islands":
                case "Coral Sea Islands":
                case "Northern Mariana Islands":
                case "Cook Islands":
                case "Jarvis Island":
                case "Federated States of Micronesia":
                case "Micronesia":
                case "French Polynesia":
                case "Baker Island":
                case "Howland Island":
                case "Johnston Atoll":
                case "Kiribati":
                case "Christmas Island":
                case "Palmyra Atoll":
                case "Niue":
                case "Norfolk Island":
                case "Vanuatu":
                case "Nauru":
                case "Pitcairn Islands":
                case "Palau":
                case "Marshall Islands":
                case "Tokelau":
                case "Tonga":
                case "Wallis and Futuna":
                case "Wake Island":
                    continent = "Oceania";
                    break;

                case "Antarctica":
                case "French Southern and Antarctic Lands":
                    continent = "Antarctica";
                    break;

                default:
                    break;
            }
            return continent;
        }

        public static DateTime GetLastWeekdayOfMonth(DateTime date, DayOfWeek day)
        {
            DateTime lastDayOfMonth = new DateTime(date.Year, date.Month, 1).AddMonths(1).AddDays(-1);
            int wantedDay = (int)day;
            int lastDay = (int)lastDayOfMonth.DayOfWeek;
            return lastDayOfMonth.AddDays(
                lastDay >= wantedDay ? wantedDay - lastDay : wantedDay - lastDay - 7);
        }

        public static DateTime GetWeekdayOfMonthByPosition(DateTime date, DayOfWeek day, int position)
        {
            DateTime firstDayOfMonth = new DateTime(date.Year, date.Month, 1);
            int wantedDay = (int)day;
            int firstDay = (int)firstDayOfMonth.DayOfWeek;
            int positionToAdd = firstDay >= wantedDay ? 7 - firstDay + wantedDay : wantedDay - firstDay;
            positionToAdd = positionToAdd + ((position - 1) * 7);
            firstDayOfMonth = firstDayOfMonth.AddDays(positionToAdd);
            return firstDayOfMonth;
        }

        public static int GetTimezoneDST(TimeZoneInfo timeZoneInfo, DateTime? currentDate = null)
        {
            DateTime date = DateTime.Now;
            if (currentDate != null)
                date = (DateTime)currentDate;

            if (date.Hour < 5)
                date.AddHours(5);

            if (timeZoneInfo.IsDaylightSavingTime(date))
                return 1;
            else
                return 0;
        }

        public static int GetDSTByRegion(string country, string region, string city, double timeZone, DateTime currentTime)
        {
            int dst = 0;
            DateTime april = new DateTime(currentTime.Year, 4, 1);
            DateTime september = new DateTime(currentTime.Year, 9, 1);
            DateTime march = new DateTime(currentTime.Year, 3, 1);
            DateTime october = new DateTime(currentTime.Year, 10, 1);
            DateTime november = new DateTime(currentTime.Year, 11, 1);
            DateTime january = new DateTime(currentTime.Year, 1, 1);
            DateTime february = new DateTime(currentTime.Year, 2, 1);
            switch (country)
            {
                // Asia
                case "Afghanistan":
                case "Armenia":
                case "Bangladesh":
                case "Bahrain":
                case "Bhutan":
                case "British Indian Ocean Territory":
                case "Brunei":
                case "Cambodia":
                case "China":
                case "East Timor":
                case "Georgia":
                case "Hong Kong":
                case "India":
                case "Indonesia":
                case "Iraq":
                case "Japan":
                case "Kazakhstan":
                case "Kyrgyzstan":
                case "Kuwait":
                case "Laos":
                case "Macau":
                case "Maldives":
                case "Malaysia":
                case "Mongolia":
                case "Myanmar":
                case "Nepal":
                case "North Korea":
                case "Oman":
                case "Pakistan":
                case "Papua New Guinea":
                case "Paracel Islands":
                case "Philippines":
                case "Qatar":
                case "Saudi Arabia":
                case "Singapore":
                case "South Korea":
                case "Spratly Islands":
                case "Sri Lanka":
                case "Thailand":
                case "Tajikistan":
                case "Taiwan":
                case "Turkmenistan":
                case "United Arab Emirates":
                case "Uzbekistan":
                case "Vietnam":
                case "Yemen":
                // Africa
                case "Algeria":
                case "Angola":
                case "Bassas da India":
                case "Botswana":
                case "Benin":
                case "Burkina Faso":
                case "Burundi":
                case "Cameroon":
                case "Cape Verde":
                case "Central African Republic":
                case "Chad":
                case "Comoros":
                case "Congo":
                case "Democratic Republic of Congo":
                case "Djibouti":
                case "Egypt":
                case "Equatorial Guinea":
                case "Eritrea":
                case "Ethiopia":
                case "Europa Island":
                case "The Gambia":
                case "Gambia":
                case "Gabon":
                case "Ghana":
                case "Glorioso Islands":
                case "Guinea":
                case "Guinea-Bissau":
                case "Cote d'Ivoire":
                case "Ivory Coast":
                case "Juan de Nova Island":
                case "Kenya":
                case "Liberia":
                case "Lesotho":
                case "Libya":
                case "Madagascar":
                case "Malawi":
                case "Mali":
                case "Mauritania":
                case "Mauritius":
                case "Mayotte":
                case "Mozambique":
                case "Niger":
                case "Nigeria":
                case "Republic of the Congo":
                case "Rwanda":
                case "Saint Helena":
                case "Saint Helena Ascension and Tristan da Cunha":
                case "St Ascension and Tristan da Cunha":
                case "Sao Tome and Principe":
                case "Senegal":
                case "Seychelles":
                case "Sierra Leone":
                case "Somalia":
                case "South Africa":
                case "South Sudan":
                case "Sudan":
                case "Swaziland":
                case "Tanzania":
                case "Tromelin Island":
                case "Tunisia":
                case "Togo":
                case "Uganda":
                case "Zambia":
                case "Zimbabwe":
                // Americas
                case "Anguilla":
                case "Aruba":
                case "Antigua and Barbuda":
                case "Argentina":
                case "Barbados":
                case "Bolivia":
                case "Belize":
                case "British Virgin Islands":
                case "Cayman Islands":
                case "Clipperton Island":
                case "Costa Rica":
                case "Colombia":
                case "Dominica":
                case "Dominican Republic":
                case "Ecuador":
                case "El Salvador":
                case "Falkland Islands":
                case "Falkland Islands (Islas Malvinas)":
                case "French Guiana":
                case "Grenada":
                case "Guadeloupe":
                case "Guatemala":
                case "Guyana":
                case "Honduras":
                case "Jamaica":
                case "Martinique":
                case "Midway Islands":
                case "Montserrat":
                case "Navassa Island":
                case "Netherlands Antilles":
                case "Nicaragua":
                case "Panama":
                case "Puerto Rico":
                case "Peru":
                case "Saint Kitts and Nevis":
                case "Saint Lucia":
                case "Saint Martin":
                case "Saint Vincent and the Grenadines":
                case "South Georgia and the South Sandwich Islands":
                case "Suriname":
                case "Trinidad and Tobago":
                case "US Virgin Islands":
                case "Venezuela":
                // Europe
                case "Belarus":
                case "Iceland":
                case "Russia":
                // Oceania
                case "Guam":
                case "New Caledonia":
                case "Tuvalu":
                case "American Samoa":
                case "Ashmore and Cartier Islands":
                case "Solomon Islands":
                case "Cocos (Keeling) Islands":
                case "Coral Sea Islands":
                case "Northern Mariana Islands":
                case "Cook Islands":
                case "Jarvis Island":
                case "Federated States of Micronesia":
                case "Micronesia":
                case "French Polynesia":
                case "Baker Island":
                case "Howland Island":
                case "Johnston Atoll":
                case "Kiribati":
                case "Christmas Island":
                case "Palmyra Atoll":
                case "Niue":
                case "Norfolk Island":
                case "Vanuatu":
                case "Nauru":
                case "Pitcairn Islands":
                case "Palau":
                case "Marshall Islands":
                case "Tokelau":
                case "Tonga":
                case "Wallis and Futuna":
                case "Wake Island":
                    dst = 0;
                    break;

                // Europe
                case "Åland Islands":
                case "Albania":
                case "Andorra":
                case "Austria":
                case "Belgium":
                case "Bosnia and Herzegovina":
                case "Bulgaria":
                case "Croatia":
                case "Cyprus":
                case "Czech Republic":
                case "Denmark":
                case "Estonia":
                case "Faroe Islands":
                case "Finland":
                case "France":
                case "Germany":
                case "Gibraltar":
                case "Greece":
                case "Guernsey":
                case "Hungary":
                case "Italy":
                case "Ireland":
                case "Isle of Man":
                case "Jersey":
                case "Jan Mayen":
                case "Kosovo":
                case "Latvia":
                case "Lithuania":
                case "Liechtenstein":
                case "Luxembourg":
                case "The Former Yugoslav Republic of Macedonia":
                case "Macedonia":
                case "Malta":
                case "Moldova":
                case "Monaco":
                case "Montenegro":
                case "Netherlands":
                case "The Netherlands":
                case "Norway":
                case "Poland":
                case "Portugal":
                case "Romania":
                case "San Marino":
                case "Serbia":
                case "Slovakia":
                case "Slovenia":
                case "Spain":
                case "Svalbard":
                case "Sweden":
                case "Switzerland":
                case "United Kingdom":
                case "Ukraine":
                case "Vatican City":
                case "Serbia and Montenegro":
                // Asia
                case "Azerbaijan":
                case "Lebanon":
                case "Turkey":
                    DateTime c = currentTime.ToUniversalTime();
                    string s = c.ToString();
                    int a = c.Hour;
                    DateTime lastSundayInMarch = GetLastWeekdayOfMonth(march, DayOfWeek.Sunday);//.AddHours(1);
                    DateTime lastSundayInOctober = GetLastWeekdayOfMonth(october, DayOfWeek.Sunday);//.AddHours(1);
                    c = lastSundayInOctober.ToUniversalTime();
                    if (currentTime.Ticks >= lastSundayInMarch.Ticks && currentTime.Ticks < lastSundayInOctober.Ticks)
                        dst = 1;
                    break;

                case "Morocco":
                case "Western Sahara":
                    DateTime lastSundayInApril = GetLastWeekdayOfMonth(april, DayOfWeek.Sunday);//.AddHours(2); // 2 a.m.
                    DateTime lastSundayInSeptember = GetLastWeekdayOfMonth(september, DayOfWeek.Sunday);//.AddHours(2);
                    if (currentTime.Ticks >= lastSundayInApril.Ticks && currentTime.Ticks < lastSundayInSeptember.Ticks)
                        dst = 1;
                    break;

                case "Namibia":
                    DateTime firstSundayInSeptember = GetWeekdayOfMonthByPosition(september, DayOfWeek.Sunday, 1);//.AddHours(-1);
                    DateTime firstSundayInApril = GetWeekdayOfMonthByPosition(april, DayOfWeek.Sunday, 1);//.AddHours(-2);
                    if (currentTime.Ticks < firstSundayInApril.Ticks || currentTime.Ticks >= firstSundayInSeptember.Ticks)
                        dst = 1;
                    break;

                case "Iran":
                    DateTime IranStart;
                    DateTime IranStop;
                    if (DateTime.IsLeapYear(currentTime.Year))
                    {
                        IranStart = new DateTime(currentTime.Year, 3, 21);//.AddHours(-3.5);
                        IranStop = new DateTime(currentTime.Year, 9, 21);//.AddHours(-4.5);
                    }
                    else
                    {
                        IranStart = new DateTime(currentTime.Year, 3, 22);//.AddHours(-3.5);
                        IranStop = new DateTime(currentTime.Year, 9, 22);//.AddHours(-4.5);
                    }
                    if (currentTime.Ticks >= IranStart.Ticks && currentTime.Ticks < IranStop.Ticks)
                        dst = 1;
                    break;

                case "Israel":
                    throw new NotSupportedException("Israel is not supported");
                    break;

                case "Brazil":
                    //much of Mato Grosso
                    switch (region)
                    {
                        case "Amazonas":
                        case "Pernambuco":
                        case "Sergipe":
                        case "Para":
                        case "Paraíba":
                        case "Ceará":
                        case "Amapá":
                        case "Alagoas":
                        case "Rondônia":
                        case "Rio Grande do Norte":
                        case "Piauí":
                        case "Maranhão":
                        case "Acre":
                        case "Roraima":
                        case "Tocantins":
                            dst = 0;
                            break;

                        default:
                            DateTime thirdSundayInOctober = GetWeekdayOfMonthByPosition(october, DayOfWeek.Sunday, 3);//.AddHours(timeZone * -1);
                            DateTime thirdSundayInFebruary = GetWeekdayOfMonthByPosition(february, DayOfWeek.Sunday, 3);//.AddHours((timeZone * -1) - 1);
                            if (february.Year == 2015
                                || february.Year == 2023
                                || february.Year == 2026
                                || february.Year == 2034
                                || february.Year == 2037)
                            {
                                thirdSundayInFebruary = thirdSundayInFebruary.AddDays(7);
                            }
                            if (currentTime.Ticks < thirdSundayInFebruary.Ticks || currentTime.Ticks >= thirdSundayInOctober.Ticks)
                                dst = 1;
                            break;
                    }
                    break;

                case "Chile":
                    // TODO : Easter Island
                    if (currentTime.Year == 2013)
                    {
                        DateTime end2013 = new DateTime(2013, 04, 28);//.AddHours((timeZone * -1) - 1);
                        DateTime start2013 = new DateTime(2013, 09, 08);//.AddHours(timeZone * -1);
                        if (currentTime.Ticks < end2013.Ticks || currentTime.Ticks >= start2013.Ticks)
                            dst = 1;
                    }
                    else
                    {
                        DateTime secondSundayInOctober = GetWeekdayOfMonthByPosition(october, DayOfWeek.Sunday, 2);//.AddHours(timeZone * -1);
                        DateTime secondSundayInMarch = GetWeekdayOfMonthByPosition(march, DayOfWeek.Sunday, 2);//.AddHours((timeZone * -1) - 1);
                        if (currentTime.Ticks < secondSundayInMarch.Ticks || currentTime.Ticks >= secondSundayInOctober.Ticks)
                            dst = 1;
                    }
                    break;

                case "Paraguay":
                    if (currentTime.Year == 2013)
                    {
                        DateTime end2013 = new DateTime(2013, 03, 24);
                        DateTime start2013 = new DateTime(2013, 10, 06);
                        if (currentTime.Ticks < end2013.Ticks || currentTime.Ticks >= start2013.Ticks)
                            dst = 1;
                    }
                    else
                    {
                        DateTime firstSundayInOctober = GetWeekdayOfMonthByPosition(october, DayOfWeek.Sunday, 1);//.AddHours(timeZone * -1);
                        DateTime secondSundayInApril = GetWeekdayOfMonthByPosition(april, DayOfWeek.Sunday, 2);//.AddHours((timeZone * -1) - 1);
                        if (currentTime.Ticks < secondSundayInApril.Ticks || currentTime.Ticks >= firstSundayInOctober.Ticks)
                            dst = 1;
                    }
                    break;

                case "Uruguay":
                    DateTime firstSundayInOctoberUruguay = GetWeekdayOfMonthByPosition(october, DayOfWeek.Sunday, 1);//.AddHours(timeZone * -1);
                    DateTime secondSundayInMarchUruguay = GetWeekdayOfMonthByPosition(march, DayOfWeek.Sunday, 2);//.AddHours((timeZone * -1) - 1);
                    if (currentTime.Ticks < secondSundayInMarchUruguay.Ticks || currentTime.Ticks >= firstSundayInOctoberUruguay.Ticks)
                        dst = 1;
                    break;

                case "Cuba":
                    DateTime secondSundayInMarchCuba = GetWeekdayOfMonthByPosition(march, DayOfWeek.Sunday, 2);//.AddHours(timeZone * -1);
                    DateTime firstSundayInNovemberCuba = GetWeekdayOfMonthByPosition(november, DayOfWeek.Sunday, 1);//.AddHours((timeZone * -1) - 1);
                    if (currentTime.Ticks >= secondSundayInMarchCuba.Ticks && currentTime.Ticks < firstSundayInNovemberCuba.Ticks)
                        dst = 1;
                    break;

                case "Bermuda":
                    DateTime secondSundayInMarchBermuda = GetWeekdayOfMonthByPosition(march, DayOfWeek.Sunday, 2);//.AddHours(timeZone * -1);
                    DateTime firstSundayInNovemberBermuda = GetWeekdayOfMonthByPosition(november, DayOfWeek.Sunday, 1);//.AddHours((timeZone * -1) - 1);
                    if (currentTime.Ticks >= secondSundayInMarchBermuda.Ticks && currentTime.Ticks < firstSundayInNovemberBermuda.Ticks)
                        dst = 1;
                    break;

                case "Canada":
                    bool isDST;
                    switch (region)
                    {
                        case "British Columbia":
                            if (city == "Chetwynd"
                                || city == "Dawson Creek"
                                || city == "Hudson's Hope"
                                || city == "Fort St John"
                                || city == "Taylor"
                                || city == "Tumbler Ridge"
                                || city == "Creston")
                            {
                                isDST = false;
                            }
                            else
                            {
                                isDST = true;
                            }
                            break;

                        case "Nunavut":
                            if (city == "Coral Harbour")
                                isDST = false;
                            else
                                isDST = true;
                            break;

                        case "Ontario":
                            if (city == "Pickle Lake" || city == "New Osnaburgh" || city == "Atikokan")
                                isDST = false;
                            else
                                isDST = true;
                            break;

                        case "Quebec":
                            // TODO partie du Quebec sans DST
                            isDST = true;
                            break;

                        case "Saskatchewan":
                            if (city == "Denare Beach" || city == "Creighton")
                                isDST = true;
                            else
                                isDST = false;
                            break;

                        default: isDST = true;
                            break;
                    }

                    if (isDST)
                    {
                        DateTime secondSundayInMarchNA = GetWeekdayOfMonthByPosition(march, DayOfWeek.Sunday, 2);//.AddHours((timeZone * -1) + 2);
                        DateTime firstSundayInNovemberNA = GetWeekdayOfMonthByPosition(november, DayOfWeek.Sunday, 1);//.AddHours((timeZone * -1) + 2 - 1);
                        if (currentTime.Ticks >= secondSundayInMarchNA.Ticks && currentTime.Ticks < firstSundayInNovemberNA.Ticks)
                            dst = 1;
                    }
                    else
                        dst = 0;
                    break;

                case "Mexico":
                    DateTime firstSundayInAprilMexico = GetWeekdayOfMonthByPosition(april, DayOfWeek.Sunday, 1);//.AddHours((timeZone * -1) + 2);
                    DateTime lastSundayInOctoberMexico = GetLastWeekdayOfMonth(october, DayOfWeek.Sunday);//.AddHours((timeZone * -1) + 2 - 1);

                    if (region == "Sonora")
                        dst = 0;
                    else if (region == "Baja California" || region == "Tamaulipas" || region == "Chihuahua" || region == "Coahuila" || region == "Nuevo León")
                    {
                        if (city == "Matamoros"
                            || city == "Reynosa"
                            || city == "Nuevo Laredo"
                            || city == "Anáhuac"
                            || city == "Acuña"
                            || city == "Piedras Negras"
                            || city == "Mexicali"
                            || city == "Tijuana"
                            || city == "Ojinaga"
                            || city == "Juárez")
                        {
                            DateTime secondSundayInMarchNA = GetWeekdayOfMonthByPosition(march, DayOfWeek.Sunday, 2);//.AddHours((timeZone * -1) + 2);
                            DateTime firstSundayInNovemberNA = GetWeekdayOfMonthByPosition(november, DayOfWeek.Sunday, 1);//.AddHours((timeZone * -1) + 2 - 1);
                            if (currentTime.Ticks >= secondSundayInMarchNA.Ticks && currentTime.Ticks < firstSundayInNovemberNA.Ticks)
                                dst = 1;
                        }
                        else
                        {
                            if (currentTime.Ticks >= firstSundayInAprilMexico.Ticks && currentTime.Ticks < lastSundayInOctoberMexico.Ticks)
                                dst = 1;
                        }
                    }
                    else
                    {
                        if (currentTime.Ticks >= firstSundayInAprilMexico.Ticks && currentTime.Ticks < lastSundayInOctoberMexico.Ticks)
                            dst = 1;
                    }
                    break;

                case "United States":
                case "Bahamas":
                case "The Bahamas":
                case "Haiti":
                case "Saint Pierre and Miquelon":
                case "Turks and Caicos Islands":
                    if (region == "Arizona" || region == "Hawaii" || region == "Puerto Rico")
                        dst = 0;
                    else
                    {
                        DateTime secondSundayInMarchNA = GetWeekdayOfMonthByPosition(march, DayOfWeek.Sunday, 2);//.AddHours((timeZone * -1) + 2);
                        DateTime firstSundayInNovemberNA = GetWeekdayOfMonthByPosition(november, DayOfWeek.Sunday, 1);//.AddHours((timeZone * -1) + 2 - 1);
                        if (currentTime.Ticks >= secondSundayInMarchNA.Ticks && currentTime.Ticks < firstSundayInNovemberNA.Ticks)
                            dst = 1;
                    }
                    break;

                case "Australia":
                    switch (region)
                    {
                        case "New South Wales":
                        case "Victoria":
                        case "Tasmania":
                        case "Australian Capital Territory":
                        case "South Australia":
                            DateTime firstSundayInOctoberAustralia = GetWeekdayOfMonthByPosition(october, DayOfWeek.Sunday, 1);//.AddHours(timeZone * -1);
                            DateTime firstSundayInAprilAustralia = GetWeekdayOfMonthByPosition(april, DayOfWeek.Sunday, 1);//.AddHours((timeZone * -1) - 1);
                            if (currentTime.Ticks < firstSundayInAprilAustralia.Ticks || currentTime.Ticks >= firstSundayInOctoberAustralia.Ticks)
                                dst = 1;
                            break;

                        default: dst = 0;
                            break;
                    }
                    break;

                case "Fiji":
                    if (currentTime.Year == 2013)
                    {
                        DateTime fourthSundayInOctober = GetWeekdayOfMonthByPosition(october, DayOfWeek.Sunday, 4);//.AddHours(timeZone * -1);
                        DateTime thirdSundayInJanuary = GetWeekdayOfMonthByPosition(january, DayOfWeek.Sunday, 3);//.AddHours((timeZone * -1) - 1);
                        if (currentTime.Ticks < thirdSundayInJanuary.Ticks || currentTime.Ticks >= fourthSundayInOctober.Ticks)
                            dst = 1;
                    }
                    else
                    {
                        DateTime fourthSundayInOctober = GetWeekdayOfMonthByPosition(october, DayOfWeek.Sunday, 4);//.AddHours(timeZone * -1);
                        DateTime fourthSundayInJanuary = GetWeekdayOfMonthByPosition(january, DayOfWeek.Sunday, 4);//.AddHours((timeZone * -1) - 1);
                        if (currentTime.Ticks < fourthSundayInJanuary.Ticks || currentTime.Ticks >= fourthSundayInOctober.Ticks)
                            dst = 1;
                    }
                    break;

                case "New Zealand":
                    DateTime lastSundayInSeptemberNZ = GetLastWeekdayOfMonth(september, DayOfWeek.Sunday);//.AddHours(-10); // -12 NZST +2 a.m. 
                    DateTime firstSundayInAprilNZ = GetWeekdayOfMonthByPosition(april, DayOfWeek.Sunday, 1);//.AddHours(-11);
                    if (currentTime.Ticks < firstSundayInAprilNZ.Ticks || currentTime.Ticks >= lastSundayInSeptemberNZ.Ticks)
                        dst = 1;
                    break;

                case "Samoa":
                    DateTime lastSundayInSeptemberSamoa = GetLastWeekdayOfMonth(september, DayOfWeek.Sunday);//.AddHours(-13); // -13 UTC
                    DateTime firstSundayInAprilSamoa = GetWeekdayOfMonthByPosition(april, DayOfWeek.Sunday, 1);//.AddHours(-14);
                    if (currentTime.Ticks < lastSundayInSeptemberSamoa.Ticks || currentTime.Ticks >= lastSundayInSeptemberSamoa.Ticks)
                        dst = 1;
                    break;

                case "Gaza Strip":
                    DateTime lastFridayInMarch = GetLastWeekdayOfMonth(march, DayOfWeek.Friday);//.AddHours(timeZone * -1);
                    DateTime lastFridayInSeptemberGaza = GetLastWeekdayOfMonth(september, DayOfWeek.Friday);//.AddHours((timeZone * -1) - 1);
                    if (currentTime.Ticks >= lastFridayInMarch.Ticks && currentTime.Ticks < lastFridayInSeptemberGaza.Ticks)
                        dst = 1;
                    break;

                case "Jordan":
                    DateTime lastFridayInMarchJordan = GetLastWeekdayOfMonth(march, DayOfWeek.Friday);//.AddHours(-2);
                    DateTime lastFridayInOctoberJordan = GetLastWeekdayOfMonth(october, DayOfWeek.Friday);//.AddHours(-3);
                    if (currentTime.Ticks >= lastFridayInMarchJordan.Ticks && currentTime.Ticks < lastFridayInOctoberJordan.Ticks)
                        dst = 1;
                    break;

                case "Syria":
                    if (currentTime.Year == 2013)
                    {
                        DateTime start2013 = new DateTime(2013, 03, 29);
                        DateTime end2013 = new DateTime(2013, 11, 01);
                        if (currentTime.Ticks >= start2013.Ticks && currentTime.Ticks < end2013.Ticks)
                            dst = 1;
                    }
                    else
                    {
                        DateTime lastFridayInMarchSyria = GetLastWeekdayOfMonth(march, DayOfWeek.Friday);//.AddHours(-2);
                        DateTime lastFridayInOctoberSyria = GetLastWeekdayOfMonth(october, DayOfWeek.Friday);//.AddHours(-3);
                        if (currentTime.Ticks >= lastFridayInMarchSyria.Ticks && currentTime.Ticks < lastFridayInOctoberSyria.Ticks)
                            dst = 1;
                    }
                    break;

                case "West Bank":
                case "Palestinian Territories":
                case "Palestine":
                    DateTime lastFridayInMarchPalestine = GetLastWeekdayOfMonth(march, DayOfWeek.Friday);//.AddHours(-2);
                    DateTime lastFridayInSeptember = GetLastWeekdayOfMonth(september, DayOfWeek.Friday);//.AddHours(-3);
                    if (currentTime.Ticks >= lastFridayInMarchPalestine.Ticks && currentTime.Ticks < lastFridayInSeptember.Ticks)
                        dst = 1;
                    break;

                case "Greenland":
                    throw new NotSupportedException("Greenland is not supported");
                    break;

                case "Antarctica":
                    throw new NotSupportedException("Antarctica is not supported");
                    break;

                default:
                    break;
            }
            return dst;
        }

        public static bool IsCachedLocationExpired(Location location)
        {
            int dst = GetDSTByRegion(location.Country, location.State, location.City, location.TimeZone, DateTime.Now);
            return dst != location.Dst;
        }

        public async static Task<List<Location>> GetLocationsFromCache()
        {
            List<Location> locations = new List<Location>();
            Windows.Storage.StorageFolder folder = Windows.Storage.ApplicationData.Current.LocalFolder;
            StorageFile file = null;
            try
            {
                file = await folder.GetFileAsync(Filenames.Locations);
                IList<string> text = await FileIO.ReadLinesAsync(file);
                foreach (string item in text)
                {
                    string[] line = item.Split(':');
                    Location location = new Location(line[0], line[1], line[2], Convert.ToDouble(line[3], CultureInfo.InvariantCulture), Convert.ToDouble(line[4], CultureInfo.InvariantCulture), Convert.ToDouble(line[5], CultureInfo.InvariantCulture), Convert.ToInt32(line[6]), line[7]);

                    if (!locations.Contains(location))
                        locations.Add(location);
                }
            }
            catch (FileNotFoundException)
            {
                folder.CreateFileAsync(Filenames.Locations, CreationCollisionOption.ReplaceExisting);
            }

            //var stream = await file.OpenAsync(FileAccessMode.Read);
            //StreamReader sr = new StreamReader(stream.AsStream());
            //while (sr.ReadLine() != null)
            //{
            //    string[] line = sr.ReadLine().Split(':');
            //    Location location = new Location(line[0], Convert.ToDouble(line[1]), Convert.ToDouble(line[2]));
            //    if(!locations.Contains(location))
            //        locations.Add(location);
            //}
            return locations;
        }

        public async static Task CacheLocation(string city, string state, string country, double latitude, double longitude, double timeZone, int dst, string timezoneName)
        {
            try
            {
                CultureInfo culture = new CultureInfo("en-US");
                Windows.Storage.StorageFolder folder = Windows.Storage.ApplicationData.Current.LocalFolder;
                StorageFile file = null;
                file = await folder.CreateFileAsync(Filenames.Locations, CreationCollisionOption.OpenIfExists);
                await FileIO.AppendTextAsync(file, String.Format("{0}:{1}:{2}:{3}:{4}:{5}:{6}:{7}\n", city, state, country, latitude.ToString("0.#####", culture), longitude.ToString("0.#####", culture), timeZone.ToString("0.#", culture), dst, timezoneName));
            }
            catch (Exception)
            {
            }
        }

        public async static Task AddLocationToFavorites(string city, string state, string country, double latitude, double longitude, double timeZone, int dst, string timezoneName)
        {
            try
            {
                CultureInfo culture = new CultureInfo("en-US");
                Windows.Storage.StorageFolder folder = Windows.Storage.ApplicationData.Current.LocalFolder;
                StorageFile file = null;
                file = await folder.CreateFileAsync(Filenames.Favorites, CreationCollisionOption.OpenIfExists);
                await FileIO.AppendTextAsync(file, String.Format("{0}:{1}:{2}:{3}:{4}:{5}:{6}:{7}\n", city, state, country, latitude.ToString("0.#####", culture), longitude.ToString("0.#####", culture), timeZone.ToString("0.#", culture), dst, timezoneName));
            }
            catch (Exception)
            {
            }
        }

        public async static void DeleteFile(string fileName)
        {
            Windows.Storage.StorageFolder folder = Windows.Storage.ApplicationData.Current.LocalFolder;
            StorageFile file = null;
            file = await folder.GetFileAsync(fileName);
            if (file != null)
                await file.DeleteAsync();
        }

        public async static Task RemoveLocationFromFile(string city, string state, string country, string fileName)
        {
            try
            {
                Windows.Storage.StorageFolder folder = Windows.Storage.ApplicationData.Current.LocalFolder;
                StorageFile file, filecopy = null;
                file = await folder.CreateFileAsync(Filenames.Favorites, CreationCollisionOption.OpenIfExists);
                await file.RenameAsync(fileName + "2");
                filecopy = await folder.CreateFileAsync(Filenames.Favorites, CreationCollisionOption.OpenIfExists);
                //Stream writer = await folder.OpenStreamForWriteAsync(Filenames.Favorites, CreationCollisionOption.OpenIfExists);

                var streamReader = await file.OpenAsync(FileAccessMode.Read);
                var streamWriter = await filecopy.OpenAsync(FileAccessMode.ReadWrite);
                string search = String.Format("{0}:{1}:{2}", city, state, country);
                string line = null;
                using (StreamReader reader = new StreamReader(streamReader.AsStreamForRead()))
                {
                    using (StreamWriter writer = new StreamWriter(streamWriter.AsStreamForWrite()))
                    {
                        while ((line = reader.ReadLine()) != null)
                        {
                            if (line.Contains(search))
                                continue;
                            writer.WriteLine(line);
                        }
                    }
                }
                await file.DeleteAsync();
            }
            catch (Exception)
            {
            }
        }

        public async static Task<List<Location>> GetFavoriteLocations()
        {
            List<Location> locations = new List<Location>();
            Windows.Storage.StorageFolder folder = Windows.Storage.ApplicationData.Current.LocalFolder;
            StorageFile file = null;
            try
            {
                file = await folder.GetFileAsync(Filenames.Favorites);
                IList<string> text = await FileIO.ReadLinesAsync(file);
                foreach (string item in text)
                {
                    string[] line = item.Split(':');
                    Location location = new Location(line[0], line[1], line[2], Convert.ToDouble(line[3], CultureInfo.InvariantCulture), Convert.ToDouble(line[4], CultureInfo.InvariantCulture), Convert.ToDouble(line[5], CultureInfo.InvariantCulture), Convert.ToInt32(line[6]), line[7]);
                    if (!locations.Contains(location))
                        locations.Add(location);
                }
            }
            catch (Exception)
            {
            }
            return locations;
        }
    }
}
