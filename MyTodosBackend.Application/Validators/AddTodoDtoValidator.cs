using FluentValidation;
using MyTodosBackend.Application.DTOs;

namespace MyTodosBackend.Application.Validators
{
    public class AddTodoDtoValidator : AbstractValidator<AddTodoDto>
    {
        public AddTodoDtoValidator()
        {
            RuleFor(x => x.Title)
                .NotEmpty()
                .MinimumLength(10);
        }
    }
}
