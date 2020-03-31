using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Grpc.Core;
using Microsoft.Extensions.Logging;
using NATS.Client;
using Publisher;
using StackExchange.Redis;

namespace BackendApi.Services
{
    public class JobService : Job.JobBase
    {
        static IDatabase db = ConnectionMultiplexer.Connect("localhost").GetDatabase();
        static  GreeterService greeterService = new GreeterService();
        static  IConnection _connection = new ConnectionFactory().CreateConnection("nats://127.0.0.1");

        private readonly static Dictionary<string, string> _jobs = new Dictionary<string, string>();
        private readonly ILogger<JobService> _logger;

        public JobService(ILogger<JobService> logger)
        {
            _logger = logger;
        }

        public override Task<RegisterResponse> Register(RegisterRequest request, ServerCallContext context)
        {
            string id = Guid.NewGuid().ToString();
            var resp = new RegisterResponse
            {
                Id = id
            };
            _jobs[id] = request.Description;
            
            db.StringSet(id, request.Description);
            greeterService.RunAsync(_connection, id).Wait();
        
            
            return Task.FromResult(resp);
        }
    }
}