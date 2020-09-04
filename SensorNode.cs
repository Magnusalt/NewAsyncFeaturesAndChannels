using System;
using System.Threading;
using System.Threading.Channels;

namespace NewAsyncFeatures
{
    public class SensorNode
    {
        private readonly string _id;
        private readonly Channel<SensorReading> _channel;
        private readonly Random _noiseGenerator;

        public SensorNode(string id, int interval, Random noiseGenerator)
        {
            _id = id;
            _noiseGenerator = noiseGenerator;

            var channelOptions = new BoundedChannelOptions(1) { FullMode = BoundedChannelFullMode.DropNewest };
            _channel = Channel.CreateBounded<SensorReading>(channelOptions);

            var timer = new Timer(OnIntervalElapsed);
            var noisedInterval = interval + _noiseGenerator.Next(1000);
            timer.Change(noisedInterval, noisedInterval);
        }

        public ChannelReader<SensorReading> ChannelOutput => _channel.Reader;

        private void OnIntervalElapsed(object state)
        {
            var value = 25 * _noiseGenerator.NextDouble();
            var reading = new SensorReading(_id, value, DateTime.Now);
            _channel.Writer.TryWrite(reading);
        }
    }

    public struct SensorReading
    {
        public SensorReading(string sensorId, double value, DateTime timeStamp)
        {
            SensorId = sensorId;
            Value = value;
            TimeStamp = timeStamp;
        }
        public string SensorId { get; }
        public double Value { get; }
        public DateTime TimeStamp { get; }
    }
}
