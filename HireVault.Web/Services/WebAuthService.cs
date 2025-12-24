using System.Threading.Tasks;
using HireVault.Core.DTOs;
using HireVault.Core.Interfaces;
using HireVault.Web.Models.ViewModels;

namespace HireVault.Web.Services
{
    public class WebAuthService : IAuthService
    {
        private readonly IAuthService _authService;

        public WebAuthService(IAuthService authService)
        {
            _authService = authService;
        }

        public async Task<bool> LoginAsync(LoginDto loginDto)
        {
            // This method is just a pass-through for the interface
            return await _authService.LoginAsync(loginDto);
        }

        public async Task<bool> LoginAsync(LoginViewModel model)
        {
            var loginDto = new LoginDto
            {
                Email = model.Email,
                Password = model.Password,
                RememberMe = model.RememberMe
            };
            return await _authService.LoginAsync(loginDto);
        }

        public async Task<bool> RegisterAsync(SignUpDto signUpDto)
        {
            // This method is just a pass-through for the interface
            return await _authService.RegisterAsync(signUpDto);
        }

        public async Task<bool> RegisterAsync(SignUpViewModel model)
        {
            var signUpDto = new SignUpDto
            {
                FirstName = model.FirstName,
                LastName = model.LastName,
                Email = model.Email,
                Password = model.Password,
                ConfirmPassword = model.ConfirmPassword
            };
            return await _authService.RegisterAsync(signUpDto);
        }

        public Task LogoutAsync() => _authService.LogoutAsync();
        
        public Task<string> GetCurrentUserIdAsync() => _authService.GetCurrentUserIdAsync();
        
        public Task<bool> IsUserAuthenticatedAsync() => _authService.IsUserAuthenticatedAsync();
    }
}
