


namespace IsolkalkylAPI
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            
            builder.Logging.AddFilter("Microsoft.EntityFrameworkCore.Database.Command", LogLevel.Warning);

            builder.Services.AddControllers();
            builder.Services.AddSwaggerGen();
            
            // Register the database and its initializer
            builder.Services.AddSqlite<Database>("Data Source=../IsolCore/Data/IsolkalkylDB.db");
            builder.Services.AddScoped<IDatabase, Database>();
            builder.Services.AddScoped<DatabaseInitializer>();
            
            // Register user service
            builder.Services.AddScoped<IUserService, UserService>();

            var app = builder.Build();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();
            app.UseAuthorization();
            app.MapControllers();

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
        }
    }
}