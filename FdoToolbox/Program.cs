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
using System.Windows.Forms;
using ICSharpCode.Core;
using System.Reflection;
using FdoToolbox.Base;
using System.IO;
using System.Resources;
using FdoToolbox.Base.Services;
using FdoToolbox.Core;
using System.Threading;
using System.Runtime.InteropServices;
using OSGeo.MapGuide;

namespace FdoToolbox
{
    static class Program
    {
        static Mutex appMutex = new Mutex(true, "{D427D01F-C2C5-4cc6-9340-413E8F27B01B}");

        [DllImport("kernel32")]
        static extern uint SetErrorMode(uint uMode);

        const uint SEM_FAILCRITICALERRORS = 0x0001;
        const uint SEM_NOALIGNMENTFAULTEXCEPT = 0x0004;
        const uint SEM_NOGPFAULTERRORBOX = 0x0002;
        const uint SEM_NOOPENFILEERRORBOX = 0x8000;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate void PureCallHandler();

        [DllImport("vcruntime140", CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr _set_purecall_handler([MarshalAs(UnmanagedType.FunctionPtr)] PureCallHandler handler);

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            if (appMutex.WaitOne(TimeSpan.Zero, true))
            {
                //Set up CS-Map
                MgCoordinateSystemFactory fact = new MgCoordinateSystemFactory();
                MgCoordinateSystemCatalog cat = fact.GetCatalog();
                cat.SetDictionaryDir("C:\\Program Files\\OSGeo\\MapGuide\\CS-Map\\Dictionaries");
                //cat.SetDictionaryDir(Path.Combine(Application.StartupPath, "Dictionaries"));

                //Yes, we know that FDO providers like King.Oracle/MySQL/PostgreSQL require
                //additional dlls. No need to spam this error at the user everytime they launch
                SetErrorMode(SEM_FAILCRITICALERRORS);
                
                //Blah blah blah. I don't care that your FDO provider does not implement reference counting correctly
                //especially when you throw this as I'm exiting!
                _set_purecall_handler(IDontCare);

                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(true);
                Application.ThreadException += new System.Threading.ThreadExceptionEventHandler(Application_ThreadException);

                // The LoggingService is a small wrapper around log4net.
                // Our application contains a .config file telling log4net to write
                // to System.Diagnostics.Trace.
                LoggingService.Info("Application start");

                AppDomain.CurrentDomain.AssemblyLoad += new AssemblyLoadEventHandler(CurrentDomain_AssemblyLoad);

                // Get a reference to the entry assembly (Startup.exe)
                Assembly exe = typeof(Program).Assembly;

                // Set the root path of our application. ICSharpCode.Core looks for some other
                // paths relative to the application root:
                // "data/resources" for language resources, "data/options" for default options
                FileUtility.ApplicationRootPath = Path.GetDirectoryName(exe.Location);

                LoggingService.Info("Starting core services...");

                string title = "FDO Toolbox";

                // CoreStartup is a helper class making starting the Core easier.
                // The parameter is used as the application name, e.g. for the default title of
                // MessageService.ShowMessage() calls.

                CoreStartup coreStartup = new CoreStartup(title);

                // It is also used as default storage location for the application settings:
                // "%Application Data%\%Application Name%", but you can override that by setting c.ConfigDirectory

                var asmName = Assembly.GetExecutingAssembly().GetName();
                coreStartup.ConfigDirectory = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                    string.Format("{0} {1}.{2}.{3}",
                                  title,
                                  asmName.Version.Major,
                                  asmName.Version.Minor,
                                  asmName.Version.Build));


                // Specify the name of the application settings file (.xml is automatically appended)
                coreStartup.PropertiesName = "AppProperties";

                // Initializes the Core services (ResourceService, PropertyService, etc.)
                coreStartup.StartCoreServices();

                LoggingService.Info("Looking for AddIns...");
                // Searches for ".addin" files in the application directory.
                coreStartup.AddAddInsFromDirectory(Path.Combine(FileUtility.ApplicationRootPath, "AddIns"));

                // Searches for a "AddIns.xml" in the user profile that specifies the names of the
                // add-ins that were deactivated by the user, and adds "external" AddIns.
                coreStartup.ConfigureExternalAddIns(Path.Combine(PropertyService.ConfigDirectory, "AddIns.xml"));

                // Searches for add-ins installed by the user into his profile directory. This also
                // performs the job of installing, uninstalling or upgrading add-ins if the user
                // requested it the last time this application was running.
                coreStartup.ConfigureUserAddIns(Path.Combine(PropertyService.ConfigDirectory, "AddInInstallTemp"),
                                                Path.Combine(PropertyService.ConfigDirectory, "AddIns"));

                LoggingService.Info("Checking FDO");
                // Set FDO path
                string fdoPath = Preferences.FdoPath;
                bool abort = false;
                if (!FdoAssemblyResolver.IsValidFdoPath(fdoPath))
                {
                    fdoPath = Path.Combine(FileUtility.ApplicationRootPath, "FDO");
                    Preferences.FdoPath = fdoPath;

                    while (!FdoAssemblyResolver.IsValidFdoPath(fdoPath) && !abort)
                    {
                        FolderBrowserDialog fb = new FolderBrowserDialog();
                        fb.Description = "Select the directory that contains the FDO binaries";
                        if (fb.ShowDialog() == DialogResult.Cancel)
                        {
                            abort = true;
                        }
                        else
                        {
                            fdoPath = fb.SelectedPath;
                            Preferences.FdoPath = fdoPath;
                        }
                    }
                }

                if (abort)
                    return;

                AddInAssemblyResolver.RegisterLibraries(
                    fdoPath, 
                    "OSGeo.FDO.dll", 
                    "OSGeo.FDO.Common.dll", 
                    "OSGeo.FDO.Geometry.dll", 
                    "OSGeo.FDO.Spatial.dll",
                    "OSGeo.FDO.Providers.MySQL.Overrides.dll",
                    "OSGeo.FDO.Providers.ODBC.Overrides.dll",
                    "OSGeo.FDO.Providers.Rdbms.dll",
                    "OSGeo.FDO.Providers.Rdbms.Overrides.dll",
                    "OSGeo.FDO.Providers.SHP.Overrides.dll",
                    "OSGeo.FDO.Providers.SQLServerSpatial.Overrides.dll",
                    "OSGeo.FDO.Providers.WMS.Overrides.dll");

                LoggingService.Info("FDO path set to: " + fdoPath);

                LoggingService.Info("Loading AddInTree...");
                // Now finally initialize the application. This parses the ".addin" files and
                // creates the AddIn tree. It also automatically runs the commands in
                // "/Workspace/Autostart"
                coreStartup.RunInitialization();

                LoggingService.Info("Initializing Workbench...");
                // Workbench is our class from the base project, this method creates an instance
                // of the main form.
                log4net.Config.XmlConfigurator.Configure();
                Workbench.InitializeWorkbench(title);

                try
                {
                    LoggingService.Info("Running application...");
                    // Workbench.Instance is the instance of the main form, run the message loop.
                    Application.Run(Workbench.Instance);
                }
                finally
                {
                    try
                    {
                        // Save changed properties
                        PropertyService.Save();
                    }
                    catch (Exception ex)
                    {
                        MessageService.ShowError(ex, "Error storing properties");
                    }
                }
                LoggingService.Info("Application shutdown");
                appMutex.ReleaseMutex();
            }
            else
            {
                //Send our message to make the workbench be the topmost form
                NativeMethods.PostMessage(
                    (IntPtr)NativeMethods.HWND_BROADCAST,
                    NativeMethods.WM_SHOWME,
                    IntPtr.Zero,
                    IntPtr.Zero);
            }
        }

        private static void IDontCare() 
        {
            System.Diagnostics.Debug.WriteLine("Pure virtual function call you say? I can't hear you");
        }

        static void CurrentDomain_AssemblyLoad(object sender, AssemblyLoadEventArgs args)
        {
            LoggingService.InfoFormatted("Loaded assembly: {0}", args.LoadedAssembly.GetName().Name);
        }

        static void Application_ThreadException(object sender, System.Threading.ThreadExceptionEventArgs e)
        {
            MessageService.ShowError(e.Exception);
        }
    }
}
