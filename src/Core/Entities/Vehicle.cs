namespace Server.Core.Entities;

public class Vehicle : BaseEntity
{
    /// <summary>Vehicle model hash from GTA</summary>
    public required uint ModelHash { get; set; }

    /// <summary>Vehicle license plate</summary>
    public required string Plate { get; set; }

    /// <summary>Owner player ID</summary>
    public uint OwnerId { get; set; }

    /// <summary>Vehicle color primary</summary>
    public int ColorPrimary { get; set; } = 0;

    /// <summary>Vehicle color secondary</summary>
    public int ColorSecondary { get; set; } = 0;

    /// <summary>Engine health (0-1000)</summary>
    public float EngineHealth { get; set; } = 1000f;

    /// <summary>Body health (0-1000)</summary>
    public float BodyHealth { get; set; } = 1000f;

    /// <summary>Fuel amount (0-100)</summary>
    public float Fuel { get; set; } = 100f;

    /// <summary>Mileage in km</summary>
    public float Mileage { get; set; }

    /// <summary>Vehicle location X</summary>
    public float LocationX { get; set; }

    /// <summary>Vehicle location Y</summary>
    public float LocationY { get; set; }

    /// <summary>Vehicle location Z</summary>
    public float LocationZ { get; set; }

    /// <summary>Vehicle rotation (heading)</summary>
    public float Rotation { get; set; }

    /// <summary>Vehicle cost</summary>
    public long Price { get; set; }

    /// <summary>Is vehicle locked</summary>
    public bool IsLocked { get; set; } = true;

    /// <summary>Is vehicle impounded</summary>
    public bool IsImpounded { get; set; }

    /// <summary>Window state bitmask (8 bits for 8 windows)</summary>
    public byte WindowState { get; set; } = 0; // 0 = all intact, 1 = broken

    /// <summary>Door state bitmask (4 bits for 4 doors)</summary>
    public byte DoorState { get; set; } = 0; // 0 = closed, 1 = broken

    /// <summary>Total repair cost needed</summary>
    public long RepairCost { get; set; }

    // Navigation
    public virtual Player? Owner { get; set; }
    public virtual ICollection<VehicleModification> Modifications { get; set; } = new List<VehicleModification>();
}

public class VehicleModification : BaseEntity
{
    public uint VehicleId { get; set; }
    public required string ModType { get; set; } // "engine", "suspension", "paint", etc.
    public required string ModName { get; set; }
    public long Cost { get; set; }
    public int Level { get; set; } = 1;

    public virtual Vehicle? Vehicle { get; set; }
}