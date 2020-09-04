using System;
using System.Collections.Generic;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace NewAsyncFeatures
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var random = new Random();

            var gateWay = new Gateway();

            for (int i = 0; i < 10; i++)
            {
                gateWay.ConnectSensorNode(CreateSensorNode(i, random));
            }

            System.Console.WriteLine("Start reading first run");

            await foreach (var item in gateWay.GetLatestReadings())
            {
                Console.WriteLine($"{item.SensorId} - {item.Value} - {item.TimeStamp}");
            }
            System.Console.WriteLine("Done reading first run");
            
            await Task.Delay(4000);

            System.Console.WriteLine("Start reading second run");
            
            await foreach (var item in gateWay.GetLatestReadings())
            {
                Console.WriteLine($"{item.SensorId} - {item.Value} - {item.TimeStamp}");
            }
            System.Console.WriteLine("Done reading second run");

            Console.ReadLine();
        }

        private static SensorNode CreateSensorNode(int id, Random random)
        {
            var channelOptions = new BoundedChannelOptions(1) { FullMode = BoundedChannelFullMode.DropNewest };
            var channel = Channel.CreateBounded<SensorReading>(channelOptions);

            return new SensorNode($"node {id}", (id % 2 == 0 ? 2000 : 7000) + random.Next(0, 500), channel, random);
        }
    }
}