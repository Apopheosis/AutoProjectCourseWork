using System.Reflection;
using Auto.Core.Entities;
using static System.Int32;
using Microsoft.Extensions.Logging;

namespace Auto.Data;

public class AutoCsvFileStorage : IAutoStorage {
        private static readonly IEqualityComparer<string> collation = StringComparer.OrdinalIgnoreCase;

        private readonly Dictionary<string, Manufacturer> manufacturers = new Dictionary<string, Manufacturer>(collation);
        private readonly Dictionary<string, Model> models = new Dictionary<string, Model>(collation);
        private readonly Dictionary<string, Vehicle> vehicles = new Dictionary<string, Vehicle>(collation);
        private readonly Dictionary<string, Owner> owners = new Dictionary<string, Owner>(collation);
        private readonly ILogger<AutoCsvFileStorage> logger;
        private static int ownersIndexCount;
        
        public AutoCsvFileStorage(ILogger<AutoCsvFileStorage> logger) {
            this.logger = logger;
            ReadManufacturersFromCsvFile("manufacturers.csv");
            ReadModelsFromCsvFile("models.csv");
            ReadVehiclesFromCsvFile("vehicles.csv");
            ReadOwnersFromCsvFile("owners.csv");
            ownersIndexCount = owners.Count;
            ResolveReferences();
        }

        private void ResolveReferences() {
            foreach (var mfr in manufacturers.Values) {
                mfr.Models = models.Values.Where(m => m.ManufacturerCode == mfr.Code).ToList();
                foreach (var model in mfr.Models) model.Manufacturer = mfr;
            }

            foreach (var model in models.Values) {
                model.Vehicles = vehicles.Values.Where(v => v.ModelCode == model.Code).ToList();
                foreach (var vehicle in model.Vehicles) vehicle.VehicleModel = model;
            }

            foreach (var owner in owners.Values)
            {
                owner.Vehicle = vehicles[owner.VehicleRegistration];
                vehicles[owner.VehicleRegistration].Owner = owner;
            }
        }

        private string ResolveCsvFilePath(string filename) {
            var directory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var csvFilePath = Path.Combine(directory, "csv-data");
            return Path.Combine(csvFilePath, filename);
        }

        private void ReadVehiclesFromCsvFile(string filename) {
            var filePath = ResolveCsvFilePath(filename);
            foreach (var line in File.ReadAllLines(filePath)) {
                var tokens = line.Split(",");
                var vehicle = new Vehicle {
                    Registration = tokens[0],
                    ModelCode = tokens[1],
                    Color = tokens[2]
                };
                if (TryParse(tokens[3], out var year)) vehicle.Year = year;
                vehicles[vehicle.Registration] = vehicle;
            }
            logger.LogInformation($"Loaded {vehicles.Count} models from {filePath}");
        }

        private void ReadModelsFromCsvFile(string filename) {
            var filePath = ResolveCsvFilePath(filename);
            foreach (var line in File.ReadAllLines(filePath)) {
                var tokens = line.Split(",");
                var model = new Model {
                    Code = tokens[0],
                    ManufacturerCode = tokens[1],
                    Name = tokens[2]
                };
                models.Add(model.Code, model);
            }
            logger.LogInformation($"Loaded {models.Count} models from {filePath}");
        }

        private void ReadManufacturersFromCsvFile(string filename) {
            var filePath = ResolveCsvFilePath(filename);
            foreach (var line in File.ReadAllLines(filePath)) {
                var tokens = line.Split(",");
                var mfr = new Manufacturer {
                    Code = tokens[0],
                    Name = tokens[1]
                };
                manufacturers.Add(mfr.Code, mfr);
            }
            logger.LogInformation($"Loaded {manufacturers.Count} manufacturers from {filePath}");
        }
        private void ReadOwnersFromCsvFile(string filename)
        {
            var filePath = ResolveCsvFilePath(filename);
            Console.WriteLine("owners: "+ filename);
            foreach (var line in File.ReadAllLines(filePath))
            {
                var tokens = line.Split(",");
                var owr = new Owner
                {
                    Id = tokens[0],
                    FirstName = tokens[1],
                    LastName = tokens[2],
                    Email = tokens[3],
                    VehicleRegistration = tokens[4],
                };
                owners.Add(owr.Id,owr);
            }
            logger.LogInformation($"Loaded{owners.Count} manufacturers from {filePath}" );
        }

        public int CountVehicles() => vehicles.Count;
        public int CountOwners() => owners.Count;

        public IEnumerable<Vehicle> ListVehicles() => vehicles.Values;

        public IEnumerable<Manufacturer> ListManufacturers() => manufacturers.Values;

        public IEnumerable<Model> ListModels() => models.Values;
        public IEnumerable<Owner> ListOwners() => owners.Values;
        public Vehicle FindVehicle(string registration) => vehicles.GetValueOrDefault(registration);
        public Owner FindOwner(string id) => owners.GetValueOrDefault(id);
        
        public Model FindModel(string code) => models.GetValueOrDefault(code);

        public Manufacturer FindManufacturer(string code) => manufacturers.GetValueOrDefault(code);

        public void CreateVehicle(Vehicle vehicle) {
            vehicle.VehicleModel.Vehicles.Add(vehicle);
            vehicle.ModelCode = vehicle.VehicleModel.Code;
            UpdateVehicle(vehicle);
        }

        public void UpdateVehicle(Vehicle vehicle) {
            vehicles[vehicle.Registration] = vehicle;
        }

        public void DeleteVehicle(Vehicle vehicle) {
            var model = FindModel(vehicle.ModelCode);
            model.Vehicles.Remove(vehicle);
            vehicles.Remove(vehicle.Registration);
        }
        
        public string CreateOwner(Owner owner)
        {
            ownersIndexCount++;
            owner.Id = ownersIndexCount.ToString();
            vehicles[owner.VehicleRegistration].Owner = owner;
            owners.Add(ownersIndexCount.ToString(),owner);
            owner.Vehicle = vehicles[owner.VehicleRegistration];
            return ownersIndexCount.ToString();
        }
        
        public void UpdateOwner(Owner owner)
        {
            owners[owner.Id] = owner;
        }

        public void DeleteOwner(string id)
        {
            vehicles[owners[id].VehicleRegistration].Owner = null;
            owners.Remove(id);
        }
    }