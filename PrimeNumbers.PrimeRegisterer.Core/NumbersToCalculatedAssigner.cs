using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrimeNumbers.PrimeRegisterer.Core
{
    public record NumberRange(ulong Start, ulong End);
    public class NumbersToCalculatedAssigner
    {
        public NumberRange GetNumbersRange() => new NumberRange(1, 5000);
    }
}
