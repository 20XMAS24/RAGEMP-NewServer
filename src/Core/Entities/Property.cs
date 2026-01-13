namespace Server.Core.Entities;

public class Property : BaseEntity
{
    /// <summary>Property address</summary>
    public required string Address { get; set; }

    /// <summary>Property owner ID</summary>
    public uint OwnerId { get; set; }

    /// <summary>Property type: house, apartment, business</summary>
    public required string PropertyType { get; set; }

    /// <summary>Property price</summary>
    public long Price { get; set; }

    /// <summary>Monthly rent cost</summary>
    public long RentCost { get; set; }

    /// <summary>Property entrance location X</summary>
    public float EntranceX { get; set; }

    /// <summary>Property entrance location Y</summary>
    public float EntranceY { get; set; }

    /// <summary>Property entrance location Z</summary>
    public float EntranceZ { get; set; }

    /// <summary>Property interior room</summary>
    public int InteriorRoom { get; set; }

    /// <summary>Interior spawn X</summary>
    public float InteriorX { get; set; }

    /// <summary>Interior spawn Y</summary>
    public float InteriorY { get; set; }

    /// <summary>Interior spawn Z</summary>
    public float InteriorZ { get; set; }

    /// <summary>Is property for sale</summary>
    public bool ForSale { get; set; } = true;

    /// <summary>Stored money in property safe</summary>
    public long SafeMoney { get; set; }

    // Navigation
    public virtual Player? Owner { get; set; }
}