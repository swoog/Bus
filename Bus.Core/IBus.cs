using System.Threading.Tasks;

namespace Bus.Core
{
    public interface IBus
    {
        Task Publish<T>(T message);
    }
}