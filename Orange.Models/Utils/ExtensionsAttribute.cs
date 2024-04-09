using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace Orange.Models.Utils
{
    public class ExtensionsAttribute : ValidationAttribute
    {
        private readonly string[] _extensions;
        public ExtensionsAttribute(string[] extensions)
        {
            _extensions = extensions;
        }

        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            var file = value as IFormFile;

            if (file != null)
            {
                var extension = Path.GetExtension(file.FileName);
                if (!_extensions.Contains(extension.ToLower()))
                {
                    return new ValidationResult("This file extension is not allowed.");
                }
            }

            return ValidationResult.Success;
        }
    }
}
