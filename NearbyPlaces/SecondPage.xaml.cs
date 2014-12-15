using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Devices.Geolocation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Maps;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
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
            PlaceNameTextBlock.Text = (e.Parameter as Places).NameOfPlace + (e.Parameter as Places).PlaceLatitude + (e.Parameter as Places).PlaceLongitude;
            NameOfPlace = (e.Parameter as Places).NameOfPlace;
            lat = (e.Parameter as Places).PlaceLatitude;
            lng = (e.Parameter as Places).PlaceLongitude;
            

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
            MapIcon1.Title =NameOfPlace;
            DisplayPlaceLocationMap.MapElements.Add(MapIcon1);
            DisplayPlaceLocationMap.Center = MapIcon1.Location;
            DisplayPlaceLocationMap.ZoomLevel = 17;
            //DisplayPlaceLocationMap.LandmarksVisible = true;

        }
    }
}
