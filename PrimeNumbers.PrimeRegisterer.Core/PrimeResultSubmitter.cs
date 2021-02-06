using RabbitMQ.Client;
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
    public sealed class PrimeResultSubmitter : IDisposable
    {
        private readonly string _exchangeName;
        private readonly IConnection _connection;
        private readonly IModel _channel;

        private PrimeResultSubmitter(IConnection rmqConnection)
        {
            _exchangeName = "Exch.PrimeRegisterer.Results";
            _connection = rmqConnection;
            _channel = rmqConnection.CreateModel();
        }

        public static PrimeResultSubmitter CreateNew(string hostname, int port)
        {
            ConnectionFactory connectionFactory = new()
            {
                HostName = hostname,
                Port = port,
                VirtualHost = "primeRegistration",
                UserName = "guest",
                Password = "guest",
            };
            IConnection connection = connectionFactory.CreateConnection();

            return new PrimeResultSubmitter(connection);
        }

        public void SubmitPrimeNumber(PrimeRecordReport recordReport)
        {
            byte[] jsonBytes = JsonSerializer.SerializeToUtf8Bytes(recordReport);
            _channel.BasicPublish(_exchangeName, "", body: jsonBytes);
        }

        public void Dispose()
        {
            _connection.Dispose();
        }
    }
}
