using FdoToolbox.Core.AppFramework;
using OSGeo.FDO.ClientServices;
using OSGeo.FDO.Connections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FdoCmd.Commands
{
    public static class ConnectUtils
    {
        internal static (IEnumerable<KeyValuePair<string, string>>, int?) ValidateTokenPairSet(string parameterName, IEnumerable<string> tokens)
        {
            List<string> tokenList = null;
            if (tokens is List<string> list)
            {
                tokenList = list;
            }
            else
            {
                tokenList = tokens.ToList();
            }

            if (tokenList.Count > 0)
            {
                if ((tokenList.Count % 2) != 0)
                {
                    Console.Error.WriteLine($"Incorrect parameters format for {parameterName}. Expected: <name1> <value1> ... <nameN> <valueN>");
                    return (null, (int)CommandStatus.E_FAIL_INVALID_ARGUMENTS);
                }
            }
            return (ToKvpList(tokenList), null);

            IEnumerable<KeyValuePair<string, string>> ToKvpList(List<string> list)
            {
                for (int i = 0; i < list.Count; i += 2)
                {
                    var name = list[i];
                    var value = list[i + 1];
                    yield return new KeyValuePair<string, string>(name, value);
                }
            }
        }

        internal static (IConnection conn, string provider, int? exitCode) CreateConnection(string provider, IEnumerable<string> connParams, string connParamName)
        {
            var (connP, rc) = ValidateTokenPairSet(connParamName, connParams);
            if (rc.HasValue)
                return (null, null, rc.Value);

            var connMgr = FeatureAccessManager.GetConnectionManager();
            var conn = connMgr.CreateConnection(provider);
            var ci = conn.ConnectionInfo;
            var cnp = ci.ConnectionProperties;
            foreach (var kvp in connP)
            {
                cnp.SetProperty(kvp.Key, kvp.Value);
            }

            return (conn, provider, null);
        }
    }
}
