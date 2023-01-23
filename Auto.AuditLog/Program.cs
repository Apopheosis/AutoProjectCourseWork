using EasyNetQ;
using Microsoft.Extensions.Configuration;
using Auto.Messages;
using Auto.PricingServer;
using Auto.PricingServer.Services;

namespace Auto.AuditLog {
    internal class Program
    {
        private static PricerService _grpcClient;
        private static readonly IConfigurationRoot config = ReadConfiguration();
        private static IBus _bus;

        static async Task Main(string[] args) {
            Console.WriteLine("Starting Auto.AuditLog");
            var amqp = "amqp://user:rabbitmq@localhost:5672";
            using var bus = RabbitHutch.CreateBus(amqp);
            Console.WriteLine("Connected to bus! Listening for newOwnerMessages");
            var subscriberId = $"Auto.AuditLog@{Environment.MachineName}";
            await bus.PubSub.SubscribeAsync<NewOwnerMessage>(subscriberId, HandleNewOwnerMessage);
            await bus.PubSub.SubscribeAsync<NewVehicleMessage>(subscriberId, HandleNewVehicleMessage);
            Console.ReadLine();
        }

        private static async Task HandleNewOwnerMessage(NewOwnerMessage nom)
        {
            var csv = $"{nom.Id} - {nom.FirstName} {nom.LastName} {nom.Email}";
            Console.WriteLine(csv);
        }

        private static async Task HandleNewVehicleMessage(NewVehicleMessage nvm)
        {
            var csv = $"{nvm.Registration} {nvm.ManufacturerName} {nvm.ModelName} {nvm.Color} {nvm.Year}";
            Console.WriteLine(csv);
        }

        private static IConfigurationRoot ReadConfiguration() {
            var basePath = Directory.GetParent(AppContext.BaseDirectory).FullName;
            return new ConfigurationBuilder()
                .SetBasePath(basePath)
                .AddJsonFile("appsettings.json")
                .AddEnvironmentVariables()
                .Build();
        }
    }
}