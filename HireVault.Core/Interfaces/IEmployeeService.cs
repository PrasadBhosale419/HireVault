using HireVault.Core.Entities;
using HireVault.Core.Enums;

namespace HireVault.Core.Interfaces
{
    public interface IEmployeeService
    {
        Task<Employee> GetEmployeeByCognitoIdAsync(string cognitoUserId);
        Task<Employee> CreateEmployeeAsync(string cognitoUserId, string email, string firstName, string lastName);
        Task UpdateEmployeeProfileAsync(string cognitoUserId, string? phoneNumber, DateTime? dateOfBirth, string? address);
        Task UpdateOnboardingStatusAsync(int employeeId, OnboardingStatus status);
        Task<IEnumerable<Employee>> GetEmployeesByStatusAsync(OnboardingStatus? status = null);
    }
}
