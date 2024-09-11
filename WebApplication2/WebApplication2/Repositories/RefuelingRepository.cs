// using WebApplication2.Migrations;
// using WebApplication2.Models;
//
// namespace WebApplication2.Repositories;
//
// public class RefuelingRepository
// {
//     private readonly RefuelingDbContext _dbContext;
//
//     public RefuelingRepository(RefuelingDbContext dbContext)
//     {
//         _dbContext = dbContext;
//     }
//
//     public async Task Add(int id, DateTime StartDate, DateTime InspctionStartDate, DateTime InspectionEndDate,
//         DateTime EndDate, string Status, int FuelAmount, int CarId)
//     {
//         var Refuels = new Refueling()
//         {
//             Id = id,
//             StartDate = StartDate,
//             InspectionStartDate = InspctionStartDate,
//             InspectionEndDate = InspectionEndDate,
//             EndDate = EndDate,
//             Status = Status,
//             FuelAmount = FuelAmount,
//             CarId = CarId
//         };
//
//         await _dbContext.AddAsync(Refuels);
//         await _dbContext.SaveChangesAsync();
//     }
// }