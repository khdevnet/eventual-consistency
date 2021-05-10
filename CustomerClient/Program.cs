using Common;
using RabbitMQ.Client;
using System;
using System.Text;
using System.Threading;

namespace CustomerClient
{
    class Program
    {
        private static readonly Random _random = new Random();
        private static Timer _userEmulatorTimer;
        private static ConnectionFactory _factory;
        private static IConnection _connection;
        private static IModel _channel;

        public static void Main(string[] args)
        {
            // send each second
            _userEmulatorTimer = new Timer(SendRandomEvent, new AutoResetEvent(false), 1000, 3000);

            _factory = new ConnectionFactory() { HostName = "localhost" };
            _connection = _factory.CreateConnection();
            _channel = _connection.CreateModel();

            _channel.ExchangeDeclare(exchange: "events", type: ExchangeType.Fanout, durable: true);


            Console.WriteLine(" Press [enter] to exit.");
            Console.ReadLine();

            _userEmulatorTimer?.Dispose();
            _connection?.Dispose();
            _channel?.Dispose();
        }

        private static void SendRandomEvent(Object stateInfo)
        {
            var eventName = GetRandomEvent();

            Console.WriteLine("{0} [x] Sent {1}.",
                DateTime.Now.ToString("h:mm:ss.fff"),
                eventName);

            PublishMessage(_channel, eventName);
        }

        private static void PublishMessage(IModel channel, string message)
        {
            var body = Encoding.UTF8.GetBytes(message);
            channel.BasicPublish(exchange: "events",
                                 routingKey: "",
                                 basicProperties: null,
                                 body: body);
        }

        private static string GetRandomEvent()
            => RandomNumber() == 1
            ? ClientEvents.CustomerUpdatedEvent
            : ClientEvents.CustomerRemovedEvent;

        public static int RandomNumber() => _random.Next(1, 3);
    }
}
