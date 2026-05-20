using HotelService.Application.Repositories;
using Microsoft.EntityFrameworkCore;
using Shared.Data;
using Shared.Models;

namespace HotelService.Repositories;

public class HotelRepository : IHotelRepository
{
    private readonly BookingDbContext _db;

    public HotelRepository(BookingDbContext db)
    {
        _db = db;
    }

    public async Task<IEnumerable<Hotel>> GetAllAsync() =>
        await _db.Hotels.Include(h => h.Rooms).ToListAsync();

    public async Task<Hotel?> GetByIdAsync(int id) =>
        await _db.Hotels.Include(h => h.Rooms).FirstOrDefaultAsync(h => h.Id == id);

    public async Task<Hotel> CreateAsync(Hotel hotel)
    {
        _db.Hotels.Add(hotel);
        await _db.SaveChangesAsync();
        return hotel;
    }

    public async Task<Hotel?> UpdateAsync(int id, Hotel hotel)
    {
        var existing = await _db.Hotels.FindAsync(id);
        if (existing is null) return null;

        existing.Name = hotel.Name;
        existing.Address = hotel.Address;
        existing.City = hotel.City;
        existing.StarRating = hotel.StarRating;
        existing.Description = hotel.Description;

        await _db.SaveChangesAsync();
        return existing;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var hotel = await _db.Hotels.FindAsync(id);
        if (hotel is null) return false;

        _db.Hotels.Remove(hotel);
        await _db.SaveChangesAsync();
        return true;
    }
}
