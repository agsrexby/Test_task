namespace WebApplication2.Models;

public class InfoOfDocInCar
{
    public Boolean HasFolder { get; set; }
    public Boolean HasSts { get; set; }
    public Boolean HasOsago { get; set; }
    public Boolean HasCardOfFuel { get; set; }
    public int RefuelingId { get; set; }
    public int Id { get; set; }
    
    public Refueling? RefuelDoc { get; set; }  
}