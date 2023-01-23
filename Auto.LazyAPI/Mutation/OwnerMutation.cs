using Auto.Core.Entities;
using Auto.Data;

namespace Auto.LazyAPI.Mutation;

public class OwnerMutation
{
    private readonly IAutoStorage _db;

    public OwnerMutation(IAutoStorage db)
    {
        _db = db;
    }

    public async Task<Owner> CreateOwner(string id, string email, string firstname, string lastname)
    {
        var owner = new Owner
        {
            Id = id,
            Email = email,
            FirstName = firstname,
            LastName = lastname
        };
        _db.CreateOwner(owner);
        return owner;
    }
}