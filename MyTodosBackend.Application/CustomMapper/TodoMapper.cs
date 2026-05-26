using MyTodosBackend.Application.DTOs;
using MyTodosBackend.Domain.Entities;

namespace MyTodosBackend.Application.CustomMapper
{
    public class TodoMapper
    {
        public static GetTodoDto ToDto(Todo entity)
        {
            return new GetTodoDto
            {
                Id = entity.Id,
                Title = entity.Title,
                IsCompleted = entity.IsCompleted,
                DueDate = entity.DueDate
            };
        }

        public static Todo ToEntity(AddTodoDto dto)
        {
            return new Todo
            {
                Title = dto.Title,
                IsCompleted = false
            };
        }
    }
}
