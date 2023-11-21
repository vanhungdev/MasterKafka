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
           /* var topic3 = "events3";
            var topic4 = "events4";
            var topic5 = "events5";*/
            // More Topic

            _consumerManager.AddConsumer(topic1, 
                message => { _orderProcess.CreateOrderProcess(message, topic1); return Task.CompletedTask; }, 
                KafkaConfiguration.ConsumerConfig); // Configuration can be changed

            _consumerManager.AddConsumer(topic2, 
                message => { _orderProcess.CreateOrderProcess(message, topic2); return Task.CompletedTask; }, 
                KafkaConfiguration.ConsumerConfig);

         /*   _consumerManager.AddConsumer(topic3,
                message => { _orderProcess.CreateOrderProcess(message, topic3); return Task.CompletedTask; },
                KafkaConfiguration.ConsumerConfig);

            _consumerManager.AddConsumer(topic4,
                message => { _orderProcess.CreateOrderProcess(message, topic4); return Task.CompletedTask; },
                KafkaConfiguration.ConsumerConfig); 

            _consumerManager.AddConsumer(topic5,
                message => { _orderProcess.CreateOrderProcess(message, topic5); return Task.CompletedTask; },
                KafkaConfiguration.ConsumerConfig);*/

            // More thread

            await _consumerManager.StartAllConsumersAsync(stoppingToken); // Start parallel
        }
    }
}
