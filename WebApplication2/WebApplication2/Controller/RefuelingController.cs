using System.Reflection.Metadata;
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

    [HttpPost("CreateRefueling")]
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
    
    [HttpPost("CreateInspection")]
    public async Task<ActionResult<Refueling>> CreateInspection( int refuelId)
    {
        var refuel = _context.Refuels.Include(r => r.Car).FirstOrDefault(r => r.Id == refuelId);
        
        if (refuel == null)
        {
            throw new Exception("Заправка не найдена.");
        }
        
        if (refuel.Status != "ждет заправку")
        {
            throw new Exception("Заправку можно перевести в статус осмотр только из статуса «ждет заправку».");
        }
        
        refuel.InspectionStartDate = DateTime.UtcNow;
        refuel.Status = "осмотр";
        
        if (refuel.Car != null && refuel.Car.Status == "ждет заправку")
        {
            refuel.Car.Status = "осмотр";
        }
        _context.SaveChanges();

        return refuel;
    }
    
    // [HttpPost("savePhoto")]
    // public async Task<ActionResult<Photo>> AddPhoto(int refuelId, string photoType, IFormFile file)
    // {
    //     var refuel = _context.Refuels.FirstOrDefault(r => r.Id == refuelId);
    //     
    //     if (refuel == null)
    //     {
    //         throw new Exception("Заправка не найдена.");
    //     }
    //     
    //     if (!IsValidPhotoType(refuel.Status, photoType))
    //     {
    //         throw new Exception("Тип фотографии не соответствует статусу заправки.");
    //     }
    //     
    //     var filePath = Path.Combine("../PhotoOfInspecting", file.FileName);
    //     using (var stream = new FileStream(filePath, FileMode.Create))
    //     {
    //         file.CopyTo(stream);
    //     }
    //
    //     
    //     var photo = new Photo
    //     {
    //         RefuelingId = refuelId,
    //         Type = photoType,
    //         Url = filePath,
    //         CreatedAt = DateTime.UtcNow
    //     };
    //     
    //     _context.Photos.Add(photo);
    //     _context.SaveChanges();
    //
    //     return photo;
    // }

    private bool IsValidPhotoType(string status, string photoType)
    {
        return (status == "осмотр" && photoType == "осмотр заправки");
    }
    
    
    [HttpPost("endInspection")]
    public async Task<ActionResult<Refueling>> EndInspection(int refuelingId, int docId, bool hasFolder, bool hasSts, bool hasOsago, bool hasCard)
    {
        var refuel = await _context.Refuels.FindAsync(refuelingId);
        if (refuel == null)
        {
            return NotFound("Заправка не найдена.");
        }
    
        if (refuel.Status != "осмотр")
        {
            return Conflict("Заполнение осмотра возможно только из статуса 'осмотр'.");
        }
    
        refuel.InspectionEndDate = DateTime.UtcNow;
        refuel.Status = "заправка";

        var docInfo = new InfoOfDocInCar
        {
            Id = docId,
            HasFolder = hasFolder,
            HasSts = hasSts,
            HasCardOfFuel = hasCard,
            HasOsago = hasOsago,
            RefuelingId = refuelingId
        };

        _context.Documents.Add(docInfo);
        await _context.SaveChangesAsync();

        await _context.Cars.Where(e => e.Id == refuel.CarId)
            .ExecuteUpdateAsync(s => s.SetProperty(e => e.Status, "заправка"));
    
        return Ok(refuel);
    }
    
    [HttpPost("compliteRefueling")]
    public async Task<Refueling> CompleteRefueling(int refuelingId, int fuelAmount)
    {
        var refueling = await _context.Refuels.FindAsync(refuelingId);
    
        if (refueling == null)
        {
            throw new Exception("Заправка не найдена.");
        }
        
        if (refueling.Status != "заправка")
        {
            throw new Exception("Заправку можно завершить только в статусе 'заправка'.");
        }
        
        refueling.EndDate = DateTime.UtcNow;
        refueling.Status = "завершена";
        refueling.FuelAmount = fuelAmount;
        
        var car = await _context.Cars.FindAsync(refueling.CarId);
        if (car != null)
        {
            car.Status = "свободен";
        }
        
        await _context.SaveChangesAsync();
        
        return refueling;
    }
    
    [HttpPost("cancelRefueling")]
    public async Task<Refueling> CancelRefueling(int refuelingId)
    {
        var refueling = await _context.Refuels.FindAsync(refuelingId);
    
        if (refueling == null)
        {
            throw new Exception("Заправка не найдена.");
        }
        
        if (refueling.Status != "ждет заправку" && refueling.Status != "осмотр")
        {
            throw new Exception("Заправку можно отменить только в статусе 'ждет заправку' или 'осмотр'.");
        }
        
        refueling.EndDate = DateTime.UtcNow;
        refueling.Status = "отменена";
        
        var car = await _context.Cars.FindAsync(refueling.CarId);
        if (car != null)
        {
            car.Status = "свободен";
        }

        await _context.SaveChangesAsync();

        return refueling;
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
    
    [HttpGet("dateDoc")]
    public async Task<List<InfoOfDocInCar>> GetDoc()
    {
        return await _context.Documents
            .AsNoTracking()
            .ToListAsync();
    }
    
    [HttpGet("datePhoto")]
    public async Task<List<Photo>> GetPhoto()
    {
        return await _context.Photos
            .AsNoTracking()
            .ToListAsync();
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
    
    [HttpDelete("deleteDoc")]
    public async Task DeleteDoc(int id)
    {
        await _context.Documents
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