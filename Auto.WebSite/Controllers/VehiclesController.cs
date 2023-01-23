using System.Linq;
using Auto.Core.Entities;
using Auto.Data;
using Auto.WebSite.Models;
using Microsoft.AspNetCore.Mvc;

namespace Auto.WebSite.Controllers;

public class VehiclesController : Controller {
    private readonly IAutoStorage _db;

    public VehiclesController(IAutoStorage db) {
        this._db = db;
    }
    public IActionResult Index() {
        var vehicles = _db.ListVehicles();
        return View(vehicles);
    }

    public IActionResult Details(string id) {
        var vehicle = _db.FindVehicle(id);
        return View(vehicle);
    }

    [HttpGet]
    public IActionResult Advertise(string id) {
        var vehicleModel = _db.FindModel(id);
        var dto = new VehicleDto() {
            ModelCode = vehicleModel.Code,
            ModelName = $"{vehicleModel.Manufacturer.Name} {vehicleModel.Name}"
        };
        return View(dto);
    }

    [HttpPost]
    public IActionResult Advertise(VehicleDto dto) {
        var existingVehicle = _db.FindVehicle(dto.Registration);
        if (existingVehicle != default)
            ModelState.AddModelError(nameof(dto.Registration),
                "That registration is already listed in our database.");
        var vehicleModel = _db.FindModel(dto.ModelCode);
        if (vehicleModel == default) {
            ModelState.AddModelError(nameof(dto.ModelCode),
                $"Sorry, {dto.ModelCode} is not a valid model code.");
        }
        if (!ModelState.IsValid) return View(dto);
        var vehicle = new Vehicle() {
            Registration = dto.Registration,
            Color = dto.Color,
            VehicleModel = vehicleModel,
            Year = dto.Year
        };
        _db.CreateVehicle(vehicle);
        return RedirectToAction("Details", new { id = vehicle.Registration });
    }

    public static string ParseVehicleId(dynamic href)
    {
        var tokens = ((string)href).Split("/");
        return tokens.Last();
    }
}