using System.Text.Json.Serialization;

namespace WebApplication2.Models;

public class Car
{
    public int Id { get; set; }
    public string Number { get; set; }
    public int Region { get; set; }
    public string Status { get; set; } // Свободен, ждет заправку, осмотр, заправка
    
    [JsonIgnore]
    public List<Refueling> Refuels { get; set; }
}