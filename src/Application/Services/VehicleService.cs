using Server.Application.DTOs;
using Server.Core.Entities;
using Server.Infrastructure.Persistence;

namespace Server.Application.Services;

public interface IVehicleService
{
    Task<VehicleDTO?> GetVehicleAsync(uint id);
    Task<VehicleDTO?> GetVehicleByPlateAsync(string plate);
    Task<List<VehicleDTO>> GetPlayerVehiclesAsync(uint playerId);
    Task<VehicleDTO> CreateVehicleAsync(VehicleCreateDTO dto);
    Task UpdateVehicleAsync(uint id, VehicleDTO dto);
    Task DamageVehicleAsync(uint id, float damageAmount, float speed);
    Task RepairVehicleAsync(uint id);
    Task<bool> DeleteVehicleAsync(uint id);
}

public class VehicleService : IVehicleService
{
    private readonly IUnitOfWork _unitOfWork;

    public VehicleService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<VehicleDTO?> GetVehicleAsync(uint id)
    {
        var vehicle = await _unitOfWork.Vehicles.GetByIdAsync(id);
        return vehicle == null ? null : MapToDTO(vehicle);
    }

    public async Task<VehicleDTO?> GetVehicleByPlateAsync(string plate)
    {
        var vehicle = await _unitOfWork.Vehicles.FirstOrDefaultAsync(v => v.Plate == plate);
        return vehicle == null ? null : MapToDTO(vehicle);
    }

    public async Task<List<VehicleDTO>> GetPlayerVehiclesAsync(uint playerId)
    {
        var vehicles = await _unitOfWork.Vehicles.FindAsync(v => v.OwnerId == playerId);
        return vehicles.Select(MapToDTO).ToList();
    }

    public async Task<VehicleDTO> CreateVehicleAsync(VehicleCreateDTO dto)
    {
        // Check if plate already exists
        var existingVehicle = await _unitOfWork.Vehicles.FirstOrDefaultAsync(v => v.Plate == dto.Plate);
        if (existingVehicle != null)
            throw new InvalidOperationException($"Vehicle with plate '{dto.Plate}' already exists");

        var vehicle = new Vehicle
        {
            ModelHash = dto.ModelHash,
            Plate = dto.Plate,
            OwnerId = dto.OwnerId,
            ColorPrimary = dto.ColorPrimary,
            ColorSecondary = dto.ColorSecondary,
            EngineHealth = 1000f,
            BodyHealth = 1000f,
            Fuel = 100f,
            Price = dto.Price,
            LocationX = dto.LocationX,
            LocationY = dto.LocationY,
            LocationZ = dto.LocationZ,
            IsLocked = true
        };

        await _unitOfWork.Vehicles.AddAsync(vehicle);
        await _unitOfWork.SaveChangesAsync();

        return MapToDTO(vehicle);
    }

    public async Task UpdateVehicleAsync(uint id, VehicleDTO dto)
    {
        var vehicle = await _unitOfWork.Vehicles.GetByIdAsync(id) ?? throw new KeyNotFoundException();

        vehicle.LocationX = dto.LocationX;
        vehicle.LocationY = dto.LocationY;
        vehicle.LocationZ = dto.LocationZ;
        vehicle.EngineHealth = dto.EngineHealth;
        vehicle.BodyHealth = dto.BodyHealth;
        vehicle.Fuel = dto.Fuel;
        vehicle.Mileage = dto.Mileage;
        vehicle.IsLocked = dto.IsLocked;

        _unitOfWork.Vehicles.Update(vehicle);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task DamageVehicleAsync(uint id, float damageAmount, float speed)
    {
        var vehicle = await _unitOfWork.Vehicles.GetByIdAsync(id) ?? throw new KeyNotFoundException();

        // Calculate damage based on speed
        float actualDamage = damageAmount * (speed / 100f);
        actualDamage = Math.Min(actualDamage, 500f); // Cap damage

        vehicle.EngineHealth = Math.Max(0, vehicle.EngineHealth - actualDamage);
        vehicle.BodyHealth = Math.Max(0, vehicle.BodyHealth - actualDamage * 0.7f);

        // Calculate repair cost
        vehicle.RepairCost = (long)((1000f - vehicle.EngineHealth) * 10);

        _unitOfWork.Vehicles.Update(vehicle);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task RepairVehicleAsync(uint id)
    {
        var vehicle = await _unitOfWork.Vehicles.GetByIdAsync(id) ?? throw new KeyNotFoundException();

        vehicle.EngineHealth = 1000f;
        vehicle.BodyHealth = 1000f;
        vehicle.RepairCost = 0;
        vehicle.WindowState = 0;
        vehicle.DoorState = 0;

        _unitOfWork.Vehicles.Update(vehicle);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task<bool> DeleteVehicleAsync(uint id)
    {
        var vehicle = await _unitOfWork.Vehicles.GetByIdAsync(id) ?? throw new KeyNotFoundException();
        _unitOfWork.Vehicles.Remove(vehicle);
        await _unitOfWork.SaveChangesAsync();
        return true;
    }

    private static VehicleDTO MapToDTO(Vehicle vehicle) => new()
    {
        Id = vehicle.Id,
        ModelHash = vehicle.ModelHash,
        Plate = vehicle.Plate,
        OwnerId = vehicle.OwnerId,
        ColorPrimary = vehicle.ColorPrimary,
        ColorSecondary = vehicle.ColorSecondary,
        EngineHealth = vehicle.EngineHealth,
        BodyHealth = vehicle.BodyHealth,
        Fuel = vehicle.Fuel,
        Mileage = vehicle.Mileage,
        Price = vehicle.Price,
        IsLocked = vehicle.IsLocked,
        IsImpounded = vehicle.IsImpounded,
        RepairCost = vehicle.RepairCost,
        OwnerName = vehicle.Owner?.Username
    };
}