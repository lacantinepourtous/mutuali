﻿namespace YellowDuck.Api.DbModel.Entities.Ads
{
    public class AdAddress : IHaveLongIdentifier
    {
        public long Id { get; set; }

        public Ad Ad { get; set; }

        public string StreetNumber { get; set; }
        public string Route { get; set; }
        public string Locality { get; set; }
        public string PostalCode { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public string Neighborhood { get; set; }
        public string Sublocality { get; set; }

        public string Raw { get; set; }
    }
}
