using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace NewAsyncFeatures
{
    public class Gateway
    {
        private List<SensorNode> _sensorNodes;

        public Gateway()
        {
            _sensorNodes = new List<SensorNode>();
        }

        public void ConnectSensorNode(SensorNode sensorNode)
        {
            _sensorNodes.Add(sensorNode);
        }

        private async Task ReadValueAndCopyToOutput(SensorNode sensorNode, ChannelWriter<SensorReading> output)
        {
            var item = await sensorNode.Output.ReadAsync().ConfigureAwait(false);
            await output.WriteAsync(item).ConfigureAwait(false);
        }

        private ChannelReader<SensorReading> StartReadingFromSensors()
        {
            var aggregatingChannel = Channel.CreateUnbounded<SensorReading>();

            Task.Run(async () =>
            {
                var readOperations = _sensorNodes.Select(sn => ReadValueAndCopyToOutput(sn, aggregatingChannel));

                await Task.WhenAll(readOperations).ConfigureAwait(false);

                aggregatingChannel.Writer.Complete();
            }
            ).ConfigureAwait(false);

            return aggregatingChannel.Reader;
        }

        public IAsyncEnumerable<SensorReading> GetLatestReadings()
        {
            var channel = StartReadingFromSensors();

            return channel.ReadAllAsync();
        }
    }
}