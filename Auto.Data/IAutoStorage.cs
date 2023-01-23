using Auto.Core.Entities;

namespace Auto.Data;

public interface IAutoStorage
{
    public int CountVehicles();
    public int CountOwners();
    public IEnumerable<Vehicle> ListVehicles();
    public IEnumerable<Manufacturer> ListManufacturers();
    public IEnumerable<Model> ListModels();
    public IEnumerable<Owner> ListOwners();

    public Vehicle FindVehicle(string registration);
    public Model FindModel(string code);
    public Owner FindOwner(string id);
    public Manufacturer FindManufacturer(string code);

    public void CreateVehicle(Vehicle vehicle);
    public void UpdateVehicle(Vehicle vehicle);
    public void DeleteVehicle(Vehicle vehicle);
    public string CreateOwner(Owner owner);
    public void UpdateOwner(Owner owner);
    public void DeleteOwner(string id);
}