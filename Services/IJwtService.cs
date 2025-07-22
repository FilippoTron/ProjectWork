using ProjectWork.Models;

namespace ProjectWork.Services;

public interface IJwtService
{
    string GenerateToken(User user);
}
