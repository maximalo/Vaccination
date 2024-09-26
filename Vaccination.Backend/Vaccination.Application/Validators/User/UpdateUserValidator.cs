using FluentValidation;
using Vaccination.Application.Dtos.User;

namespace Vaccination.Application.Validators.User
{
    public class UpdateUserValidator : AbstractValidator<UpdateUserRequest>
    {
        public UpdateUserValidator()
        {
            RuleFor(x => x.Email)

                .EmailAddress()
                .WithMessage("Email is not valid");

            RuleFor(x => x.FirstName)
                .MaximumLength(150)
                .WithMessage("First name should be maximum 150 characters");

            RuleFor(x => x.LastName)
                .MaximumLength(150)
                .WithMessage("Last name should be maximum 150 characters");

            RuleFor(x => x.SocialSecurityNumber)
                .MaximumLength(13)
                .WithMessage("Social security number should be maximum 13 characters");

            RuleFor(x => x.City)
                .MaximumLength(100)
                .WithMessage("City should be maximum 100 characters");

            RuleFor(x => x.Nationality)
                .MaximumLength(100)
                .WithMessage("National should be maximum 100 characters");

            RuleFor(x => x.PostalCode)
                .MaximumLength(20)
                .WithMessage("Postal code should be maximum 20 characters");
        }
    }
}