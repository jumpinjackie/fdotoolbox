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
using FdoToolbox.Core.Configuration;
using System.Diagnostics;
using System.IO;
using FdoToolbox.Core.AppFramework;

namespace FdoToolbox.Core.ETL.Specialized
{
    /// <summary>
    /// Defines a sequence of invocations to FdoUtil.exe
    /// </summary>
    public class FdoSequentialProcess : FdoSpecializedEtlProcess
    {
        class InvokeExternalProcess : FdoSpecializedEtlProcess
        {
            private string _fileName;
            private string[] _args;

            public InvokeExternalProcess(string name, string[] args)
            {
                _fileName = name;
                _args = args;
            }

            internal SequentialOperation Definition
            {
                get;
                set;
            }

            protected override void Initialize()
            {
               
            }

            void OnErrorDataReceived(object sender, DataReceivedEventArgs e)
            {
                if (e.Data != null)
                    SendMessage(e.Data);
            }

            void OnOutputDataReceived(object sender, DataReceivedEventArgs e)
            {
                if (e.Data != null)
                    SendMessage(e.Data);
            }

            public string GetNextAction(int code)
            {
                if (this.Definition.CompleteActions == null)
                    return null;

                if (this.Definition.CompleteActions.Count == 0)
                    return null;

                foreach (var act in this.Definition.CompleteActions)
                {
                    if (act.ReturnCode == code)
                        return act.Operation;
                }

                return null;
            }

            public new int Execute()
            {
                SendMessageFormatted("== Starting process {0} with arguments: {1}", _fileName, string.Join(" ", _args));
                using (var proc = new Process())
                {
                    proc.StartInfo.FileName = _fileName;
                    proc.StartInfo.Arguments = string.Join(" ", _args);
                    proc.StartInfo.UseShellExecute = false;
                    proc.StartInfo.RedirectStandardOutput = true;
                    proc.StartInfo.CreateNoWindow = true;
                    proc.Start();

                    StreamReader srOut = proc.StandardOutput;
                    string line = srOut.ReadLine();
                    while (line != null)
                    {
                        SendMessage(line);
                        line = srOut.ReadLine();
                    }

                    proc.WaitForExit();

                    return proc.ExitCode;
                }
            }
        }

        private List<InvokeExternalProcess> _processes;

        /// <summary>
        /// Initializes a new instance of the <see cref="FdoSequentialProcess"/> class.
        /// </summary>
        /// <param name="def">The def.</param>
        public FdoSequentialProcess(SequentialProcessDefinition def)
        {
            ProcessDefinition = def;
            _processes = new List<InvokeExternalProcess>();
        }

        /// <summary>
        /// Gets or sets the process definition
        /// </summary>
        public SequentialProcessDefinition ProcessDefinition { get; set; }

        private static string Escape(string value)
        {
            if (value.Contains(" "))
                return "\"" + value.Replace("\"", "\\\\\"") + "\"";

            return value;
        }

        /// <summary>
        /// Initializes this instance.
        /// </summary>
        protected override void Initialize()
        {
            Reset();
            foreach (var opt in ProcessDefinition.Operations)
            {
                var name = "FdoUtil.exe";
                var args = new List<string>();

                args.Add("-cmd:" + opt.Command);
                foreach (var arg in opt.Arguments)
                {
                    args.Add("-" + arg.Name + ":" + Escape(arg.GetProcessedValue(ProcessDefinition.Variables)));
                }

                _processes.Add(new InvokeExternalProcess(name, args.ToArray()) { Definition = opt });
            }
        }

        private void Reset()
        {
            ClearErrors();
            subProcessErrors.Clear();
            _processes.Clear();
        }

        private bool execute = true;

        /// <summary>
        /// 
        /// </summary>
        [global::System.Serializable]
        public class InvokeProcessException : Exception
        {
            //
            // For guidelines regarding the creation of new exception types, see
            //    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/cpgenref/html/cpconerrorraisinghandlingguidelines.asp
            // and
            //    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/dncscol/html/csharp07192001.asp
            //

            /// <summary>
            /// Initializes a new instance of the <see cref="InvokeProcessException"/> class.
            /// </summary>
            public InvokeProcessException() { }
            /// <summary>
            /// Initializes a new instance of the <see cref="InvokeProcessException"/> class.
            /// </summary>
            /// <param name="message">The message.</param>
            public InvokeProcessException(string message) : base(message) { }
            /// <summary>
            /// Initializes a new instance of the <see cref="InvokeProcessException"/> class.
            /// </summary>
            /// <param name="message">The message.</param>
            /// <param name="inner">The inner.</param>
            public InvokeProcessException(string message, Exception inner) : base(message, inner) { }
            /// <summary>
            /// Initializes a new instance of the <see cref="InvokeProcessException"/> class.
            /// </summary>
            /// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo"/> that holds the serialized object data about the exception being thrown.</param>
            /// <param name="context">The <see cref="T:System.Runtime.Serialization.StreamingContext"/> that contains contextual information about the source or destination.</param>
            /// <exception cref="T:System.ArgumentNullException">
            /// The <paramref name="info"/> parameter is null.
            /// </exception>
            /// <exception cref="T:System.Runtime.Serialization.SerializationException">
            /// The class name is null or <see cref="P:System.Exception.HResult"/> is zero (0).
            /// </exception>
            protected InvokeProcessException(
              System.Runtime.Serialization.SerializationInfo info,
              System.Runtime.Serialization.StreamingContext context)
                : base(info, context) { }
        }

        /// <summary>
        /// Determines if this process is capable of persistence
        /// </summary>
        /// <value></value>
        public override bool CanSave => true;

        class ExternalProcessIterator : IEnumerator<InvokeExternalProcess>
        {
            private IEnumerator<InvokeExternalProcess> _iter;
            private FdoSequentialProcess _parent;

            public ExternalProcessIterator(FdoSequentialProcess parent, List<InvokeExternalProcess> processes)
            {
                _parent = parent;
                _iter = processes.GetEnumerator();
                //Wire up event handlers
                while (_iter.MoveNext())
                {
                    _iter.Current.ProcessMessage += new MessageEventHandler(OnSubProcessMessage);
                }
                _iter.Reset();
            }

            public InvokeExternalProcess Current => _iter.Current;

            public void Dispose()
            {
                //Un-wire event handlers
                _iter.Reset();
                while (_iter.MoveNext())
                {
                    _iter.Current.ProcessMessage -= new MessageEventHandler(OnSubProcessMessage);
                }
                _iter.Dispose();
            }

            object System.Collections.IEnumerator.Current => _iter.Current;

            private string _nextAction;

            public bool MoveNext()
            {
                bool res = false;
                if (_nextAction == null)
                {
                    res = _iter.MoveNext();
                }
                else
                {
                    //Store the current position so that in the event that we can't find
                    //the specified next action to execute, we know where to continue from
                    var current = _iter.Current;

                    //The previous execution specified the next action to execute
                    //So find that process. If we can't find it, resume from the
                    //current process
                    _iter.Reset();
                    bool found = false;
                    while (_iter.MoveNext() && !found)
                    {
                        var proc = _iter.Current;
                        if (proc.Definition.Name.Equals(_nextAction))
                        {
                            found = true;
                            break;
                        }
                    }

                    if (found)
                    {
                        res = true;
                    }
                    else
                    {
                        _parent.SendMessage("The specified operation (" + _nextAction + ") was not found. Moving to next operation in sequence");
                        _iter.Reset();
                        while (_iter.MoveNext())
                        {
                            if (_iter.Current == current)
                                break;
                        }
                        //Move to the next one after the current
                        res = _iter.MoveNext();
                    }
                }
                if (res)
                {
                    var proc = _iter.Current;

                    var op = proc.Definition;
                    _parent.SendMessage("Starting FdoUtil command: " + op.Command);
                    int result = proc.Execute();
                    _nextAction = proc.GetNextAction(result);

                    if (_nextAction == null)
                    {
                        if (result != (int)CommandStatus.E_OK)
                        {
                            _parent.LogSubProcessErrors(proc.Name, new Exception[] { new InvokeProcessException("Command: " + op.Command + " exited with non-zero code " + result) });
                            if (op.AbortProcessOnFailure)
                            {
                                _parent.SendMessage("FdoUtil command " + op.Command + " failed. Aborting process");
                                //Go straight to end
                                while (_iter.MoveNext()) { }
                                return false;
                            }
                        }
                        else
                        {
                            _parent.SendMessage("No registered jump detected for given return code. Moving to next operation in sequence");
                        }
                    }
                    else
                    {
                        _parent.SendMessage("The next action to execute is: " + _nextAction);
                    }
                    return res;
                }
                return false;
            }

            void OnSubProcessMessage(object sender, MessageEventArgs e)
            {
                _parent.SendMessage(e.Message);
            }

            public void Reset()
            {
                _iter.Reset();
            }
        }

        /// <summary>
        /// Executes this process
        /// </summary>
        public override void Execute()
        {
            Initialize();
            if (execute)
            {
                using (var iter = new ExternalProcessIterator(this, _processes))
                {
                    while (iter.MoveNext()) { }
                }
            }
        }

        private Dictionary<string, List<Exception>> subProcessErrors = new Dictionary<string, List<Exception>>();

        /// <summary>
        /// Logs all the sub-process errors.
        /// </summary>
        /// <param name="procName">Name of the process.</param>
        /// <param name="errors">The errors.</param>
        private void LogSubProcessErrors(string procName, IEnumerable<Exception> errors)
        {
            if (!subProcessErrors.ContainsKey(procName))
                subProcessErrors[procName] = new List<Exception>();

            subProcessErrors[procName].AddRange(errors);
        }

        /// <summary>
        /// Gets all errors that occured during the execution of this process
        /// </summary>
        /// <returns></returns>
        public override IEnumerable<Exception> GetAllErrors()
        {
            foreach (string key in subProcessErrors.Keys)
            {
                foreach (Exception ex in subProcessErrors[key])
                {
                    yield return ex;
                }
            }
        }

        /// <summary>
        /// Gets the file extension associated with this process. For tasks where <see cref="CanSave"/> is
        /// false, an empty string is returned
        /// </summary>
        /// <returns></returns>
        public override string GetFileExtension()
        {
            return TaskDefinitionHelper.SEQUENTIALPROCESS;
        }

        /// <summary>
        /// Gets a description of this process
        /// </summary>
        /// <returns></returns>
        public override string GetDescription()
        {
            return "Sequential Process Definition";
        }

        /// <summary>
        /// Saves this process to a file
        /// </summary>
        /// <param name="file">The file to save this process to</param>
        /// <param name="name">The name of the process</param>
        public override void Save(string file, string name)
        {
            using (var fs = new FileStream(file, FileMode.OpenOrCreate))
            {
                SequentialProcessDefinition.Serializer.Serialize(fs, ProcessDefinition);
            }
        }
    }
}
