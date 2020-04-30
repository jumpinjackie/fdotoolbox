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
using System.Windows.Forms;

namespace FdoToolbox.Base.Forms
{
    /// <summary>
    /// A message box with support for large bodies of text
    /// </summary>
    public partial class WrappedMessageBox : Form
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WrappedMessageBox"/> class.
        /// </summary>
        public WrappedMessageBox()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Gets or sets the message.
        /// </summary>
        /// <value>The message.</value>
        public string Message
        {
            get { return textBox1.Text; }
            set 
            {
                string[] lines = value.Split(new string[] { Environment.NewLine }, StringSplitOptions.None);
                textBox1.Lines = lines;
            }
        }

        /// <summary>
        /// Shows the message.
        /// </summary>
        /// <param name="title">The title.</param>
        /// <param name="message">The message.</param>
        public static void ShowMessage(string title, string message)
        {
            WrappedMessageBox msg = new WrappedMessageBox
            {
                Text = title,
                Message = message
            };
            msg.button1.Visible = false;
            msg.button2.Text = "OK";
            msg.button2.Click += delegate
            {
                msg.DialogResult = DialogResult.OK;
            };
            msg.ShowDialog();
        }

        /// <summary>
        /// Confirms the specified title.
        /// </summary>
        /// <param name="title">The title.</param>
        /// <param name="message">The message.</param>
        /// <param name="text">The text.</param>
        /// <returns></returns>
        public static bool Confirm(string title, string message, MessageBoxText text)
        {
            WrappedMessageBox msg = new WrappedMessageBox
            {
                Text = title,
                Message = message
            };
            if (text == MessageBoxText.YesNo)
            {
                msg.button1.Text = "Yes";
                msg.button2.Text = "No";
            }
            else
            {
                msg.button1.Text = "OK";
                msg.button2.Text = "Cancel";
            }
            msg.button1.Click += delegate
            {
                msg.DialogResult = DialogResult.OK;
            };
            msg.button2.Click += delegate
            {
                msg.DialogResult = DialogResult.Cancel;
            };
            return msg.ShowDialog() == DialogResult.OK;
        }
    }

    /// <summary>
    /// Determines the button format of the <see cref="WrappedMessageBox"/>
    /// </summary>
    public enum MessageBoxText
    {
        /// <summary>
        /// Buttons will be yes/no
        /// </summary>
        YesNo,
        /// <summary>
        /// Buttons will be ok/cancel
        /// </summary>
        OkCancel
    }
}