using Core.Interfaces;
using Infrastracture.Data;
using Infrastracture.Hepler;
using Infrastracture.Services;
using Infrastracture.Services.Permission;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastracture
{
 public static   class InfrastructureStartup
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, string ConnectionString)
        {
            services.AddDbContext<SiteDataContext>(a => {
                a.UseSqlServer(ConnectionString);
            });

            services.AddAuthenticationServices();

            //ForLoginTime
            //services.ConfigureApplicationCookie(options =>
            //{
            //    //options.Cookie.Name = ".AspNetCore.Identity.Application";
            //    options.ExpireTimeSpan = TimeSpan.FromMinutes(20);
            //    //options.ExpireTimeSpan = TimeSpan.FromHours(8);
            //    options.SlidingExpiration = true;
            //});

            services.AddSingleton<IAuthorizationPolicyProvider, PermissionPolicyProvider>();
            services.AddScoped<IAuthorizationHandler, PermissionAuthorizationHandler>();

            services.AddAutoMapper(typeof(AutoMapperProfiles));

            services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
            services.AddTransient<IUnitOfWork, UnitOfWork>();





            return services;

        }
    }
}