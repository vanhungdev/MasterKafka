using Confluent.Kafka;
using DotNetty.Common.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MasterKafka.Kafka.Consumer
{
    public class KafkaConsumer : IKafkaConsumer
    {
        private readonly IMessageHandler _messageHandler;
        private readonly ConsumerConfig _kafkaConfig;
        private readonly int _instance;
        private const int TIME_OUT_PROCESS_MESSAGE = 20; // 20s nếu không xử lý xong thì chặn
        const int MAX_CONSUMER_MESSAGE_BATCH = 9; // Mỗi lần consumer lấy 30 item rồi chia batch
        const int MAX_MESSAGE_PARALLELISM = 3; // Mỗi lần lop parallel 

        public KafkaConsumer(IMessageHandler messageHandler, ConsumerConfig kafkaConfig, int instance)
        {
            _messageHandler = messageHandler;
            _kafkaConfig = kafkaConfig;
            _instance = instance;
        }

        public Task StartConsuming(string topic, CancellationToken stoppingToken)
        {
            List<IConsumer<Ignore, string>> consumers = CreateInstanceConsumers(_kafkaConfig, topic, _instance);

            int threadCount = 0;
            Parallel.ForEach(consumers, new ParallelOptions { MaxDegreeOfParallelism = _instance }, consumer =>
            {
                int threadId = Interlocked.Increment(ref threadCount);
                Console.WriteLine($"Start thread #{threadId}");
                ConsumePartition(consumer, stoppingToken, topic, threadId);
            });
            return Task.CompletedTask;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="config"></param>
        /// <param name="topics"></param>
        /// <param name="consumerInstance"></param>
        /// <returns></returns>
        private List<IConsumer<Ignore, string>> CreateInstanceConsumers(ConsumerConfig config, string topics, int consumerInstance)
        {
            var consumers = new List<IConsumer<Ignore, string>>();
            for (int i = 0; i < consumerInstance; i++)
            {
                var consumer = new ConsumerBuilder<Ignore, string>(config)
                                  .SetErrorHandler((_, e) => { Console.WriteLine($"Error: {e.Reason}"); }).Build();

                consumer.Subscribe(topics);
                consumers.Add(consumer);
            }
            return consumers;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="consumer"></param>
        /// <param name="stoppingToken"></param>
        /// <param name="topic"></param>
        /// <param name="threadId"></param>
        private void ConsumePartition(IConsumer<Ignore, string> consumer, CancellationToken stoppingToken, string topic, int threadId)
        {
            try
            {
                while (!stoppingToken.IsCancellationRequested)
                {
                    var batch = ReadMessageBatchFromKafka(consumer, threadId);
                    //Console.WriteLine($"Batch {JsonConvert.SerializeObject(batch)}");
                    Console.WriteLine($"Batch Count {batch.Count()} -- Topic: {topic}");
                    Console.WriteLine($"DateTime: {DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff")}");
                    Console.WriteLine($"------------------------------------------");

                    Parallel.ForEach(batch, new ParallelOptions { MaxDegreeOfParallelism = MAX_MESSAGE_PARALLELISM },
                      async msg =>
                      {
                          try
                          {
                              await HandleMessageWithTimeout(msg, _messageHandler, topic, TIME_OUT_PROCESS_MESSAGE);
                              await _messageHandler.HandleSuccess(msg, topic);

                              /*  HandleMessageWithTimeout(msg, _messageHandler, topic, TIME_OUT_PROCESS_MESSAGE).GetAwaiter().GetResult();
                                _messageHandler.HandleSuccess(msg, topic).GetAwaiter().GetResult();*/

                          }
                          catch (Exception ex)
                          {
                               await _messageHandler.HandleFailure(msg, topic, ex);
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="consumer"></param>
        /// <param name="threadId"></param>
        /// <returns></returns>
        private List<string> ReadMessageBatchFromKafka(IConsumer<Ignore, string> consumer, int threadId)
        {
            List<string> batch = new List<string>();
            for (int i = 0; i < MAX_CONSUMER_MESSAGE_BATCH; i++)
            {
                try
                {
                    // Lấy message và add vào batch
                    var result = consumer.Consume(TimeSpan.FromMilliseconds(100)); // Chờ 3s nếu k có message mới thì trả về null || 100*30 =3000 = 3s
                    if (result != null && result.Message != null && !string.IsNullOrEmpty(result.Message.Value))
                    {
                        batch.Add(result.Message.Value);    
                        consumer.Commit(result); // Commit offset
                        var offset = result.Offset;
                        var partition = result.Partition;
                        Console.WriteLine($"ThreadId: {threadId } || Consumer offset: {offset} || partition: {partition} -- {DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff")} -- mesage: {result.Message.Value}");
                    }
                }
                catch (ConsumeException ex)
                {
                    Console.WriteLine($"DateTime: {DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff")} | Method: ReadMessageBatchFromKafka | ConsumeException:  {ex.Message}");
                    break;
                }
            }
            return batch;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="messageHandler"></param>
        /// <param name="topic"></param>
        /// <param name="timeoutSeconds"></param>
        /// <returns></returns>
        /// <exception cref="TimeoutException"></exception>
        private async Task HandleMessageWithTimeout(string msg, IMessageHandler messageHandler, string topic, int timeoutSeconds)
        {
            using (var cts = new CancellationTokenSource(TimeSpan.FromSeconds(timeoutSeconds)))
            {
                var completedTask = await Task.WhenAny(messageHandler.HandleMessage(msg, topic),
                  Task.Delay(Timeout.Infinite, cts.Token));

                if (completedTask.IsCanceled)
                {
                    throw new TimeoutException("Message handling timed out!");
                }
            }
        }
    }
}
