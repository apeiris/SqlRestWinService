using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using System.Threading;
using System.Threading.Tasks;

namespace SqlRestWinService
	{
	public class RestBackgroundWorker : BackgroundService
		{
		public ILogger Logger { get; }
		private readonly IConfiguration _configuration;

		public RestBackgroundWorker(ILoggerFactory loggerFactory, IConfiguration configuration)
			{
			Logger = loggerFactory.CreateLogger<RestBackgroundWorker>();
			_configuration = configuration;
			}

		protected override async Task ExecuteAsync(CancellationToken stoppingToken)
			{
			Logger.LogInformation("RestBackgroundWorker is running.");

			while (!stoppingToken.IsCancellationRequested)
				{
				Logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
				await Task.Delay(5000, stoppingToken);
				}

			Logger.LogInformation("RestBackgroundWorker is stopping.");
			}
		}
	}
