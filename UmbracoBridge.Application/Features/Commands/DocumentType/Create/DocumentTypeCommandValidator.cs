using FluentValidation;
using UmbracoBridge.Application.Constants;

namespace UmbracoBridge.Application.Features.Commands.DocumentType.Create
{
    public class DocumentTypeCommandValidator : AbstractValidator<DocumentTypeCommand>
    {
        public DocumentTypeCommandValidator()
        {
            RuleFor(p => p.Alias)
                .NotEmpty()
                .WithMessage(ValidationErrorConstants.AliasNotEmpty);

            RuleFor(p => p.Name)
                .NotEmpty()
                .WithMessage(ValidationErrorConstants.NameNotEmpty);

            RuleFor(p => p.Description)
                .NotEmpty()
                .WithMessage(ValidationErrorConstants.DescriptionNotEmpty);

            RuleFor(p => p.Icon)
              .Must(icon => icon.StartsWith("icon-"))
              .WithMessage(ValidationErrorConstants.IconPrefix);

        }
    }
}
