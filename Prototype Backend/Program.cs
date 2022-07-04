using Prototype_Backend.Database;
using Prototype_Backend.Services;
using System.Configuration;

namespace Prototype_Backend
{

    public class Program
    {
        public static bool InProduction;
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            if (builder.Environment.IsProduction() is true) InProduction = true;
            else InProduction = false;

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.AddTransient<UserService>();
            builder.Services.AddDbContext<UserIdentityContext>(
                //options => options.UseSqlServer(builder.Configuration["ConnectionString"])
                );

            var app = builder.Build();

            // Configure the HTTP request pipeline.

            app.UseSwagger();
            app.UseSwaggerUI();

            app.UseHttpsRedirection();

            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}