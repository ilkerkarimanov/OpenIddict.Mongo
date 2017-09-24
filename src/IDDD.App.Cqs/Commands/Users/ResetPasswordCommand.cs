using IDDD.Core.Cqs.Command;
using System.ComponentModel.DataAnnotations;
using System;
using System.Collections.Generic;
using FluentValidation;
using System.Linq;

namespace IDDD.App.Cqs.Commands.Users
{
    public class ResetPasswordCommand : ICommand, IValidatableObject
    {
        public string UserName { get; set; }
        public string NewPassword { get; set; }

        public string ConfirmationToken { get; set; }

        public IEnumerable<ValidationResult> Validate(System.ComponentModel.DataAnnotations.ValidationContext validationContext)
        {
            var validator = new ResetPasswordCommandValidator();
            var result = validator.Validate(this);
            return result.Errors.Select(item => new ValidationResult(item.ErrorMessage, new[] { item.PropertyName }));

        }

    }

    public class ResetPasswordCommandValidator : AbstractValidator<ResetPasswordCommand>
    {
        public ResetPasswordCommandValidator()
        {
            RuleFor(command => command.UserName).NotEmpty();
            RuleFor(command => command.NewPassword).NotEmpty();
            RuleFor(command => command.ConfirmationToken).NotEmpty();
        }
    }
}
