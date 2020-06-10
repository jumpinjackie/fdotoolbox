using OSGeo.FDO.ClientServices;
using OSGeo.FDO.Commands;
using OSGeo.FDO.Commands.DataStore;
using OSGeo.FDO.Connections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FdoCrash
{
    class Program
    {
        static void Main(string[] args)
        {
            var conn = FeatureAccessManager.GetConnectionManager().CreateConnection("OSGeo.SQLite");
            using (conn)
            {
                if (HasCommand(conn, CommandType.CommandType_CreateDataStore, "Creating data stores", out var _))
                {
                    using (var cmd = (ICreateDataStore)conn.CreateCommand(CommandType.CommandType_CreateDataStore))
                    {
                        using (var dict = cmd.DataStoreProperties)
                        {
                            foreach (string name in dict.PropertyNames)
                            {
                                Console.WriteLine("{0}", name);
                            }
                        }
                    }
                }
            }
        }

        static bool HasCommand(IConnection conn, CommandType cmd, string capDesc, out int? retCode)
        {
            retCode = null;
            using (var cmdCaps = conn.CommandCapabilities)
            {
                if (Array.IndexOf<int>(cmdCaps.Commands, (int)cmd) < 0)
                {
                    //WriteError("This provider does not support " + capDesc);
                    //retCode = (int)CommandStatus.E_FAIL_UNSUPPORTED_CAPABILITY;
                    return false;
                }
            }
            return true;
        }
    }
}
