namespace Server.Core.Entities;

public class Player : BaseEntity
{
    /// <summary>Player's in-game username</summary>
    public required string Username { get; set; }

    /// <summary>Password hash (BCrypt)</summary>
    public required string PasswordHash { get; set; }

    /// <summary>Player's email</summary>
    public string? Email { get; set; }

    /// <summary>Character name</summary>
    public string? CharacterName { get; set; }

    /// <summary>Money in hand</summary>
    public long Cash { get; set; }

    /// <summary>Money in bank account</summary>
    public long BankMoney { get; set; }

    /// <summary>Player's job</summary>
    public string? Job { get; set; }

    /// <summary>Job level/rank</summary>
    public int JobLevel { get; set; }

    /// <summary>Experience points</summary>
    public long Experience { get; set; }

    /// <summary>Admin level (0 = player, 1-5 = admin)</summary>
    public int AdminLevel { get; set; }

    /// <summary>Last login date</summary>
    public DateTime? LastLogin { get; set; }

    /// <summary>Total playtime in minutes</summary>
    public int PlaytimeMinutes { get; set; }

    /// <summary>Is player currently banned</summary>
    public bool IsBanned { get; set; }

    /// <summary>Ban reason if banned</summary>
    public string? BanReason { get; set; }

    /// <summary>Ban expiration date</summary>
    public DateTime? BanExpires { get; set; }

    // Navigation properties
    public virtual ICollection<Vehicle> OwnedVehicles { get; set; } = new List<Vehicle>();
    public virtual ICollection<Property> OwnedProperties { get; set; } = new List<Property>();
    public virtual ICollection<BankAccount> BankAccounts { get; set; } = new List<BankAccount>();
}