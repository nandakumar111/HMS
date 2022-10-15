using HotelManagementSystem.Models;
using HotelManagementSystem.Services;
using Microsoft.AspNetCore.Mvc;

namespace HotelManagementSystem.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class BookingController : ControllerBase
{
    private readonly ILogger<BookingController> _logger;
    private readonly BookingService _service;
    private const string Scope = "Booking";

    public BookingController(ILogger<BookingController> logger, BookingService service)
    {
        _logger = logger;
        _service = service;
    }

    [HttpPost]
    public ActionResult<Booking> AddBooking(RequestBookingData bookingData)
    {
        _logger.LogInformation("ADD {Scope} API initiated at {DT}", Scope, DateTime.UtcNow.ToLongTimeString());
        
        return _service.AddBooking(bookingData);
    }

    [HttpGet]
    public ActionResult<List<Booking>> GetBookings([FromQuery(Name= "date")] long date)
    {
        _logger.LogInformation("GET {Scope}(s) API initiated at {DT}", Scope, DateTime.UtcNow.ToLongTimeString());
        
        return _service.GetBookings(date);
    }
    
    [HttpGet("availability")]
    public ActionResult<JsonResult> CheckRoomAvailability([FromQuery(Name = "from")] long fromDate, [FromQuery(Name = "to")] long toDate, [FromQuery(Name = "type")] RoomType roomType)
    {
        _logger.LogInformation("CHECK {Scope} Room availability API initiated at {DT}", Scope, DateTime.UtcNow.ToLongTimeString());
        
        return _service.CheckRoomAvailability(fromDate, toDate, roomType);
    }
}