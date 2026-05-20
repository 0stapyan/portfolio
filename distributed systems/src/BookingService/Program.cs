using BookingService.Application.Repositories;
using BookingService.Application.Services;
using BookingService.Repositories;
using Microsoft.EntityFrameworkCore;
using Shared.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<IBookingRepository, BookingRepository>();
builder.Services.AddScoped<IBookingsService, BookingsService>();

builder.Services.AddDbContext<BookingDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("BookingDb")));

// ⚠️ Week 1: синхронна міжсервісна комунікація через HTTP
builder.Services.AddHttpClient("HotelService", client =>
    client.BaseAddress = new Uri(builder.Configuration["Services:HotelService"]!));

builder.Services.AddHttpClient("GuestService", client =>
    client.BaseAddress = new Uri(builder.Configuration["Services:GuestService"]!));

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<BookingDbContext>();
    db.Database.Migrate();
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapControllers();
app.Run();
