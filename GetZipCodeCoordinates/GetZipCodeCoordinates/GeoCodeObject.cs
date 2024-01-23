using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GetZipCodeCoordinates
{
    public class GeoCodeObject
    {
        private string _type;
        public string Type
        {
            get { return _type; }
            set { _type = value; }
        }

        private string _fullAddress;
        public string FullAddress
        {
            get { return _fullAddress; }
            set { _fullAddress = value; }
        }

        private int _zipCode;
        public int ZipCode
        {
            get { return _zipCode; }
            set { _zipCode = value; }
        }

        private string _city;
        public string City
        {
            get { return _city; }
            set { _city = value; }
        }

        private string _county;
        public string County
        {
            get { return _county; }
            set { _county = value; }
        }

        private string _stateLong;
        public string StateLong
        {
            get { return _stateLong; }
            set { _stateLong = value; }
        }

        private string _stateShort;
        public string StateShort
        {
            get { return _stateShort; }
            set { _stateShort = value; }
        }        

        private double _lattitude;
        public double Lattitude
        {
            get { return _lattitude; }
            set { _lattitude = value; }
        }

        private double _longitude;
        public double Longitude
        {
            get { return _longitude; }
            set { _longitude = value; }
        }

        private string _locationType;
        public string LocationType
        {
            get { return _locationType; }
            set { _locationType = value; }
        }

        private double _viewportSouthWestLattitude;
        public double ViewportSouthWestLattitude
        {
            get { return _viewportSouthWestLattitude; }
            set { _viewportSouthWestLattitude = value; }
        }

        private double _viewportSouthWestLongitude;
        public double ViewportSouthWestLongitude
        {
            get { return _viewportSouthWestLongitude; }
            set { _viewportSouthWestLongitude = value; }
        }

        private double _viewportNorthEastLattitude;
        public double ViewportNorthEastLattitude
        {
            get { return _viewportNorthEastLattitude; }
            set { _viewportNorthEastLattitude = value; }
        }

        private double _viewportNorthEastLongitude;
        public double ViewportNorthEastLongitude
        {
            get { return _viewportNorthEastLongitude; }
            set { _viewportNorthEastLongitude = value; }
        }

        private double _boundsSouthWestLattitude;
        public double BoundsSouthWestLattitude
        {
            get { return _boundsSouthWestLattitude; }
            set { _boundsSouthWestLattitude = value; }
        }

        private double _boundsSouthWestLongitude;
        public double BoundsSouthWestLongitude
        {
            get { return _boundsSouthWestLongitude; }
            set { _boundsSouthWestLongitude = value; }
        }

        private double _boundsNorthEastLattitude;
        public double BoundsNorthEastLattitude
        {
            get { return _boundsNorthEastLattitude; }
            set { _boundsNorthEastLattitude = value; }
        }

        private double _boundsNorthEastLongitude;
        public double BoundsNorthEastLongitude
        {
            get { return _boundsNorthEastLongitude; }
            set { _boundsNorthEastLongitude = value; }
        }
    }
}
