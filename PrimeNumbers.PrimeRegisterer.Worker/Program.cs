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

        static void Main(string[] _)
        {
            AttachToShutdown();

            PrimeRangeCalculator calculator = new ();
            PrimeResultSubmitter resultSubmitter = PrimeResultSubmitter.CreateNew(new Uri("http://192.168.1.18:30006"));
            _primeHandler = new(calculator, resultSubmitter, new NumbersToCalculatedAssigner());
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
