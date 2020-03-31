using System;
using System.Text;
using System.Threading.Tasks;
using NATS.Client;

namespace Publisher
{
    public class GreeterService
    {
        public async Task RunAsync(IConnection connection, string id)
        {
        
            byte[] payload = Encoding.Default.GetBytes(id);
            connection.Publish("greeter", payload);

            await Task.Delay(1000);
        }
    }
}