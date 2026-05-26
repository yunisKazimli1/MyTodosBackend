using FluentValidation;
using MyTodosBackend.Application.DTOs;

namespace MyTodosBackend.Application.Validators
{
    public class UpdateTodoDateDtoValidator : AbstractValidator<UpdateTodoDateDto>
    {
        public UpdateTodoDateDtoValidator()
        {
            RuleFor(x => x.DueDate)
                .NotEmpty()
                .GreaterThanOrEqualTo(DateTime.UtcNow.Date);
        }
    }
}
