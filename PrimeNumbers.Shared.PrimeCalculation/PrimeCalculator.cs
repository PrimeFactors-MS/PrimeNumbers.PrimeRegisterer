using System;
using System.Collections.Generic;

namespace PrimeNumbers.Shared.PrimeCalculation
{
    public static class PrimeCalculator
    {
        public static PrimeCalculationResult GetPrimes(ulong number)
        {
            List<ulong> primes = new List<ulong>();
            PrimeCalculationResult result = new PrimeCalculationResult();
            ulong curNumber = number;

            while (curNumber > 1)
            {
                ulong curDivider = FindLowestDivider(curNumber);
                primes.Add(curDivider);
                curNumber /= curDivider;
            }

            if(IsNumberPrime(primes))
            {
                result.IsPrime = true;
                result.Primes = null;
            }
            else
            {
                result.IsPrime = false;
                result.Primes = primes.ToArray();
            }

            return result;
        }

        /// Returns if a number is prime by its prime dividors
        private static bool IsNumberPrime(List<ulong> primesOfNumber) => primesOfNumber.Count == 1;

        private static ulong FindLowestDivider(ulong number)
        {
            ulong divider = number;
            for (ulong i = 2; i < number / 2 + 1; i++)
            {
                if (number % i == 0)
                {
                    divider = i;
                    break;
                }
            }
            return divider;
        }
    }
}
