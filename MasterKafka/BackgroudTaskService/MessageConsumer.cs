using MasterKafka.Kafka;
using MasterKafka.Kafka.Consumer;
using MasterKafka.OrderProcess;
using Microsoft.Extensions.Hosting;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace MasterKafka.BackgroudTaskService
{
    public class MessageConsumer : BackgroundService
    {
        private readonly KafkaConsumerManager _consumerManager;
        private readonly IOrdersProcess _orderProcess;

        public MessageConsumer(KafkaConsumerManager consumerManager, IOrdersProcess orderProcess)
        {
            _consumerManager = consumerManager;
            _orderProcess = orderProcess;
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var topic1 = "events1";
            var topic2 = "events2";
            // More Topic

            _consumerManager.AddConsumer(topic1, async message => _orderProcess.CreateOrderProcess(message, topic1), KafkaConfiguration.Config); // Configuration can be changed
            _consumerManager.AddConsumer(topic2, async message => _orderProcess.CreateOrderProcess(message, topic2), KafkaConfiguration.Config); // Configuration can be changed
            //More thread

            await _consumerManager.StartAllConsumersAsync(stoppingToken); // Start parallel
        }

        /// <summary>
        /// Test metod
        /// </summary>
        /// <param name="message"></param>
        /// <param name="topic"></param>
        public static void MessageProcess1(string message, string topic)
        {
            Console.WriteLine($"Received message from {topic}: {message}");
        }
    }
}
