using System;
using System.Collections.Generic;
using Microsoft.Azure.EventHubs;
using Microsoft.Azure.EventHubs.Processor;

namespace Bus.AzureBus
{
    public class EventProcessorHostFactory
    {
        private readonly Dictionary<Type, EventProcessorHost> eventHubClients = new Dictionary<Type, EventProcessorHost>();
        private readonly string connectionString;
        private readonly string name;
        private readonly string storageAccount;

        public EventProcessorHostFactory(string connectionString, string storageAccount, string name)
        {
            this.connectionString = connectionString;
            this.storageAccount = storageAccount;
            this.name = name;
        }

        public EventProcessorHost GetClient(Type t)
        {
            if (eventHubClients.ContainsKey(t))
            {
                return eventHubClients[t];
            }

            var internalGetClient = InternalGetClient(t);

            eventHubClients.Add(t, internalGetClient);

            return internalGetClient;
        }

        private EventProcessorHost InternalGetClient(Type t)
        {
            return new EventProcessorHost(t.Name,
                PartitionReceiver.DefaultConsumerGroupName,
                connectionString,
                storageAccount,
                this.name + "-" + t.Name.ToLower());
        }
    }
}