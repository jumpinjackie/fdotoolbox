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

namespace FdoToolbox.Base
{
    /// <summary>
    /// Abstract view interface.
    /// </summary>
    public interface IViewContent : ISubView
    {
        /// <summary>
        /// The title of the view
        /// </summary>
        string Title { get; set; }
        /// <summary>
        /// Fires when the title has been changed
        /// </summary>
        event EventHandler TitleChanged;
        /// <summary>
        /// Detrmines if this view can be closed
        /// </summary>
        bool CanClose { get; }
        /// <summary>
        /// Closes the view. This raises the <see cref="ViewContentClosing"/> event
        /// </summary>
        /// <returns></returns>
        void Close();
        /// <summary>
        /// Fired when the view has been closed internally
        /// </summary>
        event EventHandler ViewContentClosing;
        /// <summary>
        /// Displays an exception message
        /// </summary>
        /// <param name="ex">The exception object</param>
        void ShowError(Exception ex);
        /// <summary>
        /// Displays an error message
        /// </summary>
        /// <param name="message">The message</param>
        void ShowError(string message);
        /// <summary>
        /// Displays an alert message
        /// </summary>
        /// <param name="title">The title of this message</param>
        /// <param name="message">The message</param>
        void ShowMessage(string title, string message);
        /// <summary>
        /// Make a request for confirmation
        /// </summary>
        /// <param name="title">The title of the confirmation message</param>
        /// <param name="message">The message</param>
        /// <returns>true if confirmed, false otherwise</returns>
        bool Confirm(string title, string message);
        /// <summary>
        /// Make a request for confirmation
        /// </summary>
        /// <param name="title">The title of the confirmation message</param>
        /// <param name="format">The message template</param>
        /// <param name="args">The template values</param>
        /// <returns>true if confirmed, false otherwise</returns>
        bool ConfirmFormatted(string title, string format, params string[] args);
    }

    /// <summary>
    /// Defines the possible regions of the user interface a <see cref="IViewContent"/> can reside in 
    /// </summary>
    public enum ViewRegion
    {
        /// <summary>
        /// The view content will be docked to the left
        /// </summary>
        Left,
        /// <summary>
        /// The view content will be docked to the right
        /// </summary>
        Right,
        /// <summary>
        /// The view content will be docked to the bottom
        /// </summary>
        Bottom,
        /// <summary>
        /// The view content will be docked to the top
        /// </summary>
        Top,
        /// <summary>
        /// The view content will be docked to the center, (in a tabbed document interface)
        /// </summary>
        Document,
        /// <summary>
        /// The view content will reside in a floating dialog
        /// </summary>
        Floating,
        /// <summary>
        /// The view content will reside in a modal dialog
        /// </summary>
        Dialog
    }
}
