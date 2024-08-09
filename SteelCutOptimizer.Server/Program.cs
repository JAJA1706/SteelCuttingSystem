using SteelCutOptimizer.Server.AmplApiServices;
using SteelCutOptimizer.Server.AmplDataConverters;
using SteelCutOptimizer.Server.Middleware;

namespace SteelCutOptimizer.Server
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowLocalhost", builder =>
                {
                    builder.WithOrigins("https://localhost:5173")
                           .AllowAnyHeader()
                           .AllowAnyMethod();
                });
            });

            // Add services to the container.
            builder.Services.AddControllers();
            builder.Services.AddSingleton<IAmplDataConverterFactory, AmplDataConverterFactory>();
            builder.Services.AddSingleton<IAmplApiServiceFactory, AmplApiServiceFactory>();

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

                        

            var app = builder.Build();

            app.UseDefaultFiles();
            app.UseStaticFiles();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();
            app.UseCors("AllowLocalhost");
            app.UseAuthorization();
            app.UseMiddleware<RequestRateLimiter>();

            app.MapControllers();

            app.MapFallbackToFile("/index.html");

            app.Run();
        }
    }
}
