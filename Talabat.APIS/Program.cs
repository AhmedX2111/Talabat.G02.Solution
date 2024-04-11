using Microsoft.EntityFrameworkCore;
using Talabat.Repository.Data;

namespace Talabat.APIS
{
	public class Program
	{
		// Entry Point
		public static void Main(string[] args)
		{
			var webApplicationBuilder = WebApplication.CreateBuilder(args);

			#region Configure Services
			// Add services to the DI container.

			webApplicationBuilder.Services.AddControllers();
			// Register Required Web APIS Services to the DI Container


			// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
			webApplicationBuilder.Services.AddEndpointsApiExplorer();
			webApplicationBuilder.Services.AddSwaggerGen(); 

			webApplicationBuilder.Services.AddDbContext<StoreContext>(options =>
			{
				options.UseSqlServer(webApplicationBuilder.Configuration.GetConnectionString("DefaultConnection"));
			});
			#endregion

			var app = webApplicationBuilder.Build();

			#region Configure Kestrel MiddleWares
			// Configure the HTTP request pipeline.
			if (app.Environment.IsDevelopment())
			{
				app.UseSwagger();
				app.UseSwaggerUI();
			}

			app.UseHttpsRedirection();


			app.MapControllers(); 
			#endregion

			app.Run();
		}


	}
}
