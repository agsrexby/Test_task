using System.Runtime.InteropServices.JavaScript;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication2.Models;

namespace WebApplication2.Controller;

[Route("[controller]")]
[ApiController]
public class RefuelingController : ControllerBase
{
    private readonly RefuelingDbContext _context;

    public RefuelingController(RefuelingDbContext context)
    {
        _context = context;
    }

    [HttpPost]
    public async Task<ActionResult<Refueling>> CreateRefueling( int carId)
    {
        var car = await _context.Cars.FindAsync(carId);
        if (car == null || car.Status != "свободен")
        {
            return BadRequest("Невозможно создать заправку");
        }

        var existingRefueling = await _context.Refuels
            .AnyAsync(r => r.CarId == carId && (r.Status == "ждет заправку" || r.Status == "заправка"));

        if (existingRefueling)
        {
            return BadRequest("Активная заправка уже существует");
        }

        var refueling = new Refueling
        {
            CarId = carId,
            StartDate = DateTime.UtcNow,
            Status = "ждет заправку"
        };

        _context.Refuels.Add(refueling);
        car.Status = "ждет заправку";
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(CreateRefueling), new { id = refueling.Id }, refueling);
    }

    [HttpGet("dateCar")]
    public async Task<List<Car>> GetCar()
    {
        return await _context.Cars
            .AsNoTracking()
            .ToListAsync();
    }
    [HttpGet("dateRefuel")]
    public async Task<List<Refueling>> GetRefuel()
    {
        return await _context.Refuels
            .AsNoTracking()
            .ToListAsync();
    }

    [HttpPost("carAdd")]
    public async Task Add(int id, string number, int region, string status)
    {
        var car = new Car
        {
            Id = id,
            Number = number,
            Region = region,
            Status = status
        };

        await _context.AddAsync(car);
        await _context.SaveChangesAsync();
    }

    [HttpPut("updateCar")]
    public async Task UpdateCar(int id,int carid, string number, int region, string status)
    {
        await _context.Cars
            .Where(e => e.Id == id)
            .ExecuteUpdateAsync(s => s
                .SetProperty(e => e.Id, carid)
                .SetProperty(e => e.Number, number)
                .SetProperty(e => e.Status, status)
                .SetProperty(e => e.Region, region));
    }
    [HttpPut("updateRefuels")]
    public async Task UpdateRefuels(int id, int refuelid, string status, float fuelamount, int carid)
    {
        await _context.Refuels
            .Where(e => e.Id == id)
            .ExecuteUpdateAsync(s => s
                .SetProperty(e => e.Id, refuelid)
                .SetProperty(e => e.FuelAmount, fuelamount)
                .SetProperty(e => e.Status, status)
                .SetProperty(e => e.CarId, carid));
    }
    [HttpDelete("deleteCar")]
    public async Task DeleteCar(int id)
    {
        await _context.Cars
            .Where(e => e.Id == id)
            .ExecuteDeleteAsync();
    }
    [HttpDelete("deleteRefuel")]
    public async Task DeleteRefuel(int id)
    {
        await _context.Refuels
            .Where(e => e.Id == id)
            .ExecuteDeleteAsync();
    }
}