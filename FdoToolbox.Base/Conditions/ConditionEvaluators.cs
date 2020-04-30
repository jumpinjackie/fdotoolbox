#region LGPL Header
// Copyright (C) 2009, Jackie Ng
// https://github.com/jumpinjackie/fdotoolbox, jumpinjackie@gmail.com
// 
// This library is free software; you can redistribute it and/or
// modify it under the terms of the GNU Lesser General Public
// License as published by the Free Software Foundation; either
// version 2.1 of the License, or (at your option) any later version.
// 
// This library is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
// Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public
// License along with this library; if not, write to the Free Software
// Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301  USA
// 
//
// See license.txt for more/additional licensing information
#endregion

using System;
using ICSharpCode.Core;
using FdoToolbox.Base.Controls;
using System.Windows.Forms;
using FdoToolbox.Base.Services;
using FdoToolbox.Core.Feature;

namespace FdoToolbox.Base.Conditions
{
    /// <summary>
    /// A condition evaluator that allows commands to be enabled/disabled based on FDO command support.
    /// </summary>
    internal class CommandSupportConditionEvaluator : IConditionEvaluator
    {
        private IFdoConnectionManager connMgr = ServiceManager.Instance.GetService<IFdoConnectionManager>();

        public bool IsValid(object caller, Condition condition)
        {
            IObjectExplorer objectExplorer = caller as IObjectExplorer;
            if (objectExplorer == null)
                return false;

            TreeNode node = objectExplorer.GetSelectedNode();

            //Might be the one. Check it's root parent
            if (node.Level >= 1)
            {
                TreeNode root = node.Parent;
                while (root.Level > 0)
                {
                    root = root.Parent;
                }

                TreeNode rootCmp = objectExplorer.GetRootNode(FdoObjectExplorerExtender.RootNodeName);
                if (root == rootCmp)
                {
                    TreeNode connNode = node;
                    while (connNode.Level > 1)
                    {
                        connNode = connNode.Parent;
                    }
                    FdoConnection conn = connMgr.GetConnection(connNode.Name);
                    if (conn != null)
                    {
                        string cmd = "CommandType_" + condition.Properties["command"];
                        try
                        {
                            OSGeo.FDO.Commands.CommandType ctype = (OSGeo.FDO.Commands.CommandType)Enum.Parse(typeof(OSGeo.FDO.Commands.CommandType), cmd);
                            Array commands = conn.Capability.GetArrayCapability(CapabilityType.FdoCapabilityType_CommandList);
                            return (Array.IndexOf(commands, ctype) >= 0);
                        }
                        catch
                        {
                            return false;
                        }
                    }
                }
            }
            return false;
        }
    }

    /// <summary>
    /// A condition evaluator that allows commands to be enabled/disabled based on whether a given provider is
    /// registered with this installation
    /// </summary>
    public class ProviderSupportConditionEvaluator : IConditionEvaluator
    {
        public bool IsValid(object caller, Condition condition)
        {
            var provider = condition.Properties["Provider"];

            return ProviderExists(provider);
        }

        private bool ProviderExists(string provider)
        {
            var list = FdoFeatureService.GetProviders();
            foreach (var prov in list)
            {
                if (prov.Name.StartsWith(provider))
                    return true;
            }
            return false;
        }
    }

}
