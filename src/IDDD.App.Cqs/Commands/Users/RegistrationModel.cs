using FluentValidation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace IDDD.App.Cqs.Commands.Users
{
    public class RegistrationModel: IValidatableObject
    {
        public string Email { get; set; }
        public string ClientKey { get; set; }


        public IEnumerable<ValidationResult> Validate(System.ComponentModel.DataAnnotations.ValidationContext validationContext)
        {
            var validator = new RegistrationModelValidator();
            var result = validator.Validate(this);
            return result.Errors.Select(item => new ValidationResult(item.ErrorMessage, new[] { item.PropertyName }));

        }
    }
    
    public class RegistrationModelValidator: AbstractValidator<RegistrationModel>
    {
        public RegistrationModelValidator()
        {
            RuleFor(command => command.Email).NotEmpty().EmailAddress();
            RuleFor(command => command.ClientKey).NotEmpty();
        }
    }
}
