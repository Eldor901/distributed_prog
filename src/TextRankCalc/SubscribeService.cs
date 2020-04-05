using System;
using System.Text;
using NATS.Client;
using NATS.Client.Rx;
using NATS.Client.Rx.Ops;
using System.Linq;
using StackExchange.Redis;


namespace TextRankCalc
{
    public class SubscriberService
    {
        
        static IDatabase db = ConnectionMultiplexer.Connect("localhost").GetDatabase();

        private readonly char[] consonants = new char[]
            {'e', 'E', 'i', 'I', 'o', 'O', 'a', 'A', 'u', 'U', 'y', 'Y'};
            
        

        public void Run(IConnection connection)
        {
            var greetings = connection.Observe("greeter")
                .Where(m => m.Data?.Any() == true)
                .Select(m => Encoding.Default.GetString(m.Data));

            greetings.Subscribe(msg =>
            { 
               string data = db.HashGet(msg, "data");
               int con = 0;
               int vovl = 0;

               foreach (char ch in data)
               {
                   if (Char.IsLetter(ch))
                   {
                       if (consonants.Contains(ch))
                       {
                           con++;
                       }
                       else
                       {
                           vovl++;
                       }
                   }
               }
               
               db.HashSet(msg, "text_rank", $"{con}/{vovl}");
            });
        }    
    }
}