using System.Text.Json;
using Auto.Messages;
using Microsoft.AspNetCore.SignalR.Client;
using Newtonsoft.Json;
using JsonSerializer = System.Text.Json.JsonSerializer;
using Auto.Core.Entities;
using EasyNetQ;

namespace Auto.Notifier
{
    internal class Program
    {
        const string SIGNALR_HUB_URL = "http://localhost:7041/hub";
        private static HubConnection hub;

        static async Task Main(string[] args)
        {
            hub = new HubConnectionBuilder().WithUrl(SIGNALR_HUB_URL).Build();
            await hub.StartAsync();
            Console.WriteLine("Hub started!");
            Console.WriteLine("Press any key to send a message (Ctrl-C to quit)");
            var amqp = "amqp://user:rabbitmq@localhost:5672";
            using var bus = RabbitHutch.CreateBus(amqp);
            Console.WriteLine("Connected to bus, listening to NewOwnerMessages");
            var subscriberId = $"Auto.Notifier@{Environment.MachineName}";
            await bus.PubSub.SubscribeAsync<NewVehicleOwnerMessage>(subscriberId, HandleNewOwnerMessage);
            Console.ReadLine();
        }

        private static async void HandleNewOwnerMessage(NewVehicleOwnerMessage nopm)
        {
            var csvRow = $"{nopm.ModelCode} {nopm.Year} {nopm.Registration}: {nopm.FirstName} {nopm.LastName} {nopm.Email}";
            Console.WriteLine(csvRow);
            var json = JsonSerializer.Serialize(nopm, JsonSettings());
            await hub.SendAsync("NotifyWebUsers", "Auto.Notifier", json);
        }

        static JsonSerializerOptions JsonSettings() =>
            new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };
    }
}