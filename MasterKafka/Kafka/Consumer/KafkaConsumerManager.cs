using Confluent.Kafka;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MasterKafka.Kafka.Consumer
{
    public class KafkaConsumerManager
    {
        private readonly Dictionary<string, IKafkaConsumer> _consumers;
        public KafkaConsumerManager()
        {
            _consumers = new Dictionary<string, IKafkaConsumer>();
        }

        /// <summary>
        /// Add a consumer thread
        /// </summary>
        /// <param name="topic"></param>
        /// <param name="messageHandler"></param>
        /// <param name="config"></param>
        public void AddConsumer(string topic, Func<string, Task> messageHandler, ConsumerConfig config, int instance)
        {
            if (!_consumers.ContainsKey(topic))
            {
                var consumer = new KafkaConsumer(messageHandler, config, instance);
                _consumers.Add(topic, consumer);
            }
        }

        /// <summary>
        /// Start parallel
        /// </summary>
        /// <param name="stoppingToken"></param>
        /// <returns></returns>
        public async Task StartAllConsumersAsync(CancellationToken stoppingToken)
        {
            var tasks = _consumers.Select(kv => Task.Run(() => kv.Value.StartConsuming(kv.Key, stoppingToken)));
            await Task.WhenAll(tasks);
        }
    }
}
