using System;

namespace AzUtil.Core
{

    public struct Coordinate : IEquatable<Coordinate>
    {
        public Coordinate(double latitude, double longitude, bool isUnknown = false)
        {
            Latitude = latitude;
            Longitude = longitude;
            IsUnknown = isUnknown;
        }
        public double Latitude { get; }
        public double Longitude { get; }

        public bool IsUnknown { get; }
        public static Coordinate Unknown => new(0, 0, true);

        public override bool Equals(object obj)
        {
            return obj is Coordinate coordinate &&
                   Latitude == coordinate.Latitude &&
                   Longitude == coordinate.Longitude &&
                   IsUnknown == coordinate.IsUnknown;
        }

        public bool Equals(Coordinate other)
        {
            return Latitude == other.Latitude &&
                   Longitude == other.Longitude &&
                   IsUnknown == other.IsUnknown;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Latitude, Longitude, IsUnknown);
        }

        public static bool operator == (Coordinate left, Coordinate right)
        {
            return left.Equals(right);
        }

        public static bool operator != (Coordinate left, Coordinate right)
        {
            return !(left == right);
        }
    }
}
