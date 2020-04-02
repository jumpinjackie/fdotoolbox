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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ICSharpCode.Core;
using System.Windows.Forms;
using FdoToolbox.Base.Controls;
using FdoToolbox.Core.Feature;
using OSGeo.FDO.Commands;
using FdoToolbox.Base.Services;

namespace FdoToolbox.OverrideManager.Conditions
{
    internal class SchemaMappingConditionEvaluator : IConditionEvaluator
    {
        //The list of providers where schema overrides are applicable.
        static string[] applicableProviders = { "OSGEO.MYSQL", "OSGEO.SQLSERVERSPATIAL", "OSGEO.ODBC", "OSGEO.SHP", "OSGEO.WMS" };

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
                    FdoConnectionManager connMgr = ServiceManager.Instance.GetService<FdoConnectionManager>();
                    FdoConnection conn = connMgr.GetConnection(connNode.Name);
                    if (conn != null)
                    {
                        try
                        {
                            var applicable = (Array.IndexOf<string>(applicableProviders, conn.Provider.ToUpper()) >= 0);
                            var supportsSchemaMappings = conn.Capability.GetBooleanCapability(CapabilityType.FdoCapabilityType_SupportsSchemaOverrides);
                            var supportsDescribeSchemaMappings = conn.Capability.GetArrayCapability(CapabilityType.FdoCapabilityType_CommandList).Cast<CommandType>().Contains(CommandType.CommandType_DescribeSchemaMapping);

                            return supportsSchemaMappings && supportsDescribeSchemaMappings;
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
}
