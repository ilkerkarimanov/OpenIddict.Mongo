using IDDD.Core.Cqs.Command;
using System.ComponentModel.DataAnnotations;
using System;
using System.Collections.Generic;
using FluentValidation;
using System.Linq;

namespace IDDD.App.Cqs.Commands.Users
{
    public class CreatePasswordCommand: ICommand, IValidatableObject
    {
        public string Password { get; set; }
        
        public string UserName { get; set; }

        public IEnumerable<ValidationResult> Validate(System.ComponentModel.DataAnnotations.ValidationContext validationContext)
        {
            var validator = new CreatePasswordCommandValidator();
            var result = validator.Validate(this);
            return result.Errors.Select(item => new ValidationResult(item.ErrorMessage, new[] { item.PropertyName }));

        }
    }

    public class CreatePasswordCommandValidator: AbstractValidator<CreatePasswordCommand>
    {
        public CreatePasswordCommandValidator()
        {
            RuleFor(command => command.UserName).NotEmpty();
            RuleFor(command => command.Password).NotEmpty().Length(6, 100);
        }
    }
}
