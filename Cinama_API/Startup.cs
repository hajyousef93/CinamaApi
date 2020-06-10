
using Cinama_API.Data;
using Cinama_API.Data.Repository;
using Cinama_API.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;

namespace Cinama_API
{
    public class Startup
    {

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
          
        }
        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            //Cookies Policy 1
            services.Configure<CookiePolicyOptions>(option =>
            {
                option.CheckConsentNeeded = context => true;
                option.MinimumSameSitePolicy = Microsoft.AspNetCore.Http.SameSiteMode.None;

            });


            services.AddControllers();
            services.AddDbContext<ApplicationDb>(option => option.UseSqlServer(Configuration.GetConnectionString("MyConnection")));
            services.AddIdentity<ApplicationUser, ApplicationRole>(option =>
           {
               option.Password.RequireDigit = false;
               option.Password.RequireLowercase = false;
               option.Password.RequiredLength = 3;
               option.Password.RequireUppercase = false;
               option.Password.RequireNonAlphanumeric = false;
               option.Password.RequiredUniqueChars = 0;
               //For Confirme Email
               option.SignIn.RequireConfirmedEmail = true;
               //For Count login failed && time turnOn for Email after 10m
               option.Lockout.MaxFailedAccessAttempts = 5;
               option.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(10);
           })
                .AddEntityFrameworkStores<ApplicationDb>()
                .AddDefaultTokenProviders();

            //Cookies 2
            services.AddAuthentication(option =>
            {
                option.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                option.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                option.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;

            }).AddCookie(option =>
            {
                option.Cookie.HttpOnly = true;
                option.ExpireTimeSpan = TimeSpan.FromMinutes(30);
                option.LogoutPath = "/Account/Logout";
                option.SlidingExpiration = true;

            });
           
            services.AddScoped<IAuthRepository, AuthRepository>();
            services.AddCors();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseCors(x => x.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
            app.UseHttpsRedirection();

            app.UseRouting();
            //Authentication
            app.UseAuthentication();
            //Cookies Policy
            app.UseCookiePolicy();
            //Authorization
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
