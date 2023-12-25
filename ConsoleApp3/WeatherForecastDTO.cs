using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeatherApp
{
    public class WeatherForecastDTO
    {
        public string City { get; set; }
        public DateTime DateTime { get; set; }
        public double Temp { get; set; }
        public double Humidity { get; set; }

        public WeatherForecastDTO(string city, DateTime dateTime, double temp, double humidity)
        {
            City = city;
            DateTime = dateTime;
            Temp = temp;
            Humidity = humidity;
        }
    }
}