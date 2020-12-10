using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrimeNumbers.Shared.PrimeCalculation
{
        public record PrimeRecord(ulong Number, bool IsPrime, ulong[] PrimeFactors);
}
