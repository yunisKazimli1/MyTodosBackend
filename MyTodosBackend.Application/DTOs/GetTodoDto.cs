namespace MyTodosBackend.Application.DTOs
{
    public class GetTodoDto
    {
        public Guid Id { get; set; }
        public required string Title { get; set; }
        public DateOnly? DueDate { get; set; }
        public bool IsCompleted { get; set; }
        public bool IsOverdue => !IsCompleted && DueDate != null && DueDate < DateOnly.FromDateTime(DateTime.UtcNow);
    }
}
