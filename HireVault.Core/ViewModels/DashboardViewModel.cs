using HireVault.Core.Entities;

namespace HireVault.Web.Models.ViewModels
{
    public class DashboardViewModel
    {
        public int TotalApplicants { get; set; }
        public int PendingCount { get; set; }
        public int VerifiedCount { get; set; }
        public int ShortlistedCount { get; set; }
    }
}
