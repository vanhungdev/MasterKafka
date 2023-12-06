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
        /// Add topic consumer
        /// </summary>
        /// <param name="topic"> Topic name</param>
        /// <param name="messageHandler"> Method xử lý message</param>
        /// <param name="config"> Config consumer implement từ IMessageHandler</param>
        /// <param name="instance"> Số lượng instance tương ứng với ố partition</param>
        public void AddConsumer(string topic, IMessageHandler messageHandler, ConsumerConfig config, int instance)
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
