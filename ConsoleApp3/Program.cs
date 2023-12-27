using System;
using System.Data.SQLite;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Security.Policy;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace WeatherApp
{
    public class Program
    {
        private static string CityURL = "https://api.openweathermap.org/data/2.5/forecast?appid=c939ed043ed4f6f23c8069bb8c0bb465&q=CityName&cnt=8&units=metric";
        private static readonly HttpClient client = new HttpClient();

        async static Task Main(string[] args)
        {
            SQLiteConnection sqlite_connection = CreateConnection();
            CreateTables(sqlite_connection);
            await SHowMenuAsync(sqlite_connection);
        }

        static async Task SHowMenuAsync(SQLiteConnection sqlite_connection)
        {
            Console.WriteLine("Enter the city name");
            string city_name = Console.ReadLine();
            CityURL = CityURL.Replace("CityName", city_name);

            Console.WriteLine("Show and save today's forecast (press 1)");
            Console.WriteLine("Show all saved forecast for city (press 2)");
            Console.WriteLine("Show all saved forecast (press 3)");
            switch (Console.ReadLine())
            {
                case "1":
                    await ShowAndSaveTodaysForeacast(sqlite_connection);
                    break;
                case "2":
                    ShowAllForecastsForCity(sqlite_connection, city_name);
                    break;
                case "3":
                    ShowAllForecasts(sqlite_connection);
                    break;
                default:
                    break;
            }
        }

        static void CreateTables(SQLiteConnection sqlite_connection)
        {
            SQLiteCommand sqlite_command = sqlite_connection.CreateCommand();

            sqlite_command.CommandText = "CREATE TABLE IF NOT EXISTS WeatherForecasts ("
                + "town        VARCHAR(50) NOT NULL,"
                + "date_time   INTEGER     NOT NULL,"
                + "temperature REAL        NOT NULL,"
                + "humidity    INTEGER     NOT NULL,"
                + "wind        REAL        NOT NULL,"
                + "PRIMARY KEY (town, date_time)"
                + ");";
            sqlite_command.ExecuteNonQuery();
        }
        static async Task ShowAndSaveTodaysForeacast(SQLiteConnection sqlite_connection)
        {
            await getWeather(sqlite_connection);

            async Task getWeather(SQLiteConnection conn)
            {
                Console.WriteLine("Getting JSON...");
                var responseString = await client.GetStringAsync(CityURL);
                Console.WriteLine("Parsing JSON...\n");
                WeatherForecast weatherForecast = JsonSerializer.Deserialize<WeatherForecast>(responseString);
                Console.WriteLine($"cod: {weatherForecast?.cod}");
                Console.WriteLine($"City: {weatherForecast?.city?.name}");
                Console.WriteLine($"list count: {weatherForecast?.list?.Count}\n");

                foreach (var weather in weatherForecast?.list)
                {
                    Console.WriteLine(
                      $"date: {weather.dt_txt}\n"
                    + $"temp: {weather.main.temp} °C\n"
                    + $"humidity: {weather.main.humidity} %\n"
                    + $"wind: {weather.wind.speed} m/s\n");
                }

                WeatherRepository weatherRepository = new WeatherRepository(conn);
                weatherRepository.Save(weatherForecast);
            }
        }
        private static void ShowAllForecastsForCity(SQLiteConnection sqlite_connection, string city_name)
        {
            WeatherRepository weatherRepository = new WeatherRepository(sqlite_connection);
            foreach (var weather in weatherRepository.GetAllFromCity(city_name))
            {
                Console.WriteLine(
                      $"city: {weather.City}\n"
                    + $"date: {weather.DateTime},\n"
                    + $"temp: {weather.Temp}\n"
                    + $"humidity: {weather.Humidity}\n"
                    + $"wind: {weather.Wind}\n"
                );
            }
        }
        private static void ShowAllForecasts(SQLiteConnection sqlite_connection)
        {
            WeatherRepository weatherRepository = new WeatherRepository(sqlite_connection);
            foreach (var weather in weatherRepository.GetAll())
            {
                Console.WriteLine(
                      $"city: {weather.City}\n"
                    + $"date: {weather.DateTime},\n"
                    + $"temp: {weather.Temp}\n"
                    + $"humidity: {weather.Humidity}\n"
                    + $"wind: {weather.Wind}\n"
                );
            }
        }

        static SQLiteConnection CreateConnection()
        {
            SQLiteConnection sqlite_connection;
            sqlite_connection = new SQLiteConnection("Data Source=lab2.db; Version = 3; New = True; Compress = True; ");

            try
            {
                sqlite_connection.Open();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            return sqlite_connection;
        }
    }
}