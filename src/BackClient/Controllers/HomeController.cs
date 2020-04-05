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




namespace BackClient.Controllers
{
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
        public  async Task<IActionResult> SendTask(string inputTask, string inputData)
        {
            AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);
            using var channel = GrpcChannel.ForAddress("http://localhost:5000");
            var client = new Job.JobClient(channel);
            var reply = await client.RegisterAsync(
                new RegisterRequest { Description = inputTask, Data = inputData });
            
            
        //    return View("JobId", reply.Id);

            return RedirectToAction("ShowRanking", reply);
        }

        public IActionResult ShowRanking(RegisterResponse id)
        {
            using var channel = GrpcChannel.ForAddress("http://localhost:5000");
            var client = new Job.JobClient(channel);
            var reply = client.GetProcessingResult(id.Id);
            
            return View("JobId", reply);
        }
    }
}