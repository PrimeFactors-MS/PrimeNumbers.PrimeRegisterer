using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrimeNumbers.PrimeRegisterer.Core
{
        public record PrimeRecord(ulong Number, bool IsPrime, ulong[] PrimeFactors);
}
