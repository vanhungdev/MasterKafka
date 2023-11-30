﻿using Akka.Streams.Dsl;
using Confluent.Kafka;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using static Confluent.Kafka.ConfigPropertyNames;

namespace MasterKafka.Kafka.Consumer
{
    public class KafkaConsumer : IKafkaConsumer
    {
        private readonly Func<string, Task> _messageHandler;
        private readonly ConsumerConfig _kafkaConfig;

        public KafkaConsumer(Func<string, Task> messageHandler, ConsumerConfig kafkaConfig)
        {
            _messageHandler = messageHandler;
            _kafkaConfig = kafkaConfig;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="topic"></param>
        /// <param name="stoppingToken"></param>
        /// <returns></returns>
        public async Task StartConsuming(string topic, CancellationToken stoppingToken)
        {
            // Tạm thời chưa ghi log
            using (var consumer = new ConsumerBuilder<Ignore, string>(_kafkaConfig).Build())
            {
                
                consumer.Subscribe(topic);
                // Lấy danh sách các partitions của topic
                var partitions = consumer.Assignment;
                ConsumePartition(consumer, stoppingToken, topic);

                //Parallel.ForEach(partitions, partition =>
                //{
                //    ConsumePartition(partition, consumer, stoppingToken, topic);
                //});
            }
        }
        void ConsumePartition(IConsumer<Ignore, string> consumer, CancellationToken stoppingToken, string topic)
        {
            //Console.WriteLine($"Message from {partition}:");
            while (true)
            {
                try
                {
                    while (!stoppingToken.IsCancellationRequested)
                    {
                        // Đọc một batch message từ Kafka vào list
                        var batch = ReadMessageBatchFromKafka(consumer);
                        Console.WriteLine($"Batch {JsonConvert.SerializeObject(batch)}");
                        Console.WriteLine($"Batch Count {batch.Count()} -- Topic: {topic}");
                        Console.WriteLine($"DateTime: {DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff")}");
                        Console.WriteLine($"------------------------------------------");

                        Parallel.ForEach(batch, new ParallelOptions { MaxDegreeOfParallelism = 10 },
                          msg =>
                          {
                              try
                              {
                                  //Console.WriteLine($"DateTime: {DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff")}  || Thread {Thread.CurrentThread.ManagedThreadId} starting processing message: {msg}");
                                  _messageHandler(msg);
                              }
                              catch (Exception ex)
                              {
                                  Console.WriteLine($"DateTime: {DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff")} | Exception handler: {ex.Message} | message: {msg} | Topic {topic}:");
                              }
                          });
                    }
                }
                catch (OperationCanceledException oe)
                {
                    string exceptionMessage = oe.Message;
                }
                finally
                {
                    consumer.Close();
                }
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="consumer"></param>
        /// <returns></returns>
        List<string> ReadMessageBatchFromKafka(IConsumer<Ignore, string> consumer)
        {
            const int MAX_MESSAGES = 30;
            List<string> batch = new List<string>();

            for (int i = 0; i < MAX_MESSAGES; i++)
            {
                try
                {
                    // Lấy message và add vào batch
                    var result = consumer.Consume(TimeSpan.FromMilliseconds(10)); // Chờ 3s nếu k có message mới thì trả về null || 100*30 =3000 = 3
                    if (result != null && result.Message != null && !string.IsNullOrEmpty(result.Message.Value))
                    {
                        batch.Add(result.Message.Value);    
                        consumer.Commit(result); // Commit offset
                        var offset = result.Offset;
                        var partition = result.Partition;
                        Console.WriteLine($"Consumer offset: {offset}   || partition: {partition}");
                    }
                }
                catch (ConsumeException ex)
                {
                    Console.WriteLine($"DateTime: {DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff")} | Method: ReadMessageBatchFromKafka | ConsumeException:  {ex.Message}");
                    break;
                }
            }
            //Console.WriteLine($"Done offset:");
            //Console.WriteLine($"batch offset count:{batch.Count()}");
            return batch;
        }
        public Task StopConsuming(CancellationToken stoppingToken)
        {
            throw new NotImplementedException();
        }
    }
}
