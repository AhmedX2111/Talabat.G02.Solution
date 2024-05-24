using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using StackExchange.Redis;
using System.Net;
using System.Text.Json;
using Talabat.APIS.Errors;
using Talabat.APIS.Extensions;
using Talabat.Core.Entities.Identity;
using Talabat.Infrastructure._Identity;
using Talabat.Infrastructure.Data;
using JsonSerializer = System.Text.Json.JsonSerializer;
using StoreContextSeed = Talabat.Infrastructure.Data.StoreContextSeed;

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

			webApplicationBuilder.Services.AddControllers().AddNewtonsoftJson(options =>
			{
				options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
			});
			// Register Required Web APIS Services to the DI Container

			webApplicationBuilder.Services.AddSwaggerService();

			webApplicationBuilder.Services.AddApplicationsService();

			webApplicationBuilder.Services.AddDbContext<StoreContext>(options =>
			{
				options.UseSqlServer(webApplicationBuilder.Configuration.GetConnectionString("DefaultConnection"));
			});


			webApplicationBuilder.Services.AddDbContext<ApplicationIdentityDbContext>(options =>
			{
				options.UseSqlServer(webApplicationBuilder.Configuration.GetConnectionString("IdentityConnection"));
			});


			webApplicationBuilder.Services.AddSingleton<IConnectionMultiplexer>((serviceProvides) =>
			{
				var connection = webApplicationBuilder.Configuration.GetConnectionString("Redis");
				return ConnectionMultiplexer.Connect(connection);
			});

			webApplicationBuilder.Services.AddAuthServices(webApplicationBuilder.Configuration);

			webApplicationBuilder.Services.AddCors(options =>
			{
				options.AddPolicy("MyPolicy", policyOptions =>
				{
					policyOptions.AllowAnyHeader().AllowAnyMethod().WithOrigins(webApplicationBuilder.Configuration["FrontBaseUrl"]);
				});
			});

			#endregion

			var app = webApplicationBuilder.Build();

			//Ask CLR for creating object from DBContext explicitly
			#region Apply All Pending Migrations [Update-Database] and Data Seeding
			using var scope = app.Services.CreateScope();

			var services = scope.ServiceProvider;

			var _dbContext = services.GetRequiredService<StoreContext>();

			var _identityDbContext = services.GetRequiredService<ApplicationIdentityDbContext>();

			var loggerFactory = services.GetRequiredService<ILoggerFactory>();

			var logger = loggerFactory.CreateLogger<Program>();
			try
			{
				await _dbContext.Database.MigrateAsync(); // Update-Database

				await StoreContextSeed.SeedAsync(_dbContext); // Data Seeding

				await _identityDbContext.Database.MigrateAsync();

				var _userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
				await ApplicationIdentityContextSeed.SeedUsersAsync(_userManager);
			}
			catch (Exception ex)
			{
				logger.LogError(ex, "An error has been occured during applying the migration");
			}
			#endregion

			#region Configure Kestrel MiddleWares
			//app.UseMiddleware<ExceptionMiddleware>();
			// Configure the HTTP request pipeline.


			app.Use(async (httpContext, _next) =>
			{
				try
				{
					//take an action with the request
					await _next.Invoke(httpContext); // go to next middleware
													 //take an action with the response
				}
				catch (Exception ex)
				{
					logger.LogError(ex.Message); // development environment
					httpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
					httpContext.Response.ContentType = "application/json";
					var response = app.Environment.IsDevelopment() ? new ApiExceptionResponse((int)HttpStatusCode.InternalServerError, ex.Message, ex.StackTrace.ToString()) :
					new ApiExceptionResponse((int)HttpStatusCode.InternalServerError);
					var options = new JsonSerializerOptions() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
					var json = JsonSerializer.Serialize(response, options);
					await httpContext.Response.WriteAsync(json);
				}
			});

			if (app.Environment.IsDevelopment())
			{
				// app.UseDeveloperExceptionPage();
				app.UseSwaggerMiddleware();
			}

			app.UseStatusCodePagesWithReExecute("/errors/{0}");

			app.UseHttpsRedirection();

			app.UseStaticFiles();

			app.UseCors("MyPolicy");

			app.MapControllers();

			app.UseAuthentication();

			app.UseAuthorization();
			#endregion

			app.Run();
		}

	}
}