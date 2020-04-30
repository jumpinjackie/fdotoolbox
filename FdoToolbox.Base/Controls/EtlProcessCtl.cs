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
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using FdoToolbox.Base;
using ICSharpCode.Core;
using FdoToolbox.Core.ETL;
using FdoToolbox.Core.ETL.Specialized;

namespace FdoToolbox.Base.Controls
{
    /// <summary>
    /// Dialog view used to relay the activity of a running <see cref="EtlProcess"/>
    /// </summary>
    [ToolboxItem(false)]
    public partial class EtlProcessCtl : ViewContent, IViewContent
    {
        private EtlProcessCtl()
        {
            InitializeComponent();
        }

        private EtlBackgroundRunner _runner;

        /// <summary>
        /// Initializes a new instance of the <see cref="EtlProcessCtl"/> class.
        /// </summary>
        /// <param name="proc">The proc.</param>
        public EtlProcessCtl(IFdoSpecializedEtlProcess proc)
            : this()
        {
            _runner = new EtlBackgroundRunner(proc);
            _runner.ProcessMessage += new MessageEventHandler(OnMessageSent);
        }

        void OnMessageSent(object sender, FdoToolbox.Core.MessageEventArgs e)
        {
            if (txtOutput.InvokeRequired)
            {
                txtOutput.Invoke(new AppendTextHandler(this.AppendText), e.Message);
            }
            else
            {
                this.AppendText(e.Message);
            }
        }

        private delegate void AppendTextHandler(string msg);

        private void AppendText(string msg)
        {
            txtOutput.AppendText(msg + "\n");
            txtOutput.ScrollToCaret();
        }

        /// <summary>
        /// Raises the <see cref="E:System.Windows.Forms.UserControl.Load"/> event.
        /// </summary>
        /// <param name="e">An <see cref="T:System.EventArgs"/> that contains the event data.</param>
        protected override void OnLoad(EventArgs e)
        {
            bgEtlProc.RunWorkerAsync();
            base.OnLoad(e);
        }

        /// <summary>
        /// The title of the view
        /// </summary>
        /// <value></value>
        public override string Title => ResourceService.GetString("TITLE_ETL_PROCESS");

        private void bgEtlProc_DoWork(object sender, DoWorkEventArgs e)
        {
            _runner.Run();
        }

        private void bgEtlProc_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                AppendText(string.Format("An unhandled exception was thrown during process execution: {0}", e.Error.ToString()));
            }
            btnOK.Enabled = true;
            btnCancel.Enabled = false;
            if (e.Cancelled)
            {
                AppendText("ETL Process Cancelled!");
            }
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            base.Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            if (_runner.ExecutingThread.IsAlive)
                _runner.ExecutingThread.Abort();
            else
                base.Close();
        }
    }
}
