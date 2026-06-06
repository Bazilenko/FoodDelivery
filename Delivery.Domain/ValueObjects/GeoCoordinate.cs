using Delivery.Domain.Common;
using Delivery.Domain.Exceptions;

namespace Delivery.Domain.ValueObjects
{
    public class GeoCoordinate : ValueObject
    {
        public double Latitude { get; }
        public double Longitude { get; }

        public GeoCoordinate(double latitude, double longitude)
        {
            if (latitude < -90 || latitude > 90)
                throw new InvalidPosition();
            if (longitude < -180 || longitude > 180)
                throw new InvalidPosition();

            Latitude = latitude;
            Longitude = longitude;
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Latitude;
            yield return Longitude;
        }

        public double DistanceTo(GeoCoordinate other)
        {
            var d1 = Latitude * (Math.PI / 180.0);
            var num1 = Longitude * (Math.PI / 180.0);
            var d2 = other.Latitude * (Math.PI / 180.0);
            var num2 = other.Longitude * (Math.PI / 180.0);
            var d3 = d2 - d1;
            var num3 = num2 - num1;
            
            var a = Math.Sin(d3 / 2.0) * Math.Sin(d3 / 2.0) +
                    Math.Cos(d1) * Math.Cos(d2) *
                    Math.Sin(num3 / 2.0) * Math.Sin(num3 / 2.0);
            
            var c = 2.0 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1.0 - a));
            const double earthRadius = 6371.0; // km
            
            return earthRadius * c;
        }
    }
}
