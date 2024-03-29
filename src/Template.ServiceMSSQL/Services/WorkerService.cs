﻿using Template.ServiceMSSQL.Models;

namespace Template.ServiceMSSQL.Services
{
    public class WorkerService : BackgroundService
    {
        private readonly ILogger<WorkerService> logger;
        private readonly IConfigurationModel config;

        public WorkerService(ILogger<WorkerService> _logger, IConfigurationModel _config)
        {
            logger = _logger;
            config = _config;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                if (logger.IsEnabled(LogLevel.Information))
                {
                    logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                }
                await Task.Delay(1000, stoppingToken);
            }
        }
    }
}
