#region LGPL Header
// Copyright (C) 2009, Jackie Ng
// http://code.google.com/p/fdotoolbox, jumpinjackie@gmail.com
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
using FdoToolbox.Core.AppFramework;
using FdoToolbox.Core.Utility;
using FdoToolbox.Core.Feature;

namespace FdoUtil
{
    public class CreateFileCommand : ConsoleCommand
    {
        private string _file;
        private string _schema;

        public CreateFileCommand(string file)
        {
            _file = file;
        }

        public CreateFileCommand(string file, string schema)
            : this(file)
        {
            _schema = schema;
        }

        public override int Execute()
        {
            CommandStatus retCode = CommandStatus.E_OK;

            bool create = ExpressUtility.CreateFlatFileDataSource(_file);
            if (!create)
            {
                WriteLine("Failed to create file {0}", _file);
                retCode = CommandStatus.E_FAIL_CREATE_DATASTORE;
                return (int)retCode;
            }
            WriteLine("File {0} created", _file);
            if (_schema != null)
            {
                try
                {
                    FdoConnection conn = ExpressUtility.CreateFlatFileConnection(_file);
                    conn.Open();
                    using (FdoFeatureService service = conn.CreateFeatureService())
                    {
                        service.LoadSchemasFromXml(_schema);
                        WriteLine("Schema applied to {0}", _file);
                    }
                    retCode = CommandStatus.E_OK;
                }
                catch (Exception ex)
                {
                    WriteException(ex);
                    retCode = CommandStatus.E_FAIL_APPLY_SCHEMA;
                }
            }
            return (int)retCode;
        }
    }
}
