using HireVault.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HireVault.Core.DTOs
{
    public class ViewDocumentDTO
    {
        public string DocumentName { get; set; }
        public string DocumentUrl { get; set; }    
        public string ContentType { get; set; }
        public DocumentType DocumentType { get; set; }
    }
}
