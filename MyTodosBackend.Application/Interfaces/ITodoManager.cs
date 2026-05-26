using MyTodosBackend.Application.DTOs;
using MyTodosBackend.Application.Queries;
using MyTodosBackend.Application.Utility.Responses;

namespace MyTodosBackend.Application.Interfaces
{
    public interface ITodoManager
    {
        public Task<GetTodoDto> GetTodoById(Guid id);

        public Task<PagedResult<GetTodoDto>> GetTodos(GetTodosQuery query);

        public Task<GetTodoDto> AddTodo(AddTodoDto addTodoDto);

        public Task DeleteTodo(Guid id);

        public Task CompleteTodo(Guid id);

        public Task UpdateTodoDate(Guid id, UpdateTodoDateDto updateTodoDateDto);
    }
}
