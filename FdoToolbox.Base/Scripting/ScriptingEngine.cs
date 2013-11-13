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
using Microsoft.Scripting;
using Microsoft.Scripting.Hosting;
using IronPython.Hosting;
using System.Windows.Forms;
using System.IO;
using ICSharpCode.Core;
using IronPython.Runtime.Types;
using FdoToolbox.Base.Services;
using FdoToolbox.Core.Feature;

namespace FdoToolbox.Base.Scripting
{
    /// <summary>
    /// Represents a method that performs an action on an application script
    /// </summary>
    public delegate void ScriptEventHandler(ApplicationScript script);

    /// <summary>
    /// Application script engine
    /// </summary>
    public class ScriptingEngine
    {
        internal const string STARTUP_SCRIPT = "startup.py";

        private ScriptEngine _engine;
        private ScriptScope _scope;

        /// <summary>
        /// Occurs when [script loaded].
        /// </summary>
        public event ScriptEventHandler ScriptLoaded;

        private static ScriptingEngine _instance;

        /// <summary>
        /// Gets the instance.
        /// </summary>
        /// <value>The instance.</value>
        public static ScriptingEngine Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new ScriptingEngine();
                }
                return _instance;
            }
        }

        private ScriptingEngine()
        {
            if (Preferences.ScriptDebug)
            {
                Dictionary<string, object> options = new Dictionary<string, object>();
                options["Debug"] = true;
                _engine = Python.CreateEngine(options);
            }
            else
            {
                _engine = Python.CreateEngine();
            }

            _scope = _engine.CreateScope();
            HostApplication.InitializeScriptScope(_scope);

            List<string> paths = new List<string>();
            paths.Add(Path.Combine(Application.StartupPath, "Scripts"));
            paths.AddRange(Preferences.ScriptModulePaths);
            _engine.SetSearchPaths(paths);
            
            //mscorlib
            _engine.Runtime.LoadAssembly(typeof(string).Assembly);
            //System
            _engine.Runtime.LoadAssembly(typeof(Uri).Assembly);
            //System.Windows.Forms
            _engine.Runtime.LoadAssembly(typeof(Form).Assembly);
            //FdoToolbox.Base
            _engine.Runtime.LoadAssembly(typeof(Workbench).Assembly);
            //FdoToolbox.Core
            _engine.Runtime.LoadAssembly(typeof(FdoFeatureService).Assembly);
            //OSGeo.FDO
            _engine.Runtime.LoadAssembly(typeof(OSGeo.FDO.IConnectionManager).Assembly);
            //OSGeo.FDO.Common
            _engine.Runtime.LoadAssembly(typeof(OSGeo.FDO.Common.Exception).Assembly);
            //OSGeo.FDO.Geometry
            _engine.Runtime.LoadAssembly(typeof(OSGeo.FDO.Geometry.FgfGeometryFactory).Assembly);
        }

        /// <summary>
        /// Executes the statements.
        /// </summary>
        /// <param name="script">The script.</param>
        public void ExecuteStatements(string script)
        {
            ScriptSource src = _engine.CreateScriptSourceFromString(script, SourceCodeKind.Statements);
            src.Execute(_scope);
        }

        private Dictionary<string, ApplicationScript> loadedScripts = new Dictionary<string, ApplicationScript>();

        /// <summary>
        /// Gets the loaded scripts.
        /// </summary>
        /// <value>The loaded scripts.</value>
        public ICollection<string> LoadedScripts
        {
            get
            {
                return loadedScripts.Keys;
            }
        }

        /// <summary>
        /// Invokes the loaded script
        /// </summary>
        /// <param name="key">The key.</param>
        public void InvokeLoadedScript(string key)
        {
            if (loadedScripts.ContainsKey(key))
            {
                loadedScripts[key].Run();
            }
        }

        /// <summary>
        /// Runs the script.
        /// </summary>
        /// <param name="scriptPath">The script path.</param>
        public void RunScript(string scriptPath)
        {
            try
            {
                ScriptSource src = _engine.CreateScriptSourceFromFile(scriptPath);
                CompiledCode code = src.Compile();
                code.Execute(_scope);
            }
            catch (SyntaxErrorException ex)
            {
                ExceptionOperations eo = _engine.GetService<ExceptionOperations>();
                string error = eo.FormatException(ex);
                string msg = "Syntax error in \"{0}\"{1}Details:{1}{2}";
                msg = string.Format(msg, Path.GetFileName(scriptPath), Environment.NewLine, error);
                MessageService.ShowError(msg);
            }
        }

        /// <summary>
        /// Loads the script
        /// </summary>
        /// <param name="scriptPath">The script path.</param>
        public void LoadScript(string scriptPath)
        {
            if (!loadedScripts.ContainsKey(scriptPath))
            {
                try
                {
                    ScriptSource src = _engine.CreateScriptSourceFromFile(scriptPath);
                    CompiledCode code = src.Compile();
                    ApplicationScript cpy = new ApplicationScript(scriptPath, code, _scope);
                    loadedScripts.Add(scriptPath, cpy);

                    ScriptEventHandler handler = this.ScriptLoaded;
                    if (handler != null)
                        handler(cpy);
                }
                catch (SyntaxErrorException ex)
                {
                    ExceptionOperations eo = _engine.GetService<ExceptionOperations>();
                    string error = eo.FormatException(ex);
                    string msg = "Syntax error in \"{0}\"{1}Details:{1}{2}";
                    msg = string.Format(msg, Path.GetFileName(scriptPath), Environment.NewLine, error);
                    MessageService.ShowError(msg);
                }
            }
        }

        /// <summary>
        /// Recompiles the script. Call this if the referenced script has changed since it was loade
        /// </summary>
        /// <param name="scriptPath">The script path.</param>
        public void RecompileScript(string scriptPath)
        {
            if (loadedScripts.ContainsKey(scriptPath))
            {
                try
                {
                    ScriptSource src = _engine.CreateScriptSourceFromFile(scriptPath);
                    CompiledCode code = src.Compile();
                    ApplicationScript cpy = new ApplicationScript(scriptPath, code, _scope);
                    loadedScripts[scriptPath] = cpy;
                }
                catch (SyntaxErrorException ex)
                {
                    ExceptionOperations eo = _engine.GetService<ExceptionOperations>();
                    string error = eo.FormatException(ex);
                    string msg = "Syntax error in \"{0}\"{1}Details:{1}{2}";
                    msg = string.Format(msg, Path.GetFileName(scriptPath), Environment.NewLine, error);
                    MessageService.ShowError(msg);
                }
            }
        }

        /// <summary>
        /// Unloads the script.
        /// </summary>
        /// <param name="script">The script.</param>
        public void UnloadScript(string script)
        {
            if (loadedScripts.ContainsKey(script))
            {
                ApplicationScript cpy = loadedScripts[script];
                loadedScripts.Remove(script);
            }
        }
    }

    /// <summary>
    /// Represents a piece of executable script code. 
    /// </summary>
    public class ApplicationScript
    {
        private CompiledCode _code;
        private ScriptScope _scope;

        private string _Path;

        /// <summary>
        /// Gets the path of the script
        /// </summary>
        /// <value>The path.</value>
        public string Path
        {
            get { return _Path; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ApplicationScript"/> class.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <param name="code">The code.</param>
        /// <param name="scope">The scope.</param>
        internal ApplicationScript(string path, CompiledCode code, ScriptScope scope)
        {
            _Path = path;
            _code = code;
            _scope = scope;
        }

        /// <summary>
        /// Runs this instance.
        /// </summary>
        public void Run()
        {
            _code.Execute(_scope);
        }
    }
}
