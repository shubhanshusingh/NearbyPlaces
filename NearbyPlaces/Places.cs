using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NearbyPlaces
{
    public class Places : INotifyPropertyChanged
    {
        private Double placeLatitude;

        public Double PlaceLatitude
        {
            get { return placeLatitude; }
            set
            {
                placeLatitude = value;
                NotifyPropertyChanged("PlaceLatitude");
            }
        }
        private Double placeLongitude;

        public Double PlaceLongitude
        {
            get { return placeLongitude; }
            set
            {
                placeLongitude = value;
                NotifyPropertyChanged("PlaceLongitude");
            }
        }


        private string placeId;

        public string PlaceID
        {
            get { return placeId; }
            set
            {
                placeId = value;
                NotifyPropertyChanged("PlaceID");
            }
        }

        private string nameOfPlace;

        public string NameOfPlace
        {
            get { return nameOfPlace; }
            set
            {
                nameOfPlace = value;
                NotifyPropertyChanged("NameOfPlace");
            }
        }

        private string typesOfThePlace;

        public string TypesOfThePlaces
        {
            get { return typesOfThePlace; }
            set
            {
                typesOfThePlace = value;
                NotifyPropertyChanged("TypesOfThePlaces");
            }
        }

        private string distanceFromCurrentLocation;

        public string DistanceFromCurrentLocation
        {
            get { return distanceFromCurrentLocation; }
            set
            {
                distanceFromCurrentLocation = value;
                NotifyPropertyChanged("DistanceFromCurrentLocation");
            }
        }

        private string addressOfThePlace;

        public string AddressOfThePlace
        {
            get { return addressOfThePlace; }
            set
            {
                addressOfThePlace = value;
                NotifyPropertyChanged("AddressOfThePlace");
            }
        }

        private string photoReference;

        public string PhotoReference
        {
            get { return photoReference; }
            set
            {
                photoReference = value;
                NotifyPropertyChanged("PhotoReference");
            }
        }



        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
