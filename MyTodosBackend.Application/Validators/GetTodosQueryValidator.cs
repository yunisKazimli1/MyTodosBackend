using FluentValidation;
using MyTodosBackend.Application.Queries;

namespace MyTodosBackend.Application.Validators
{
    public class GetTodosQueryValidator : AbstractValidator<GetTodosQuery>
    {
        public GetTodosQueryValidator()
        {
            RuleFor(x => x.Page)
                .GreaterThan(0);

            RuleFor(x => x.PageSize)
                .GreaterThan(0);
        }
    }
}
