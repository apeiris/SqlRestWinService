using Microsoft.OpenApi.Models;
using SqlRestWinService;

internal class Program
	{
	private static void Main(string[] args)
		{
		var builder = WebApplication.CreateBuilder(args);

		builder.Services.AddLogging(logging =>
		{
			logging.ClearProviders();
			logging.AddConsole();
			logging.AddDebug();
		});

		builder.Services.AddSingleton<IConfiguration>(builder.Configuration);
		builder.Services.AddSingleton<RestBackgroundWorker>();

		builder.Services.AddControllers(); // Enable controllers for swagger compatibility

		builder.Services.AddEndpointsApiExplorer();
		builder.Services.AddSwaggerGen(c =>
		{
			c.SwaggerDoc("v1", new OpenApiInfo { Title = "SqlRestWinService API", Version = "v1" });
		});

		builder.Services.AddWindowsService();

		builder.WebHost.ConfigureKestrel(options =>
		{
			options.ListenLocalhost(5000);
		});

		var app = builder.Build();

		app.UseSwagger();
		app.UseSwaggerUI(c =>
		{
			c.SwaggerEndpoint("/swagger/v1/swagger.json", "SqlRestWinService API v1");
			c.RoutePrefix = string.Empty;
		});

		app.UseHttpsRedirection();
		app.UseAuthorization();

		app.MapControllers();  // Automatically map all controllers
		app.Run();
		}
	}
