using AngularSecurity.Api.EntityClasses;
using AngularSecurity.Api.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace AngularSecurity.Api
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
            // Tell this project to allow CORS
            services.AddCors();

            ConfigureJwt(services);

            services.AddAuthorization(cfg => {
                // NOTE: The claim key and value are case-sensitive
                cfg.AddPolicy("CanAccessProducts", p =>
                   p.RequireClaim("CanAccessProducts", "true"));
            });
            // Convert JSON from Camel Case to Pascal Case
            services.AddControllers().AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
            });

            // Setup the PTC DB Context
            // Read in connection string from 
            // appSettings.json file
            services.AddDbContext<PtcDbContext>(options => options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "AngularSecurity.Api", Version = "v1" });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "AngularSecurity.Api v1"));
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseCors(options =>
                options
                .WithOrigins("http://localhost:4100")
                .AllowAnyMethod().AllowAnyHeader()
            );
            app.UseAuthentication();
            app.UseAuthorization();
            

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }

        public void ConfigureJwt(IServiceCollection services)
        {
            // Get JWT Token Settings from appsettings.json file
            JwtSettings settings = GetJwtSettings();
            services.AddSingleton<JwtSettings>(settings);
            services.AddAuthentication(options => {
                options.DefaultAuthenticateScheme = "JwtBearer";
                options.DefaultChallengeScheme = "JwtBearer";
            })
            .AddJwtBearer("JwtBearer", jwtBearerOptions => {
                jwtBearerOptions.TokenValidationParameters = new TokenValidationParameters
                  {
                      ValidateIssuerSigningKey = true,
                      IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(settings.Key)),
                      ValidateIssuer = true,
                      ValidIssuer = settings.Issuer,

                      ValidateAudience = true,
                      ValidAudience = settings.Audience,

                      ValidateLifetime = true,
                      ClockSkew = TimeSpan.FromMinutes(settings.MinutesToExpiration)
                  };
            });
        }


        public JwtSettings GetJwtSettings()
        {
            JwtSettings settings = new JwtSettings();

            settings.Key = Configuration["JwtToken:key"];
            settings.Audience = Configuration["JwtToken:audience"];
            settings.Issuer = Configuration["JwtToken:issuer"];
            settings.MinutesToExpiration =
             Convert.ToInt32(
                Configuration["JwtToken:minutestoexpiration"]);

            return settings;
        }

    }
}
