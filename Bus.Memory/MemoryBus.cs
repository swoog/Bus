using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Bus.Core;

namespace Bus.Memory
{
    public class MemoryBus : IBus
    {
        private Dictionary<Type, List<Func<object, Task>>> subscribers = new Dictionary<Type, List<Func<object, Task>>>();

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
        }
    }
}