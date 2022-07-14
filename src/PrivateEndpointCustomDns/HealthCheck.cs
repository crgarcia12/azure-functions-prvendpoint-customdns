using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Azure.ResourceManager;
using Azure.ResourceManager.Resources;
using Azure.Core;

namespace PrivateEndpointCustomDns
{
    public static class HealthCheck
    {
        [FunctionName("HealthCheck")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation($"[{DateTime.UtcNow.ToString()}] running HealthCheck function");
            string name = req.Query["name"];

            ArmClient client = AzureServices.GetArmClient();
            ResourceIdentifier id = new ResourceIdentifier("/subscriptions/930c11b0-5e6d-458f-b9e3-f3dda0734110/resourceGroups/crgar-acrgr-we-rg/providers/Microsoft.ContainerRegistry/registries/crgaracrgrweacr");
            GenericResource acr = client.GetGenericResource(id);

            string responseMessage = $"[{DateTime.UtcNow.ToString()}][V0.1] Hello, {name}. This HTTP triggered function executed successfully.";
            return new OkObjectResult(responseMessage);
        }
    }
}
