using System.Security.Claims;

namespace HireVault.Core.Interfaces
{
    public interface IAwsCognitoService
    {
        Task<string> CreateUserAsync(string email, string firstName, string lastName, string? phoneNumber, string password, string userGroup);
        Task<string> SignInUserAsync(string username, string password);
        Task AddUserToGroupAsync(string username, string groupName);
        Task RemoveUserFromGroupAsync(string username, string groupName);
        Task<bool> IsUserInGroupAsync(string username, string groupName);
        Task<IList<string>> GetUserGroupsAsync(string username);
        Task<ClaimsPrincipal> GetUserPrincipalAsync(string token);
        Task SendPasswordResetEmailAsync(string email);
        Task ConfirmPasswordResetAsync(string username, string confirmationCode, string newPassword);
    }
}
