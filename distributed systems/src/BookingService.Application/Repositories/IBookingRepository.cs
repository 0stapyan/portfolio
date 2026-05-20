using Shared.Models;

namespace BookingService.Application.Repositories;

public interface IBookingRepository
{
    Task<IEnumerable<Booking>> GetAllAsync();
    Task<Booking?> GetByIdAsync(int id);
    Task<Booking> CreateAsync(Booking booking);
    Task<Booking?> UpdateStatusAsync(int id, BookingStatus status);
    Task<bool> DeleteAsync(int id);
}
