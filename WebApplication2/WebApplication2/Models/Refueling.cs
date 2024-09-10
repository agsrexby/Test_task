namespace WebApplication2.Models;

public class Refueling
{
    public int Id { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime? InspectionStartDate { get; set; }
    public DateTime? InspectionEndDate { get; set; }
    public DateTime? EndDate { get; set; }
    public string Status { get; set; } // Ждет заправку, осмотр, заправка, завершен, отменен, сохранены фотографии
    public int FuelAmount { get; set; }
    public int CarId { get; set; }

    public Car? Car { get; set; }
    public List<InfoOfDocInCar> Documents { get; set; }
     
    public List<Photo> Photos { get; set; }

}