namespace Server.Application.DTOs;

public class PlayerDTO
{
    public uint Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public string? CharacterName { get; set; }
    public long Cash { get; set; }
    public long BankMoney { get; set; }
    public string? Job { get; set; }
    public int JobLevel { get; set; }
    public long Experience { get; set; }
    public int AdminLevel { get; set; }
    public int PlaytimeMinutes { get; set; }
    public DateTime? LastLogin { get; set; }
    public bool IsBanned { get; set; }
}

public class PlayerCreateDTO
{
    public required string Username { get; set; }
    public required string Password { get; set; }
    public string? Email { get; set; }
}

public class PlayerLoginDTO
{
    public required string Username { get; set; }
    public required string Password { get; set; }
}

public class PlayerUpdateDTO
{
    public string? CharacterName { get; set; }
    public string? Job { get; set; }
}