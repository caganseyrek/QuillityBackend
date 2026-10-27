using FluentValidation;
using Quillity.Application.Workspaces.Core.Commands;
using Quillity.Common.Helpers;
using Quillity.Common.Response;

namespace Quillity.Application.Workspaces.Core.Validators;

public class DeleteWorkspaceCommandValidator : AbstractValidator<DeleteWorkspaceCommand>
{
    public DeleteWorkspaceCommandValidator()
    {
        RuleFor(x => x.WorkspaceId)
            .NotEmpty()
                .WithMessage(ErrorMessages.GetCannotBeEmptyError(ErrorMessageModel.Workspace, "id"))
            .Must(ValidationHelper.IsValidOrNonEmptyGuid)
                .WithMessage(ErrorMessages.GetInvalidIdFormatError(ErrorMessageModel.Workspace));
    }
}
