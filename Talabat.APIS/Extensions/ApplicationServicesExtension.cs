using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authentication;

using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Talabat.APIS.Errors;
using Talabat.APIS.Helpers;
using Talabat.APIS.Middelwares;
using Talabat.Core.Repositories.Contract;
using Talabat.Core.Services.Contract;
using Talabat.Infrastructure;
using Talabat.Service.AuthService;
using Talabat.Core;
using Talabat.Service.OrderService;
using Talabat.Service.ProductService;
using Microsoft.AspNetCore.Identity;
using Talabat.Core.Entities.Identity;
using Talabat.Infrastructure._Identity;
using Talabat.Service.PaymentService;

namespace Talabat.APIS.Extensions
{
    public static class ApplicationServicesExtension
    {
        public static IServiceCollection AddApplicationsService(this IServiceCollection services)
        {
            services.AddScoped<ExceptionMiddleware>();

			services.AddScoped(typeof(IPaymentService), typeof(PaymentService)); 

			services.AddScoped<IProductService, ProductService>();

			services.AddScoped<IOrderService, OrderService>();

			services.AddScoped<IUnitOfWork, UnitOfWork>();

			services.AddScoped(typeof(IBasketRepository), typeof(BasketRepository));

            services.AddScoped(typeof(IGenericRepository<>), typeof(GenericInfrastructure<>));

            services.AddAutoMapper(typeof(MappingProfiles));

            services.Configure<ApiBehaviorOptions>(options =>
            {
                options.InvalidModelStateResponseFactory = (actionContext) =>
                {
                    var errors = actionContext.ModelState.Where(P => P.Value.Errors.Count > 0)
                                                         .SelectMany(P => P.Value.Errors)
                                                         .Select(E => E.ErrorMessage)
                                                         .ToList();
                    var response = new ApiValidationErrorResponse()
                    {
                        Errors = errors
                    };
                    return new BadRequestObjectResult(response);
                };
            }
            );
            return services;
        }
		public static IServiceCollection AddAuthServices(this IServiceCollection services, IConfiguration configuration)
		{
			services.AddIdentity<ApplicationUser, IdentityRole>()
					.AddEntityFrameworkStores<ApplicationIdentityDbContext>();

			services.AddAuthentication(options =>
			{
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options =>
			{
				options.TokenValidationParameters = new TokenValidationParameters()
				{
					ValidateIssuer = true,
					ValidIssuer = configuration["JWT:ValidIssuer"],
					ValidateAudience = true,
					ValidAudience = configuration["JWT:ValidAudience"],
					ValidateIssuerSigningKey = true,
					IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JWT:AuthKey"] ?? string.Empty)),
					ValidateLifetime = true,
					ClockSkew = TimeSpan.Zero
				};
			});

			services.AddScoped(typeof(IAuthService), typeof(AuthService));

			return services;

		}
	}

	
}
