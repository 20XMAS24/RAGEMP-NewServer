using Server.Core.Entities;
using Server.Infrastructure.Data;

namespace Server.Infrastructure.Persistence;

public interface IUnitOfWork : IDisposable
{
    IRepository<Player> Players { get; }
    IRepository<Vehicle> Vehicles { get; }
    IRepository<VehicleModification> VehicleModifications { get; }
    IRepository<Property> Properties { get; }
    IRepository<BankAccount> BankAccounts { get; }
    IRepository<BankTransaction> BankTransactions { get; }
    IRepository<Job> Jobs { get; }
    Task SaveChangesAsync();
}

public class UnitOfWork : IUnitOfWork
{
    private readonly AppDbContext _context;
    private readonly Dictionary<Type, object> _repositories = new();

    public UnitOfWork(AppDbContext context)
    {
        _context = context;
    }

    public IRepository<Player> Players => GetRepository<Player>();
    public IRepository<Vehicle> Vehicles => GetRepository<Vehicle>();
    public IRepository<VehicleModification> VehicleModifications => GetRepository<VehicleModification>();
    public IRepository<Property> Properties => GetRepository<Property>();
    public IRepository<BankAccount> BankAccounts => GetRepository<BankAccount>();
    public IRepository<BankTransaction> BankTransactions => GetRepository<BankTransaction>();
    public IRepository<Job> Jobs => GetRepository<Job>();

    private IRepository<T> GetRepository<T>() where T : BaseEntity
    {
        var type = typeof(T);
        if (!_repositories.ContainsKey(type))
        {
            var repositoryType = typeof(Repository<>).MakeGenericType(type);
            _repositories[type] = Activator.CreateInstance(repositoryType, _context) ?? throw new InvalidOperationException();
        }

        return (IRepository<T>)_repositories[type];
    }

    public async Task SaveChangesAsync() => await _context.SaveChangesAsync();

    public void Dispose()
    {
        _context?.Dispose();
        GC.SuppressFinalize(this);
    }
}