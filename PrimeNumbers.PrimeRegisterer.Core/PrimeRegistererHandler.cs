using Microsoft.Extensions.Logging;
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
        private readonly ILogger _logger;
        private readonly int _keepAliveIntervalSeconds;
        private CancellationTokenSource _cts;
        private Task _worker;
        private Task _keepAliveSender;

        public PrimeRegistererHandler(PrimeRangeCalculator calculator, PrimeResultSubmitter resultSubmitter,
                                      NumbersToCalculateAssigner numbersAssigner, ILogger logger,
                                      int keepAliveIntervalSeconds)
        {
            _calculator = calculator;
            _resultSubmitter = resultSubmitter;
            _numbersAssigner = numbersAssigner;
            _logger = logger;
            _calculator.PrimesCalculated += OnPrimeCalculated;
            _keepAliveIntervalSeconds = keepAliveIntervalSeconds;
        }

        private void OnPrimeCalculated(PrimeRecordReport recordReport)
        {
            try
            {
                _resultSubmitter.SubmitPrimeNumber(recordReport);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error submitting prime");
            }
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
            _keepAliveSender.Wait();
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
            catch (TaskCanceledException) { }
            catch (Exception e)
            {
                _logger.LogCritical(e, "Main thread CRASHED");
            }
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
                catch (Exception)
                {
                    _logger.LogError("KeepAlive thread error");
                }
            }
        }
    }
}
