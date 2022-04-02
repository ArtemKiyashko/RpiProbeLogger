using Sense.Led;

namespace RpiProbeLogger.Interfaces
{
    public interface IResponse
    {
        public bool Status { get; }
        public Cell StatusPosition { get; }
    }
}
