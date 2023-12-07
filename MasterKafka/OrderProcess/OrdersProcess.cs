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

        public async Task HandleMessage(string message, string topic, CancellationToken cancellationToken)
        {
            try
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    return;
                }
                //throw new Exception("throw exception proactive.");
                var data = JsonConvert.DeserializeObject<MessageValuesDto>(message);
                Console.WriteLine($"Start process message dateTime: {DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff")} Topic: {topic}: Message: {data.Value}");
                await Task.Delay(TimeSpan.FromSeconds(22), cancellationToken); // delay
                if (cancellationToken.IsCancellationRequested)
                {
                    return;
                }
                Console.WriteLine($"Done process message dateTime: {DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff")} Topic: {topic}: Message: {data.Value}");
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex); // The exception message can be passed in
            }
        }

        public async Task HandleSuccess(string message, string topic)
        {
            // TODO
            Console.WriteLine($"Handle success dateTime: {DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff")} Topic:  {topic} Message: {message}");
        }

        public async Task HandleFailure(string message, string topic, Exception exception)
        {
            // TODO
            //await Task.Delay(TimeSpan.FromSeconds(25));
            Console.WriteLine($"Handle failure dateTime: {DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff")} Topic:  {topic} Message: {message}");
        }
    }
    public class MessageValuesDto
    {
        public string Value { get; set; }
    }
}
