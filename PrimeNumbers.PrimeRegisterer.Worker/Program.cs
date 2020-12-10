using PrimeNumbers.Shared.PrimeCalculation;
using System;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace PrimeNumbers.PrimeRegisterer.Worker
{
    class Program
    {
        static void Main(string[] _)
        {
            Random random = new();
            ulong number = (ulong)random.Next();
            var primeResult = PrimeCalculator.GetPrimes(number);

            var a = SubmitPrimeNumber(new PrimeRecord(number, primeResult.IsPrime, primeResult.Primes)).Result;
            Console.WriteLine(a);
        }



        private static async Task<bool> SubmitPrimeNumber(PrimeRecord record)
        {
            HttpClient client = new HttpClient()
            {
                BaseAddress = new Uri("http://192.168.1.18:30006")
            };

            HttpContent content;
            using (MemoryStream jsonStream = new ())
            using (StreamReader reader = new (jsonStream, Encoding.UTF8))
            {
                await JsonSerializer.SerializeAsync(jsonStream, record);
                jsonStream.Position = 0;
                string jsonString = await reader.ReadToEndAsync();
                content = new StringContent(jsonString, Encoding.UTF8, "application/json");
            }
            HttpResponseMessage response = await client.PostAsync("Primes", content);

            return response.IsSuccessStatusCode;
        }
    }
}
