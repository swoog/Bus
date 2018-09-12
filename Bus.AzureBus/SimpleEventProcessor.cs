using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.EventHubs;
using Microsoft.Azure.EventHubs.Processor;
using Newtonsoft.Json;

namespace Bus.AzureBus
{
    public class SimpleEventProcessor<T> : IEventProcessor
    {
        private readonly Func<T, Task> handleMessage;

        public SimpleEventProcessor(Func<T, Task> handleMessage)
        {
            this.handleMessage = handleMessage;
        }

        public Task CloseAsync(PartitionContext context, CloseReason reason)
        {
            Console.WriteLine($"Processor Shutting Down. Partition '{context.PartitionId}', Reason: '{reason}'.");
            return Task.CompletedTask;
        }

        public Task OpenAsync(PartitionContext context)
        {
            Console.WriteLine($"SimpleEventProcessor initialized. Partition: '{context.PartitionId}'");
            return Task.CompletedTask;
        }

        public Task ProcessErrorAsync(PartitionContext context, Exception error)
        {
            Console.WriteLine($"Error on Partition: {context.PartitionId}, Error: {error.Message}");
            return Task.CompletedTask;
        }

        public async Task ProcessEventsAsync(PartitionContext context, IEnumerable<EventData> messages)
        {
            foreach (var eventData in messages)
            {
                var data = Encoding.UTF8.GetString(eventData.Body.Array, eventData.Body.Offset, eventData.Body.Count);

                var obj = JsonConvert.DeserializeObject<T>(data);

                Console.WriteLine($"Message received. Partition: '{context.PartitionId}', Data: '{data}'");

                try
                {
                    await this.handleMessage(obj);

                    await context.CheckpointAsync(eventData);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error in handle : {ex}");
                }
            }
        }
    }
}