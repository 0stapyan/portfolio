using System.Net.Http.Json;
using BookingService.Application.DTOs;
using BookingService.Application.Repositories;
using Microsoft.Extensions.Logging;
using Shared.Models;

namespace BookingService.Application.Services;

// ⚠️ Week 1: міжсервісна комунікація через HTTP — синхронна, антипатерн для продакшн
public class BookingsService : IBookingsService
{
    private readonly IBookingRepository _bookingRepository;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<BookingsService> _logger;

    public BookingsService(
        IBookingRepository bookingRepository,
        IHttpClientFactory httpClientFactory,
        ILogger<BookingsService> logger)
    {
        _bookingRepository = bookingRepository;
        _httpClientFactory = httpClientFactory;
        _logger = logger;
    }

    public Task<IEnumerable<Booking>> GetAllBookingsAsync() =>
        _bookingRepository.GetAllAsync();

    public Task<Booking?> GetBookingByIdAsync(int id) =>
        _bookingRepository.GetByIdAsync(id);

    public async Task<(Booking? booking, string? error)> CreateBookingAsync(CreateBookingDto dto)
    {
        // 1. Отримати номер з HotelService та перевірити доступність
        Room? room = null;
        try
        {
            var hotelClient = _httpClientFactory.CreateClient("HotelService");
            room = await hotelClient.GetFromJsonAsync<Room>($"api/rooms/{dto.RoomId}");
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "HotelService недоступний при отриманні кімнати {RoomId}", dto.RoomId);
            return (null, "HotelService недоступний");
        }

        if (room is null)
            return (null, $"Кімната {dto.RoomId} не знайдена");

        if (!room.IsAvailable)
            return (null, $"Кімната {dto.RoomId} недоступна");

        // 2. Перевірити всіх гостей у GuestService
        if (dto.GuestIds is null || dto.GuestIds.Count == 0)
            return (null, "Необхідно вказати хоча б одного гостя");

        if (dto.GuestIds.Count > room.MaxGuests)
            return (null, $"Кімната розрахована максимум на {room.MaxGuests} гостей, передано {dto.GuestIds.Count}");

        var guestClient = _httpClientFactory.CreateClient("GuestService");
        foreach (var guestId in dto.GuestIds)
        {
            try
            {
                var guest = await guestClient.GetFromJsonAsync<Guest>($"api/guests/{guestId}");
                if (guest is null)
                    return (null, $"Гість {guestId} не знайдений");
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "GuestService недоступний при перевірці гостя {GuestId}", guestId);
                return (null, "GuestService недоступний");
            }
        }

        // 3. Розрахувати вартість
        var nights = (dto.CheckOutDate - dto.CheckInDate).Days;
        if (nights <= 0)
            return (null, "Дата виїзду має бути пізніше дати заїзду");

        var totalPrice = room.PricePerNight * nights;

        // 4. Зберегти бронювання
        var booking = new Booking
        {
            RoomId = dto.RoomId,
            CheckInDate = dto.CheckInDate,
            CheckOutDate = dto.CheckOutDate,
            TotalPrice = totalPrice,
            Status = BookingStatus.Pending,
            CreatedAt = DateTime.UtcNow,
            Guests = dto.GuestIds.Select(gid => new BookingGuest { GuestId = gid }).ToList()
        };

        var created = await _bookingRepository.CreateAsync(booking);

        // 5. Позначити кімнату як недоступну
        try
        {
            var hotelClient = _httpClientFactory.CreateClient("HotelService");
            await hotelClient.PatchAsJsonAsync($"api/rooms/{dto.RoomId}/availability", new { IsAvailable = false });
        }
        catch (Exception ex)
        {
            // ⚠️ Week 1: деградація — бронювання вже збережено, але кімната не заблокована
            _logger.LogWarning(ex, "Не вдалося оновити доступність кімнати {RoomId}", dto.RoomId);
        }

        return (created, null);
    }

    public async Task<Booking?> UpdateBookingStatusAsync(int id, BookingStatus status)
    {
        var booking = await _bookingRepository.UpdateStatusAsync(id, status);

        if (booking is not null && status == BookingStatus.Cancelled)
        {
            // Звільнити кімнату при скасуванні
            try
            {
                var hotelClient = _httpClientFactory.CreateClient("HotelService");
                await hotelClient.PatchAsJsonAsync($"api/rooms/{booking.RoomId}/availability", new { IsAvailable = true });
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Не вдалося звільнити кімнату {RoomId} після скасування", booking.RoomId);
            }
        }

        return booking;
    }

    public Task<bool> DeleteBookingAsync(int id) =>
        _bookingRepository.DeleteAsync(id);
}
