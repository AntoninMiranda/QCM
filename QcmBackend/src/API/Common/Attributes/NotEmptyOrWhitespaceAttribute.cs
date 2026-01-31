using System;
using System.ComponentModel.DataAnnotations;

namespace QcmBackend.API.Common.Attributes;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter)]
public class NotEmptyOrWhitespaceAttribute : ValidationAttribute
{
    public NotEmptyOrWhitespaceAttribute() : base("Le champ {0} ne peut pas être vide ou contenir uniquement des espaces.")
    {
    }

    public NotEmptyOrWhitespaceAttribute(string errorMessage) : base(errorMessage)
    {
    }

    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value is string stringValue)
        {
            if (string.IsNullOrWhiteSpace(stringValue))
            {
                return new ValidationResult(FormatErrorMessage(validationContext.DisplayName), 
                    new[] { validationContext.MemberName ?? validationContext.DisplayName });
            }
        }

        return ValidationResult.Success;
    }
}
