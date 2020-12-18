using PrimeNumbers.Shared.PrimeCalculation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PrimeNumbers.PrimeRegisterer.Core
{
    public class PrimeRangeCalculator
    {
        private readonly CancellationTokenSource _cts;
        public event Action<PrimeRecord> PrimesCalculated = delegate { };
        private Task _calculationTask;

        public PrimeRangeCalculator()
        {
            _cts = new CancellationTokenSource();
        }

        public Task CalculatePrimes(ulong start, ulong end)
        {
            _calculationTask = Task.Run(() =>
            {
                PrimeCalculationResult result;
                for (ulong i = start; i <= end; i++)
                {
                    result = PrimeCalculator.GetPrimes(i);
                    PrimesCalculated.Invoke(new PrimeRecord(i, result.IsPrime, result.Primes));
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
