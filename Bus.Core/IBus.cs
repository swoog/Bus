using System;
using System.Threading.Tasks;

namespace Bus.Core
{
    public interface IBus
    {
        void Subscriber<T>(Func<T, Task> handleMessage);

        Task Publish<T>(T message);
    }
}