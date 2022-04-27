using System.Threading.Tasks;

namespace RpiProbeLogger.Bus
{
    public interface IBusReporter
    {
        public Task<bool> Send<T>(T model);
        public void BindPort(uint port);
    }
}
