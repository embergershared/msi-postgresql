using System;
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
        private readonly IPostgreSql _postgreSql;

        public Console(
            ILogger<Console> logger,
            IConfiguration config,
            IPostgreSql postgreSql
        )
        {
            _logger = logger;
            _config = config;
            _postgreSql = postgreSql;
        }

        public async Task<bool> RunAsync()
        {
            using (_logger.BeginScope("RunAsync()"))
            {
                _logger.LogTrace("Method start");

                _logger.LogInformation("Loading Environment variables");

                Environment.SetEnvironmentVariable("AZURE_TENANT_ID", "Value1");
                Environment.SetEnvironmentVariable("AZURE_CLIENT_ID", "Value1");
                Environment.SetEnvironmentVariable("AZURE_CLIENT_SECRET", "Value1");




                _logger.LogDebug("Launching PostgreSql.ConnectAsync()");
                await _postgreSql.ConnectAsync();
                _logger.LogDebug("Exited PostgreSql.ConnectAsync()");

                // await DoSomethingAsync()

                WriteLine($"Press Enter to exit....");
                ReadLine();

                _logger.LogTrace("Method end");
            }

            return true;
        }
    }
}
