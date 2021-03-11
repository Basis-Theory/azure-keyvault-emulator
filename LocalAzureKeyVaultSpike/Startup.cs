using System.IdentityModel.Tokens.Jwt;
using System.Threading.Tasks;
using LocalAzureKeyVaultSpike.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

namespace LocalAzureKeyVaultSpike
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
            services.AddControllers()
                .AddJsonOptions(o =>
                {
                    o.JsonSerializerOptions.IgnoreNullValues = true;
                });
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo {Title = "LocalAzureKeyVaultSpike", Version = "v1"});
            });

            services.AddScoped<IKeyVaultKeyService, KeyVaultKeyService>();

            services.AddAuthentication(x =>
                {
                    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, x =>
                {
                    // x.Authority = "https://localhost:5001/";
                    // x.RequireHttpsMetadata = !environment.IsAcceptance();
                    // x.SaveToken = true;


                    x.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidIssuer = "https://localhost:5001/",
                        ValidateIssuer = false,
                        ValidateAudience = false,
                        ValidateLifetime = false,
                        RequireSignedTokens = false,
                        ValidateIssuerSigningKey = false,
                        TryAllIssuerSigningKeys = false,
                        SignatureValidator = delegate(string token, TokenValidationParameters parameters)
                        {
                            var jwt = new JwtSecurityToken(token);

                            return jwt;
                        },
                    };

                    x.Events = new JwtBearerEvents
                    {
                        OnChallenge = context =>
                        {
                            context.Response.Headers["WWW-Authenticate"] = "Bearer authorization=localhost:5001,scope=foobar";
                            return Task.CompletedTask;
                        }
                    };
                });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "LocalAzureKeyVaultSpike v1"));
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }
    }
}
