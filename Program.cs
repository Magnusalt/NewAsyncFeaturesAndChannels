using System;
using System.Threading.Tasks;
using System.Linq;

namespace NewAsyncFeatures
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var random = new Random();
            var gateway = new Gateway();

            for (int i = 0; i < 10; i++)
                gateway.ConnectSensorNode(new SensorNode($"node {i}", (i % 2 == 0 ? 2000 : 7000), random));

            System.Console.WriteLine("Start reading first run");

            await foreach (var item in gateway.GetLatestReadings())
                Console.WriteLine($"{item.SensorId} - {item.Value} - {item.TimeStamp}");
            
            System.Console.WriteLine("Done reading first run");

            await Task.Delay(4000);

            System.Console.WriteLine("Start reading second run");

            await foreach (var item in gateway.GetLatestReadings())
                Console.WriteLine($"{item.SensorId} - {item.Value} - {item.TimeStamp}");
            
            System.Console.WriteLine("Done reading second run");

            await Task.Delay(4000);

            System.Console.WriteLine("Start reading third run");

            await foreach (var item in gateway.GetLatestReadings().Where(reading => reading.Value > 10.0))
                Console.WriteLine($"{item.SensorId} - {item.Value} - {item.TimeStamp}");

            System.Console.WriteLine("Done reading third run");
            Console.ReadLine();
        }
    }
}