using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UmbracoBridge.Application.Constants
{
    [ExcludeFromCodeCoverage]
    public static class ValidationErrorConstants
    {
        public const string AliasNotEmpty = "Alias must not be empty";
        public const string NameNotEmpty = "Name must not be empty";
        public const string DescriptionNotEmpty = "Description must not be empty";
        public const string IconPrefix = "Icon must start with icon-";
        
    }
}
