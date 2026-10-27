using FluentValidation;
using Quillity.Application.Workspaces.Core.Commands;
using Quillity.Common.Helpers;
using Quillity.Common.Response;
using Quillity.Common.Values;

namespace Quillity.Application.Workspaces.Core.Validators;

public class CreateWorkspaceCommandValidator : AbstractValidator<CreateWorkspaceCommand>
{
    public CreateWorkspaceCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
                .WithMessage(ErrorMessages.GetCannotBeEmptyError(ErrorMessageModel.Workspace, "name"))
            .MinimumLength(Constraints.MIN_STRING_LENGTH)
                .WithMessage(ErrorMessages.GetStringLengthError(ErrorMessageModel.Workspace, "name", Constraints.NAME_STRING_LENGTH))
            .MaximumLength(Constraints.NAME_STRING_LENGTH)
                .WithMessage(ErrorMessages.GetStringLengthError(ErrorMessageModel.Workspace, "name", Constraints.NAME_STRING_LENGTH));

        RuleFor(x => x.Description)
            .NotEmpty()
                .WithMessage(ErrorMessages.GetCannotBeEmptyError(ErrorMessageModel.Workspace, "description"))
            .MinimumLength(Constraints.MIN_STRING_LENGTH)
                .WithMessage(ErrorMessages.GetStringLengthError(ErrorMessageModel.Workspace, "name", Constraints.NAME_STRING_LENGTH))
            .MaximumLength(Constraints.NAME_STRING_LENGTH)
                .WithMessage(ErrorMessages.GetStringLengthError(ErrorMessageModel.Workspace, "name", Constraints.NAME_STRING_LENGTH));

        RuleFor(x => x.UrlKey)
            .Matches(Constraints.Regex.LetterOnly)
                .WithMessage(ErrorMessages.GetUrlKeyCharacterError(ErrorMessageModel.Workspace))
            .MinimumLength(Constraints.MIN_STRING_LENGTH)
                .WithMessage(ErrorMessages.GetUrlKeyLengthError(ErrorMessageModel.Workspace))
            .MaximumLength(Constraints.NAME_STRING_LENGTH)
                .WithMessage(ErrorMessages.GetUrlKeyLengthError(ErrorMessageModel.Workspace));

        RuleFor(x => x.LogoImageUrl)
            .NotEmpty()
                .WithMessage(ErrorMessages.GetCannotBeEmptyError(ErrorMessageModel.Workspace, "logo image url"))
            .Must(ValidationHelper.IsValidURL)
                .WithMessage(ErrorMessages.GetInvalidUrlError(ErrorMessageModel.Workspace, "logo image url"));
    }
}
