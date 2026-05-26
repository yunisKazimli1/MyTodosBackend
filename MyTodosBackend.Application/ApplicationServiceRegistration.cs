using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using MyTodosBackend.Application.DTOs;
using MyTodosBackend.Application.Implementations;
using MyTodosBackend.Application.Interfaces;
using MyTodosBackend.Application.Queries;
using MyTodosBackend.Application.Validators;

namespace MyTodosBackend.Application
{
    public static class ApplicationServiceRegistration
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            services.AddScoped<ITodoManager, TodoManager>();
            services.AddScoped<IValidator<GetTodosQuery>, GetTodosQueryValidator>();
            services.AddScoped<IValidator<AddTodoDto>, AddTodoDtoValidator>();
            services.AddScoped<IValidator<UpdateTodoDateDto>, UpdateTodoDateDtoValidator>();

            return services;
        }
    }
}
