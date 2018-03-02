﻿using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Evol.TMovie.Website.Services;
using Evol.TMovie.Data;
using Evol.Domain;
using Evol.Configuration;
using System.Threading.Tasks;
using Evol.Web.Middlewares;
using Swashbuckle.AspNetCore.Swagger;
using Microsoft.Extensions.Logging.Abstractions;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Evol.Extensions.Configuration;
using System.IO;

namespace Evol.TMovie.Website
{
    public partial class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true);

            if (env.IsDevelopment())
            {
                // For more details on using the user secret store see https://go.microsoft.com/fwlink/?LinkID=532709
                //builder.AddUserSecrets<Startup>();
            }

            builder.AddEnvironmentVariables();
            ConfigurationRoot = builder.Build();

            //添加自定义配置
            var typedBuilder = new TypedConfigurationBuilder();
            typedBuilder.SetBasePath(Path.Combine(env.ContentRootPath, "config"));
            //.AddJsonFile<ModuleShip>("moudleShip.json", true, true)
            //.AddXmlFile<AdminArea>("areaCode.xml", true, true);
            TypedConfiguration = typedBuilder.Build();
        }

        public IConfigurationRoot ConfigurationRoot { get; }

        public ITypedConfigurationRoot TypedConfiguration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Add framework services.
            services.AddDbContext<TMovieDbContext>(options =>
                options.UseSqlServer(ConfigurationRoot.GetConnectionString("TMConnection"), opt => opt.UseRowNumberForPaging()));
           

            services.AddMvc();

            // Add application services.
            services.AddTransient<IEmailSender, AuthMessageSender>();
            services.AddTransient<ISmsSender, AuthMessageSender>();

            var builder = new ContainerBuilder();
            builder.Populate(services);


            //~~AppConfig.Init(services);
            ConfigureModules(services);
            //~~AppConfig.ConfigServiceProvider(services.BuildServiceProvider());
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(ConfigurationRoot.GetSection("Logging"));
            loggerFactory.AddDebug();
            loggerFactory.AddProvider(NullLoggerProvider.Instance);

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                //app.UseDatabaseErrorPage();
                app.UseBrowserLink();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();

            //app.UseIdentity();

            // Add external authentication middleware below. To configure them please see https://go.microsoft.com/fwlink/?LinkID=532715

            app.UserAppConfigRequestServicesMiddleware();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "area",
                    template: "{area:exists}/{controller=Home}/{action=Index}/{id?}");


                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });

            // Enable middleware to serve generated Swagger as a JSON endpoint.
            app.UseSwagger();

            // Enable middleware to serve swagger-ui (HTML, JS, CSS etc.), specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "TmAPI V1");
            });
        }
    }
}
