using GuestService.Application.DTOs;
using GuestService.Application.Repositories;
using Shared.Models;

namespace GuestService.Application.Services;

public class GuestsService : IGuestsService
{
    private readonly IGuestRepository _guestRepository;

    public GuestsService(IGuestRepository guestRepository)
    {
        _guestRepository = guestRepository;
    }

    public Task<IEnumerable<Guest>> GetAllGuestsAsync() =>
        _guestRepository.GetAllAsync();

    public Task<Guest?> GetGuestByIdAsync(int id) =>
        _guestRepository.GetByIdAsync(id);

    public Task<Guest?> GetGuestByEmailAsync(string email) =>
        _guestRepository.GetByEmailAsync(email);

    public Task<Guest> CreateGuestAsync(CreateGuestDto dto)
    {
        var guest = new Guest
        {
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            Email = dto.Email,
            Phone = dto.Phone,
            CreatedAt = DateTime.UtcNow
        };
        return _guestRepository.CreateAsync(guest);
    }

    public Task<Guest?> UpdateGuestAsync(int id, UpdateGuestDto dto)
    {
        var guest = new Guest
        {
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            Email = dto.Email,
            Phone = dto.Phone
        };
        return _guestRepository.UpdateAsync(id, guest);
    }

    public Task<bool> DeleteGuestAsync(int id) =>
        _guestRepository.DeleteAsync(id);
}
