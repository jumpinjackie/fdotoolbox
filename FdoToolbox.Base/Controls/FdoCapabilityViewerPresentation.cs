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
using System.Text;
using FdoToolbox.Base.Services;
using FdoToolbox.Core.Feature;
using ICSharpCode.Core;
using OSGeo.FDO.Connections.Capabilities;

namespace FdoToolbox.Base.Controls
{
    internal interface IFdoCapabilityView : IViewContent
    {
        CapabilityEntry[] Capabilities { set; }
    }

    /// <summary>
    /// Represents a FDO capability item
    /// </summary>
    public class CapabilityEntry
    {
        /// <summary>
        /// Gets or sets the name of the capability
        /// </summary>
        /// <value>The name.</value>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the type of the capability
        /// </summary>
        /// <value>The type.</value>
        public string Type { get; set; }

        /// <summary>
        /// Gets or sets the value of the capability
        /// </summary>
        /// <value>The value.</value>
        public string Value { get; set; }
    }

    internal class FdoCapabilityViewerPresenter
    {
        private readonly IFdoCapabilityView _view;
        private readonly string _connName;

        public FdoCapabilityViewerPresenter(IFdoCapabilityView view, string connName)
        {
            _view = view;
            _connName = connName;
            _view.Title = ResourceService.GetString("TITLE_CAPABILITIES") + ": " + connName;
        }

        public void Init()
        {
            FdoConnectionManager connMgr = ServiceManager.Instance.GetService<FdoConnectionManager>();
            FdoConnection conn = connMgr.GetConnection(_connName);

            ICapability cap = conn.Capability;
            CapabilityType[] capTypes = (CapabilityType[])Enum.GetValues(typeof(CapabilityType));
            List<CapabilityEntry> ents = new List<CapabilityEntry>();

            foreach (CapabilityType ct in capTypes)
            {
                Type t = cap.GetCapabilityValueType(ct);
                if (t != null)
                {
                    CapabilityEntry ent = new CapabilityEntry
                    {
                        Name = ct.ToString(),
                        Type = t.ToString()
                    };

                    object value = cap.GetObjectCapability(ct);
                    if (t == typeof(Array))
                        ent.Value = GetArrayValues((Array)value);
                    else if (t == typeof(FunctionDefinitionCollection))
                        ent.Value = GetFunctions((FunctionDefinitionCollection)value);
                    else
                        ent.Value = value.ToString();
                    ents.Add(ent);
                }
            }

            _view.Capabilities = ents.ToArray();
        }

        private string GetFunctions(FunctionDefinitionCollection functionDefinitionCollection)
        {
            List<string> values = new List<string>();
            foreach (FunctionDefinition fd in functionDefinitionCollection)
            {
                values.Add(fd.Name);
            }
            return String.Join(", ", values.ToArray());
        }

        private string GetArrayValues(Array array)
        {
            List<string> values = new List<string>();
            foreach (object o in array)
            {
                values.Add(o.ToString());
            }
            return String.Join(", ", values.ToArray());
        }
    }
}
