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

        public SensorNode(string id, int interval, Channel<SensorReading> channel, Random noiseGenerator)
        {
            var timer = new Timer(Callback);
            timer.Change(interval, interval);
            _id = id;
            _channel = channel;
            _noiseGenerator = noiseGenerator;
        }
        
        public ChannelReader<SensorReading> ChannelOutput => _channel.Reader;

        private void Callback(object state)
        {
            var value = 25 * _noiseGenerator.NextDouble();
            var reading = new SensorReading(_id, value, DateTime.Now);
            _channel.Writer.TryWrite(reading);
        }
    }

    public class SensorReading
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
