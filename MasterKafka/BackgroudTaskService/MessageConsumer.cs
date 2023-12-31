﻿using MasterKafka.Kafka;
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

        public MessageConsumer(KafkaConsumerManager consumerManager)
        {
            _consumerManager = consumerManager;
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            // Example config for topic 1
            var topic1 = "events1";
            var instanceTopic1 = 1; // Number of instances (number of instances is greater than or equal to the number of partitions in topic)
            _consumerManager.AddConsumer(topic1, new OrdersProcess(), KafkaConfiguration.ConsumerConfig, instanceTopic1);
            // More thread

            await _consumerManager.StartAllConsumersAsync(stoppingToken); // Start consumer threads
        }
    }
}
