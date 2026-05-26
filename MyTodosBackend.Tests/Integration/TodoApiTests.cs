using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using MyTodosBackend.Application.DTOs;
using MyTodosBackend.Application.Utility.Responses;
using MyTodosBackend.Infrastructure.Context;
using System.Net;
using System.Net.Http.Json;
using Xunit;

namespace MyTodosBackend.Tests.Integration;

public class TodoApiTests(WebApplicationFactory<Program> factory) : IClassFixture<WebApplicationFactory<Program>>
{
    private static readonly CancellationToken Ct = CancellationToken.None;

    private HttpClient CreateClient()
    {
        return factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureServices(services =>
            {
                // Remove existing DbContext
                var descriptor = services.SingleOrDefault(
                    d => d.ServiceType == typeof(DbContextOptions<AppDbContext>));

                if (descriptor != null)
                    services.Remove(descriptor);

                // Add SQLite in-memory DB (real relational behavior)
                var connection = new SqliteConnection("Filename=:memory:");
                connection.Open();

                services.AddSingleton(connection);

                services.AddDbContext<AppDbContext>(options =>
                {
                    options.UseSqlite(connection);
                });

                // Build service provider to create DB
                var sp = services.BuildServiceProvider();

                using var scope = sp.CreateScope();
                var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

                db.Database.EnsureCreated();
            });
        }).CreateClient();
    }

    [Fact]
    public async Task POST_CreateTodo_ShouldReturnCreated()
    {
        var client = CreateClient();

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
        var client = CreateClient();

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
        var client = CreateClient();

        var createResponse = await client.PostAsJsonAsync("/api/todo",
            new AddTodoDto { Title = "Delete test" }, Ct);

        var created = await createResponse.Content.ReadFromJsonAsync<GetTodoDto>(Ct);

        var deleteResponse = await client.DeleteAsync($"/api/todo/{created!.Id}", Ct);

        Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);

        var getResponse = await client.GetAsync($"/api/todo/{created.Id}", Ct);

        Assert.Equal(HttpStatusCode.InternalServerError, getResponse.StatusCode);
    }

    [Fact]
    public async Task PATCH_Complete_ShouldMarkAsCompleted()
    {
        var client = CreateClient();

        var createResponse = await client.PostAsJsonAsync("/api/todo",
            new AddTodoDto { Title = "Complete test" }, Ct);

        var created = await createResponse.Content.ReadFromJsonAsync<GetTodoDto>(Ct);

        var response = await client.PatchAsync($"/api/todo/{created!.Id}/complete", null, Ct);

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    [Fact]
    public async Task GET_All_ShouldReturnPagedResult()
    {
        var client = CreateClient();

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