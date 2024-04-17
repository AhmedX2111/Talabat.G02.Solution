using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Text.Json;
using Talabat.APIS.Errors;
using Talabat.APIS.Extensions;
using Talabat.APIS.Helpers;
using Talabat.APIS.Middelwares;
using Talabat.Core.Repositories.Contract;
using Talabat.Infrastructure;
using Talabat.Infrastructure.Data;

namespace Talabat.APIS
{
	public class Program
	{
		// Entry Point
		public static async Task Main(string[] args)
		{

			var webApplicationBuilder = WebApplication.CreateBuilder(args);

			#region Configure Services
			// Add services to the DI container.

			webApplicationBuilder.Services.AddControllers();
			// Register Required Web APIS Services to the DI Container


			webApplicationBuilder.Services.AddSwaggerService();

			webApplicationBuilder.Services.AddDbContext<StoreContext>(options =>
			{
				options.UseSqlServer(webApplicationBuilder.Configuration.GetConnectionString("DefaultConnection"));
			});

            webApplicationBuilder.Services.AddApplicationsService();

            #endregion

            var app = webApplicationBuilder.Build();

            //Ask CLR for creating object from DBContext explicitly
            using var scope = app.Services.CreateScope();

            var services = scope.ServiceProvider;

            var _dbContext = services.GetRequiredService<StoreContext>();

            var loggerFactory = services.GetRequiredService<ILoggerFactory>();

            var logger = loggerFactory.CreateLogger<Program>();
            try
            {
                await _dbContext.Database.MigrateAsync(); // Update-Database

                await StoreContextSeed.SeedAsync(_dbContext); // Data Seeding
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error has been occured during applying the migration");
            }

            #region Configure Kestrel MiddleWares
            app.UseMiddleware<ExceptionMiddleware>();
            // Configure the HTTP request pipeline.

            ///app.Use(async (httpContext, _next) =>
            ///{
            ///    try
            ///    {
            ///        //take an action with the request
            ///        await _next.Invoke(httpContext); // go to next middleware
            ///                                         //take an action with the response
            ///    }
            ///    catch (Exception ex)
            ///    {
            ///        logger.LogError(ex.Message); // development environment
            ///        httpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            ///        httpContext.Response.ContentType = "application/json";
            ///        var response = app.Environment.IsDevelopment() ? new ApiExceptionResponse((int)HttpStatusCode.InternalServerError, ex.Message, ex.StackTrace.ToString()) :
            ///        new ApiExceptionResponse((int)HttpStatusCode.InternalServerError);
            ///        var options = new JsonSerializerOptions() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
            ///        var json = JsonSerializer.Serialize(response, options);
            ///        await httpContext.Response.WriteAsync(json);
            ///    }
            ///});

            if (app.Environment.IsDevelopment())
			{
                // app.UseDeveloperExceptionPage();
				app.UseSwaggerMiddleware();
			}

            app.UseStatusCodePagesWithReExecute("/errors/{0}");

			app.UseHttpsRedirection();

			app.UseStaticFiles();

			app.MapControllers(); 
			#endregion

			app.Run();
		}


	}
}
