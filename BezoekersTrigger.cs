using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Data.SqlClient;
using MCT.Functions.Models;

namespace MCT.Functions
{

    public static class BezoekersTrigger
    {
        [FunctionName("GetDagen")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "days")] HttpRequest req,
            ILogger log)
        {
            List<string> days = new List<string>();
            try
            {
                using (SqlConnection connection = new SqlConnection(Environment.GetEnvironmentVariable("ConnectionString")))
                {
                    await connection.OpenAsync();
                    using (SqlCommand command = new SqlCommand())
                    {
                        command.Connection = connection;
                        string sql = "SELECT DISTINCT DagVanDeWeek FROM Bezoekers";
                        command.CommandText = sql;
                        SqlDataReader reader = await command.ExecuteReaderAsync();
                        while (reader.Read())
                        {
                            days.Add(reader["DagVanDeWeek"].ToString());
                        }
                    }
                }
                return new OkObjectResult(days);
            }
            catch (Exception ex)
            {
                return new BadRequestObjectResult(ex.Message);
            }
        }

        [FunctionName("GetBezoekers")]
        public static async Task<IActionResult> Runn(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "days/{dag}")] HttpRequest req,
            string dag,
            ILogger log)
        {
            List<Visit> bezoekers = new List<Visit>();
            try
            {
                using (SqlConnection connection = new SqlConnection(Environment.GetEnvironmentVariable("ConnectionString")))
                {
                    await connection.OpenAsync();
                    using (SqlCommand command = new SqlCommand())
                    {
                        command.Connection = connection;
                        string sql = $"SELECT * FROM Bezoekers WHERE DagVanDeWeek = '{dag}'";
                        command.CommandText = sql;
                        SqlDataReader reader = await command.ExecuteReaderAsync();
                        while (reader.Read())
                        {
                            Visit test = new Visit
                            {
                                DagVanDeWeek = reader["DagVanDeWeek"].ToString(),
                                Tijdstip = Convert.ToInt32(reader["TijdstipDag"]),
                                AantalBezoekers = Convert.ToInt32(reader["AantalBezoekers"])
                            };
                            bezoekers.Add(test);
                        }
                    }
                }
                return new OkObjectResult(bezoekers);
            }
            catch (Exception ex)
            {
                //log error in console
                log.LogError(ex.ToString());
                return new BadRequestObjectResult(ex.Message);
            }
        }
    }
}
