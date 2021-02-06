using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PrimeNumbers.PrimeRegisterer.Core
{
    public class PrimeRegistererHandler
    {
        private readonly PrimeRangeCalculator _calculator;
        private readonly PrimeResultSubmitter _resultSubmitter;
        private readonly NumbersToCalculateAssigner _numbersAssigner;
        private readonly int _keepAliveIntervalSeconds;
        private CancellationTokenSource _cts;
        private Task _worker;
        private Task _keepAliveSender;

        public PrimeRegistererHandler(PrimeRangeCalculator calculator, PrimeResultSubmitter resultSubmitter,
                                      NumbersToCalculateAssigner numbersAssigner,
                                      int keepAliveIntervalSeconds)
        {
            _calculator = calculator;
            _resultSubmitter = resultSubmitter;
            _numbersAssigner = numbersAssigner;
            _calculator.PrimesCalculated += OnPrimeCalculated;
            _keepAliveIntervalSeconds = keepAliveIntervalSeconds;
        }

        private void OnPrimeCalculated(PrimeRecord record)
        {
            _resultSubmitter.SubmitPrimeNumber(record).ContinueWith(task =>
            {
                if (task.Exception != null) Console.WriteLine(task.Exception + "\n\n\n");
            }).Wait();
        }

        public void Start()
        {
            _cts = new CancellationTokenSource();
            _worker = MainWork();
            _keepAliveSender = KeepAliveSender();
        }

        public void Stop()
        {
            _cts.Cancel();
            _calculator.Stop();
            _worker.Wait();

        }


        private async Task MainWork()
        {
            try
            {
                while (_cts.Token.IsCancellationRequested == false)
                {
                    NumberRange range = await _numbersAssigner.LoadNumbersRange();
                    await _calculator.CalculatePrimes(range.Start, range.End).ConfigureAwait(false);
                    await _numbersAssigner.FinishWorkingRange().ConfigureAwait(false);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("MAIN THREAD CRASHED\n" + e.ToString());
            }
            Console.WriteLine("ENDED");
        }

        private async Task KeepAliveSender()
        {
            while (_cts.Token.IsCancellationRequested == false)
            {
                try
                {
                    await Task.Delay(TimeSpan.FromSeconds(_keepAliveIntervalSeconds), _cts.Token);
                }
                catch (TaskCanceledException)
                {
                    break;
                }

                try
                {
                    await _numbersAssigner.SendKeepAlive();
                }
                catch (Exception e)
                {
                    Console.WriteLine("KEEP ALIVE THREAD ERROR\n" + e.ToString());
                }
            }
        }
    }
}
