using FluentValidation;
using Vaccination.Application.Dtos.User;

namespace Vaccination.Application.Validators.User
{
    public class GetUserValidator : AbstractValidator<UserDetailsRequest>
    {
        public GetUserValidator()
        {
            RuleFor(x => x.UserId)
                .NotEmpty()
                .WithMessage("Token invalid, User not found");
        }
    }
}