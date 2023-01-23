using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Auto.Data;

namespace Auto.WebSite.Controllers;

public class ManufacturersController : Controller {
    private readonly IAutoStorage _db;

    public ManufacturersController(IAutoStorage db) {
        this._db = db;
    }
    public IActionResult Index() {
        var vehicles = _db.ListManufacturers();
        return View(vehicles);
    }

    public IActionResult Models(string id) {
        var manufacturer = _db.ListManufacturers().FirstOrDefault(m => m.Code == id);
        return View(manufacturer);
    }
}