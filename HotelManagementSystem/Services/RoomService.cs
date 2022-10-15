using System.Net;
using HotelManagementSystem.Models;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;

namespace HotelManagementSystem.Services;

public class RoomService
{
    private readonly IMongoCollection<Room> _roomCollection;
    private readonly BookingService _bookingService;

    #region Constructor

        public RoomService(IDatabaseSettings databaseSettings, BookingService bookingService)
        {
            var database = CoreService.GetDatabase(databaseSettings);
            _roomCollection = database.GetCollection<Room>(databaseSettings.Collections.Room);
            _bookingService = bookingService;
        }

    #endregion

    public ActionResult<Room> AddRoom(RequestRoomData roomData)
    {
        try
        {
            var existData = _roomCollection.Find(ele => ele.Number == roomData.Number).FirstOrDefault();
            if (existData != null)
            {
                return new JsonResult(new { errorInfo = new { statusCode = HttpStatusCode.AlreadyReported, message = "Room number already exist"}})
                {
                    StatusCode = (int) HttpStatusCode.AlreadyReported
                };
            }
            
            var insertData = Room.CreateNewModel(roomData);
            _roomCollection.InsertOne(insertData);
            return new JsonResult(new { data = new { statusCode = HttpStatusCode.Created, message = "Created successfully"}})
            {
                StatusCode = (int) HttpStatusCode.Created
            };
        }
        catch (Exception exception)
        {
            return new JsonResult(exception)
            {
                StatusCode = (int) HttpStatusCode.InternalServerError
            };
        }
    }
    
    public ActionResult<List<dynamic>> GetRooms(long date = 0)
    {
        try
        {
            var roomList = new List<dynamic>();
            foreach (var room in _roomCollection.Find(Builders<Room>.Filter.Empty).ToList())
            {
                roomList.Add(new
                {
                    id = room.Id,
                    number = room.Number,
                    type = room.Type,
                    booked = _bookingService.CheckRoomNumberBookingStatus(date, room.Number)
                });
            }
            return roomList;
        }
        catch (Exception exception)
        {
            return new JsonResult(exception)
            {
                StatusCode = (int) HttpStatusCode.InternalServerError
            };
        }
    }

}