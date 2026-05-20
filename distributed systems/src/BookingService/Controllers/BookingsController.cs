using BookingService.Application.DTOs;
using BookingService.Application.Services;
using Microsoft.AspNetCore.Mvc;
using Shared.Models;

namespace BookingService.Controllers;

[ApiController]
[Route("api/bookings")]
public class BookingsController : ControllerBase
{
    private readonly IBookingsService _bookingsService;

    public BookingsController(IBookingsService bookingsService)
    {
        _bookingsService = bookingsService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll() =>
        Ok(await _bookingsService.GetAllBookingsAsync());

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var booking = await _bookingsService.GetBookingByIdAsync(id);
        return booking is null ? NotFound() : Ok(booking);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateBookingDto dto)
    {
        var (booking, error) = await _bookingsService.CreateBookingAsync(dto);
        if (error is not null)
            return BadRequest(new { error });

        return CreatedAtAction(nameof(GetById), new { id = booking!.Id }, booking);
    }

    [HttpPatch("{id}/status")]
    public async Task<IActionResult> UpdateStatus(int id, [FromBody] UpdateBookingStatusDto dto)
    {
        var booking = await _bookingsService.UpdateBookingStatusAsync(id, dto.Status);
        return booking is null ? NotFound() : Ok(booking);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var deleted = await _bookingsService.DeleteBookingAsync(id);
        return deleted ? NoContent() : NotFound();
    }
}
