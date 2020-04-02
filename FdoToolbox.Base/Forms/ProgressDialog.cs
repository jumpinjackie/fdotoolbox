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
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Threading;

namespace FdoToolbox.Base.Forms
{
    // Based on code by Manfred Ramoser
    // http://manfred-ramoser.blogspot.com/2008/04/generic-progress-dialog.html
    
    /// <summary>
    /// Generic progress dialog
    /// </summary>
    public partial class ProgressDialog : Form
    {
        /// <summary>
        /// Represents a method that sets the title
        /// </summary>
        public delegate void SetTitleCallback(string text);
        /// <summary>
        /// Represents a method that sets the message
        /// </summary>
        public delegate void SetMessageCallback(string text);
        /// <summary>
        /// Represents a method that sets the value
        /// </summary>
        public delegate void SetValueCallback(int value);
        /// <summary>
        /// Represents a method that sets the maximum value
        /// </summary>
        public delegate void SetMaxCallback(int max);
        /// <summary>
        /// Represents a method that is called when cancelled
        /// </summary>
        public delegate void CancelEventHandler();
        /// <summary>
        /// Represents a method that is called when stopped
        /// </summary>
        public delegate void StopCallback();
        /// <summary>
        /// Occurs when [cancel event].
        /// </summary>
        public static event CancelEventHandler CancelEvent;

        private static ProgressDialog instance;
        private static Thread thread = null;

        /// <summary>
        /// text which can be shown as action which is done at the moment
        /// </summary>
        public static string actionText;

        /// <summary>
        /// text which is shown in front of the numbers; e.g. actionText 102/999
        /// </summary>
        public static string actionBaseText = "";

        /// <summary>
        /// Shows the progress dialog
        /// </summary>
        /// <param name="max">The maximum value. If less than or equal to 0, the progress bar will use the marquee animation</param>
        /// <param name="canCancel">if set to <c>true</c>, the user can cancel the operation. In this case, the <see cref="CancelEvent"/> must be handled</param>
        public static void Show(int max, bool canCancel)
        {
            instance = new ProgressDialog();
            instance.btnCancel.Enabled = canCancel;
            SetMax(max);
            
            thread = new Thread(new ThreadStart(LaunchForm));
            thread.IsBackground = true;
            thread.Start();
        }

        /// <summary>
        /// Stops and closes the progress dialog
        /// </summary>
        public static void Stop()
        {
            if (instance != null)
            {
                if (instance.InvokeRequired)
                {
                    StopCallback s = new StopCallback(Stop);
                    instance.Invoke(s);
                }
                else
                {
                    instance.Close();
                    instance.Dispose();
                    instance = null;
                }
            }

            if (thread != null)
            {
                Thread.Sleep(0);
                thread = null;
            }
        }

        private static void LaunchForm()
        {
            ProgressDialog.instance.ShowDialog();
        }

        private ProgressDialog()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Sets the title of the progress dialog
        /// </summary>
        /// <param name="title">The title.</param>
        public static void SetTitle(string title)
        {
            if (instance.InvokeRequired) //if another thread called this method
            {
                SetTitleCallback s = new SetTitleCallback(SetTitle);
                instance.Invoke(s, title);
            }
            else
            {
                instance.Text = title;
            }
        }

        /// <summary>
        /// Sets the message of the progress dialog
        /// </summary>
        /// <param name="message">The message.</param>
        public static void SetMessage(string message)
        {
            if (instance.InvokeRequired) //if another thread called this method
            {
                SetMessageCallback s = new SetMessageCallback(SetMessage);
                instance.Invoke(s, message);
            }
            else
            {
                instance.lblMessage.Text = message;
            }
        }

        /// <summary>
        /// Sets the number of items processed
        /// </summary>
        /// <param name="value">The value.</param>
        public static void SetValue(int value)
        {
            if (instance.InvokeRequired) //if another thread called this method
            {
                SetValueCallback s = new SetValueCallback(SetValue);
                instance.Invoke(s, value);
            }
            else
            {
                if (instance.progressBar.Maximum > 0)
                {
                    instance.progressBar.Value = value;
                    if (!actionBaseText.Equals(""))
                        instance.lblProgress.Text = actionBaseText + " " + value + "/" + instance.progressBar.Maximum;
                    else
                        instance.lblProgress.Text = actionText;
                }
                else
                {
                    instance.lblProgress.Text = actionText + " " + value;
                }
            }
        }

        /// <summary>
        /// Sets the maximum value. If the maximum is less than or equal to 0, then the
        /// progress animation will become marquee.
        /// </summary>
        /// <param name="max">The max.</param>
        public static void SetMax(int max)
        {
            if (instance.InvokeRequired) //if another thread called this method
            {
                SetMaxCallback s = new SetMaxCallback(SetMax);
                instance.Invoke(s, max);
            }
            else
            {
                instance.progressBar.Maximum = (max < 0) ? 0 : max;
                if (max <= 0)
                    instance.progressBar.Style = ProgressBarStyle.Marquee;

            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("Are You sure that you want to cancel the operation?", "Question", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                CancelEvent(); //raise the cancel event
            }
        }
    }
}