using Auto.Messages;

using Auto.PricingServerEngine;
using EasyNetQ;
using Grpc.Net.Client;

class Program
{
    private static VehicleService.VehicleServiceClient grpcClient;
    private static IBus bus;

    static async Task Main(string[] args)
    {
        Console.WriteLine("Starting Auto.OwnerClient");
        var amqp = "amqp://user:rabbitmq@localhost:5672";
        bus = RabbitHutch.CreateBus(amqp);
        Console.WriteLine("Connected to bus; Listening for newOwnerMessages");
        var grpcAddress = "http://localhost:5297";
        using var channel = GrpcChannel.ForAddress(grpcAddress);
        grpcClient = new VehicleService.VehicleServiceClient(channel);
        Console.WriteLine($"Connected to gRPC on {grpcAddress}");
        var subscriberId = $"Auto.OwnerClient@{Environment.MachineName}";
        await bus.PubSub.SubscribeAsync<NewOwnerMessage>(subscriberId, HandleNewOwnerMessage);
        Console.ReadLine();
    }

    private static async Task HandleNewOwnerMessage(NewOwnerMessage msg)
    {
        Console.WriteLine($"new owner; {msg.FirstName} {msg.VehicleRegistration}");
        var vehicleRequest = new VehicleCodeRequest()
        {
            VehicleRegistration = msg.VehicleRegistration
        };
        var vehicleReply = await grpcClient.GetVehicleAsync(vehicleRequest);
        
        var newOwnerMessage = new NewVehicleOwnerMessage(msg, vehicleReply.ModelCode, vehicleReply.Year, vehicleReply.Registration);
        
        Console.WriteLine($"Owner {msg.FirstName} {msg.LastName} with e-mail {msg.Email} has a car with code {msg.VehicleRegistration}");
        await bus.PubSub.PublishAsync(newOwnerMessage);
    }

}