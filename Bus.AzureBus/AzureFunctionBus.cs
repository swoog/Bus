using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Bus.Core;
using Microsoft.Azure.EventHubs;
using Newtonsoft.Json;

namespace Bus.AzureBus
{
    public class AzureFunctionBus : IBus
    {
        private readonly EventHubClientFactory eventHubClientFactory;
        private readonly Dictionary<Type, List<Func<object, Task>>> subscribers = new Dictionary<Type, List<Func<object, Task>>>();

        public AzureFunctionBus(EventHubClientFactory eventHubClientFactory)
        {
            this.eventHubClientFactory = eventHubClientFactory;
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

            var client = this.eventHubClientFactory.GetClient(message.GetType());

            await client.SendAsync(new EventData(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(message))));
        }
    }
}