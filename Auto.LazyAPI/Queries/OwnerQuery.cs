using Auto.Core.Entities;
using Auto.Data;

namespace Auto.LazyAPI.Queries;

public class OwnerQuery {
    private readonly IAutoStorage _db;

    public OwnerQuery(IAutoStorage db) {
        this._db = db;
    }
    
    public IEnumerable<Owner> GetOwners() => _db.ListOwners();

    public Owner GetOwner(string id) => _db.FindOwner(id);

    public IEnumerable<Owner> GetOwnersByEmail(string email) => 
        _db.ListOwners().Where(v => v.Email.Contains(email, StringComparison.InvariantCultureIgnoreCase));
      
}