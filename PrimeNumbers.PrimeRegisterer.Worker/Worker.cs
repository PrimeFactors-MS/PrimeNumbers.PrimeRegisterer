using PrimeNumbers.PrimeRegisterer.Core;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace PrimeNumbers.PrimeRegisterer.Worker
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private PrimeRegistererHandler _primeHandler;

        public Worker(ILogger<Worker> logger)
        {
            _logger = logger;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            string rabbitmqHostname = "rabbitmq-msprimes-service";
            string numberAssignerUrl = "http://number-assigner-service:30007";
            //string rabbitmqHostname = "localhost";
            //string numberAssignerUrl = "http://localhost:30007";
            int rabbitmqPort = 30101;

            PrimeRangeCalculator calculator = new();
            PrimeResultSubmitter resultSubmitter = PrimeResultSubmitter.CreateNew(rabbitmqHostname, rabbitmqPort);
            NumbersToCalculateAssigner numbersAssigner = NumbersToCalculateAssigner.CreateNew(new Uri(numberAssignerUrl));
            _primeHandler = new PrimeRegistererHandler(calculator, resultSubmitter, numbersAssigner, _logger, 5);
            _primeHandler.Start();

            _logger.LogInformation("Started successfully");

            return Task.CompletedTask;
        }

        public override Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Stopping");
            _primeHandler.Stop();
            return Task.CompletedTask;
        }
    }
}
