using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Moq;
using MyTodosBackend.Application.DTOs;
using MyTodosBackend.Application.Implementations;
using MyTodosBackend.Application.Queries;
using MyTodosBackend.Infrastructure.Context;
using ValidationResult = FluentValidation.Results.ValidationResult;

namespace MyTodosBackend.Tests;

public class TodoManagerTests
{
    private readonly AppDbContext _context;

    private readonly Mock<IValidator<AddTodoDto>> _addValidator;
    private readonly Mock<IValidator<GetTodosQuery>> _queryValidator;
    private readonly Mock<IValidator<UpdateTodoDateDto>> _updateValidator;

    private readonly TodoManager _todoManager;

    public TodoManagerTests()
    {
        DbContextOptions<AppDbContext> options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        _context = new AppDbContext(options);

        _addValidator = new Mock<IValidator<AddTodoDto>>();
        _queryValidator = new Mock<IValidator<GetTodosQuery>>();
        _updateValidator = new Mock<IValidator<UpdateTodoDateDto>>();

        _addValidator
            .Setup(v => v.ValidateAsync(It.IsAny<ValidationContext<AddTodoDto>>(), default))
            .ReturnsAsync(new ValidationResult());

        _queryValidator
            .Setup(v => v.ValidateAsync(It.IsAny<ValidationContext<GetTodosQuery>>(), default))
            .ReturnsAsync(new ValidationResult());

        _updateValidator
            .Setup(v => v.ValidateAsync(It.IsAny<ValidationContext<UpdateTodoDateDto>>(), default))
            .ReturnsAsync(new ValidationResult());

        _todoManager = new TodoManager(
            _context,
            _addValidator.Object,
            _queryValidator.Object,
            _updateValidator.Object);
    }
}