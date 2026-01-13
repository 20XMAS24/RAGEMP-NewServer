namespace Server.Core.Entities;

public class BankAccount : BaseEntity
{
    /// <summary>Account owner ID</summary>
    public uint OwnerId { get; set; }

    /// <summary>Account number</summary>
    public required string AccountNumber { get; set; }

    /// <summary>Account balance</summary>
    public long Balance { get; set; }

    /// <summary>Account type: personal, business, shared</summary>
    public required string AccountType { get; set; }

    /// <summary>Account PIN for security</summary>
    public required string PIN { get; set; }

    /// <summary>Is account locked</summary>
    public bool IsLocked { get; set; }

    // Navigation
    public virtual Player? Owner { get; set; }
    public virtual ICollection<BankTransaction> Transactions { get; set; } = new List<BankTransaction>();
}

public class BankTransaction : BaseEntity
{
    /// <summary>Bank account ID</summary>
    public uint AccountId { get; set; }

    /// <summary>Transaction amount</summary>
    public long Amount { get; set; }

    /// <summary>Transaction type: deposit, withdrawal, transfer</summary>
    public required string TransactionType { get; set; }

    /// <summary>Transaction description</summary>
    public string? Description { get; set; }

    /// <summary>Previous balance</summary>
    public long PreviousBalance { get; set; }

    /// <summary>New balance</summary>
    public long NewBalance { get; set; }

    // Navigation
    public virtual BankAccount? Account { get; set; }
}