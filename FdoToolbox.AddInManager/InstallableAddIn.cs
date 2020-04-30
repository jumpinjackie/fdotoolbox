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
using ICSharpCode.Core;
using ICSharpCode.SharpZipLib.Zip;
using System.IO;

namespace FdoToolbox.AddInManager
{
    public class InstallableAddIn
    {
        string fileName;
        bool isPackage;

        public AddIn AddIn { get; private set; }

        public InstallableAddIn(string fileName, bool isPackage)
        {
            this.fileName = fileName;
            this.isPackage = isPackage;
            if (isPackage)
            {
                ZipFile file = new ZipFile(fileName);
                try
                {
                    LoadAddInFromZip(file);
                }
                finally
                {
                    file.Close();
                }
            }
            else
            {
                AddIn = AddIn.Load(fileName);
            }
            if (AddIn.Manifest.PrimaryIdentity == null)
                throw new AddInLoadException(ResourceService.GetString("AddInManager.AddInMustHaveIdentity"));
        }

        void LoadAddInFromZip(ZipFile file)
        {
            ZipEntry addInEntry = null;
            foreach (ZipEntry entry in file)
            {
                if (entry.Name.EndsWith(".addin"))
                {
                    if (addInEntry != null)
                        throw new AddInLoadException("The package may only contain one .addin file.");
                    addInEntry = entry;
                }
            }
            if (addInEntry == null)
                throw new AddInLoadException("The package must contain one .addin file.");
            using (Stream s = file.GetInputStream(addInEntry))
            {
                using (StreamReader r = new StreamReader(s))
                {
                    AddIn = AddIn.Load(r);
                }
            }
        }

        public void Install(bool isUpdate)
        {
            foreach (string identity in AddIn.Manifest.Identities.Keys)
            {
                ICSharpCode.Core.AddInManager.AbortRemoveUserAddInOnNextStart(identity);
            }
            if (isPackage)
            {
                string targetDir = Path.Combine(ICSharpCode.Core.AddInManager.AddInInstallTemp,
                                                AddIn.Manifest.PrimaryIdentity);
                if (Directory.Exists(targetDir))
                    Directory.Delete(targetDir, true);
                Directory.CreateDirectory(targetDir);
                FastZip fastZip = new FastZip
                {
                    CreateEmptyDirectories = true
                };
                fastZip.ExtractZip(fileName, targetDir, null);

                AddIn.Action = AddInAction.Install;
                if (!isUpdate)
                {
                    AddInTree.InsertAddIn(AddIn);
                }
            }
            else
            {
                ICSharpCode.Core.AddInManager.AddExternalAddIns(new AddIn[] { AddIn });
            }
        }

        public static void CancelUpdate(IList<AddIn> addIns)
        {
            foreach (AddIn addIn in addIns)
            {
                foreach (string identity in addIn.Manifest.Identities.Keys)
                {
                    // delete from installation temp (if installation or update is pending)
                    string targetDir = Path.Combine(ICSharpCode.Core.AddInManager.AddInInstallTemp,
                                                    identity);
                    if (Directory.Exists(targetDir))
                        Directory.Delete(targetDir, true);
                }
            }
        }

        public static void Uninstall(IList<AddIn> addIns)
        {
            CancelUpdate(addIns);
            foreach (AddIn addIn in addIns)
            {
                foreach (string identity in addIn.Manifest.Identities.Keys)
                {
                    // remove the user AddIn
                    string targetDir = Path.Combine(ICSharpCode.Core.AddInManager.UserAddInPath, identity);
                    if (Directory.Exists(targetDir))
                    {
                        if (!addIn.Enabled)
                        {
                            try
                            {
                                Directory.Delete(targetDir, true);
                                continue;
                            }
                            catch
                            {
                            }
                        }
                        ICSharpCode.Core.AddInManager.RemoveUserAddInOnNextStart(identity);
                    }
                }
            }
        }
    }
}
