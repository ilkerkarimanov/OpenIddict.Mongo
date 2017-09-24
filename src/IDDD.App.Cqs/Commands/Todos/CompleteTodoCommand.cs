using IDDD.Core.Cqs.Command;
using IDDD.Domain.Todos;
using FluentValidation;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace IDDD.App.Cqs.Commands.Todos
{
    public class CompleteTodoCommand : ICommand, IValidatableObject
    {
        public string Id { get; set; }
        public IEnumerable<System.ComponentModel.DataAnnotations.ValidationResult> Validate(System.ComponentModel.DataAnnotations.ValidationContext validationContext)
        {
            var validator = new CompleteTodoCommandValidator();
            var result = validator.Validate(this);
            return result.Errors.Select(item => new System.ComponentModel.DataAnnotations.ValidationResult(item.ErrorMessage, new[] { item.PropertyName }));

        }
    }

    public class CompleteTodoCommandValidator : AbstractValidator<CompleteTodoCommand>
    {
        public CompleteTodoCommandValidator()
        {
            RuleFor(command => command.Id).NotEmpty();
        }
    }

    public static class CompleteTodoFactory
    {
        public static TodoAction Create(CompleteTodoCommand command)
        {
            return new TodoAction(
                TodoState.Completed
                );
        }
    }

}
