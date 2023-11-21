using Newtonsoft.Json;
using System;

namespace MasterKafka.OrderProcess
{
    public class OrdersProcess : IOrdersProcess
    {
        public void CreateOrderProcess(string message, string topic)
        {
            var data = JsonConvert.DeserializeObject<MyEventDto>(message);
            Console.WriteLine($"DateTime: {DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff -- ")} {topic}: {data.Value}");
        }
    }
    public class MyEventDto
    {
        public string Value { get; set; }
    }
}
