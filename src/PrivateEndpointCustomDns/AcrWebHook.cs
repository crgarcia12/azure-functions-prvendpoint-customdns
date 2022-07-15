using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Microsoft.Azure.Management.ContainerRegistry;
using Microsoft.Azure.Management.ContainerRegistry.Models;
using Azure.Identity;
using Microsoft.Azure.Management.ResourceManager.Fluent.Authentication;
using System.Collections.Generic;
using Microsoft.Azure.Management.ResourceManager.Fluent;

namespace PrivateEndpointCustomDns
{
    public static class AcrWebHook
    {
        [FunctionName("AcrWebHook")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {          
            // Build image name
            string imageName = "";
            try{
                dynamic jsonContent = JsonConvert.DeserializeObject(await req.ReadAsStringAsync());
                imageName = $"{jsonContent.target.repository}:{jsonContent.target.tag}";
                log.LogInformation($"Action: {jsonContent.action} - image:{imageName}");
            } catch {
                throw;
            }

            try{
                ImportImage(
                    sourceImage: imageName
                );
            }
            catch(Exception ex)
            {
                 log.LogError(ex.Message);
                 throw;
            }
            log.LogInformation($"Image imported");
            return new OkResult();  
        }

        public static void ImportImage(string sourceImage)
        {
            var subscriptionId="930c11b0-5e6d-458f-b9e3-f3dda0734110";
            var clientId = "9c62f932-a439-45e6-b238-d75671aa41df";
            var clientSecret = "";
            var tenantId = "72f988bf-86f1-41af-91ab-2d7cd011db47";

            var credential = new AzureCredentials(
                new ServicePrincipalLoginInformation
                {
                    ClientId = clientId,
                    ClientSecret = clientSecret
                },
                tenantId,
                AzureEnvironment.AzureGlobalCloud);

            var managementClient = new ContainerRegistryManagementClient(credential.WithDefaultSubscription(subscriptionId));
            managementClient.SubscriptionId = subscriptionId;

            var importSource = new ImportSource
            {
                SourceImage = sourceImage, //"hello-world",
                RegistryUri = "crgaracrgrneacr.azurecr.io" //"mcr.microsoft.com",
            };

            managementClient.Registries.ImportImage(
                resourceGroupName: "crgar-acrgr-we-rg",
                registryName: "crgaracrgrweacr",
                parameters:
                    new ImportImageParameters
                    {
                        Mode = ImportMode.Force,
                        Source = importSource,
                        TargetTags = new List<String> { sourceImage }
                    });

            
        }
    }
}
