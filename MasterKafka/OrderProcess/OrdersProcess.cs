using Akka.Streams.Implementation.Fusing;
using MasterKafka.Kafka.Consumer;
using Newtonsoft.Json;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace MasterKafka.OrderProcess
{
    public class OrdersProcess : IMessageHandler
    {
        public OrdersProcess() 
        {

        }

        public async Task<bool> HandleMessage(string message, string topic, CancellationToken cancellationToken)
        {
            try
            {
                //throw new Exception("throw exception proactive.");
                var data = JsonConvert.DeserializeObject<MessageValuesDto>(message);
                //Console.WriteLine($"Start process message dateTime: {DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff")} Topic: {topic}: Message: {data.Value}");
                //await Task.Delay(TimeSpan.FromSeconds(22), cancellationToken); // delay

                // Check for cancellation and throw TaskCanceledException
                //cancellationToken.ThrowIfCancellationRequested();
                //Console.WriteLine($"Done process message dateTime: {DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff")} Topic: {topic}: Message: {data.Value}");
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex); // The exception message can be passed in
            }
            return true;
        }

        public async Task<bool> HandleSuccess(string message, string topic, CancellationToken cancellationToken)
        {
            try
            {
                // TODO
                //await Task.Delay(TimeSpan.FromSeconds(1), cancellationToken);
                cancellationToken.ThrowIfCancellationRequested();
               // Console.WriteLine($"Handle success dateTime: {DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff")} Topic:  {topic} Message: {message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Handle success exception dateTime: {DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff")} exception: {ex.Message} ");
            }
            Console.WriteLine($"Handle success dateTime: {DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff")} Topic:  {topic} Message: {message}");
            return true;
        }

        public async Task<bool> HandleFailure(string message, string topic, Exception exception, CancellationToken cancellationToken)
        {
            try
            {
                // TODO
                //await Task.Delay(TimeSpan.FromSeconds(1), cancellationToken);
                //cancellationToken.ThrowIfCancellationRequested();
                Console.WriteLine($"Handle failure dateTime: {DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff")} Topic:  {topic} Message: {message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Handle failure exception dateTime: {DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff")} exception: {ex.Message} ");
                return false;
            }
            return true;
        }
    }
    public class MessageValuesDto
    {
        public string Value { get; set; }
    }
}
