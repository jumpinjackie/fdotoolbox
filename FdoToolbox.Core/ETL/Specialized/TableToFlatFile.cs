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
using FdoToolbox.Core.Feature;
using FdoToolbox.Core.Utility;
using OSGeo.FDO.Schema;
using FdoToolbox.Core.ETL.Operations;

namespace FdoToolbox.Core.ETL.Specialized
{
    /// <summary>
    /// A <see cref="FdoFeatureTable"/> to flat file data source conversion process
    /// </summary>
    public class TableToFlatFile : FdoSpecializedEtlProcess
    {
        private int _ReportFrequency = 50;

        /// <summary>
        /// Gets or sets the frequency at which progress feedback is made
        /// </summary>
        /// <value>The report frequency.</value>
        public int ReportFrequency
        {
            get { return _ReportFrequency; }
            set { _ReportFrequency = value; }
        }

        private FdoFeatureTable _table;
        private string _outputFile;

        /// <summary>
        /// Initializes a new instance of the <see cref="TableToFlatFile"/> class.
        /// </summary>
        /// <param name="table">The table.</param>
        /// <param name="file">The file.</param>
        public TableToFlatFile(FdoFeatureTable table, string file)
        {
            _table = table;
            _outputFile = file;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TableToFlatFile"/> class.
        /// </summary>
        /// <param name="table">The table.</param>
        /// <param name="file">The file.</param>
        /// <param name="reportFrequency">The report frequency.</param>
        public TableToFlatFile(FdoFeatureTable table, string file, int reportFrequency)
            : this(table, file)
        {
            _ReportFrequency = reportFrequency;
        }

        private FdoConnection _outConn;

        /// <summary>
        /// Initializes this instance.
        /// </summary>
        protected override void Initialize()
        {
            SendMessage("Creating target data source");
            if (!ExpressUtility.CreateFlatFileDataSource(_outputFile))
                throw new FdoETLException(ResourceUtil.GetStringFormatted("ERR_CANNOT_CREATE_DATA_FILE", _outputFile));

            _outConn = ExpressUtility.CreateFlatFileConnection(_outputFile);
            _outConn.Open();

            ClassDefinition cd = _table.CreateClassDefinition(true);

            using (FdoFeatureService service = _outConn.CreateFeatureService())
            {
                SendMessage("Applying schema to target");
                FeatureSchema schema = new FeatureSchema("Schema1", "Default schema");
                schema.Classes.Add(cd);
                service.ApplySchema(schema);
            }

            SendMessage("Copying any attached spatial contexts");
            ExpressUtility.CopyAllSpatialContexts(_table.SpatialContexts, _outConn, true);

            Register(new FdoFeatureTableInputOperation(_table));
            Register(new FdoOutputOperation(_outConn, cd.Name));
        }

        /// <summary>
        /// Called when a row is processed.
        /// </summary>
        /// <param name="op">The operation.</param>
        /// <param name="dictionary">The dictionary.</param>
        protected override void OnFeatureProcessed(FdoOperationBase op, FdoRow dictionary)
        {
            if (op.Statistics.OutputtedRows % this.ReportFrequency == 0)
            {
                if (op is FdoOutputOperation)
                {
                    string className = (op as FdoOutputOperation).ClassName;
                    SendMessageFormatted("[Conversion => {0}]: {1} features processed", className, op.Statistics.OutputtedRows);
                }
            }
        }

        /// <summary>
        /// Called when this process has finished processing.
        /// </summary>
        /// <param name="op">The op.</param>
        protected override void OnFinishedProcessing(FdoOperationBase op)
        {
            if (op is FdoOutputOperation)
            {
                string className = (op as FdoOutputOperation).ClassName;
                SendMessageFormatted("[Conversion => {0}]: {1} features processed in {2}", className, op.Statistics.OutputtedRows, op.Statistics.Duration.ToString());
            }
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _outConn.Close();
                _outConn.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
