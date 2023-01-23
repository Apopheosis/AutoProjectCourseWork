using Auto.Data;
using Auto.PricingServerEngine;
using Grpc.Core;
namespace Auto.PricingServer.Services;


public class PricerService : VehicleService.VehicleServiceBase {
    private readonly IAutoStorage _db;

    public PricerService(IAutoStorage db)
    {
        _db = db;
    }
    

    public override Task<VehicleCodeReply> GetVehicle(VehicleCodeRequest request, ServerCallContext context) {
        var vehicle = _db.FindVehicle(request.VehicleRegistration);
        Console.WriteLine(request.VehicleRegistration);
        return Task.FromResult(new VehicleCodeReply()
        {
            ModelCode = vehicle.ModelCode,
            Year = vehicle.Year,
            Registration = vehicle.Registration
        });
    }

}