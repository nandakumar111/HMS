using HotelManagementSystem.Services;
using MongoDB.Bson.Serialization.Attributes;

namespace HotelManagementSystem.Models;

public class Booking
{
    [BsonElement("_id")]
    public string Id { get; set; }
    
    [BsonElement("userEmail")]
    public string UserEmail { get; set; }
    
    [BsonElement("from")]
    public long From { get; set; }
    
    [BsonElement("to")]
    public long To { get; set; }
    
    [BsonElement("roomType")]
    public RoomType RoomType { get; set; }
    
    [BsonElement("roomNumber")]
    public string RoomNumber { get; set; }
    
    public static Booking CreateNewModel(RequestBookingData data)
    {
        return new Booking()
        {
            Id = CoreService.GetNewGuid(),
            UserEmail = CoreService.Encrypt(data.UserEmail),
            From = data.From,
            To = data.To,
            RoomType = data.RoomType
        };
    } 
}

public class RequestBookingData
{
    public string UserEmail { get; set; }
    
    public long From { get; set; }
    
    public long To { get; set; }
    
    public RoomType RoomType { get; set; }
}