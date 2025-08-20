
using System.Security.Cryptography;
using DotNetEnv;

namespace IsolkalkylAPI
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            // Load .env file in development
            if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development")
            {
                Env.Load();
            }

            var builder = WebApplication.CreateBuilder(args);

            // get JWT secret key
            var jwtKey = Environment.GetEnvironmentVariable("JWT_SECRET_KEY");
            if (string.IsNullOrEmpty(jwtKey) || jwtKey.Length < 32)
            {
                throw new InvalidOperationException("JWT_SECRET_KEY environment variable must be set and at least 32 characters long.");
            }
            var jwtIssuer = Environment.GetEnvironmentVariable("JWT_ISSUER") ?? "IsolkalkylAPI";
            var jwtAudience = Environment.GetEnvironmentVariable("JWT_AUDIENCE") ?? "IsolkalkylAPI-Users";

            // JWT
            builder.Configuration["Jwt:Key"] = jwtKey;
            builder.Configuration["Jwt:Issuer"] = jwtIssuer;
            builder.Configuration["Jwt:Audience"] = jwtAudience;

            Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Debug()
            .WriteTo.Console()
            .CreateLogger();

            builder.Logging.AddFilter("Microsoft.EntityFrameworkCore.Database.Command", LogLevel.Warning);

            builder.Services.AddAuthentication("Bearer")
                .AddJwtBearer("Bearer", options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidIssuer = jwtIssuer,

                        ValidateAudience = true,
                        ValidAudience = jwtAudience,

                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,

                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
                    };
                });
            builder.Services.AddAuthorization();
            builder.Services.AddControllers();
            builder.Services.AddSwaggerGen();
            // Register the database and its initializer
            builder.Services.AddSqlite<Database>("Data Source=../IsolCore/Data/IsolkalkylDB.db");
            builder.Services.AddScoped<IDatabase, Database>();
            builder.Services.AddScoped<DatabaseInitializer>();
            builder.Services.AddSingleton<Validator>();
            // Register user service
            builder.Services.AddScoped<IUserService, UserService>();
            // Register organization service                   
            builder.Services.AddScoped<IOrganizationService, OrganizationService>();
            builder.Services.AddScoped<IAuthService, AuthService>();

            var app = builder.Build();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();
            app.UseAuthentication();
            app.UseAuthorization();
            app.MapControllers();

            Log.Information("Application starting up");


            // Ensure SQLite DB is created and seeded on first boot
            bool resetDatabaseToDefault = true;
            using (var scope = app.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<DatabaseInitializer>();
                if (resetDatabaseToDefault)
                    await db.ResetDatabase();
                else
                    await db.InitDatabase();
            }

            app.Run();

            Log.CloseAndFlush();
        }
    }
}