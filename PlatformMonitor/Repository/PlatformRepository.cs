
using DFCommonLib.DataAccess;
using DFCommonLib.Logger;
using PlatformMonitor.Models;

namespace PlatformMonitor.Repository
{
    public interface IPlatformRepository
    {
        IList<Platform> GetAllPlatforms();
    }

    public class PlatformRepository : IPlatformRepository
    {
        private IDbConnectionFactory _connection;

        private readonly IDFLogger<PlatformRepository> _logger;

        public PlatformRepository(IDbConnectionFactory connection,
            IDFLogger<PlatformRepository> logger)
        {
            _connection = connection;
            _logger = logger;
        }

        public IList<Platform> GetAllPlatforms()
        {
            IList<Platform> platforms = new List<Platform>();
            using (var cmd = _connection.CreateCommand(@"select * from platforms"))
            {
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var platform = new Platform();
                        platform.Id = Convert.ToInt32(reader["id"]);
                        platform.Name = reader["name"].ToString();
                        platform.Url = reader["url"].ToString();
                        platform.Environment = reader.IsDBNull(reader.GetOrdinal("environment"))
                            ? null
                            : reader.GetString(reader.GetOrdinal("environment"));
                        platforms.Add(platform);
                    }
                }
            }

            return platforms;
        }
    }
}