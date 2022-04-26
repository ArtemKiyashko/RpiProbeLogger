using System.Threading.Tasks;

namespace RpiProbeLogger.Bus
{
    public interface IBusReporter
    {
        public Task<bool> Send<T>(T model, string topic);
    }
}
