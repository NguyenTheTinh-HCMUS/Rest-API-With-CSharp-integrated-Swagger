namespace WebAplicationAPI1.Installers
{
    using FluentValidation.AspNetCore;
    using Microsoft.AspNetCore.Authentication.JwtBearer;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.IdentityModel.Logging;
    using Microsoft.IdentityModel.Tokens;
    using Microsoft.OpenApi.Models;
    using System.Collections.Generic;
    using System.Text;
    using WebAplicationAPI1.Authorization;
    using WebAplicationAPI1.Filters;
    using WebAplicationAPI1.Options;
    using WebAplicationAPI1.Services;

    /// <summary>
    /// Defines the <see cref="MvcInstaller" />.
    /// </summary>
    public class MvcInstaller : IInstaller
    {
        /// <summary>
        /// The InstallServices.
        /// </summary>
        /// <param name="services">The services<see cref="IServiceCollection"/>.</param>
        /// <param name="configuration">The configuration<see cref="IConfiguration"/>.</param>
        public void InstallServices(IServiceCollection services, IConfiguration configuration)
        {


            // configure jwt
            var jwtSettings = new JwtSettings();
            configuration.Bind(nameof(JwtSettings), jwtSettings);

            var tokenValidationParamaters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(jwtSettings.Secret)),
                ValidateIssuer = false,
                ValidateAudience = false,
                RequireExpirationTime = false,
                ValidateLifetime = false

            };
            services.AddSingleton(tokenValidationParamaters);
            services.AddSingleton(jwtSettings);
            services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;

            })
            .AddJwtBearer(x =>
            {
                x.SaveToken = true;
                x.TokenValidationParameters = tokenValidationParamaters;
            });
            // authorizations
            services.AddAuthorization(options => {
                //options.AddPolicy("TagViewer", builder =>
                //{
                //    builder.RequireClaim("tags.view","true");
                //});

                options.AddPolicy("MustWorkForThetinh", policy=> {
                    policy.AddRequirements(new WorksForCompanyRequirement(".thetinh.com"));
                });
            });
            services.AddSingleton<IAuthorizationHandler, WorksForCompanyHandler>();
            //add indentity
            services.AddScoped<IIdentityService,IdentityService>();
            IdentityModelEventSource.ShowPII = true;


            services.AddMvc(options=>
                {
                    options.EnableEndpointRouting = false;
                    options.Filters.Add<ValidationFilter>();
                })
                .AddFluentValidation(mvcConfiguration=>mvcConfiguration.RegisterValidatorsFromAssemblyContaining<Startup>())
                .SetCompatibilityVersion(Microsoft.AspNetCore.Mvc.CompatibilityVersion.Latest);
       
        }
    }
}
