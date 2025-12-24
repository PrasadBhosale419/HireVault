using HireVault.Core.Entities;

namespace HireVault.Core.Interfaces
{
    public interface IEmployeeRepository : IRepository<Employee>
    {
        Task<Employee?> GetByCognitoUserIdAsync(string cognitoUserId);
        Task<IReadOnlyList<Employee>> GetEmployeesByStatusAsync(Enums.OnboardingStatus status);
    }
}
