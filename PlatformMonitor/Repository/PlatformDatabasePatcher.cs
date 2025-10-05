using DFCommonLib.DataAccess;
using DFCommonLib.HttpApi.OAuth2;
using DFCommonLib.Logger;

namespace PlatformMonitor.Repository
{
    public class PlatformDatabasePatcher : StartupDatabasePatcher
    {
        private static string PATCHER = "PlatformMonitor";

        public PlatformDatabasePatcher(IDBPatcher dbPatcher) : base(dbPatcher)
        {
        }

        public override bool RunPatcher()
        {
            base.RunPatcher();

            // User Accounts
            _dbPatcher.Patch(PATCHER,2, "CREATE TABLE `platforms` ("
            + " `id` int(11) NOT NULL AUTO_INCREMENT, " 
            + " `name` varchar(100) NOT NULL DEFAULT '', "
            + " `url` varchar(255) NOT NULL DEFAULT '', "
            + " `environment` varchar(20) NOT NULL DEFAULT '', "
            + " PRIMARY KEY (`id`)"
            + ")"
            );

            return _dbPatcher.Successful();
        }
    }
}