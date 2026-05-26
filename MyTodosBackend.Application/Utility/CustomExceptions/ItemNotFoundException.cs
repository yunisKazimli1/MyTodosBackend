namespace MyTodosBackend.Application.Utility.CustomExceptions
{
    public class ItemNotFoundException(Guid id) : Exception($"Item with id '{id}' not found")
    {
    }
}
