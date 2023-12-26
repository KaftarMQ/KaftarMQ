namespace Consumer;

using Microsoft.Extensions.Configuration;
using System.IO;

namespace KaftarMQConsumer
{
    public class KaftarMQSettings
    {
        public string QueueName { get; set; }
        public string PullEndpoint { get; set; }
        public string ApiKey { get; set; }
    }

    class Program
    {
        static void Main(string[] args)
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();

            var kaftarMQSettings = configuration.GetSection("KaftarMQ").Get<KaftarMQSettings>();

            var consumer = new Consumer(kaftarMQSettings.QueueName, kaftarMQSettings.PullEndpoint, kaftarMQSettings.ApiKey);
            // Rest of your code...
        }
    }
}
