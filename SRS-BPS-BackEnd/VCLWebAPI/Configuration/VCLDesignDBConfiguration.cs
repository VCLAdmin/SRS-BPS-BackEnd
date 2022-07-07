using System.Data.Entity;

namespace VCLWebAPI.Configurations
{
    class VCLDesignDBConfiguration : DbConfiguration
    {
        public VCLDesignDBConfiguration()
        {
            SetProviderFactory("MySql.Data.MySqlClient", new MySql.Data.MySqlClient.MySqlClientFactory());
            SetProviderServices("MySql.Data.MySqlClient", new MySql.Data.MySqlClient.MySqlProviderServices());
        }
    }
}
