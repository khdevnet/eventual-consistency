﻿using Common;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;

namespace OrdersClient
{
    class Program
    {
        public static void Main()
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.ExchangeDeclare(exchange: "events", type: ExchangeType.Fanout, durable: true);

                var queueName = channel.QueueDeclare("orders-service", durable: true, exclusive: false, autoDelete: false).QueueName;
                channel.QueueBind(queue: queueName,
                                  exchange: "events",
                                  routingKey: "");

                Console.WriteLine(" [*] Waiting for events.");

                var consumer = new EventingBasicConsumer(channel);
                consumer.Received += (model, ea) =>
                {
                    var body = ea.Body.ToArray();
                    var message = Encoding.UTF8.GetString(body);
                    PrintMessage(message);

                    channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);

                    Console.WriteLine(" {0}: Processed", message);
                };
                channel.BasicConsume(queue: queueName,
                                     autoAck: false,
                                     consumer: consumer);

                Console.WriteLine(" Press [enter] to exit.");
                Console.ReadLine();
            }
        }

        private static void PrintMessage(string message)
        {
            switch (message)
            {
                case ClientEvents.CustomerUpdatedEvent:
                    Console.WriteLine(" {0}: Customer Entity Updated", message);
                    break;

                case ClientEvents.CustomerRemovedEvent:
                    Console.WriteLine(" {0}: Customer Orders Archieved", message);
                    break;

                default:
                    Console.WriteLine(" {0}: skiped", message);
                    break;
            }
        }
    }
}
