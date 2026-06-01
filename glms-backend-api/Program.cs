using Microsoft.EntityFrameworkCore;
using TechMove.Data;
using TechMove.Factories;
using TechMove.Service;
using TechMove.Strategies;
using System.Text.Json.Serialization;

namespace glms_backend_api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers()
             .AddJsonOptions(options =>
            {
             options.JsonSerializerOptions.ReferenceHandler =
             ReferenceHandler.IgnoreCycles;
             });

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            builder.Services.AddDbContext<TechMoveDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            builder.Services.AddHttpClient<CurrencyService>();

            builder.Services.AddScoped<IServiceRequestFactory, ServiceRequestFactory>();


            builder.Services.AddScoped<IServiceCostStrategy, UsdToZarCostStrategy>();

            builder.Services.AddScoped<FileValidationService>();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
