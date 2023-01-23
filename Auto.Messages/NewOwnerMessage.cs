namespace Auto.Messages;

public class NewOwnerMessage
{
    public string Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public string VehicleRegistration { get; set; }

    public NewOwnerMessage() {
    }

    public NewOwnerMessage(string firstName, string lastName, string email)
    {
        this.FirstName = firstName;
        this.LastName = lastName;
        this.Email = email;
    }
}