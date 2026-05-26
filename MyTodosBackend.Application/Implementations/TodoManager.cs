using FluentValidation;
using Microsoft.EntityFrameworkCore;
using MyTodosBackend.Application.CustomMapper;
using MyTodosBackend.Application.DTOs;
using MyTodosBackend.Application.Interfaces;
using MyTodosBackend.Application.Queries;
using MyTodosBackend.Application.Utility.CustomExceptions;
using MyTodosBackend.Domain.Entities;
using MyTodosBackend.Domain.Enums;
using MyTodosBackend.Infrastructure.Context;
using System.Linq.Dynamic.Core;

namespace MyTodosBackend.Application.Implementations
{
    public class TodoManager(AppDbContext _appDbContext, IValidator<AddTodoDto> _addTodoDtoValidator,
        IValidator<GetTodosQuery> getTodosQueryValidator, IValidator<UpdateTodoDateDto> _updateTodoDateDtoValidator) : ITodoManager
    {

        public async Task<GetTodoDto> AddTodo(AddTodoDto addTodoDto)
        {
            await _addTodoDtoValidator.ValidateAndThrowAsync(addTodoDto);

            Todo todo = TodoMapper.ToEntity(addTodoDto);
            todo.CreatedAt = DateTime.UtcNow;

            _appDbContext.Todos.Add(todo);
            await _appDbContext.SaveChangesAsync();

            return TodoMapper.ToDto(todo);
        }

        public async Task CompleteTodo(Guid id)
        {
            Todo? todo = await _appDbContext.Todos.FindAsync(id);

            if (todo != null) todo.IsCompleted = true;
            else throw new ItemNotFoundException(id);

            await _appDbContext.SaveChangesAsync();
        }

        public async Task DeleteTodo(Guid id)
        {
            Todo? todo = await _appDbContext.Todos.FindAsync(id);

            if (todo != null) _appDbContext.Remove(todo);
            else throw new ItemNotFoundException(id);

            await _appDbContext.SaveChangesAsync();
        }

        public async Task<GetTodoDto> GetTodoById(Guid id)
        {
            Todo? todo = await _appDbContext.Todos.FindAsync(id);

            if (todo == null) throw new ItemNotFoundException(id);

            return TodoMapper.ToDto(todo);
        }

        public async Task<Utility.Responses.PagedResult<GetTodoDto>> GetTodos(GetTodosQuery query)
        {
            await getTodosQueryValidator.ValidateAndThrowAsync(query);

            IQueryable<Todo> queryable = _appDbContext.Todos.AsQueryable();

            queryable = query.FilterBy switch
            {
                TodoFilterEnum.All => queryable,
                TodoFilterEnum.Active => queryable.Where(el => !el.IsCompleted),
                TodoFilterEnum.Completed => queryable.Where(el => el.IsCompleted),
                TodoFilterEnum.Overdue => queryable.Where(el => !el.IsCompleted && el.DueDate != null && el.DueDate < DateOnly.FromDateTime(DateTime.UtcNow)),
                _ => queryable
            };

            queryable = query.SortBy switch
            {
                TodoSortingEnum.Az => queryable.OrderBy(el => el.Title),
                TodoSortingEnum.Za => queryable.OrderByDescending(el => el.Title),
                TodoSortingEnum.DueDateEarliestFirst => queryable.OrderBy(el => el.DueDate),
                TodoSortingEnum.DueDateLatestFirst => queryable.OrderByDescending(el => el.DueDate),
                _ => queryable
            };

            int totalCount = await queryable.CountAsync();

            List<GetTodoDto> items = await queryable
                .Skip((query.Page - 1) * query.PageSize)
                .Take(query.PageSize)
                .Select(el => TodoMapper.ToDto(el))
                .ToListAsync();

            Utility.Responses.PagedResult<GetTodoDto> result = new Utility.Responses.PagedResult<GetTodoDto>()
            {
                Items = items,
                Page = query.Page,
                PageSize = query.PageSize,
                TotalCount = totalCount
            };

            return result;
        }

        public async Task UpdateTodoDate(Guid id, UpdateTodoDateDto updateTodoDateDto)
        {
            await _updateTodoDateDtoValidator.ValidateAndThrowAsync(updateTodoDateDto);

            Todo? todo = await _appDbContext.Todos.FindAsync(id);

            if (todo != null) todo.DueDate = DateOnly.FromDateTime(updateTodoDateDto.DueDate);
            else throw new ItemNotFoundException(id);
            
            await _appDbContext.SaveChangesAsync();
        }
    }
}
