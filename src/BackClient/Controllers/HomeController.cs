﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Xml;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using BackClient.Models;
using Grpc.Net.Client;
using BackendApi;
 using BackClient.Models;
 using Microsoft.AspNetCore.Routing;


 namespace BackClient.Controllers
{
    
    public class TryToRiderct
    {
        public string  Resp { get; set; }
    }

    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel {RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier});
        }

        [HttpPost]
        public async Task<IActionResult> SendTask(string inputTask, string inputData)
        {
            AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);
            using var channel = GrpcChannel.ForAddress("http://localhost:5000");
            var client = new Job.JobClient(channel);
            RegisterResponse reply = await client.RegisterAsync(
                new RegisterRequest {Description = inputTask, Data = inputData});

            TryToRiderct id = new TryToRiderct{ Resp = reply.Id};
        
            return RedirectToAction("ShowRanking", id);
        }
        
        [HttpGet] 
        public IActionResult ShowRanking(TryToRiderct id)
        {
            Console.WriteLine("bb" + id + "bb");
            
            RegisterResponse reply  = new RegisterResponse{ Id = id.Resp};
            

            using var channel = GrpcChannel.ForAddress("http://localhost:5000");
            var client = new Job.JobClient(channel);
            var repl = client.GetProcessingResult(reply);
            
            
            
            return View("JobId", repl);
        }
    }
}