using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WFF.Models;
using WFF.Utils;
using WFF.Services;
using Microsoft.Extensions.Caching.Memory;
using WFF.ViewModels;
using Microsoft.Extensions.FileProviders;
using System.IO;

namespace WFF
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

           Configuration = builder.Build();
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();
            services.AddDbContext<DataContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("WFF")));

            services.AddDistributedMemoryCache();
            services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(10);
                options.Cookie.HttpOnly = true;
            });
            services.AddHttpContextAccessor();
            services.AddMemoryCache();

            services.Configure<Configuration>(Configuration.GetSection("MyConfig"));

            services.AddScoped<ILoginService, LoginService>();
            services.AddScoped<IHeaderViewModel, HeaderViewModel>();
            services.AddScoped<IADUserViewModel, ADUserViewModel>();
            services.AddScoped<IActionViewModel, ActionViewModel>();
            services.AddScoped<IUserUtil, UserUtil>();
            services.AddScoped<IBitacora, Bitacora>();
            services.AddScoped<IUserProfile, UserProfile>();
            services.AddScoped<IDesktopViewModel, DesktopViewModel>();
            services.AddScoped<IFormRequestService, FormRequestService>();
            services.AddScoped<IEmailService, EmailService>();
            services.AddScoped<IHistoryService, HistoryService>();
            services.AddScoped<IAttachmentService, AttachmentService>();
            services.AddScoped<IFormXmlService, FormXmlService>();
            services.AddScoped<IFormRequestViewModel, FormRequestViewModel>();
            services.AddScoped<IFormsBoardViewModel, FormsBoardViewModel>();
            services.AddScoped<IFormRequest, FormRequest>();

            MappingConfig.Configure();
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
            }

            app.UseStaticFiles();

            // This will add "Libs" as another valid static content location
            //app.UseStaticFiles(new StaticFileOptions()
            //{
            //    FileProvider = new PhysicalFileProvider(
            //         Path.Combine(Directory.GetCurrentDirectory(), @"Content")),
            //    RequestPath = new PathString("/libs")
            //});

            app.UseSession();
            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Login}/{action=Index}/{id?}");
            });
        }
    }
}
