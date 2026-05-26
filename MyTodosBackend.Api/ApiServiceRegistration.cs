namespace MyTodosBackend.Api
{
    public static class ApiServiceRegistration
    {
        public static IServiceCollection AddApi(this IServiceCollection services)
        {
            services.AddCors(options =>
            {
                options.AddPolicy("AllowFrontend", policy =>
                {
                    policy
                        .WithOrigins("http://localhost:5173", "http://localhost:5174")
                        .AllowAnyHeader()
                        .AllowAnyMethod();
                });
            });

            return services;
        }
    }
}
