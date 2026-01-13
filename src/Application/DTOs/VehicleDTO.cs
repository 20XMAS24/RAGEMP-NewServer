namespace Server.Application.DTOs;

public class VehicleDTO
{
    public uint Id { get; set; }
    public uint ModelHash { get; set; }
    public required string Plate { get; set; }
    public uint OwnerId { get; set; }
    public int ColorPrimary { get; set; }
    public int ColorSecondary { get; set; }
    public float EngineHealth { get; set; }
    public float BodyHealth { get; set; }
    public float Fuel { get; set; }
    public float Mileage { get; set; }
    public long Price { get; set; }
    public bool IsLocked { get; set; }
    public bool IsImpounded { get; set; }
    public long RepairCost { get; set; }
    public string? OwnerName { get; set; }
}

public class VehicleCreateDTO
{
    public required uint ModelHash { get; set; }
    public required string Plate { get; set; }
    public uint OwnerId { get; set; }
    public int ColorPrimary { get; set; }
    public int ColorSecondary { get; set; }
    public long Price { get; set; }
    public float LocationX { get; set; }
    public float LocationY { get; set; }
    public float LocationZ { get; set; }
}

public class VehicleDamageDTO
{
    public uint VehicleId { get; set; }
    public float Damage { get; set; }
    public float Speed { get; set; }
}