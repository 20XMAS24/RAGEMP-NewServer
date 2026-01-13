using BCrypt.Net;

namespace Server.Infrastructure.Security;

public interface IPasswordHasher
{
    string Hash(string password);
    bool Verify(string password, string hash);
}

public class PasswordHasher : IPasswordHasher
{
    private readonly int _workFactor;

    public PasswordHasher(int workFactor = 12)
    {
        _workFactor = workFactor;
    }

    public string Hash(string password) => BCrypt.EnhancedHashPassword(password, workFactor: _workFactor);

    public bool Verify(string password, string hash)
    {
        try
        {
            return BCrypt.EnhancedVerifyPassword(password, hash);
        }
        catch
        {
            return false;
        }
    }
}