using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrimeNumbers.PrimeRegisterer.Core
{
    public class PrimeRegistererHandler
    {
        private readonly PrimeRangeCalculator _calculator;
        private readonly PrimeResultSubmitter _resultSubmitter;
        private readonly NumbersToCalculatedAssigner _numbersAssigner;

        public PrimeRegistererHandler(PrimeRangeCalculator calculator, PrimeResultSubmitter resultSubmitter,
                                      NumbersToCalculatedAssigner numbersAssigner)
        {
            _calculator = calculator;
            _resultSubmitter = resultSubmitter;
            _numbersAssigner = numbersAssigner;
            _calculator.PrimesCalculated += OnPrimeCalculated;
        }

        private void OnPrimeCalculated(PrimeRecord record)
        {
            _resultSubmitter.SubmitPrimeNumber(record).ContinueWith(task =>
            {
                if (task.Exception != null) Console.WriteLine(task.Exception + "\n\n\n");
            }).Wait(); ;
        }

        public void Start()
        {
            NumberRange range = _numbersAssigner.GetNumbersRange();
            _calculator.CalculatePrimes(range.Start, range.End);
        }

        public void Stop()
        {
            _calculator.Stop();
        }
    }
}
