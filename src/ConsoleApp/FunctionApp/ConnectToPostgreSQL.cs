using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Azure.Identity;
using Microsoft.Extensions.Hosting;
using Npgsql;

namespace FunctionApp
{
    public static class ConnectToPostgreSQL
    {
        // https://learn.microsoft.com/en-us/azure/postgresql/flexible-server/how-to-connect-with-managed-identity?source=recommendations#connect-using-managed-identity-in-c
        private static string Host = "pgresql-azdb.postgres.database.azure.com";
        private static string User = "azfunction-msi-user";
        private static string Database = "msi_test_db";
        private static readonly string _nl = Environment.NewLine;

        [FunctionName("ConnectToPostgreSQL")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            var responseMessage = String.Empty;

            var msg = $"C# HTTP trigger of function ConnectToPostgreSQL processed a request.{_nl}{_nl}";
            log.LogInformation(msg);
            responseMessage += msg;

            //
            // Get an access token for PostgreSQL.
            //
            msg = $"Getting access token from Azure AD{_nl}{_nl}";
            log.LogInformation(msg);
            responseMessage += msg;

            // Azure AD resource ID for Azure Database for PostgreSQL is https://ossrdbms-aad.database.windows.net/
            string accessToken = null;

            try
            {
                // Call managed identities for Azure resources endpoint.
                var sqlServerTokenProvider = new DefaultAzureCredential();
                accessToken = (await sqlServerTokenProvider.GetTokenAsync(
                    new Azure.Core.TokenRequestContext(scopes: new string[] { "https://ossrdbms-aad.database.windows.net/.default" }) { })).Token;

            }
            catch (Exception e)
            {
                msg = $"{e.Message} {_nl}";
                _ = e.InnerException != null ? e.InnerException.Message : "Acquire token failed";
                _ = _nl ;

                log.LogError(msg);
                responseMessage += msg;
            }

            msg = $"Retrieved accessToken:{_nl}{accessToken}{_nl}{_nl}";
            log.LogInformation(msg);
            responseMessage += msg;

            //
            // Open a connection to the PostgreSQL server using the access token.
            //
            string connString =
                String.Format(
                    "Server={0}; User Id={1}; Database={2}; Port={3}; Password={4}; SSLMode=Prefer",
                    Host,
                    User,
                    Database,
                    5432,
                    accessToken);
            msg = $"Connection string built:{_nl}{connString}{ _nl}";
            log.LogInformation(msg);
            responseMessage += msg;

            //await using (var conn = new NpgsqlConnection(connString))
            //{
            //    log.LogInformation("Opening connection using access token");
            //    conn.Open();

            //    await using (var command = new NpgsqlCommand("SELECT version()", conn))
            //    {

            //        var reader = command.ExecuteReader();
            //        while (reader.Read())
            //        {
            //            log.LogInformation("\nConnected!\n\nPostgres version: {0}", reader.GetString(0));
            //        }
            //    }
            //}

            //string name = req.Query["name"];

            //string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            //dynamic data = JsonConvert.DeserializeObject(requestBody);
            //name = name ?? data?.name;

            //string responseMessage = string.IsNullOrEmpty(name)
            //    ? "This HTTP triggered function executed successfully. Pass a name in the query string or in the request body for a personalized response."
            //    : $"Hello, {name}. This HTTP triggered function executed successfully.";

            return new OkObjectResult(responseMessage);
        }
    }
}
