using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace DbScriptDeploy.Services
{
    public class BackgroundScriptService : IHostedService, IDisposable
    {
        private readonly ILogger<BackgroundScriptService> _logger;
        private Timer _timer;
        private int _number;

        public BackgroundScriptService(ILogger<BackgroundScriptService> logger, IModelBinderService bs)
        {
            _logger = logger;
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }

        private void ExecuteScripts(object state)
        {
            Interlocked.Increment(ref _number);
            _logger.LogInformation($"Printing the worker number {_number}");
       
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Background script service starting");
            _timer = new Timer(
                new TimerCallback(ExecuteScripts),
                null,
                TimeSpan.Zero,
                TimeSpan.FromSeconds(5));

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Background script service stopping");
            return Task.CompletedTask;
        }
    }
}
