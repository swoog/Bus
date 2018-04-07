using System;
using System.Collections.Generic;
using Microsoft.Azure.EventHubs;

namespace Bus.AzureBus
{
    public class EventHubClientFactory
    {
        private readonly Dictionary<Type, EventHubClient> eventHubClients = new Dictionary<Type, EventHubClient>();
        private readonly string connectionString;

        public EventHubClientFactory(string connectionString)
        {
            this.connectionString = connectionString;
        }

        public EventHubClient GetClient(Type t)
        {
            if (eventHubClients.ContainsKey(t))
            {
                return eventHubClients[t];
            }

            var internalGetClient = InternalGetClient(t);

            eventHubClients.Add(t, internalGetClient);

            return internalGetClient;
        }

        private EventHubClient InternalGetClient(Type t)
        {
            var connectionStringBuilder = new EventHubsConnectionStringBuilder(connectionString)
            {
                EntityPath = t.Name
            };

            return EventHubClient.CreateFromConnectionString(connectionStringBuilder.ToString());
        }
    }
}