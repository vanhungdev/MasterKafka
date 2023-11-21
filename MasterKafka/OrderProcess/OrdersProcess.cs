using Newtonsoft.Json;
using System;

namespace MasterKafka.OrderProcess
{
    public class OrdersProcess : IOrdersProcess
    {
        /// <summary>
        /// Nhớ try catch when DeserializeObject
        /// Cẩn thận exception là dừng thread
        /// </summary>
        /// <param name="message"></param>
        /// <param name="topic"></param>
        public void CreateOrderProcess(string message, string topic)
        {
            var data1 = JsonConvert.DeserializeObject<MyEventDto>(message);
            try
            {
                var data = JsonConvert.DeserializeObject<MyEventDto>(message);
                Console.WriteLine($"DateTime: {DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff")} {topic}: {data.Value}");
            }
            catch (Exception ex) 
            {
                Console.WriteLine($"DateTime: {DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff")} {topic}: {ex.Message} : message: {message}");
            }
        }
    }
    public class MyEventDto
    {
        public string Value { get; set; }
    }
}
