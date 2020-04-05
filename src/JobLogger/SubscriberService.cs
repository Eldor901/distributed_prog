using System;
using System.Text;
using NATS.Client;
using NATS.Client.Rx;
using NATS.Client.Rx.Ops;
using System.Linq;
using StackExchange.Redis;


namespace Logger
{
    public class SubscriberService
    {
        
        static IDatabase db = ConnectionMultiplexer.Connect("localhost").GetDatabase();
        
        public void Run(IConnection connection)
        {
            var greetings = connection.Observe("greeter")
                .Where(m => m.Data?.Any() == true)
                .Select(m => Encoding.Default.GetString(m.Data));

            greetings.Subscribe(msg =>
            {
                string description = db.HashGet(msg, "description");
                Console.WriteLine($"id: {msg}; description: {description}");
            });
        }    
    }
}