using ContactManager.Core.Domain.IdentityEntities;
using ContactManager.Core.Domain.RepositoryContracts;
using CRUDExample.Filters.ActionFilters;
using Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Repositories;
using ServiceContracts;
using Services;

namespace CRUDExample
{
    public static class ConfigureServicesExtension
    {
        public static IServiceCollection ConfigureServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddTransient<ResponseHeaderActionFilter>();

            //it adds controllers and views as services
            services.AddControllersWithViews(options =>
            {
                //options.Filters.Add<ResponseHeaderActionFilter>(5);

                var logger = services.BuildServiceProvider().GetRequiredService<ILogger<ResponseHeaderActionFilter>>();

                options.Filters.Add(new ResponseHeaderActionFilter(logger)
                {
                    Key = "My-Key-From-Global",
                    Value = "My-Value-From-Global",
                    Order = 2
                });

                //automaticaly applit aniforgery toe to each post 
                options.Filters.Add(new AutoValidateAntiforgeryTokenAttribute());
            });

            //add services into IoC container
            services.AddScoped<ICountriesRepository, CountriesRepository>();
            services.AddScoped<IPersonsRepository, PersonsRepository>();

            services.AddScoped<ICountriesService, CountriesService>();




            //V1_Few Excel Column

            //person services
            services.AddScoped<IPersonsExcelWith3Column, PersonsExcelWith3ColumnService>();
            services.AddScoped<IPersonsGetterService, PersonsGetterService>();
            services.AddScoped<IPersonsAdderService, PersonsadderService>();
            services.AddScoped<IPersonsDeleteService, PersonsDeleteService>();
            services.AddScoped<IPersonsSortederService, PersonsSortederService>();
            services.AddScoped<IPersonsUpdateService, PersonsUpdateService>();

            services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"));
            });

            services.AddTransient<PersonsListActionFilter>();

            //Enable Identity in this project
            //use define property save in aps use and role table automatically
            services.AddIdentity<ApplicationUser, ApplicationRole>((options) =>
            {
                options.Password.RequiredLength = 5;
            })

                .AddEntityFrameworkStores<ApplicationDbContext>()

                .AddDefaultTokenProviders()

                .AddUserStore<UserStore<ApplicationUser, ApplicationRole, ApplicationDbContext, Guid>>()

                .AddRoleStore<RoleStore<ApplicationRole, ApplicationDbContext, Guid>>();



            services.AddAuthorization((options) =>
            {

                //enforce autherorazation ploli (for authenticate dUser) Forall action methos in application
                options.FallbackPolicy = new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build();

                //Any Name of polocy
                options.AddPolicy("NotAuthenticatedUser", polocy =>
                {
                    polocy.RequireAssertion(context =>
                    {
                        return !context.User.Identity.IsAuthenticated;
                    });
                });
            });


            services.ConfigureApplicationCookie((options) =>
            {
                options.LoginPath = "/Account/Login";
            });

            services.AddHttpLogging(options =>
            {
                options.LoggingFields = Microsoft.AspNetCore.HttpLogging.HttpLoggingFields.RequestProperties | Microsoft.AspNetCore.HttpLogging.HttpLoggingFields.ResponsePropertiesAndHeaders;
            });

            return services;
        }
    }
}
