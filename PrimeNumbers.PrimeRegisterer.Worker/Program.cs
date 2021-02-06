using PrimeNumbers.PrimeRegisterer.Core;
using System;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace PrimeNumbers.PrimeRegisterer.Worker
{
    class Program
    {
        private static PrimeRegistererHandler _primeHandler;
        private static readonly object _waitForStopMutex = new object();

        static void Main(string[] args)
        {
            string primesDbUrl = "http://primes-db-service:30006";
            string numberAssignerUrl = "http://number-assigner-service:30007";
            //string primesDbUrl = "http://localhost:30006";
            //string numberAssignerUrl = "http://localhost:30007";

            AttachToShutdown();

            PrimeRangeCalculator calculator = new ();
            PrimeResultSubmitter resultSubmitter = PrimeResultSubmitter.CreateNew(new Uri(primesDbUrl));
            NumbersToCalculateAssigner numbersAssigner = NumbersToCalculateAssigner.CreateNew(new Uri(numberAssignerUrl));
            _primeHandler = new PrimeRegistererHandler(calculator, resultSubmitter, numbersAssigner, 5);
            _primeHandler.Start();

            lock (_waitForStopMutex)
            {
                Monitor.Wait(_waitForStopMutex);
            }
        }

        static void AttachToShutdown()
        {
            AppDomain.CurrentDomain.ProcessExit += (_, _) => OnStop();
            Console.CancelKeyPress += (_, _) => OnStop();
        }

        static void OnStop()
        {
            Console.WriteLine("STOPPING");
            _primeHandler.Stop();
            lock (_waitForStopMutex)
            {
                Monitor.Pulse(_waitForStopMutex);
            }
        }

    }
}
