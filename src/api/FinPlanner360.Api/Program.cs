using FinPlanner360.Api.Configuration;
using FinPlanner360.Busines.Settings;

internal class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        #region Settings configuration
        var configBuilder = new ConfigurationBuilder()
            .SetBasePath(AppContext.BaseDirectory)
            //.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddJsonFile("databaseSettings.json", optional: false, reloadOnChange: true)
            .AddJsonFile("jwtSettings.json", optional: false, reloadOnChange: true);
        var configuration = configBuilder.Build();

        //builder.Services.Configure<AppSettings>(configuration.GetSection(nameof(AppSettings)));
        builder.Services.Configure<DatabaseSettings>(configuration.GetSection(nameof(DatabaseSettings)));
        builder.Services.Configure<JwtSettings>(configuration.GetSection(nameof(JwtSettings)));

        DatabaseSettings databaseSettings = configuration.GetSection(nameof(DatabaseSettings)).Get<DatabaseSettings>();
        JwtSettings jwtSettings = configuration.GetSection(nameof(JwtSettings)).Get<JwtSettings>();
        #endregion

        #region Extended Services configuration
        builder.Services.AddHttpContextAccessor();
        builder.Services.AddJwtConfiguration(jwtSettings);
        builder.Services.AddBusinesConfiguration(databaseSettings);
        builder.Services.AddRepositoryConfiguration(databaseSettings, builder.Environment.IsProduction());
        builder.Services.AddApiConfiguration();
        builder.Services.AddJwtConfiguration(appSettings.JwtSettings);
        builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
        builder.Services.AddCorsConfiguration();
        builder.Services.AddJsonConfiguration();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerConfiguration();
        #endregion

        var app = builder.Build();
        app.ExecuteEnvironmentConfiguration();
        app.Run();
    }
}