using System.Net;
using HotelManagementSystem.Models;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace HotelManagementSystem.Services;

public class BookingService
{
    private readonly MongoClient _mongoClient;
    private readonly IMongoCollection<Booking> _bookingCollection;
    private readonly IMongoCollection<Room> _roomCollection;
    private readonly bool _haveReplicaSet;

    #region Constructor

    public BookingService(IDatabaseSettings databaseSettings)
    {
        var database = CoreService.GetDatabase(databaseSettings);
        _bookingCollection = database.GetCollection<Booking>(databaseSettings.Collections.Booking);
        _roomCollection = database.GetCollection<Room>(databaseSettings.Collections.Room);
        _mongoClient = CoreService.GetMongoClient(databaseSettings.ConnectionString);
        _haveReplicaSet = databaseSettings.HaveReplicaSet;
    }

    #endregion

    public ActionResult<Booking> AddBooking(RequestBookingData requestBookingData)
    {
        // MongoDB Standalone server should not support transaction. It should have replica set or shared cluster.
        try
        {
            if (_haveReplicaSet)
            {
                var session = _mongoClient.StartSession();
                session.StartTransaction(new TransactionOptions(readPreference: ReadPreference.Primary, readConcern: ReadConcern.Local, writeConcern: WriteConcern.WMajority));
                try
                {
                    var bookings = _bookingCollection.Find(booking => 
                        booking.RoomType == requestBookingData.RoomType && (booking.From <= requestBookingData.To &&
                                                                            booking.To >= requestBookingData.From)
                        ).ToList();

                    var availableRoom = _roomCollection.Find(room => room.Type == requestBookingData.RoomType 
                                                                      && bookings.Where(booking => booking.RoomNumber == room.Number).ToList().Count == 0 
                                                                      ).FirstOrDefault();
                    if (availableRoom == null)
                    {
                        session.AbortTransactionAsync();
                        return new JsonResult(new { errorInfo = new { statusCode = HttpStatusCode.Forbidden, message = "Room not available"}})
                        {
                            StatusCode = (int) HttpStatusCode.Forbidden
                        };
                    }
                    var newBookingData = Booking.CreateNewModel(requestBookingData);
                    newBookingData.RoomNumber = availableRoom.Number;
                    _bookingCollection.InsertOne(newBookingData);
                    
                    session.CommitTransaction();
                    
                    return new JsonResult(new
                    {
                        data = new
                        {
                            statusCode = HttpStatusCode.Created,
                            message = "Booking completed successfully",
                            roomNumber = newBookingData.RoomNumber
                        }
                    });
                }
                catch (Exception exception)
                {
                    session.AbortTransactionAsync();
                    return new JsonResult(new { errorInfo = new { statusCode = HttpStatusCode.InternalServerError, message = exception.Message}})
                    {
                        StatusCode = (int) HttpStatusCode.InternalServerError
                    };
                }
            }
            else
            {
                var bookings = _bookingCollection.Find(booking => 
                    booking.RoomType == requestBookingData.RoomType && (booking.From <= requestBookingData.To &&
                                                                        booking.To >= requestBookingData.From)
                ).ToList();
                
                var availableRooms = _roomCollection.Find(room => room.Type == requestBookingData.RoomType).ToList();;
                if (bookings.Count != 0 && availableRooms.Count != 0)
                {
                    availableRooms = availableRooms.Where(room => bookings.Any(booking => booking.RoomNumber != room.Number)).ToList();
                }
                
                if (availableRooms.Count == 0)
                {
                    return new JsonResult(new { errorInfo = new { statusCode = HttpStatusCode.Forbidden, message = "Room not available"}})
                    {
                        StatusCode = (int) HttpStatusCode.Forbidden
                    };
                }
                var newBookingData = Booking.CreateNewModel(requestBookingData);
                newBookingData.RoomNumber = availableRooms[0].Number;
                _bookingCollection.InsertOne(newBookingData);

                return new JsonResult(new
                {
                    data = new
                    {
                        statusCode = HttpStatusCode.Created,
                        message = "Booking completed successfully",
                        roomNumber = newBookingData.RoomNumber
                    }
                });
            }
            
        }
        catch (Exception exception)
        {
            return new JsonResult(new { errorInfo = new { statusCode = HttpStatusCode.InternalServerError, message = exception.Message, error = exception}})
            {
                StatusCode = (int) HttpStatusCode.InternalServerError
            };
        }
    }

    public ActionResult<List<Booking>> GetBookings(long date)
    {
        try
        {
            return _bookingCollection.Find(booking => booking.From <= date && booking.To >= date).ToList();
        }
        catch (Exception exception)
        {
            return new JsonResult(new { errorInfo = new { statusCode = HttpStatusCode.InternalServerError, message = exception.Message, error = exception}})
            {
                StatusCode = (int) HttpStatusCode.InternalServerError
            };
        }
    }

    private bool CheckRoomAvailabilityUtil(long fromDate, long toDate, RoomType roomType)
    {
        var bookedRoomCount = _bookingCollection.Find(
            booking => booking.RoomType == roomType
                       && (booking.From <= toDate && booking.To >= fromDate)
        ).ToList().Count;
        var roomCount = _roomCollection.Find(room => room.Type == roomType).ToList().Count;
        return roomCount > bookedRoomCount;
    }

    public bool CheckRoomNumberBookingStatus(long date, string roomNumber)
    {
        var bookedRoomCount = _bookingCollection.Find(
            booking => booking.RoomNumber == roomNumber
                       && (booking.From <= date && booking.To >= date)
        ).ToList();
        return bookedRoomCount.Count > 0;
    }
    
    public ActionResult<JsonResult> CheckRoomAvailability(long fromDate, long toDate, RoomType roomType)
    {
        try
        {
            return new JsonResult(new
            {
                available = CheckRoomAvailabilityUtil(fromDate, toDate, roomType)
            })
            {
                StatusCode = (int) HttpStatusCode.OK
            };
        }
        catch (Exception exception)
        {
            return new JsonResult(new { errorInfo = new { statusCode = HttpStatusCode.InternalServerError, message = exception.Message, error = exception}})
            {
                StatusCode = (int) HttpStatusCode.InternalServerError
            };
        }
    }
}