using Microsoft.AspNetCore.Mvc;
using MyTodosBackend.Application.DTOs;
using MyTodosBackend.Application.Interfaces;
using MyTodosBackend.Application.Queries;
using MyTodosBackend.Application.Utility.Responses;

namespace MyTodosBackend.Api.Controllers
{
    [ApiController]
    [Route("api/todo")] 
    public class TodoController(
        ITodoManager _todoManager) : ControllerBase
    {
        [HttpGet("{id}")]
        public async Task<IActionResult> GetTodoById(Guid id)
        {
            var response = await _todoManager.GetTodoById(id);

            return Ok(response);
        }

        [HttpGet]
        public async Task<ActionResult<PagedResult<GetTodoDto>>> GetTodos([FromQuery]GetTodosQuery query)
        {
            var response = await _todoManager.GetTodos(query);

            return Ok(response);
        }

        [HttpPost]
        public async Task<ActionResult<GetTodoDto>> AddTodo([FromBody]AddTodoDto addTodoDto)
        {
            var response = await _todoManager.AddTodo(addTodoDto);

            return CreatedAtAction
            (
                nameof(GetTodoById),
                new { id = response?.Id },
                response
            );
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTodo(Guid id)
        {
            await _todoManager.DeleteTodo(id);

            return NoContent();
        }

        [HttpPatch("{id}/complete")]
        public async Task<IActionResult> CompleteTodo(Guid id)
        {
            await _todoManager.CompleteTodo(id);

            return NoContent();
        }

        [HttpPatch("{id}/updateDate")]
        public async Task<IActionResult> UpdateTodoDate(Guid id, [FromQuery]UpdateTodoDateDto updateTodoDateDto)
        {
            await _todoManager.UpdateTodoDate(id, updateTodoDateDto);

            return NoContent();
        }
    }
}
