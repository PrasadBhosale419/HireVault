namespace HireVault.Core.Entities
{
    public class DocumentType
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public bool IsRequired { get; set; }
        
        // Navigation property
        public ICollection<Document> Documents { get; set; } = new List<Document>();
    }
}
