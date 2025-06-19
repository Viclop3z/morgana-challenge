using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UmbracoBridge.Domain.Models
{
    public class DocumentTypeRequest
    {

        public string Alias { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Icon { get; set; }
        public bool AllowedAsRoot { get; set; }
        public string Title { get; set; }
        public bool VariesByCulture { get; set; }
        public bool VariesBySegment { get; set; }
        public object? Collection { get; set; }
        public bool IsElement { get; set; }
    }
}
