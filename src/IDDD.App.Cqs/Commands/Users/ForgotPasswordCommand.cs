using IDDD.Core.Cqs.Command;
using System.ComponentModel.DataAnnotations;
using System;
using System.Collections.Generic;
using FluentValidation;
using System.Linq;

namespace IDDD.App.Cqs.Commands.Users
{
    public class ForgotPasswordCommand : ICommand, IValidatableObject
    {
        public string Email { get; set; }
        public string ClientId { get; set; }

        public IEnumerable<ValidationResult> Validate(System.ComponentModel.DataAnnotations.ValidationContext validationContext)
        {
            var validator = new RequestResetPasswordCommandValidator();
            var result = validator.Validate(this);
            return result.Errors.Select(item => new ValidationResult(item.ErrorMessage, new[] { item.PropertyName }));

        }

    }

    public class RequestResetPasswordCommandValidator : AbstractValidator<ForgotPasswordCommand>
    {
        public RequestResetPasswordCommandValidator()
        {
            RuleFor(command => command.Email).NotEmpty();
            RuleFor(command => command.ClientId).NotEmpty();
        }
    }
}
