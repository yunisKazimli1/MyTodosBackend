using MyTodosBackend.Api;
using MyTodosBackend.Api.Middleware.Extensions;
using MyTodosBackend.Application;
using MyTodosBackend.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

#region Services

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddApi();
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddApplication();

#endregion

var app = builder.Build();

#region Middleware Pipeline

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseGlobalExceptionHandling();

app.UseHttpsRedirection();

app.UseCors("AllowFrontend");

app.MapControllers();

#endregion

app.Run();

public partial class Program { }
