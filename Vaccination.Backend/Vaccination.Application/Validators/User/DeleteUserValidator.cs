using FluentValidation;
using Vaccination.Application.Dtos.User;

namespace Vaccination.Application.Validators.User
{
    public class DeleteUserValidator : AbstractValidator<DeleteUserRequest>
    {
        public DeleteUserValidator()
        {
            RuleFor(x => x.UserId)
                .NotEmpty()
                .WithMessage("Token invalid, User not found");
        }
    }
}