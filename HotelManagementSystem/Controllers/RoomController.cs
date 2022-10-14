using HotelManagementSystem.Models;
using HotelManagementSystem.Services;
using Microsoft.AspNetCore.Mvc;

namespace HotelManagementSystem.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class RoomController : ControllerBase
{
    private readonly ILogger<RoomController> _logger;
    private readonly RoomService _service;
    private const string Scope = "Room";

    public RoomController(ILogger<RoomController> logger, RoomService service)
    {
        _service = service;
        _logger = logger;
    }

    [HttpPost]
    public ActionResult<Room> AddRoom(RequestRoomData roomData)
    {
        _logger.LogInformation("ADD {Scope} API initiated at {DT}", Scope, DateTime.UtcNow.ToLongTimeString());
        
        return _service.AddRoom(roomData);
    }
    
    [HttpGet]
    public ActionResult<List<Room>> GetRooms()
    {
        _logger.LogInformation("GET {Scope}(s) API initiated at {DT}", Scope, DateTime.UtcNow.ToLongTimeString());
        return _service.GetRooms();
    }
}