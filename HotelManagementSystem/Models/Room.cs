using HotelManagementSystem.Services;
using MongoDB.Bson.Serialization.Attributes;

namespace HotelManagementSystem.Models;

public enum RoomType
{
    SINGLE,
    DOUBLE
}

public class Room
{
    [BsonElement("_id")] public string Id { get; set; }
    [BsonElement("number")] public string Number { get; set; }
    [BsonElement("type")] public RoomType Type { get; set; }

    public static Room CreateNewModel(RequestRoomData data)
    {
        return new Room()
        {
            Id = CoreService.GetNewGuid(),
            Number = data.Number,
            Type = data.Type
        };
    } 
}

public class RequestRoomData
{
    [BsonElement("number")]
    public string Number { get; set; }
    
    [BsonElement("type")]
    public RoomType Type { get; set; }
}