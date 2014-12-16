using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.Media;
using Windows.UI.Xaml.Media.Imaging;
using System.Net.Http;
using System.Net;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Text;
using Windows.Devices.Geolocation;
using Windows.UI.Popups;
using System.Diagnostics;
using System.Device.Location;



// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=391641

namespace NearbyPlaces
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public static string UserLocationLatitude;
        public static string UserLocationLongitude;
        public static string JsonResults;
        public Dictionary<string, Places> PlacesDictionary;
        public MainPage()
        {
            this.InitializeComponent();

            this.NavigationCacheMode = NavigationCacheMode.Required;
        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.
        /// This parameter is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            //App.Locator.PositionChanged += Locator_PositionChanged;
            fetchNearbyProgress.IsActive = true;
            UserLocationBlock.Text = "Fetching User Location & Nearby Places";

            Task getUserLocationAndNearbyPlaces = new Task(OnGetUserLocationAndNearbyPlacesStart);
            getUserLocationAndNearbyPlaces.Start();

            

            // TODO: Prepare page for display here.

            // TODO: If your application contains multiple pages, ensure that you are
            // handling the hardware Back button by registering for the
            // Windows.Phone.UI.Input.HardwareButtons.BackPressed event.
            // If you are using the NavigationHelper provided by some templates,
            // this event is handled for you.
        }

        //void Locator_PositionChanged(Geolocator sender, PositionChangedEventArgs args)
        //{
        //    Task getUserLocationAndNearbyPlaces = new Task(OnGetUserLocationAndNearbyPlacesStart);
        //    getUserLocationAndNearbyPlaces.Start();

        //    //throw new NotImplementedException();
        //}
        public static double ConvertToRadians(double angle)
        {
            return (Math.PI / 180) * angle;
        }
        public static double DistanceTo(double latitude1,
                                double longitude1,
                                double latitude2,
                                double longitude2)
        {
            // The radius of the earth in Km.
            // You could also use a better estimation of the radius of the earth
            // using decimals digits, but you have to change then the int to double.
            int R = 6371;

            double f1 = ConvertToRadians(latitude1);
            double f2 = ConvertToRadians(latitude2);

            double df = ConvertToRadians(latitude1 - latitude2);
            double dl = ConvertToRadians(longitude1 - longitude2);

            double a = Math.Sin(df / 2) * Math.Sin(df / 2) +
            Math.Cos(f1) * Math.Cos(f2) *
            Math.Sin(dl / 2) * Math.Sin(dl / 2);

            double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));

            // Calculate the distance.
            double d = R * c;

            return d;
        }
        async private void OnGetUserLocationAndNearbyPlacesStart()
        {
            PlacesDictionary = new Dictionary<string, Places>();
            PlacesDictionary.Clear();
            Geoposition GeoPos = await App.Locator.GetGeopositionAsync();
            //GeoCoordinate GeoCoord = new GeoCoordinate(GeoPos.Coordinate.Point.Position.Latitude,GeoPos.Coordinate.Point.Position.Longitude);

            HttpClient clientToFetchNearbyPlaces = new HttpClient();
            StringBuilder stringUri = new StringBuilder();
            stringUri.Append("https://maps.googleapis.com/maps/api/place/nearbysearch/json?location=");
            stringUri.Append(GeoPos.Coordinate.Point.Position.Latitude.ToString());
            stringUri.Append(",");
            stringUri.Append(GeoPos.Coordinate.Point.Position.Longitude.ToString());
            stringUri.Append("&radius=500&key=AIzaSyAsb6JTOXn-p2dwf5qiQyUreo8y_lybpTo");

            Uri rUri = new Uri(stringUri.ToString());

            //try
            //{
            JsonResults = await clientToFetchNearbyPlaces.GetStringAsync(rUri);
            // Debug.WriteLine(JsonResults);
            //}
            //catch (Exception ex)
            //{
            //    Debug.WriteLine(ex.Message);

            //}

            JObject test = JObject.Parse(JsonResults);
            JArray placesArray = JArray.Parse(test["results"].ToString());

            // var placesArray =   from p in test["results"]
            //                     select p;
            //JObject results =(JObject)test["results"];
            List<Places> listOfPlaces = new List<Places>();
            UserLocationLatitude = GeoPos.Coordinate.Point.Position.Latitude.ToString();
            UserLocationLongitude = GeoPos.Coordinate.Point.Position.Longitude.ToString();


            //UserLocationBlock.Text = UserLocationLatitude + UserLocationLongitude;


            foreach (var place in placesArray)
            {
                Places nearbyPlace = new Places();
                nearbyPlace.PlaceID = place.SelectToken("place_id").ToString();
                nearbyPlace.NameOfPlace = place.SelectToken("name").ToString();
                nearbyPlace.AddressOfThePlace = place.SelectToken("vicinity").ToString();

                try
                {
                    JArray testPhotosArray = JArray.Parse(place.SelectToken("photos").ToString());
                    Debug.WriteLine(testPhotosArray.ToString());
                    foreach (var photo in testPhotosArray)
                    {
                        Debug.WriteLine(photo.SelectToken("photo_reference").ToString());
                        string photoref = photo.SelectToken("photo_reference").ToString();
                        if (photoref != null)
                        {
                            nearbyPlace.PhotoReference = photoref;
                        }
                        else
                        { nearbyPlace.PhotoReference = "PhotoNotAvailable"; }

                    }

                }
                catch
                {
                }

                IList<string> placeTypes = place.SelectToken("types").Select(t => (string)t).ToList();
                StringBuilder typesOfPlaceString = new StringBuilder();

                foreach (var s in placeTypes)
                {
                    typesOfPlaceString.Append("| ");
                    typesOfPlaceString.Append(s);
                    typesOfPlaceString.Append(" |");
                }
                nearbyPlace.TypesOfThePlaces = typesOfPlaceString.ToString();
                double placeLat = Convert.ToDouble(place.SelectToken("geometry").SelectToken("location").SelectToken("lat").ToString());
                nearbyPlace.PlaceLatitude = placeLat;
                double placeLong = Convert.ToDouble(place.SelectToken("geometry").SelectToken("location").SelectToken("lng").ToString());
                nearbyPlace.PlaceLongitude = placeLong;
                double distance = DistanceTo(Convert.ToDouble(UserLocationLatitude), Convert.ToDouble(UserLocationLongitude), placeLat, placeLong);
                nearbyPlace.DistanceFromCurrentLocation = Math.Round(distance, 2).ToString() + " KM";
                PlacesDictionary.Add(nearbyPlace.PlaceID, nearbyPlace);

                listOfPlaces.Add(nearbyPlace);


            }
            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.High, () =>
            {
                UserLocationBlock.Text = GeoPos.Coordinate.Point.Position.Latitude.ToString() + ", " + GeoPos.Coordinate.Point.Position.Longitude.ToString();
                // CollectionOfPlaces.Source = listOfPlaces;
                ListViewOfNearbyPlaces.ItemsSource = listOfPlaces;
                UserLocationBlock.Text = "Nearby Places";
                fetchNearbyProgress.IsActive = false;
            });
        }



        private void StackPanel_Tapped(object sender, TappedRoutedEventArgs e)
        {
            string xyz = (sender as StackPanel).Tag.ToString();
            Places p = new Places();
            PlacesDictionary.TryGetValue(xyz, out p);
            this.Frame.Navigate(typeof(SecondPage), p);
        }
    }
}
