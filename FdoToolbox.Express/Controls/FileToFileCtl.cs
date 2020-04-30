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
using FdoToolbox.Base;
using ICSharpCode.Core;
using FdoToolbox.Base.Services;
using FdoToolbox.Core.ETL.Specialized;
using FdoToolbox.Core.Utility;
using FdoToolbox.Base.Controls;
using System.IO;

namespace FdoToolbox.Express.Controls
{
    [ToolboxItem(false)]
    public partial class FileToFileCtl : ViewContent
    {
        public FileToFileCtl()
        {
            InitializeComponent();
        }

        public override string Title => ResourceService.GetString("TITLE_EXPRESS_BULK_COPY");

        private void btnOpen_Click(object sender, EventArgs e)
        {
            string file = FileService.OpenFile(ResourceService.GetString("TITLE_OPEN_FILE"), ResourceService.GetString("FILTER_EXPRESS_BCP"));
            if (file != null)
                txtSource.Text = file;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            string file = FileService.SaveFile(ResourceService.GetString("TITLE_SAVE_FILE"), ResourceService.GetString("FILTER_EXPRESS_BCP"));
            if (file != null)
                txtTarget.Text = file;
        }

        private bool DeleteRelated(string target)
        {
            string[] files = ExpressUtility.GetRelatedFiles(target);
            List<string> delete = new List<string>();
            foreach (string f in files)
            {
                if (File.Exists(f))
                {
                    delete.Add(f);
                }
            }

            if (delete.Count > 0)
            {
                if (this.Confirm("Express Bulk Copy",
                    string.Format("The following files will be deleted along with the shape file: {0}\n\n{1}\n\nAre you sure you want to continue?",
                        target,
                        string.Join("\n", delete.ToArray()))))
                {
                    return true;
                }
            }

            return false;
        }

        private void btnExecute_Click(object sender, EventArgs e)
        {
            string source = txtSource.Text;
            string target = txtTarget.Text;
            
            if (FileService.FileExists(source) && !string.IsNullOrEmpty(target))
            {
                if (ExpressUtility.IsShp(target))
                {
                    if (File.Exists(target))
                    {
                        if (!DeleteRelated(target))
                            return;
                        else
                            DeleteRelatedFiles(target);
                    }
                    else if (Directory.Exists(target))
                    {
                        string shp = Path.Combine(target, Path.GetFileNameWithoutExtension(source) + ".shp");
                        if (File.Exists(shp) && !DeleteRelated(shp))
                            return;
                        else
                            DeleteRelatedFiles(shp);

                        // if target is a shape then the names of the files corresponds to the classname
                        string[] classnames = ExpressUtility.GetClassNames(source);
                        foreach (string classname in classnames)
                        {
                            shp = Path.Combine(target, classname + ".shp");
                            if (File.Exists(shp) && !DeleteRelated(shp))
                                return;
                            else
                                DeleteRelatedFiles(shp);
                        }
                    }
                }

                using (FdoBulkCopy bcp = ExpressUtility.CreateFileToFileBulkCopy(source, target, chkCopySpatialContexts.Checked, true, chkFlatten.Checked))
                {
                    EtlProcessCtl ctl = new EtlProcessCtl(bcp);
                    Workbench.Instance.ShowContent(ctl, ViewRegion.Dialog);
                    base.Close();
                }
            }
            else
            {
                this.ShowError("Source and Target fields are required");
            }
        }

        private void DeleteRelatedFiles(string shp)
        {
            string[] files = ExpressUtility.GetRelatedFiles(shp);
            foreach (string f in files)
            {
                if (File.Exists(f))
                {
                    File.Delete(f);
                    LoggingService.Info("File deleted: " + f);
                }
            }
            if (File.Exists(shp))
                File.Delete(shp);
        }

        private void btnBrowseDir_Click(object sender, EventArgs e)
        {
            string dir = FileService.GetDirectory(ResourceService.GetString("TITLE_CHOOSE_DIRECTORY"));
            if (dir != null)
                txtTarget.Text = dir;
        }
    }
}
