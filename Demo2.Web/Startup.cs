﻿using System;
using System.IO;
using AutoMapper;
using Demo2.Data;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.AzureAD.UI;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Demo2.Web
{
    public class Startup
    {
        public Startup(
            IConfiguration configuration,
            ILoggerFactory loggerFactory,
            IServiceProvider serviceProvider,
            IHostingEnvironment hostingEnvironment)
        {
            Configuration = configuration;
            // Add logging to Azure Application Insights.
            loggerFactory.AddApplicationInsights(serviceProvider, LogLevel.Debug);
            CreatePicturesDirectory(hostingEnvironment);
        }

        private static void CreatePicturesDirectory(IHostingEnvironment hostingEnvironment)
        {
            string path = Path.Combine(hostingEnvironment.WebRootPath, "pictures");
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            services.AddAuthentication(AzureADDefaults.AuthenticationScheme)
                .AddAzureAD(options => Configuration.Bind("AzureAd", options));

            services.AddMvc(options =>
            {
                /*var policy = new AuthorizationPolicyBuilder()
                    .RequireAuthenticatedUser()
                    .Build();
                options.Filters.Add(new AuthorizeFilter(policy));*/
            })
			.SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            services.AddDbContext<AppDatabaseContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("AppDb")));

            // Register AutoMapper profiles
            services.AddAutoMapper();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }
            
            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCookiePolicy();

            app.UseAuthentication();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
