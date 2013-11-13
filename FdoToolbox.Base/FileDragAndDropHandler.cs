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
using System.Windows.Forms;
using FdoToolbox.Base.Services;
using FdoToolbox.Base.Services.DragDropHandlers;
using FdoToolbox.Base.Controls;

namespace FdoToolbox.Base
{
    internal sealed class FileDragAndDropHandler
    {
        internal static void OnDragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                Array a = e.Data.GetData(DataFormats.FileDrop) as Array;
                if (a != null && a.Length > 0)
                {
                    e.Effect = DragDropEffects.Copy;
                }
                else
                    e.Effect = DragDropEffects.None;
            }
            else
                e.Effect = DragDropEffects.None;
        }

        internal static void OnDragDrop(object sender, DragEventArgs e)
        {
            Array a = e.Data.GetData(DataFormats.FileDrop) as Array;
            if (a != null && a.Length > 0)
            {
                IDragDropHandlerService handlerSvc = ServiceManager.Instance.GetService<IDragDropHandlerService>();
                for (int i = 0; i < a.Length; i++)
                {
                    string file = a.GetValue(i).ToString();

                    IList<IDragDropHandler> handlers = handlerSvc.GetHandlersForFile(file);

                    if (handlers.Count == 0)
                        continue;

                    if (handlers.Count == 1)
                    {
                        using (TempCursor cur = new TempCursor(Cursors.WaitCursor))
                        {
                            handlers[0].HandleDrop(file);
                        }
                    }

                    if (handlers.Count > 1)
                    {
                        //Resolve which handler to use
                    }
                }
            }
        }
    }
}
