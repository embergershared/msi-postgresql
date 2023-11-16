using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Azure.Identity;
using ConsoleApp.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using static System.Console;


namespace ConsoleApp.Classes
{
    internal class PostgreSQL : IPostgreSql
    {
        private readonly ILogger<Console> _logger;
        private readonly IConfiguration _config;

        public PostgreSQL(
            ILogger<Console> logger,
            IConfiguration config
            )
        {
            _logger = logger;
            _config = config;
        }

        public async Task<bool> ConnectAsync()
        {
            using (_logger.BeginScope("ConnectAsync()"))
            {
                //
                // Get an access token for PostgreSQL.
                //
                _logger.LogInformation("Getting access token from Azure AD");

                // Azure AD resource ID for Azure Database for PostgreSQL is https://ossrdbms-aad.database.windows.net/
                string accessToken = null;

                string clientId = "ab62ff6d-c006-4777-b7b1-31dc44016ec2";
                var credential = new ManagedIdentityCredential(clientId);

                try
                {
                    // Call managed identities for Azure resources endpoint.
                    // var sqlServerTokenProvider = new DefaultAzureCredential();
                    accessToken = (
                        //await sqlServerTokenProvider.GetTokenAsync(
                        await credential.GetTokenAsync(
                        new Azure.Core.TokenRequestContext(
                            scopes: new string[] { "https://ossrdbms-aad.database.windows.net/.default" }
                            )
                        { }
                        )
                    ).Token;

                }
                catch (Exception e)
                {
                    WriteLine("{0} \n\n{1}", e.Message, e.InnerException != null ? e.InnerException.Message : "Acquire token failed");
                    System.Environment.Exit(1);
                }

            }

            return true;
        }

        public Task<string> GetDataAsync()
        {
            throw new NotImplementedException();
        }
    }
}
