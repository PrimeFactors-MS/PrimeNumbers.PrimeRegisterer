using PrimeNumbers.Shared.PrimeCalculation;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PrimeNumbers.PrimeRegisterer.Core
{
    public class PrimeRangeCalculator
    {
        private readonly CancellationTokenSource _cts;
        public event Action<PrimeRecordReport> PrimesCalculated = delegate { };
        private Task _calculationTask;

        public PrimeRangeCalculator()
        {
            _cts = new CancellationTokenSource();
        }

        public Task CalculatePrimes(ulong start, ulong end)
        {
            _calculationTask = Task.Run(() =>
            {
                Stopwatch stopwatch = new Stopwatch();
                PrimeCalculationResult result;
                for (ulong i = start; i <= end; i++)
                {
                    stopwatch.Start();
                    result = PrimeCalculator.GetPrimes(i);
                    stopwatch.Stop();
                    
                    PrimeRecord record = new(i, result.IsPrime, result.Primes);
                    PrimeRecordReport recordReport = new(record, stopwatch.ElapsedMilliseconds);
                    
                    PrimesCalculated.Invoke(recordReport);
                }
            }, _cts.Token);
            return _calculationTask;
        }

        public void Stop()
        {
            _cts.Cancel();
        }
    }
}
