
using CookerAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace CookerAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            
            var builder = WebApplication.CreateBuilder(args);

			// Add Database context to the container
			builder.Services.AddDbContext<CookerDbContext>(options =>
	            options.UseSqlServer(
		            builder.Configuration.GetConnectionString("DefaultConnection")));

			// Add services to the container.
			builder.Services.AddControllers();

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            //live notifications
			builder.Services.AddSignalR();

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

            app.UseStaticFiles();

            app.Run();
        }
    }
}
