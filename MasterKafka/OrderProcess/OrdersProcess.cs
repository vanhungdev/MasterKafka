using MasterKafka.Kafka.Consumer;
using Newtonsoft.Json;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace MasterKafka.OrderProcess
{
    public class OrdersProcess : IMessageHandler
    {
        public OrdersProcess() { }
        public async Task HandleFailure(string message, string topic, Exception exception)
        {
            //Console.WriteLine($"DateTime: {DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff")} | Exception handler: {exception.Message} | message: {message} | Topic {topic}:");
        }

        public async Task HandleMessage(string message, string topic)
        {
            try
            {
                var data = JsonConvert.DeserializeObject<MessageValuesDto>(message);
                Console.WriteLine($"DateTime start process message: {DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff")} {topic}: {data.Value}");
                await Task.Delay(TimeSpan.FromSeconds(2));
                Console.WriteLine($"DateTime Done process message: {DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff")} {topic}: {data.Value}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"DateTime: {DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff")} {topic}: {ex.Message} : message: {message}");
                throw new Exception(ex.Message, ex); // có thể truyền vào
            }
        }

        public async Task HandleSuccess(string message, string topic)
        {
            //Console.WriteLine($"Handle success: {DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff")} topic:  {topic} message: {message}");
        }
    }
    public class MessageValuesDto
    {
        public string Value { get; set; }
    }
}
