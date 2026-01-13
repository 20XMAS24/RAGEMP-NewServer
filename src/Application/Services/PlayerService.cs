using Server.Application.DTOs;
using Server.Core.Entities;
using Server.Infrastructure.Persistence;
using Server.Infrastructure.Security;

namespace Server.Application.Services;

public interface IPlayerService
{
    Task<PlayerDTO?> GetPlayerAsync(uint id);
    Task<PlayerDTO?> GetPlayerByUsernameAsync(string username);
    Task<PlayerDTO> RegisterAsync(PlayerCreateDTO dto);
    Task<PlayerDTO?> LoginAsync(PlayerLoginDTO dto);
    Task UpdatePlayerAsync(uint id, PlayerUpdateDTO dto);
    Task<bool> AddCashAsync(uint id, long amount);
    Task<bool> DeductCashAsync(uint id, long amount);
    Task<bool> AddExperienceAsync(uint id, long amount);
    Task<bool> SetJobAsync(uint id, string jobName);
}

public class PlayerService : IPlayerService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPasswordHasher _passwordHasher;

    public PlayerService(IUnitOfWork unitOfWork, IPasswordHasher passwordHasher)
    {
        _unitOfWork = unitOfWork;
        _passwordHasher = passwordHasher;
    }

    public async Task<PlayerDTO?> GetPlayerAsync(uint id)
    {
        var player = await _unitOfWork.Players.GetByIdAsync(id);
        return player == null ? null : MapToDTO(player);
    }

    public async Task<PlayerDTO?> GetPlayerByUsernameAsync(string username)
    {
        var player = await _unitOfWork.Players.FirstOrDefaultAsync(p => p.Username == username);
        return player == null ? null : MapToDTO(player);
    }

    public async Task<PlayerDTO> RegisterAsync(PlayerCreateDTO dto)
    {
        // Check if user already exists
        var existingPlayer = await _unitOfWork.Players.FirstOrDefaultAsync(p => p.Username == dto.Username);
        if (existingPlayer != null)
            throw new InvalidOperationException($"Username '{dto.Username}' already exists");

        var player = new Player
        {
            Username = dto.Username,
            PasswordHash = _passwordHasher.Hash(dto.Password),
            Email = dto.Email,
            Cash = 5000, // Starting money
            AdminLevel = 0
        };

        await _unitOfWork.Players.AddAsync(player);
        await _unitOfWork.SaveChangesAsync();

        return MapToDTO(player);
    }

    public async Task<PlayerDTO?> LoginAsync(PlayerLoginDTO dto)
    {
        var player = await _unitOfWork.Players.FirstOrDefaultAsync(p => p.Username == dto.Username);
        if (player == null)
            return null;

        if (!_passwordHasher.Verify(dto.Password, player.PasswordHash))
            return null;

        if (player.IsBanned)
            throw new InvalidOperationException($"Player is banned: {player.BanReason}");

        player.LastLogin = DateTime.UtcNow;
        _unitOfWork.Players.Update(player);
        await _unitOfWork.SaveChangesAsync();

        return MapToDTO(player);
    }

    public async Task UpdatePlayerAsync(uint id, PlayerUpdateDTO dto)
    {
        var player = await _unitOfWork.Players.GetByIdAsync(id) ?? throw new KeyNotFoundException();

        if (!string.IsNullOrEmpty(dto.CharacterName))
            player.CharacterName = dto.CharacterName;

        if (!string.IsNullOrEmpty(dto.Job))
            player.Job = dto.Job;

        _unitOfWork.Players.Update(player);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task<bool> AddCashAsync(uint id, long amount)
    {
        var player = await _unitOfWork.Players.GetByIdAsync(id) ?? throw new KeyNotFoundException();
        player.Cash += amount;
        _unitOfWork.Players.Update(player);
        await _unitOfWork.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeductCashAsync(uint id, long amount)
    {
        var player = await _unitOfWork.Players.GetByIdAsync(id) ?? throw new KeyNotFoundException();
        if (player.Cash < amount)
            return false;

        player.Cash -= amount;
        _unitOfWork.Players.Update(player);
        await _unitOfWork.SaveChangesAsync();
        return true;
    }

    public async Task<bool> AddExperienceAsync(uint id, long amount)
    {
        var player = await _unitOfWork.Players.GetByIdAsync(id) ?? throw new KeyNotFoundException();
        player.Experience += amount;
        _unitOfWork.Players.Update(player);
        await _unitOfWork.SaveChangesAsync();
        return true;
    }

    public async Task<bool> SetJobAsync(uint id, string jobName)
    {
        var player = await _unitOfWork.Players.GetByIdAsync(id) ?? throw new KeyNotFoundException();
        var job = await _unitOfWork.Jobs.FirstOrDefaultAsync(j => j.Name == jobName) ?? throw new KeyNotFoundException("Job not found");

        player.Job = jobName;
        player.JobLevel = 1;
        _unitOfWork.Players.Update(player);
        await _unitOfWork.SaveChangesAsync();
        return true;
    }

    private static PlayerDTO MapToDTO(Player player) => new()
    {
        Id = player.Id,
        Username = player.Username,
        CharacterName = player.CharacterName,
        Cash = player.Cash,
        BankMoney = player.BankMoney,
        Job = player.Job,
        JobLevel = player.JobLevel,
        Experience = player.Experience,
        AdminLevel = player.AdminLevel,
        PlaytimeMinutes = player.PlaytimeMinutes,
        LastLogin = player.LastLogin,
        IsBanned = player.IsBanned
    };
}