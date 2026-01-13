namespace Server.Application.DTOs;

public class PropertyDTO
{
    public uint Id { get; set; }
    public required string Address { get; set; }
    public uint OwnerId { get; set; }
    public required string PropertyType { get; set; }
    public long Price { get; set; }
    public long RentCost { get; set; }
    public bool ForSale { get; set; }
    public long SafeMoney { get; set; }
    public string? OwnerName { get; set; }
}

public class PropertyCreateDTO
{
    public required string Address { get; set; }
    public required string PropertyType { get; set; }
    public long Price { get; set; }
    public long RentCost { get; set; }
    public float EntranceX { get; set; }
    public float EntranceY { get; set; }
    public float EntranceZ { get; set; }
    public int InteriorRoom { get; set; }
}