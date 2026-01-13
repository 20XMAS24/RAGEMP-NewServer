using Server.Application.DTOs;
using Server.Core.Entities;
using Server.Infrastructure.Persistence;

namespace Server.Application.Services;

public interface IPropertyService
{
    Task<PropertyDTO?> GetPropertyAsync(uint id);
    Task<List<PropertyDTO>> GetPlayerPropertiesAsync(uint playerId);
    Task<List<PropertyDTO>> GetAvailablePropertiesAsync();
    Task<PropertyDTO> CreatePropertyAsync(PropertyCreateDTO dto);
    Task<bool> BuyPropertyAsync(uint propertyId, uint playerId, long playerCash);
    Task<bool> SellPropertyAsync(uint propertyId);
    Task<bool> UpdatePropertyAsync(uint id, PropertyDTO dto);
}

public class PropertyService : IPropertyService
{
    private readonly IUnitOfWork _unitOfWork;

    public PropertyService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<PropertyDTO?> GetPropertyAsync(uint id)
    {
        var property = await _unitOfWork.Properties.GetByIdAsync(id);
        return property == null ? null : MapToDTO(property);
    }

    public async Task<List<PropertyDTO>> GetPlayerPropertiesAsync(uint playerId)
    {
        var properties = await _unitOfWork.Properties.FindAsync(p => p.OwnerId == playerId);
        return properties.Select(MapToDTO).ToList();
    }

    public async Task<List<PropertyDTO>> GetAvailablePropertiesAsync()
    {
        var properties = await _unitOfWork.Properties.FindAsync(p => p.ForSale);
        return properties.Select(MapToDTO).ToList();
    }

    public async Task<PropertyDTO> CreatePropertyAsync(PropertyCreateDTO dto)
    {
        var existingProperty = await _unitOfWork.Properties.FirstOrDefaultAsync(p => p.Address == dto.Address);
        if (existingProperty != null)
            throw new InvalidOperationException($"Property at '{dto.Address}' already exists");

        var property = new Property
        {
            Address = dto.Address,
            PropertyType = dto.PropertyType,
            Price = dto.Price,
            RentCost = dto.RentCost,
            EntranceX = dto.EntranceX,
            EntranceY = dto.EntranceY,
            EntranceZ = dto.EntranceZ,
            InteriorRoom = dto.InteriorRoom,
            ForSale = true
        };

        await _unitOfWork.Properties.AddAsync(property);
        await _unitOfWork.SaveChangesAsync();

        return MapToDTO(property);
    }

    public async Task<bool> BuyPropertyAsync(uint propertyId, uint playerId, long playerCash)
    {
        var property = await _unitOfWork.Properties.GetByIdAsync(propertyId) ?? throw new KeyNotFoundException();
        var player = await _unitOfWork.Players.GetByIdAsync(playerId) ?? throw new KeyNotFoundException();

        if (playerCash < property.Price)
            return false;

        if (!property.ForSale)
            throw new InvalidOperationException("Property is not for sale");

        property.OwnerId = playerId;
        property.ForSale = false;

        _unitOfWork.Properties.Update(property);
        await _unitOfWork.SaveChangesAsync();

        return true;
    }

    public async Task<bool> SellPropertyAsync(uint propertyId)
    {
        var property = await _unitOfWork.Properties.GetByIdAsync(propertyId) ?? throw new KeyNotFoundException();
        property.ForSale = true;
        _unitOfWork.Properties.Update(property);
        await _unitOfWork.SaveChangesAsync();
        return true;
    }

    public async Task<bool> UpdatePropertyAsync(uint id, PropertyDTO dto)
    {
        var property = await _unitOfWork.Properties.GetByIdAsync(id) ?? throw new KeyNotFoundException();
        property.SafeMoney = dto.SafeMoney;
        _unitOfWork.Properties.Update(property);
        await _unitOfWork.SaveChangesAsync();
        return true;
    }

    private static PropertyDTO MapToDTO(Property property) => new()
    {
        Id = property.Id,
        Address = property.Address,
        OwnerId = property.OwnerId,
        PropertyType = property.PropertyType,
        Price = property.Price,
        RentCost = property.RentCost,
        ForSale = property.ForSale,
        SafeMoney = property.SafeMoney,
        OwnerName = property.Owner?.Username
    };
}