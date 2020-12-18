using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace PrimeNumbers.PrimeRegisterer.Core
{
    public class PrimeResultSubmitter : IDisposable
    {
        private readonly HttpClient _client;

        private PrimeResultSubmitter(HttpClient client)
        {
            _client = client;
        }

        public static PrimeResultSubmitter CreateNew(Uri connectionUri)
        {
            HttpClient client = new ()
            {
                BaseAddress = connectionUri
            };
            return new PrimeResultSubmitter(client);
        }

        public async Task<bool> SubmitPrimeNumber(PrimeRecord record)
        {
            HttpContent content;
            using (MemoryStream jsonStream = new())
            using (StreamReader reader = new(jsonStream, Encoding.UTF8))
            {
                await JsonSerializer.SerializeAsync(jsonStream, record);
                jsonStream.Position = 0;
                string jsonString = await reader.ReadToEndAsync().ConfigureAwait(false);
                content = new StringContent(jsonString, Encoding.UTF8, "application/json");
            }
            HttpResponseMessage response = await _client.PostAsync("Primes", content);

            return response.IsSuccessStatusCode;
        }

        public void Dispose()
        {
            _client.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
