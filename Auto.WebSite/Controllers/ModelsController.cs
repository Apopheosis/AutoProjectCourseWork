using System.Linq;
using Auto.Data;
using Microsoft.AspNetCore.Mvc;

namespace Auto.WebSite.Controllers;

public class ModelsController : Controller {
    private readonly IAutoStorage _db;

    public ModelsController(IAutoStorage db) {
        this._db = db;
    }

    public IActionResult Vehicles(string id) {
        var model = _db.ListModels().FirstOrDefault(m => m.Code == id);
        return View(model);
    }

    public IActionResult Index() {
        var models = _db.ListModels();
        return View(models);
    }
}