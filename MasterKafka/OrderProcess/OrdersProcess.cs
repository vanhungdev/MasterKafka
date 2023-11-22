using Newtonsoft.Json;
using System;
using System.Threading.Tasks;

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
        public async void CreateOrderProcess(string message, string topic)
        {
            //var data1 = JsonConvert.DeserializeObject<MyEventDto>(message);

            /*   Console.WriteLine($"[DateTime]: {DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff")} || Start process message: {message}           || Topic: {topic}");
               Task.Delay(3000).Wait();
               Console.WriteLine($"[DateTime]: {DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff")} || Done process message: {message}            || Topic: {topic}");*/

            try
            {
                var data = JsonConvert.DeserializeObject<MyEventDto>(message);
                Console.WriteLine($"DateTime process message: {DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff")} {topic}: {data.Value}");
                Task.Delay(3000).Wait();
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
