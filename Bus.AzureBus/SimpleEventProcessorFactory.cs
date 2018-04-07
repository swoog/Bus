using System;
using System.Threading.Tasks;
using Microsoft.Azure.EventHubs.Processor;

namespace Bus.AzureBus
{
    public class SimpleEventProcessorFactory<T> : IEventProcessorFactory
    {
        private readonly Func<T, Task> handleMessage;

        public SimpleEventProcessorFactory(Func<T, Task> handleMessage)
        {
            this.handleMessage = handleMessage;
        }

        public IEventProcessor CreateEventProcessor(PartitionContext context)
        {
            return new SimpleEventProcessor<T>(this.handleMessage);
        }
    }
}