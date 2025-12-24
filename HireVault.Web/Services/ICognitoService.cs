using System.Threading.Tasks;
using HireVault.Web.Models;
using HireVault.Web.Models.ViewModels;

namespace HireVault.Web.Services
{
    public interface ICognitoService
    {
        Task<AuthResponse> SignUpAsync(SignUpViewModel model);
        Task<AuthResponse> ConfirmSignUpAsync(string email, string confirmationCode);
        Task<AuthResponse> LoginAsync(LoginViewModel model);
        Task<AuthResponse> RefreshTokenAsync(string refreshToken);
        Task ForgotPasswordAsync(string email);
        Task ConfirmForgotPasswordAsync(string email, string confirmationCode, string newPassword);
        Task SignOutAsync(string accessToken);
    }
}
