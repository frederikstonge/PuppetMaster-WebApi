using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using PuppetMaster.WebApi.Exceptions;
using PuppetMaster.WebApi.Hubs;
using PuppetMaster.WebApi.Models.Database;
using PuppetMaster.WebApi.Providers;
using PuppetMaster.WebApi.Repositories;
using PuppetMaster.WebApi.Services;
using Quartz;
using Swashbuckle.AspNetCore.Filters;
using static OpenIddict.Abstractions.OpenIddictConstants;

namespace PuppetMaster.WebApi
{
    public static class Program
    {
        private const string TokenUrl = "api/authorization/token";

        public static Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddApplicationInsightsTelemetry();
            builder.Services.AddAutoMapper(typeof(Program));
            builder.Services.AddCors();
            builder.Services.AddControllers(options => options.Filters.Add<ExceptionFilter>());

            builder.Services.AddSignalR().AddMessagePackProtocol();

            builder.Services.AddRouting(options => options.LowercaseUrls = true);

            builder.Services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
                options.UseOpenIddict<Guid>();
            });

            builder.Services.AddIdentity<ApplicationUser, ApplicationRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            builder.Services.Configure<IdentityOptions>(options =>
            {
                options.ClaimsIdentity.UserNameClaimType = Claims.Name;
                options.ClaimsIdentity.UserIdClaimType = Claims.Subject;
                options.ClaimsIdentity.RoleClaimType = Claims.Role;
                options.ClaimsIdentity.EmailClaimType = Claims.Email;
            });

            builder.Services.AddQuartz(options =>
            {
                options.UseMicrosoftDependencyInjectionJobFactory();
                options.UseSimpleTypeLoader();
                options.UseInMemoryStore();
            });

            builder.Services.AddQuartzHostedService(options => options.WaitForJobsToComplete = true);
            builder.Services.AddOpenIddict()

                .AddCore(options =>
                {
                    options.UseEntityFrameworkCore()
                           .UseDbContext<ApplicationDbContext>()
                           .ReplaceDefaultEntities<Guid>();

                    options.UseQuartz();
                })

                .AddServer(options =>
                {
                    options.RegisterScopes(Scopes.OfflineAccess, Scopes.Roles, Scopes.Profile, Scopes.Email, Scopes.OpenId);
                    options.SetTokenEndpointUris(TokenUrl);

                    options.AllowPasswordFlow()
                           .AllowRefreshTokenFlow();

                    options.AcceptAnonymousClients();

                    options.AddDevelopmentEncryptionCertificate();
                    options.AddDevelopmentSigningCertificate();
                    
                    options.UseAspNetCore()
                           .EnableTokenEndpointPassthrough();
                })

                .AddValidation(options =>
                {
                    options.UseLocalServer();
                    options.UseAspNetCore();
                });

            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(c =>
            {
                c.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme()
                {
                    Description = "Standard Authorization header using the Bearer scheme.",
                    Name = "Authorization",
                    Type = SecuritySchemeType.OAuth2,
                    Flows = new OpenApiOAuthFlows()
                    {
                        Password = new OpenApiOAuthFlow()
                        {
                            TokenUrl = new Uri($"/{TokenUrl}", UriKind.Relative),
                            Scopes =
                            {
                                { Scopes.OfflineAccess, "Offline Access Scope" },
                                { Scopes.Roles, "Roles Scope" },
                                { Scopes.Email, "Email Scope" },
                                { Scopes.Profile, "Profile Scope" },
                            }
                        },
                    }
                });

                c.OperationFilter<SecurityRequirementsOperationFilter>();
            });

            builder.Services.AddScoped<IAuthorizationService, AuthorizationService>();
            builder.Services.AddScoped<IAccountService, AccountService>();
            builder.Services.AddScoped<IGamesService, GamesService>();
            builder.Services.AddScoped<IMapsService, MapsService>();
            builder.Services.AddScoped<IGameUsersService, GameUsersService>();
            builder.Services.AddScoped<IRoomsService, RoomsService>();
            builder.Services.AddScoped<IMatchesService, MatchesService>();
            builder.Services.AddScoped<IHubService, HubService>();
            builder.Services.AddSingleton<IDelayedTasksService, DelayedTasksService>();
            builder.Services.AddSingleton<IUserIdProvider, UserIdProvider>();

            // Add default data in the database
            builder.Services.AddHostedService<DataWorker>();

            var app = builder.Build();

            app.UseHttpsRedirection();
            app.UseSwagger();
            app.UseSwaggerUI();

            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();
            app.MapDefaultControllerRoute();
            app.MapHub<RoomHub>("/hubs/room");

            return app.RunAsync();
        }
    }
}
