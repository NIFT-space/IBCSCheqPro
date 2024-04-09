using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IBCS_Web_Portal.Models
{
    public class City
    {
        public int cityid { get; set; }

        public string cityname { get; set; }
    }
    public class City2
    {
        public string cityinfo { get; set; }
        public string responseCode { get; set; }
        public string responseMessage { get; set; }
    }
    public class CityResponse
    {
        public string responseCode { get; set; }
        public string responseMessage { get; set; }
        public List<City> cities { get; set; }
    }

    public class CityRequest
    {
        public string userID { get; set; }
    }
}