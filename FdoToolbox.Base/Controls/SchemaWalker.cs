#region LGPL Header
// Copyright (C) 2011, Jackie Ng
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
using FdoToolbox.Core.Feature;
using OSGeo.FDO.Schema;

namespace FdoToolbox.Base.Controls
{
    public abstract class SchemaWalker
    {
        protected FdoConnection _conn;

        protected SchemaWalker(FdoConnection conn)
        {
            _conn = conn;
        }

        public static SchemaWalker GetWalker(FdoConnection conn)
        {
            using (var svc = conn.CreateFeatureService())
            {
                if (svc.SupportsPartialSchemaDiscovery())
                    return new PartialSchemaWalker(conn);
                else
                    return new FullSchemaWalker(conn);
            }
        }

        public abstract string[] GetSchemaNames();

        public abstract ClassDescriptor[] GetClassNames(string schemaName);

        public abstract ClassDefinition GetClassDefinition(string schemaName, string className);
    }

    public class ClassDescriptor
    {
        public ClassDescriptor(string schemaName, string className)
        {
            this.SchemaName = schemaName;
            this.ClassName = className;
        }

        public string SchemaName { get; private set; }

        public string ClassName { get; private set; }

        public string QualifiedName => this.SchemaName + ":" + this.ClassName;
    }

    public class FullSchemaWalker : SchemaWalker
    {
        public FullSchemaWalker(FdoConnection conn) : base(conn) { }

        private FeatureSchemaCollection _schemas;

        public override string[] GetSchemaNames()
        {
            EnsureSchemaRetrieved();

            List<string> names = new List<string>();
            for (int i = 0; i < _schemas.Count; i++)
            {
                names.Add(_schemas[i].Name);
            }
            return names.ToArray();
        }

        private void EnsureSchemaRetrieved()
        {
            if (_schemas == null)
            {
                using (var svc = _conn.CreateFeatureService())
                {
                    _schemas = svc.DescribeSchema();
                }
            }
        }

        public override ClassDescriptor[] GetClassNames(string schemaName)
        {
            EnsureSchemaRetrieved();

            List<ClassDescriptor> classNames = new List<ClassDescriptor>();

            if (_schemas.IndexOf(schemaName) >= 0)
            {
                FeatureSchema fs = _schemas[schemaName];
                ClassCollection classes = fs.Classes;

                for (int i = 0; i < classes.Count; i++)
                {
                    classNames.Add(new ClassDescriptor(fs.Name, classes[i].Name));
                }
            }

            return classNames.ToArray();
        }

        public override ClassDefinition GetClassDefinition(string schemaName, string className)
        {
            EnsureSchemaRetrieved();

            if (_schemas.IndexOf(schemaName) >= 0)
            {
                FeatureSchema fs = _schemas[schemaName];
                ClassCollection classes = fs.Classes;

                if (classes.IndexOf(className) >= 0)
                    return classes[className];
            }

            return null;
        }
    }

    public class PartialSchemaWalker : SchemaWalker
    {
        public PartialSchemaWalker(FdoConnection conn) : base(conn) { }

        public override string[] GetSchemaNames()
        {
            using (var svc = _conn.CreateFeatureService())
            {
                return svc.GetSchemaNames().ToArray();
            }
        }

        public override ClassDescriptor[] GetClassNames(string schemaName)
        {
            using (var svc = _conn.CreateFeatureService())
            {
                var names = svc.GetClassNames(schemaName);
                var result = new List<ClassDescriptor>();
                foreach (var n in names)
                {
                    result.Add(new ClassDescriptor(schemaName, n));
                }
                return result.ToArray();
            }
        }

        public override ClassDefinition GetClassDefinition(string schemaName, string className)
        {
            using (var svc = _conn.CreateFeatureService())
            {
                return svc.GetClassByName(schemaName, className); //This is already aware of partial discovery
            }
        }
    }
}
