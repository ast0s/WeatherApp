using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data.SQLite;
using System.Globalization;

namespace WeatherApp
{
    public class WeatherRepository
    {
        private readonly SQLiteConnection sqlite_connection;

        public WeatherRepository(SQLiteConnection sqlite_connection)
        {
            this.sqlite_connection = sqlite_connection;
        }

        public void Save(WeatherForecast weatherForecast)
        {
            SQLiteCommand sqlite_command = sqlite_connection.CreateCommand();
            foreach (var weather in weatherForecast?.list)
            {
                sqlite_command.CommandText = $"INSERT OR IGNORE INTO WeatherForecasts (town, date_time, temperature, humidity, wind) "
                    + $"VALUES("
                    + $"'{weatherForecast.city.name}',"
                    + $"'{weather.dt}',"
                    + $"{weather.main.temp.Value.ToString(CultureInfo.InvariantCulture)},"
                    + $"{weather.main.humidity},"
                    + $"{weather.wind.speed.Value.ToString(CultureInfo.InvariantCulture)}"
                    + $");";
                sqlite_command.ExecuteNonQuery();
            }
        }
        public List<WeatherForecastDTO> GetAllFromCity(string city_name)
        {
            SQLiteCommand sqlite_command = sqlite_connection.CreateCommand();
            sqlite_command.CommandText = $"SELECT * FROM WeatherForecasts WHERE town = '{city_name}';";
            SQLiteDataReader data_reader = sqlite_command.ExecuteReader();

            var list = new List<WeatherForecastDTO>();

            while (data_reader.Read())
            {
                NameValueCollection values = data_reader.GetValues();

                list.Add(
                    new WeatherForecastDTO(
                        city: values.Get("town"),
                        dateTime: DateTimeOffset.FromUnixTimeSeconds(
                                long.Parse(values.Get("date_time"))
                            ).ToLocalTime().DateTime,
                        temp: double.Parse(values.Get("temperature"), CultureInfo.InvariantCulture),
                        humidity: double.Parse(values.Get("humidity"), CultureInfo.InvariantCulture),
                        wind: double.Parse(values.Get("wind"), CultureInfo.InvariantCulture))
                );
            }

            return list;
        }

        public List<WeatherForecastDTO> GetAll()
        {
            SQLiteCommand sqlite_command = sqlite_connection.CreateCommand();
            sqlite_command.CommandText = $"SELECT * FROM WeatherForecasts;";
            SQLiteDataReader data_reader = sqlite_command.ExecuteReader();

            var list = new List<WeatherForecastDTO>();

            while (data_reader.Read())
            {
                NameValueCollection values = data_reader.GetValues();

                list.Add(
                    new WeatherForecastDTO(
                        city: values.Get("town"),
                        dateTime: DateTimeOffset.FromUnixTimeSeconds(
                                long.Parse(values.Get("date_time"))
                            ).ToLocalTime().DateTime,
                        temp: double.Parse(values.Get("temperature"), CultureInfo.InvariantCulture),
                        humidity: double.Parse(values.Get("humidity"), CultureInfo.InvariantCulture),
                        wind: double.Parse(values.Get("wind"), CultureInfo.InvariantCulture))
                );
            }

            return list;
        }
    }
}
