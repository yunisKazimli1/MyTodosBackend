using MyTodosBackend.Application.DTOs;
using MyTodosBackend.Application.Utility.Responses;
using System.Net;
using System.Net.Http.Json;
using Xunit;

namespace MyTodosBackend.Tests.Integration;

public class TodoApiTests(CustomWebApplicationFactory factory) : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient client = factory.CreateClient();

    private static readonly CancellationToken Ct = CancellationToken.None;

    [Fact]
    public async Task POST_CreateTodo_ShouldReturnCreated()
    {
        var request = new AddTodoDto
        {
            Title = "Integration test todo"
        };

        var response = await client.PostAsJsonAsync("/api/todo", request, Ct);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        var created = await response.Content.ReadFromJsonAsync<GetTodoDto>(Ct);

        Assert.NotNull(created);
        Assert.Equal(request.Title, created!.Title);
    }

    [Fact]
    public async Task GET_ById_ShouldReturnTodo()
    {
        var createResponse = await client.PostAsJsonAsync("/api/todo",
            new AddTodoDto { Title = "Get by id test" }, Ct);

        var created = await createResponse.Content.ReadFromJsonAsync<GetTodoDto>(Ct);

        var response = await client.GetAsync($"/api/todo/{created!.Id}", Ct);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var todo = await response.Content.ReadFromJsonAsync<GetTodoDto>(Ct);

        Assert.Equal(created.Id, todo!.Id);
    }

    [Fact]
    public async Task DELETE_ShouldRemoveTodo()
    {
        var createResponse = await client.PostAsJsonAsync("/api/todo",
            new AddTodoDto { Title = "Delete test" }, Ct);

        var created = await createResponse.Content.ReadFromJsonAsync<GetTodoDto>(Ct);

        var deleteResponse = await client.DeleteAsync($"/api/todo/{created!.Id}", Ct);

        Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);
    }

    [Fact]
    public async Task PATCH_Complete_ShouldMarkAsCompleted()
    {
        var createResponse = await client.PostAsJsonAsync("/api/todo",
            new AddTodoDto { Title = "Complete test" }, Ct);

        var created = await createResponse.Content.ReadFromJsonAsync<GetTodoDto>(Ct);

        var response = await client.PatchAsync($"/api/todo/{created!.Id}/complete", null, Ct);

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    [Fact]
    public async Task GET_All_ShouldReturnPagedResult()
    {
        for (int i = 0; i < 5; i++)
        {
            await client.PostAsJsonAsync("/api/todo",
                new AddTodoDto { Title = $"Todo {i}" }, Ct);
        }

        var response = await client.GetAsync("/api/todo?page=1&pageSize=3", Ct);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var result = await response.Content.ReadFromJsonAsync<PagedResult<GetTodoDto>>(Ct);

        Assert.NotNull(result);
        Assert.True(result!.Items?.Count <= 3);
    }
}