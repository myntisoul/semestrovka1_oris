using semestrovka1_oris.framework.core;
using System.Net;
using System.Text;
using System.Text.Json;


namespace semestrovka1_oris
{
    public class Settings
    {
        public string[] Prefixes { get; set; }
    }

    class Program
    {
        static async Task Main()
        {
            //string pathSettings = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "settings.json");
            //string jsonString = File.ReadAllText(pathSettings);
            //var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            //Settings settings = JsonSerializer.Deserialize<Settings>(jsonString, options);

            HttpServer server = new HttpServer();
            server.Start();
            Console.WriteLine("write 'stop' to stop the server");
            Console.WriteLine("http://127.0.0.1:8888/");
            while (true)
            {
                string command = Console.ReadLine();
                if (command == "stop") { server.Stop(); break; }
            }
        }
    }
}

