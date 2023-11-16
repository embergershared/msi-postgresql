using System.Threading.Tasks;
using ConsoleApp.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using static System.Console;


namespace ConsoleApp.Classes
{
    internal class Console : IConsole
    {
        private readonly ILogger<Console> _logger;
        private readonly IConfiguration _config;
        private readonly IShowInfos _showInfos;

        public Console(
            ILogger<Console> logger,
            IConfiguration config,
            IShowInfos showInfos
        )
        {
            _logger = logger;
            _config = config;
            _showInfos = showInfos;
        }

        public async Task<bool> RunAsync()
        {
            using (_logger.BeginScope("RunAsync()"))
            {
                _logger.LogTrace("Method start");

                _logger.LogDebug("Launching showInfos.Show()");
                _showInfos.Show();
                _logger.LogDebug("Exited showInfos.Show()");

                // await DoSomethingAsync()

                WriteLine($"Press Enter to exit....");
                ReadLine();

                _logger.LogTrace("Method end");
            }

            return true;
        }
    }
}
