namespace MyTodosBackend.Domain.Entities
{
    public class Todo
    {
        public Guid Id { get; set; }
        public string? Title { get; set; }
        public DateOnly? DueDate { get; set; }
        public bool IsCompleted { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
