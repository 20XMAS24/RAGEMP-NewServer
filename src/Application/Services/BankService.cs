using Server.Application.DTOs;
using Server.Core.Entities;
using Server.Infrastructure.Persistence;
using Server.Infrastructure.Security;

namespace Server.Application.Services;

public interface IBankService
{
    Task<BankAccount?> GetAccountAsync(uint id);
    Task<BankAccount?> GetAccountByNumberAsync(string accountNumber);
    Task<List<BankAccount>> GetPlayerAccountsAsync(uint playerId);
    Task<BankAccount> CreateAccountAsync(uint playerId, string pin, string accountType = "personal");
    Task<bool> DepositAsync(uint accountId, long amount, string description = "");
    Task<bool> WithdrawAsync(uint accountId, long amount, string pin, string description = "");
    Task<bool> TransferAsync(uint fromAccountId, uint toAccountId, long amount, string pin);
    Task<List<BankTransaction>> GetTransactionHistoryAsync(uint accountId, int limit = 50);
}

public class BankService : IBankService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPasswordHasher _passwordHasher;

    public BankService(IUnitOfWork unitOfWork, IPasswordHasher passwordHasher)
    {
        _unitOfWork = unitOfWork;
        _passwordHasher = passwordHasher;
    }

    public async Task<BankAccount?> GetAccountAsync(uint id)
    {
        return await _unitOfWork.BankAccounts.GetByIdAsync(id);
    }

    public async Task<BankAccount?> GetAccountByNumberAsync(string accountNumber)
    {
        return await _unitOfWork.BankAccounts.FirstOrDefaultAsync(a => a.AccountNumber == accountNumber);
    }

    public async Task<List<BankAccount>> GetPlayerAccountsAsync(uint playerId)
    {
        return await _unitOfWork.BankAccounts.FindAsync(a => a.OwnerId == playerId);
    }

    public async Task<BankAccount> CreateAccountAsync(uint playerId, string pin, string accountType = "personal")
    {
        var player = await _unitOfWork.Players.GetByIdAsync(playerId) ?? throw new KeyNotFoundException();

        var accountNumber = GenerateAccountNumber();
        var existingAccount = await _unitOfWork.BankAccounts.FirstOrDefaultAsync(a => a.AccountNumber == accountNumber);
        
        // Ensure unique account number
        while (existingAccount != null)
        {
            accountNumber = GenerateAccountNumber();
            existingAccount = await _unitOfWork.BankAccounts.FirstOrDefaultAsync(a => a.AccountNumber == accountNumber);
        }

        var account = new BankAccount
        {
            OwnerId = playerId,
            AccountNumber = accountNumber,
            AccountType = accountType,
            PIN = _passwordHasher.Hash(pin),
            Balance = 0,
            IsLocked = false
        };

        await _unitOfWork.BankAccounts.AddAsync(account);
        await _unitOfWork.SaveChangesAsync();

        return account;
    }

    public async Task<bool> DepositAsync(uint accountId, long amount, string description = "")
    {
        if (amount <= 0)
            return false;

        var account = await _unitOfWork.BankAccounts.GetByIdAsync(accountId) ?? throw new KeyNotFoundException();
        
        if (account.IsLocked)
            throw new InvalidOperationException("Account is locked");

        var previousBalance = account.Balance;
        account.Balance += amount;

        var transaction = new BankTransaction
        {
            AccountId = accountId,
            Amount = amount,
            TransactionType = "deposit",
            Description = description,
            PreviousBalance = previousBalance,
            NewBalance = account.Balance
        };

        _unitOfWork.BankAccounts.Update(account);
        await _unitOfWork.BankTransactions.AddAsync(transaction);
        await _unitOfWork.SaveChangesAsync();

        return true;
    }

    public async Task<bool> WithdrawAsync(uint accountId, long amount, string pin, string description = "")
    {
        if (amount <= 0)
            return false;

        var account = await _unitOfWork.BankAccounts.GetByIdAsync(accountId) ?? throw new KeyNotFoundException();
        
        if (account.IsLocked)
            throw new InvalidOperationException("Account is locked");

        if (!_passwordHasher.Verify(pin, account.PIN))
            throw new InvalidOperationException("Incorrect PIN");

        if (account.Balance < amount)
            return false;

        var previousBalance = account.Balance;
        account.Balance -= amount;

        var transaction = new BankTransaction
        {
            AccountId = accountId,
            Amount = amount,
            TransactionType = "withdrawal",
            Description = description,
            PreviousBalance = previousBalance,
            NewBalance = account.Balance
        };

        _unitOfWork.BankAccounts.Update(account);
        await _unitOfWork.BankTransactions.AddAsync(transaction);
        await _unitOfWork.SaveChangesAsync();

        return true;
    }

    public async Task<bool> TransferAsync(uint fromAccountId, uint toAccountId, long amount, string pin)
    {
        if (amount <= 0)
            return false;

        var fromAccount = await _unitOfWork.BankAccounts.GetByIdAsync(fromAccountId) ?? throw new KeyNotFoundException();
        var toAccount = await _unitOfWork.BankAccounts.GetByIdAsync(toAccountId) ?? throw new KeyNotFoundException();

        if (!_passwordHasher.Verify(pin, fromAccount.PIN))
            throw new InvalidOperationException("Incorrect PIN");

        if (fromAccount.Balance < amount)
            return false;

        var fromPreviousBalance = fromAccount.Balance;
        var toPreviousBalance = toAccount.Balance;

        fromAccount.Balance -= amount;
        toAccount.Balance += amount;

        var fromTransaction = new BankTransaction
        {
            AccountId = fromAccountId,
            Amount = amount,
            TransactionType = "transfer",
            Description = $"Transfer to {toAccount.AccountNumber}",
            PreviousBalance = fromPreviousBalance,
            NewBalance = fromAccount.Balance
        };

        var toTransaction = new BankTransaction
        {
            AccountId = toAccountId,
            Amount = amount,
            TransactionType = "transfer",
            Description = $"Transfer from {fromAccount.AccountNumber}",
            PreviousBalance = toPreviousBalance,
            NewBalance = toAccount.Balance
        };

        _unitOfWork.BankAccounts.Update(fromAccount);
        _unitOfWork.BankAccounts.Update(toAccount);
        await _unitOfWork.BankTransactions.AddAsync(fromTransaction);
        await _unitOfWork.BankTransactions.AddAsync(toTransaction);
        await _unitOfWork.SaveChangesAsync();

        return true;
    }

    public async Task<List<BankTransaction>> GetTransactionHistoryAsync(uint accountId, int limit = 50)
    {
        var transactions = await _unitOfWork.BankTransactions.FindAsync(t => t.AccountId == accountId);
        return transactions.OrderByDescending(t => t.CreatedAt).Take(limit).ToList();
    }

    private static string GenerateAccountNumber()
    {
        var random = new Random();
        return $"ACC{DateTime.UtcNow.Year}{random.Next(100000, 999999)}";
    }
}