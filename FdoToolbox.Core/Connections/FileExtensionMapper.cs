#region LGPL Header
// Copyright (C) 2020, Jackie Ng
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


using OSGeo.FDO.ClientServices;
using OSGeo.FDO.Connections;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml.Serialization;

namespace FdoToolbox.Core.Connections
{
    [XmlRoot("FileExtensionMappings", Namespace = "", IsNullable = false)]
    [Serializable]
    public class FileExtensionMappings
    {
        [XmlElement("FileExtension")]
        public MappedFileExtension[] FileExtensions { get; set; }
    }

    [Serializable]
    public class MappedFileExtension
    {
        [XmlElement]
        public string Name { get; set; }

        [XmlElement]
        public string Description { get; set; }

        [XmlElement]
        public string Provider { get; set; }

        [XmlElement]
        public string ConnectionString { get; set; }
    }

    public static class FileExtensionMapper
    {
        static List<MappedFileExtension> smMappings = new List<MappedFileExtension>();

        public static void Init()
        {
            var thisAsm = Assembly.GetExecutingAssembly();
            string filePath = new Uri(thisAsm.CodeBase).LocalPath;
            var path = Path.Combine(Path.GetDirectoryName(filePath), "FileExtensionMappings.xml");
            if (File.Exists(path))
            {
                using (var fs = File.OpenRead(path))
                {
                    var ser = new XmlSerializer(typeof(FileExtensionMappings));
                    var mappings = (FileExtensionMappings)ser.Deserialize(fs);
                    smMappings.AddRange(mappings.FileExtensions);
                }
            }
        }

        public static (IConnection conn, string provider) TryCreateConnection(string filePath)
        {
            var ext = Path.GetExtension(filePath);
            var entry = smMappings.FirstOrDefault(e => "." + e.Name == ext);
            if (entry != null)
            {
                var connMgr = FeatureAccessManager.GetConnectionManager();
                var conn = connMgr.CreateConnection(entry.Provider);
                conn.ConnectionString = string.Format(entry.ConnectionString, filePath);
                return (conn, entry.Provider);
            }
            return (null, null);
        }
    }
}
