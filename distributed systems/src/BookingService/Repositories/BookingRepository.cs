using BookingService.Application.Repositories;
using Microsoft.EntityFrameworkCore;
using Shared.Data;
using Shared.Models;

namespace BookingService.Repositories;

public class BookingRepository : IBookingRepository
{
    private readonly BookingDbContext _db;

    public BookingRepository(BookingDbContext db)
    {
        _db = db;
    }

    public async Task<IEnumerable<Booking>> GetAllAsync() =>
        await _db.Bookings.Include(b => b.Guests).ToListAsync();

    public async Task<Booking?> GetByIdAsync(int id) =>
        await _db.Bookings.Include(b => b.Guests).FirstOrDefaultAsync(b => b.Id == id);

    public async Task<Booking> CreateAsync(Booking booking)
    {
        _db.Bookings.Add(booking);
        await _db.SaveChangesAsync();
        return booking;
    }

    public async Task<Booking?> UpdateStatusAsync(int id, BookingStatus status)
    {
        var booking = await _db.Bookings.FindAsync(id);
        if (booking is null) return null;

        booking.Status = status;
        await _db.SaveChangesAsync();
        return booking;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var booking = await _db.Bookings.FindAsync(id);
        if (booking is null) return false;

        _db.Bookings.Remove(booking);
        await _db.SaveChangesAsync();
        return true;
    }
}
