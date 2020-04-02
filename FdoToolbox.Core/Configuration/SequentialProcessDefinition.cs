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
using System.Xml.Serialization;
using System.IO;
using System.ComponentModel;

namespace FdoToolbox.Core.Configuration
{
    /// <summary>
    /// Defines a sequence of FdoUtil.exe calls
    /// </summary>
    [Serializable]
    public class SequentialProcessDefinition
    {
        [XmlIgnore]
        private static XmlSerializer _ser;

        [XmlIgnore]
        public static XmlSerializer Serializer
        {
            get
            {
                if (_ser == null)
                    _ser = new XmlSerializer(typeof(SequentialProcessDefinition));

                return _ser;
            }
        }
        /// <summary>
        /// Gets the variables that can be referenced by child operations
        /// </summary>
        [XmlElement(ElementName = "Variable")]
        public BindingList<ProcessVariable> Variables { get; set; }

        /// <summary>
        /// Gets the sequence of FdoUtil.exe calls
        /// </summary>
        [XmlElement(ElementName = "SequentialOperation")]
        public BindingList<SequentialOperation> Operations { get; set; }

        public void AddVariable(string name, string value)
        {
            var v = new ProcessVariable() { Name = name, Value = value };
            if (this.Variables == null)
                this.Variables = new BindingList<ProcessVariable>();

            this.Variables.Add(v);
        }

        public void AddOperation(SequentialOperation op)
        {
            if (this.Operations == null)
                this.Operations = new BindingList<SequentialOperation>();

            this.Operations.Add(op);
        }

        public void ClearOperations()
        {
            if (this.Operations != null)
                this.Operations.Clear();
        }

        public void Save(string file)
        {
            using (var fs = File.OpenWrite(file))
            {
                Serializer.Serialize(fs, this);
            }
        }
    }

    /// <summary>
    /// Defines a variable that can be referenced by any child operation
    /// </summary>
    [Serializable]
    public class ProcessVariable
    {
        /// <summary>
        /// Gets or sets the name of the variable. Child operations can access
        /// this variable by using %name%
        /// </summary>
        [XmlAttribute(AttributeName = "name")]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the value of the variable
        /// </summary>
        [XmlAttribute(AttributeName = "value")]
        public string Value { get; set; }
    }

    /// <summary>
    /// Defines a blocking call to FdoUtil.exe
    /// </summary>
    [Serializable]
    public class SequentialOperation
    {
        /// <summary>
        /// Gets or sets the name of the operation
        /// </summary>
        [XmlAttribute(AttributeName = "name")]
        public string Name { get; set; }

        /// <summary>
        /// Gets or set the FdoUtil.exe command
        /// </summary>
        [XmlAttribute(AttributeName = "command")]
        [ReadOnly(true)]
        public string Command { get; set; }

        /// <summary>
        /// Gets or sets additional arguments for this operation
        /// </summary>
        [XmlElement(ElementName = "OperationArgument")]
        public BindingList<SequentialOperationArgument> Arguments { get; set; }

        /// <summary>
        /// Gets or sets the branching conditions of this operation. If none of the
        /// conditions are satisfied here, the next operation in sequence will be
        /// executed
        /// </summary>
        [XmlElement(ElementName = "CompleteAction")]
        public BindingList<OnCompleteAction> CompleteActions { get; set; }

        public void AddArgument(string name, string value)
        {
            if (this.Arguments == null)
                this.Arguments = new BindingList<SequentialOperationArgument>();

            this.Arguments.Add(new SequentialOperationArgument() { Name = name, Value = value });
        }

        public void AddCompleteAction(int returnCode, string operation)
        {
            if (this.CompleteActions == null)
                this.CompleteActions = new BindingList<OnCompleteAction>();

            this.CompleteActions.Add(new OnCompleteAction() { ReturnCode = returnCode, Operation = operation });
        }

        /// <summary>
        /// Gets or sets whether the sequential process is aborted if this operation
        /// returns a failure result
        /// </summary>
        [XmlAttribute]
        public bool AbortProcessOnFailure { get; set; }

        public override string ToString()
        {
            return string.Format("Command: {0}, AbortOnFailure: {1}", this.Command, this.AbortProcessOnFailure);
        }
    }

    /// <summary>
    /// Defines the next operation to execute on completion of the current one
    /// </summary>
    public class OnCompleteAction
    {
        /// <summary>
        /// Gets or sets the return code to check for
        /// </summary>
        [XmlAttribute(AttributeName = "returnCode")]
        public int ReturnCode { get; set; }

        /// <summary>
        /// Gets or sets the name of the operation to execute if the current operation
        /// signals the specified return code
        /// </summary>
        [XmlAttribute(AttributeName = "operation")]
        public string Operation { get; set; }
    }

    /// <summary>
    /// Defines an argument of FdoUtil.exe
    /// </summary>
    [Serializable]
    public class SequentialOperationArgument
    {
        /// <summary>
        /// Gets or sets the argument name
        /// </summary>
        [XmlAttribute(AttributeName = "name")]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the argument value
        /// </summary>
        [XmlAttribute(AttributeName = "value")]
        public string Value { get; set; }

        /// <summary>
        /// Gets the argument value with any variable references
        /// substituted for actual values
        /// </summary>
        /// <param name="variables"></param>
        /// <returns></returns>
        internal string GetProcessedValue(IEnumerable<ProcessVariable> variables)
        {
            StringBuilder val = new StringBuilder(this.Value);
            foreach (var v in variables)
            {
                string token = "%" + v.Name + "%";
                val.Replace(token, v.Value);
            }
            return val.ToString();
        }

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return string.Format("-{0}:{1}", this.Name, this.Value);
        }
    }
}
