using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using DatingApp.API.Data;
using DatingApp.API.Helpers;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;

namespace DatingApp.API
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureDevelopmentServices(IServiceCollection services)
        {
            services.AddDbContext<DataContext>(x =>
            {
                x.UseLazyLoadingProxies();
                x.UseSqlite(Configuration.GetConnectionString("DefaultConnection"));
            });
            ConfigureServices(services);
        }

        public void ConfigureProductionServices(IServiceCollection services)
        {
            services.AddDbContext<DataContext>(x =>
            {
                x.UseLazyLoadingProxies();
                x.UseMySql(Configuration.GetConnectionString("DefaultConnection"));
            });
            ConfigureServices(services);
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {   //ordering of service is not important in here
            services.AddControllers().AddNewtonsoftJson(opt =>
            {
                opt.SerializerSettings.ReferenceLoopHandling =
                    Newtonsoft.Json.ReferenceLoopHandling.Ignore;
                // ignore Self referencing loop detected for property 'user' error
            });

            //for .net core 2.0 to 3.0
            //services.AddCors(); //CROS policy for HTTP header
            //for .net core 3.1
            //cannot use both options.AllowAnyOrigin() and authentication middleware.
            //have to explicitly define allowed origins. 
            services.AddCors(o => o.AddPolicy("CorsPolicy", builder =>
            {
                builder
                    .WithOrigins(new[] { "http://localhost:4200" })
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .AllowCredentials();
            }));

            // binding the appsettings to data class
            services.Configure<CloudinarySettings>(Configuration.GetSection("CloudinarySettings"));

            // 3 ways to make AuthRepository available to other classes in this application
            // use servcies container to inject repository to other classess

            // need to specify which Assembly we will use Auto Mapper
            services.AddAutoMapper(typeof(DatingRepository).Assembly);

            //services.AddSingleton(); //create single instance of repository thru-out the application and reuse it, have issue on concurrent requests
            //services.AddTransient(); // for lightweight stateless service, new instance of repository is created on each call of repository
            services.AddScoped<IAuthRepository, AuthRepository>(); // repository is created once per each HTTP request, reuse it during the same session.
            services.AddScoped<IDatingRepository, DatingRepository>();
            services.AddScoped<LogUserActivity>();

            services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(x =>
            {
                x.RequireHttpsMetadata = false;
                x.SaveToken = true;
                x.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.ASCII.GetBytes(Configuration.GetSection("AppSettings:Token").Value)),
                    ValidateIssuer = false,
                    ValidateAudience = false
                };
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {   //ordering is important here
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler(builder =>
                {
                    builder.Run(async context =>
                    {
                        context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

                        var error = context.Features.Get<IExceptionHandlerFeature>();
                        if (error != null)
                        {
                            context.Response.AddApplicationError(error.Error.Message); //add new header
                            await context.Response.WriteAsync(error.Error.Message);
                        }
                    });
                });
                //app.UseHsts();
            }

            // app.UseHttpsRedirection();

            app.UseRouting();

            //write a CROS header response before response back to client
            //.net core 2.0, go before app.UseMvc(); 3.0 after app.UseRouting() and before app.UseEndpoints()
            //app.UseCors(options => options.AllowAnyOrigin().AllowAnyMethod().AllowAnyMethod());

            // for .net 3.1, you cannot use both options.AllowAnyOrigin() and authentication middleware
            app.UseCors("CorsPolicy");

            app.UseAuthentication();
            app.UseAuthorization();

            // look for index.html
            app.UseDefaultFiles();
            // serve static files (auto look for wwwroot folder)
            app.UseStaticFiles();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapFallbackToController("Index", "Fallback");
            });
        }
    }
}
