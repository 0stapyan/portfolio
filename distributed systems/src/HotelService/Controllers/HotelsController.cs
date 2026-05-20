using HotelService.Application.DTOs;
using HotelService.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace HotelService.Controllers;

[ApiController]
[Route("api/hotels")]
public class HotelsController : ControllerBase
{
    private readonly IHotelsService _hotelsService;

    public HotelsController(IHotelsService hotelsService)
    {
        _hotelsService = hotelsService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll() =>
        Ok(await _hotelsService.GetAllHotelsAsync());

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var hotel = await _hotelsService.GetHotelByIdAsync(id);
        return hotel is null ? NotFound() : Ok(hotel);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateHotelDto dto)
    {
        var hotel = await _hotelsService.CreateHotelAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = hotel.Id }, hotel);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateHotelDto dto)
    {
        var hotel = await _hotelsService.UpdateHotelAsync(id, dto);
        return hotel is null ? NotFound() : Ok(hotel);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var deleted = await _hotelsService.DeleteHotelAsync(id);
        return deleted ? NoContent() : NotFound();
    }
}
