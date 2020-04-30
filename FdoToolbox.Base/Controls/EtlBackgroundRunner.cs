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
using FdoToolbox.Core.ETL;
using System.Threading;
using FdoToolbox.Core.ETL.Specialized;
using FdoToolbox.Core;
using System.IO;

namespace FdoToolbox.Base.Controls
{
    /// <summary>
    /// A <see cref="EtlProcess"/> runner class. Provides delegation of events raised by the <see cref="EtlProcess"/> to any
    /// interested user interface elements.
    /// </summary>
    public class EtlBackgroundRunner
    {
        private IFdoSpecializedEtlProcess _proc;

        /// <summary>
        /// Initializes a new instance of the <see cref="EtlBackgroundRunner"/> class.
        /// </summary>
        /// <param name="proc">The proc.</param>
        public EtlBackgroundRunner(IFdoSpecializedEtlProcess proc)
        {
            _proc = proc;
            _proc.FeatureProcessed += delegate(object sender, FeatureCountEventArgs e)
            {
                this.FeatureProcessed(sender, e);
            };
            _proc.ProcessMessage += delegate(object sender, MessageEventArgs e)
            {
                this.ProcessMessage(sender, e);
            };
        }

        /// <summary>
        /// Gets or sets the executing thread.
        /// </summary>
        /// <value>The executing thread.</value>
        public Thread ExecutingThread { get; private set; }

        /// <summary>
        /// Runs this instance.
        /// </summary>
        public void Run()
        {
            this.ExecutingThread = Thread.CurrentThread;
            try
            {
                _proc.Execute();
                EtlProcess p = _proc.ToEtlProcess();
                List<Exception> errors = new List<Exception>(p.GetAllErrors());
                if (errors.Count > 0)
                {
                    this.ProcessMessage(this, new MessageEventArgs(errors.Count + " errors in total were found during the ETL process."));
                    //Log these errors
                    LogExceptions(errors);
                }
            }
            catch (ThreadAbortException)
            {
                //FIXME: Sometimes this doesn't terminate cleanly. I'm guessing the proper
                //way would be to implement a "kill switch" in EtlProcess and
                //for this code block to flick this switch
                Thread.ResetAbort();
            }
        }

        private void LogExceptions(List<Exception> errors)
        {
            string logfile = Path.Combine(Preferences.LogPath, Guid.NewGuid().ToString() + ".log");
            if (!Directory.Exists(Preferences.LogPath))
                Directory.CreateDirectory(Preferences.LogPath);

            using (StreamWriter writer = new StreamWriter(logfile, false))
            {
                for (int i = 0; i < errors.Count; i++)
                {
                    writer.WriteLine("------- EXCEPTION #" + (i+1) + " -------");
                    if (errors[i].Data.Count > 0)
                    {
                        writer.WriteLine("\n== Exception User Data: ==");
                        foreach (var key in errors[i].Data.Keys)
                        {
                            writer.WriteLine("{0} = {1}", key, errors[i].Data[key]);
                        }
                        writer.WriteLine();
                    }
                    writer.WriteLine(errors[i].ToString());
                    writer.WriteLine("------- EXCEPTION END -------");
                }
            }
            this.ProcessMessage(this, new MessageEventArgs("Errors have been written to: " + logfile));
        }

        /// <summary>
        /// Fires when a feature has been processed
        /// </summary>
        public event FeatureCountEventHandler FeatureProcessed = delegate { };

        /// <summary>
        /// Fires when a message has been sent
        /// </summary>
        public event MessageEventHandler ProcessMessage = delegate { };
    }
}
