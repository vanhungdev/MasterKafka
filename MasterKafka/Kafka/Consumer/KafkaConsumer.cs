using Akka.Configuration.Hocon;
using Akka.Streams.Dsl;
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
        private const int TIME_OUT_PROCESS_MESSAGE = 20; // After 20 seconds, if the process is not completed, block it
        const int MAX_CONSUMER_MESSAGE_BATCH = 30; // Each time the consumer takes 30 items and then batches them
        const int MAX_MESSAGE_PARALLELISM = 5; // Number of threads per parallel lop 
        private SemaphoreSlim semaphore = new SemaphoreSlim(MAX_MESSAGE_PARALLELISM);

        public KafkaConsumer(IMessageHandler messageHandler, ConsumerConfig kafkaConfig, int instance)
        {
            _messageHandler = messageHandler;
            _kafkaConfig = kafkaConfig;
            _instance = instance;
        }

        public Task StartConsuming(string topic, CancellationToken stoppingToken)
        {
            int threadCount = 0;
            List<IConsumer<Ignore, string>> consumers = CreateInstanceConsumers(_kafkaConfig, topic, _instance);

            Parallel.ForEach(consumers, new ParallelOptions { MaxDegreeOfParallelism = _instance }, consumer =>
            {
                int threadId = Interlocked.Increment(ref threadCount);
                //Console.WriteLine($"Start thread #{threadId}");

                ConsumePartition(consumer, stoppingToken, topic, threadId);
            });
            return Task.CompletedTask;
        }

        /// <summary>
        /// Create the number of suspended consumer instances from the consumerInstance variable
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
        /// Commumer handling for each instance
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
                    Console.WriteLine($"DateTime: {DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff")} Batch Count {batch.Count()} -- Topic: {topic}");
                    Console.WriteLine($"------------------------------------------");

                    Parallel.ForEach(batch, new ParallelOptions { MaxDegreeOfParallelism = MAX_MESSAGE_PARALLELISM },
                      async msg =>
                      {
                          try
                          {
                              await HandleMessageWithTimeout(msg, _messageHandler, topic, TIME_OUT_PROCESS_MESSAGE, null, true);
                          }
                          catch (Exception ex)
                          {
                              await HandleMessageWithTimeout(msg, _messageHandler, topic, TIME_OUT_PROCESS_MESSAGE, ex, false);
                          }
                      });
                }
            }
            catch (OperationCanceledException oe)
            {
                Console.WriteLine($"DateTime: {DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff")} | OperationCanceledException:  {oe.Message}");
            }
            finally
            {
                consumer.Close();
            }
        }

        /// <summary>
        /// Collection message
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
                    // Wait 0,3 seconds, if there is no new message, return null - 100*30 =3000 = 3s
                    var result = consumer.Consume(TimeSpan.FromMilliseconds(100));
                    if (result != null && result.Message != null && !string.IsNullOrEmpty(result.Message.Value))
                    {
                        batch.Add(result.Message.Value);    
                        consumer.Commit(result);
                        var offset = result.Offset;
                        var partition = result.Partition;
                        //Console.WriteLine($"ThreadId: {threadId } || Consumer offset: {offset} || partition: {partition} -- {DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff")} -- mesage: {result.Message.Value}");
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
        /// Handle message time out
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="messageHandler"></param>
        /// <param name="topic"></param>
        /// <param name="timeoutSeconds"></param>
        /// <param name="ex"></param>
        /// <returns></returns>
        /// <exception cref="TimeoutException"></exception>
        private async Task HandleMessageWithTimeout(string msg, IMessageHandler messageHandler, string topic, int timeoutSeconds, Exception ex, bool isHandleMessage)
        {
            using (var cts = new CancellationTokenSource(TimeSpan.FromSeconds(timeoutSeconds)))
            {
                try
                {
                    await semaphore.WaitAsync(cts.Token);
                    if (isHandleMessage)
                    {
                        var handleMessage = await messageHandler.HandleMessage(msg, topic, cts.Token);
                        if (handleMessage)
                        {
                            await messageHandler.HandleSuccess(msg, topic, cts.Token);
                        }
                    }
                    else 
                    {
                        await messageHandler.HandleFailure(msg, topic, ex, cts.Token);
                    }
                }
                catch(OperationCanceledException oce)
                {
                    if (isHandleMessage)
                    {
                        throw new TimeoutException($"Message handling timed out! Exception: {oce.Message}");
                    }
                    else
                    {
                        Console.WriteLine($"DateTime: {DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff")} | Exception:  {oce.Message}");
                    }
                }
                finally
                {
                    semaphore.Release();
                }
            }
        }
    }
}
