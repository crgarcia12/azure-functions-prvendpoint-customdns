using System;
using System.Threading.Tasks;
using Azure.Containers.ContainerRegistry;
using Azure.Identity;
using Azure.Core;
using Azure.ResourceManager;

namespace PrivateEndpointCustomDns
{
    public class AzureServices
    {
        /// Endpoint = "https://myregistry.azurecr.io"
        public static ContainerRegistryClient GetRegistryClient(string endpointUrl)
        {
            Uri endpoint = new Uri(endpointUrl);
            return new ContainerRegistryClient(endpoint, new DefaultAzureCredential(), new ContainerRegistryClientOptions()
            {
                Audience = ContainerRegistryAudience.AzureResourceManagerPublicCloud
            });
        }

        public static ArmClient GetArmClient()
        {
            return new ArmClient(new DefaultAzureCredential());
        }
    }
}
