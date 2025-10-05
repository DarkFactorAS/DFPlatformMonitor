

using PlatformMonitor.Repository;

namespace PlatformMonitor.Provider
{
    public interface IPlatformProvider
    {
        IList<Models.Platform> GetAllPlatforms();
    }

    public class PlatformProvider : IPlatformProvider
    {
        private readonly IPlatformRepository _platformRepository;

        public PlatformProvider(IPlatformRepository platformRepository)
        {
            _platformRepository = platformRepository;
        }

        public IList<Models.Platform> GetAllPlatforms()
        {
            return _platformRepository.GetAllPlatforms();
        }
    }
}