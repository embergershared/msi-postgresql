using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Azure.Identity;
using Npgsql;
using System.Diagnostics;

namespace FunctionApp
{
    public static class ConnectToPostgreSql
    {
        // https://learn.microsoft.com/en-us/azure/postgresql/flexible-server/how-to-connect-with-managed-identity?source=recommendations#connect-using-managed-identity-in-c
        //private const string Host = "pgresql-azdb.postgres.database.azure.com";
        //private const string User = "msi-postgresql";
        //private const string Database = "test-msi";
        private static readonly string Nl = Environment.NewLine;

        [FunctionName("ConnectToPostgreSQL")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            // Initiating method duration measurement
            var stopwatch = new Stopwatch();
            stopwatch.Start();

            var responseMessage = string.Empty;
            var msg = $"C# HTTP trigger of function ConnectToPostgreSQL started to process a request:";
            LogMsg(log, msg, ref responseMessage);

            // Getting Environment variables
            string[] envVarNames = { "AZDB_PSQL_FL_SERVER", "AZDB_PSQL_USER", "AZDB_PSQL_DB"};
            foreach (var envVar in envVarNames)
            {
                msg = $"Checking Environment variable: {GetEnvironmentVariable(envVar, true)}";
                LogMsg(log, msg, ref responseMessage);
            }
            var dbHost = GetEnvironmentVariable("AZDB_PSQL_FL_SERVER");
            var dbUser = GetEnvironmentVariable("AZDB_PSQL_USER");
            var dbName = GetEnvironmentVariable("AZDB_PSQL_DB");

            //
            // Get an access token for PostgreSQL.
            //
            msg = $"Acquiring access token from Azure AD";
            LogMsg(log, msg, ref responseMessage);

            // Azure AD resource ID for Azure Database for PostgreSQL is https://ossrdbms-aad.database.windows.net/
            string accessToken = null;

            try
            {
                // Call managed identities for Azure resources endpoint.
                var sqlServerTokenProvider = new DefaultAzureCredential();
                accessToken = (await sqlServerTokenProvider.GetTokenAsync(
                    new Azure.Core.TokenRequestContext(scopes: new[] { "https://ossrdbms-aad.database.windows.net/.default" }))).Token;
            }
            catch (Exception e)
            {
                msg = e.Message;
                _ = e.InnerException != null ? e.InnerException.Message : "Access token acquisition failed";
                _ = Nl;
                LogMsg(log, msg, ref responseMessage, true);
            }

            msg = $"Retrieved accessToken:{Nl}{accessToken}";
            LogMsg(log, msg, ref responseMessage);

            //
            // Open a connection to the PostgreSQL server using the access token.
            //
            var connString =
                $"Server={dbHost}; User Id={dbUser}; Database={dbName}; Port={5432}; Password={accessToken}; SSLMode=Prefer";
            msg = $"Connection string built:{Nl}{connString}";
            LogMsg(log, msg, ref responseMessage);

            await using (var conn = new NpgsqlConnection(connString))
            {
                msg = $"Opening connection using access token";
                LogMsg(log, msg, ref responseMessage);

                try
                {
                    conn.Open();
                    msg = $"Opening connection succeeded";
                    LogMsg(log, msg, ref responseMessage);
                }
                catch (Exception e)
                {
                    msg = $"{e.Message} {Nl}";
                    _ = e.InnerException != null ? e.InnerException.Message : $"Opening connection failed!!!";
                    _ = Nl;
                    LogMsg(log, msg, ref responseMessage, true);
                }

                await using (var command = new NpgsqlCommand("SELECT version()", conn))
                {
                    try
                    {
                        var reader = command.ExecuteReader();
                        while (reader.Read())
                        {
                            msg = $"Connected to \"{dbName}\" database with user \"{dbUser}\"{Nl}Postgres version = {reader.GetString(0)}";
                            LogMsg(log, msg, ref responseMessage);
                        }
                    }
                    catch (Exception e)
                    {
                        msg = e.Message;
                        _ = e.InnerException != null ? e.InnerException.Message : "Getting Postgres version failed!!!";
                        LogMsg(log, msg, ref responseMessage, true);
                    }
                }
            }

            msg = $"C# HTTP trigger of function ConnectToPostgreSQL finished to process a request.";
            LogMsg(log, msg, ref responseMessage);

            // Stopping the execution measurement
            stopwatch.Stop();
            TimeSpan duration = stopwatch.Elapsed;
            msg = $"ConnectToPostgreSQL execution took: {duration.TotalMilliseconds} milliseconds";
            LogMsg(log, msg, ref responseMessage);

            return new OkObjectResult(responseMessage);
        }

        private static void LogMsg(ILogger log, string msg, ref string responseMessage, bool isError = false)
        {
            //responseMessage += $"Using LogMsg(){_nl}";
            if (isError)
            {
                log.LogError(msg);
            }
            else
            {
                log.LogInformation(msg);
            }

            responseMessage += msg + $"{Nl}{Nl}";
        }
        private static string GetEnvironmentVariable(string name, bool withName = false)
        {
            return withName ? $"{name}({Environment.GetEnvironmentVariable(name, EnvironmentVariableTarget.Process)})" : Environment.GetEnvironmentVariable(name, EnvironmentVariableTarget.Process);
        }
    }
}
