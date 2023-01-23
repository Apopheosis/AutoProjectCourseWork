namespace Auto.Messages;

public class NewVehicleOwnerMessage
{
    public string ModelCode { get; set; }
    public int Year { get; set; }
    public string Registration { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    
    public NewVehicleOwnerMessage() {}

    public NewVehicleOwnerMessage(NewOwnerMessage msg, string modelCode, int year, string registration)
    {
        this.ModelCode = modelCode;
        this.Year = year;
        this.Registration = registration;
        this.FirstName = msg.FirstName;
        this.LastName = msg.LastName;
        this.Email = msg.Email;
    }
}