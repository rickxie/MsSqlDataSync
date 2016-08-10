using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MiniAbp.Ado;
using MiniAbp.DataAccess;
using MiniAbp.Reflection;

namespace Mc.DataSync
{
    [DependsOn(typeof(AdoModule))]
    public class DataSyncModule : MabpModule
    {
        public override void PreInitialize()
        {
            Configuration.Database.ConnectionString = ConfigurationManager.ConnectionStrings["Default"].ConnectionString;
            Configuration.Database.Dialect = Dialect.SqlServer;
        }

        public override void Initialize()
        {
            base.Initialize();
        }
    }
}
