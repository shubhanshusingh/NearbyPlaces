using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Geolocation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage.Streams;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Maps;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkID=390556

namespace NearbyPlaces
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class SecondPage : Page
    {
        public string NameOfPlace;
        public double lat;
        public double lng;
        public string photoRef;
        public SecondPage()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.
        /// This parameter is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            fetchImageProgress.IsActive = false;
            PlaceNameTextBlock.Text = (e.Parameter as Places).NameOfPlace;
            if ((e.Parameter as Places).PhotoReference != null)
            {
                photoRef = (e.Parameter as Places).PhotoReference;
                ImageInfo.Text = "PHOTO: Fetching Image,Please Wait";
                fetchImageProgress.IsActive = true;
                Task getImageFromHttpClient = new Task(OnGetImageFromHttpClient);
                getImageFromHttpClient.Start();
                Debug.WriteLine("got image client task start");
            }
            else
            {
                ImageInfo.Text = "PHOTO NOT AVAILABLE.";
            }


            NameOfPlace = (e.Parameter as Places).NameOfPlace;
            lat = (e.Parameter as Places).PlaceLatitude;
            lng = (e.Parameter as Places).PlaceLongitude;


        }

        async private void OnGetImageFromHttpClient()
        {
            HttpClient clientToGetImage = new HttpClient();
            StringBuilder stringUri = new StringBuilder();
            stringUri.Append("https://maps.googleapis.com/maps/api/place/photo?maxwidth=400&photoreference=");
            stringUri.Append(photoRef);
            stringUri.Append("&key=AIzaSyAsb6JTOXn-p2dwf5qiQyUreo8y_lybpTo");
            Uri imageUri = new Uri(stringUri.ToString());
            byte[] imageByteArray = await clientToGetImage.GetByteArrayAsync(imageUri);
            Debug.WriteLine("got image array");
          await Dispatcher.RunAsync( Windows.UI.Core.CoreDispatcherPriority.High, async() =>
          {
              //WriteableBitmap wBmp = BitmapFactory.New(400, 800).FromByteArray(imageByteArray);
              BitmapImage img = new BitmapImage();
              //img.DecodePixelWidth = 250;
              //img.DecodePixelHeight = 250;
              using(MemoryStream stream = new MemoryStream(imageByteArray))
              {
                  Debug.WriteLine("using memorystream");
                  using (IRandomAccessStream rStream = stream.AsRandomAccessStream())
                  {
                     await img.SetSourceAsync(rStream);
                     Debug.WriteLine("inside random");
                  }
             
             
              PlaceImageView.Source = img;
              }
              fetchImageProgress.IsActive = false;
              ImageInfo.Text = "PHOTO";
          });

          Debug.WriteLine("Dispatcher Exit");
        }

        private void DisplayPlaceLocationMap_Loaded(object sender, RoutedEventArgs e)
        {
            MapIcon MapIcon1 = new MapIcon();
            MapIcon1.Location = new Geopoint(new BasicGeoposition()
            {
                Latitude = lat,
                Longitude = lng
            });
            MapIcon1.NormalizedAnchorPoint = new Point(0.5, 1.0);
            MapIcon1.Title = NameOfPlace;
            DisplayPlaceLocationMap.MapElements.Add(MapIcon1);
            DisplayPlaceLocationMap.Center = MapIcon1.Location;
            DisplayPlaceLocationMap.ZoomLevel = 17;
            DisplayPlaceLocationMap.LandmarksVisible = true;

        }
    }
}
