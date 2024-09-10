namespace WebApplication2.Models;

public class Photo
{
    public int Id { get; set; }
    public DateTime CreatedAt { get; set; }
    public string Url { get; set; }
    public string Type { get; set; } // Осмотр заправки, завершение заправки
    public int RefuelingId { get; set; }

    public Refueling Refuel { get; set; }

}