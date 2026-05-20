using HotelService.Application.DTOs;
using HotelService.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace HotelService.Controllers;

[ApiController]
[Route("api/rooms")]
public class RoomsController : ControllerBase
{
    private readonly IRoomsService _roomsService;

    public RoomsController(IRoomsService roomsService)
    {
        _roomsService = roomsService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] int? hotelId)
    {
        if (hotelId.HasValue)
            return Ok(await _roomsService.GetRoomsByHotelIdAsync(hotelId.Value));
        return Ok(await _roomsService.GetAllRoomsAsync());
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var room = await _roomsService.GetRoomByIdAsync(id);
        return room is null ? NotFound() : Ok(room);
    }

    [HttpGet("{id}/availability")]
    public async Task<IActionResult> CheckAvailability(int id, [FromQuery] DateTime checkIn, [FromQuery] DateTime checkOut)
    {
        var available = await _roomsService.CheckAvailabilityAsync(id, checkIn, checkOut);
        return Ok(new { roomId = id, isAvailable = available });
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateRoomDto dto)
    {
        var room = await _roomsService.CreateRoomAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = room.Id }, room);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateRoomDto dto)
    {
        var room = await _roomsService.UpdateRoomAsync(id, dto);
        return room is null ? NotFound() : Ok(room);
    }

    [HttpPatch("{id}/availability")]
    public async Task<IActionResult> SetAvailability(int id, [FromBody] SetAvailabilityDto dto)
    {
        var updated = await _roomsService.SetAvailabilityAsync(id, dto.IsAvailable);
        return updated ? NoContent() : NotFound();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var deleted = await _roomsService.DeleteRoomAsync(id);
        return deleted ? NoContent() : NotFound();
    }
}

public record SetAvailabilityDto(bool IsAvailable);
