using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using MCT.Functions.Models;

namespace MCT.Functions
{
    public static class CalculatorTrigger
    {
        [FunctionName("CalculatorTrigger")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "calculator/{getal1}/{getal2}/{operator}")] HttpRequest req,
            int getal1,
            int getal2,
            string @operator,
            ILogger log)
        {
            //eerste versie
            // switch (@operator)
            // {
            //     case "plus":
            //         return new OkObjectResult(getal1 + getal2);
            //     case "min":
            //         return new OkObjectResult(getal1 - getal2);
            //     case "keer":
            //         return new OkObjectResult(getal1 * getal2);
            //     case "delen":
            //         return new OkObjectResult(getal1 / getal2);
            //     default:
            //         return new BadRequestObjectResult("Operator niet herkend");
            // }
            return new OkObjectResult(new CalculationResult
            {
                Result = @operator switch
                {
                    "plus" => getal1 + getal2,
                    "min" => getal1 - getal2,
                    "keer" => getal1 * getal2,
                    "delen" => getal1 / getal2,
                    _ => throw new ArgumentException("Operator niet herkend")
                },
                Operator = @operator
            });

        }
        [FunctionName("CalculatorTriggerpost")]
        public static async Task<IActionResult> Runpost(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "calculator")] HttpRequest req,
            ILogger log)
        {
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var data = JsonConvert.DeserializeObject<CalculationRequest>(requestBody);
            return new OkObjectResult(new CalculationResult
            {
                Result = data.Operator switch
                {
                    "plus" => data.Getal1 + data.Getal2,
                    "min" => data.Getal1 - data.Getal2,
                    "keer" => data.Getal1 * data.Getal2,
                    "delen" => data.Getal1 / data.Getal2,
                    _ => throw new ArgumentException("Operator niet herkend")
                },
                Operator = data.Operator
            });
        }
    }
}
