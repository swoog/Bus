using System;
using Bus.AzureBus;
using Bus.Core;
using Pattern.Config;
using Pattern.Core.Interfaces;

namespace Bus.Azure.Pattern
{
    public static class AzureBusExtensions
    {
        public static IKernel BindAzureBus(this IKernel kernel, string connectionString, string storageAccount, string name)
        {
            kernel.Bind<EventHubClientFactory>().ToMethod(() => new EventHubClientFactory(connectionString));
            kernel.Bind<EventProcessorHostFactory>().ToMethod(() => new EventProcessorHostFactory(connectionString, storageAccount, name));
            kernel.Bind<IBus>().To<AzureBus.AzureBus>();

            return kernel;
        }
    }
}
