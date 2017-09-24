using IDDD.Core.Cqs.Command;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace IDDD.App.Cqs.Commands.Users
{
    public class ConfirmEmailCommand: ICommand, IValidatableObject
    {
        public string UserName  { get; set; }
        
        public string ConfirmationToken { get; set; }

        public IEnumerable<ValidationResult> Validate(System.ComponentModel.DataAnnotations.ValidationContext validationContext)
        {
            var validator = new ConfirmUserCommandValidator();
            var result = validator.Validate(this);
            return result.Errors.Select(item => new ValidationResult(item.ErrorMessage, new[] { item.PropertyName }));

        }
    }

    public class ConfirmUserCommandValidator: AbstractValidator<ConfirmEmailCommand>
    {
        public ConfirmUserCommandValidator()
        {
            RuleFor(command => command.UserName).NotEmpty();
            RuleFor(command => command.ConfirmationToken).NotEmpty();
        }
    }
}
