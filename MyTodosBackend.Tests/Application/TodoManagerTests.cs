using FluentValidation;
using FluentValidation.Results;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Moq;
using MyTodosBackend.Application.DTOs;
using MyTodosBackend.Application.Implementations;
using MyTodosBackend.Application.Queries;
using MyTodosBackend.Application.Utility.CustomExceptions;
using MyTodosBackend.Domain.Entities;
using MyTodosBackend.Infrastructure.Context;
using Xunit;

namespace MyTodosBackend.Tests.Application;

public class TodoManagerTests : IDisposable
{
    private readonly SqliteConnection _connection;
    private readonly AppDbContext _context;

    private readonly Mock<IValidator<AddTodoDto>> _addValidatorMock;
    private readonly Mock<IValidator<GetTodosQuery>> _getTodosValidatorMock;
    private readonly Mock<IValidator<UpdateTodoDateDto>> _updateDateValidatorMock;

    private readonly TodoManager _todoManager;

    private static readonly CancellationToken Ct = CancellationToken.None;

    public TodoManagerTests()
    {
        _connection = new SqliteConnection("Filename=:memory:");
        _connection.Open();

        DbContextOptions<AppDbContext> options =
            new DbContextOptionsBuilder<AppDbContext>()
                .UseSqlite(_connection)
                .Options;

        _context = new AppDbContext(options);

        _context.Database.EnsureCreated();

        _addValidatorMock = new Mock<IValidator<AddTodoDto>>();
        _getTodosValidatorMock = new Mock<IValidator<GetTodosQuery>>();
        _updateDateValidatorMock = new Mock<IValidator<UpdateTodoDateDto>>();

        SetupValidators();

        _todoManager = new TodoManager(
            _context,
            _addValidatorMock.Object,
            _getTodosValidatorMock.Object,
            _updateDateValidatorMock.Object);
    }

    private void SetupValidators()
    {
        _addValidatorMock
            .Setup(v => v.ValidateAsync(
                It.IsAny<ValidationContext<AddTodoDto>>(),
                default))
            .ReturnsAsync(new ValidationResult());

        _getTodosValidatorMock
            .Setup(v => v.ValidateAsync(
                It.IsAny<ValidationContext<GetTodosQuery>>(),
                default))
            .ReturnsAsync(new ValidationResult());

        _updateDateValidatorMock
            .Setup(v => v.ValidateAsync(
                It.IsAny<ValidationContext<UpdateTodoDateDto>>(),
                default))
            .ReturnsAsync(new ValidationResult());
    }

    [Fact]
    public async Task AddTodo_ShouldCreateTodo_WhenRequestIsValid()
    {
        AddTodoDto dto = new()
        {
            Title = "This is valid todo title"
        };

        GetTodoDto result = await _todoManager.AddTodo(dto);

        Assert.NotNull(result);

        Todo? savedTodo = await _context.Todos.FirstOrDefaultAsync(Ct);

        Assert.NotNull(savedTodo);
        Assert.Equal(dto.Title, savedTodo.Title);
        Assert.False(savedTodo.IsCompleted);
        Assert.True(savedTodo.CreatedAt <= DateTime.UtcNow);
    }

    [Fact]
    public async Task AddTodo_ShouldThrowValidationException_WhenValidationFails()
    {
        _addValidatorMock
            .Setup(v => v.ValidateAsync(It.IsAny<ValidationContext<AddTodoDto>>(), default))
            .ReturnsAsync(new ValidationResult(new[]
            {
            new ValidationFailure("Title", "Title too short")
            }));

        AddTodoDto dto = new()
        {
            Title = "short"
        };

        await Assert.ThrowsAsync<ValidationException>(() =>
            _todoManager.AddTodo(dto));
    }

    [Fact]
    public async Task GetTodoById_ShouldReturnTodo_WhenTodoExists()
    {
        Todo todo = new()
        {
            Id = Guid.NewGuid(),
            Title = "Test todo",
            CreatedAt = DateTime.UtcNow
        };

        _context.Todos.Add(todo);
        await _context.SaveChangesAsync(Ct);

        GetTodoDto result = await _todoManager.GetTodoById(todo.Id);

        Assert.NotNull(result);
        Assert.Equal(todo.Id, result.Id);
        Assert.Equal(todo.Title, result.Title);
    }

    [Fact]
    public async Task GetTodoById_ShouldThrow_WhenTodoDoesNotExist()
    {
        Guid id = Guid.NewGuid();

        await Assert.ThrowsAsync<ItemNotFoundException>(() =>
            _todoManager.GetTodoById(id));
    }

    [Fact]
    public async Task DeleteTodo_ShouldRemoveTodo_WhenTodoExists()
    {
        Todo todo = new()
        {
            Id = Guid.NewGuid(),
            Title = "Delete me",
            CreatedAt = DateTime.UtcNow
        };

        _context.Todos.Add(todo);
        await _context.SaveChangesAsync(Ct);

        await _todoManager.DeleteTodo(todo.Id);

        Todo? deletedTodo = await _context.Todos.FindAsync([todo.Id], TestContext.Current.CancellationToken);

        Assert.Null(deletedTodo);
    }

    [Fact]
    public async Task CompleteTodo_ShouldMarkTodoAsCompleted()
    {
        Todo todo = new()
        {
            Id = Guid.NewGuid(),
            Title = "Todo",
            IsCompleted = false,
            CreatedAt = DateTime.UtcNow
        };

        _context.Todos.Add(todo);
        await _context.SaveChangesAsync(Ct);

        await _todoManager.CompleteTodo(todo.Id);

        Todo? updatedTodo = await _context.Todos.FindAsync([todo.Id], TestContext.Current.CancellationToken);

        Assert.NotNull(updatedTodo);
        Assert.True(updatedTodo.IsCompleted);
    }

    [Fact]
    public async Task UpdateTodoDate_ShouldUpdateDueDate()
    {
        Todo todo = new()
        {
            Id = Guid.NewGuid(),
            Title = "Todo",
            CreatedAt = DateTime.UtcNow
        };

        _context.Todos.Add(todo);
        await _context.SaveChangesAsync(Ct);

        DateTime newDate = DateTime.UtcNow.AddDays(5);

        UpdateTodoDateDto dto = new()
        {
            DueDate = newDate
        };

        await _todoManager.UpdateTodoDate(todo.Id, dto);

        Todo? updatedTodo = await _context.Todos.FindAsync([todo.Id], TestContext.Current.CancellationToken);

        Assert.NotNull(updatedTodo);
        Assert.Equal(
            DateOnly.FromDateTime(newDate),
            updatedTodo.DueDate);
    }

    [Fact]
    public async Task GetTodos_ShouldReturnPagedResult()
    {
        List<Todo> todos = Enumerable.Range(1, 10)
            .Select(i => new Todo
            {
                Id = Guid.NewGuid(),
                Title = $"Todo {i}",
                CreatedAt = DateTime.UtcNow
            })
            .ToList();

        _context.Todos.AddRange(todos);
        await _context.SaveChangesAsync(Ct);

        GetTodosQuery query = new()
        {
            Page = 1,
            PageSize = 5
        };

        var result = await _todoManager.GetTodos(query);

        Assert.Equal(5, result.Items?.Count);
        Assert.Equal(10, result.TotalCount);
    }

    public void Dispose()
    {
        _context.Dispose();
        _connection.Dispose();
    }
}