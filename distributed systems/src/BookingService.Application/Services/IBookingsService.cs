using BookingService.Application.DTOs;
using Shared.Models;

namespace BookingService.Application.Services;

public interface IBookingsService
{
    Task<IEnumerable<Booking>> GetAllBookingsAsync();
    Task<Booking?> GetBookingByIdAsync(int id);
    Task<(Booking? booking, string? error)> CreateBookingAsync(CreateBookingDto dto);
    Task<Booking?> UpdateBookingStatusAsync(int id, BookingStatus status);
    Task<bool> DeleteBookingAsync(int id);
}
