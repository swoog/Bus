using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Bus.Core;
using Microsoft.Azure.EventHubs;
using Newtonsoft.Json;

namespace Bus.AzureBus
{
    public class EventHubConnectionFactory
    {
        private readonly Dictionary<Type, EventHubClient> eventHubClients = new Dictionary<Type, EventHubClient>();
        private readonly string connectionString;

        public EventHubConnectionFactory(string connectionString)
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

    public class AzureFunctionBus : IBus
    {
        private readonly EventHubConnectionFactory eventHubConnectionFactory;
        private readonly Dictionary<Type, List<Func<object, Task>>> subscribers = new Dictionary<Type, List<Func<object, Task>>>();

        public AzureFunctionBus(EventHubConnectionFactory eventHubConnectionFactory)
        {
            this.eventHubConnectionFactory = eventHubConnectionFactory;
        }

        public async Task InternalPublish<T>(T message)
        {
            var type = typeof(T);
            if (subscribers.ContainsKey(type))
            {
                foreach (var func in subscribers[type])
                {
                    await func(message);
                }
            }
        }

        public void Subscriber<T>(Func<T, Task> handleMessage)
        {
            var t = typeof(T);

            if (!subscribers.ContainsKey(t))
            {
                subscribers.Add(t, new List<Func<object, Task>>()
                {
                    o => handleMessage((T) o)
                });
            }
            else
            {
                subscribers[t].Add(o => handleMessage((T)o));
            }
        }

        public async Task Publish<T>(T message)
        {
            var type = typeof(T);
            if (subscribers.ContainsKey(type))
            {
                foreach (var func in subscribers[type])
                {
                    await func(message);
                }
            }

            var client = this.eventHubConnectionFactory.GetClient(message.GetType());

            await client.SendAsync(new EventData(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(message))));
        }
    }
}