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
    public class NumbersToCalculateAssigner
    {
        private readonly HttpClient _client;
        private NumberRange _workerRange;
        private uint _workerId;


        private NumbersToCalculateAssigner(HttpClient client)
        {
            _client = client;
        }

        public static NumbersToCalculateAssigner CreateNew(Uri connectionUri)
        {
            HttpClient client = new()
            {
                BaseAddress = connectionUri
            };
            return new NumbersToCalculateAssigner(client);
        }
        public async Task<NumberRange> LoadNumbersRange()
        {
            HttpResponseMessage response = await _client.GetAsync("/NumbersAssignment/GetNumberAssignment");

            Stream content = response.Content.ReadAsStream();
            using StreamReader reader = new(content, Encoding.UTF8);
            NumberRangeResponse result = JsonSerializer.Deserialize<NumberRangeResponse>(reader.ReadToEnd());

            _workerRange = result.Range;
            _workerId = result.WorkerId;
            return result.Range;
        }

        public Task SendKeepAlive()
        {
            StringContent body = new($"{{ \"WorkerId\": {_workerId} }}", Encoding.UTF8, "application/json");
            return _client.PostAsync("/NumbersAssignment/SendKeepAlive", body);

        }

        public Task FinishWorkingRange()
        {
            StringContent body = new($"{{ \"WorkerId\": {_workerId} }}", Encoding.UTF8, "application/json");
            return _client.PostAsync("/NumbersAssignment/FinishedAssignment", body);
        }
    }
}
