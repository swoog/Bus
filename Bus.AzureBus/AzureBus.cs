using System;
using System.Text;
using System.Threading.Tasks;
using Bus.Core;
using Microsoft.Azure.EventHubs;
using Newtonsoft.Json;

namespace Bus.AzureBus
{
    public class AzureBus : IBus
    {
        private readonly EventHubClientFactory eventHubClientFactory;
        private readonly EventProcessorHostFactory eventProcessorHostFactory;

        public AzureBus(EventHubClientFactory eventHubClientFactory, EventProcessorHostFactory eventProcessorHostFactory)
        {
            this.eventHubClientFactory = eventHubClientFactory;
            this.eventProcessorHostFactory = eventProcessorHostFactory;
        }

        public void Subscriber<T>(Func<T, Task> handleMessage)
        {
            var client = eventProcessorHostFactory.GetClient(typeof(T));

            Task.WaitAll(client.RegisterEventProcessorFactoryAsync(new SimpleEventProcessorFactory<T>(handleMessage)));
        }

        public async Task Publish<T>(T message)
        {
            var client = this.eventHubClientFactory.GetClient(message.GetType());

            await client.SendAsync(new EventData(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(message))));
        }
    }
}
