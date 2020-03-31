using System;
using Logger;
using NATS.Client;


namespace JobLogger
{
    class Program
    {
        static void Main(string[] args)
        {
            var subscriberService = new SubscriberService();

            using(IConnection connection = new ConnectionFactory().CreateConnection()){
                subscriberService.Run(connection);
                Console.WriteLine("Events listening started. Press any key to exit");
                Console.ReadKey();
            }
        }
    }
}
