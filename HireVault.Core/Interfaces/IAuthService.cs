using System.Threading.Tasks;
using HireVault.Core.DTOs;

namespace HireVault.Core.Interfaces
{
    public interface IAuthService
    {
        Task<bool> LoginAsync(LoginDto loginDto);
        Task<bool> RegisterAsync(SignUpDto signUpDto);
        Task LogoutAsync();
        Task<string> GetCurrentUserIdAsync();
        Task<bool> IsUserAuthenticatedAsync();
    }
}