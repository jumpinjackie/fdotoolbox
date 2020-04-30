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
using System.Linq;
using System.Windows.Forms;

using FdoToolbox.Base.Services;
using FdoToolbox.Core.Feature;
using FdoToolbox.Core.Utility;
using ICSharpCode.Core;
using OSGeo.FDO.Common.Io;
using OSGeo.FDO.Common.Xml;
using OSGeo.FDO.Schema;

namespace FdoToolbox.Base.Forms
{
    public partial class PartialSchemaSaveDialog : Form
    {
        const int IDX_CLASS = 0;
        const int IDX_FEATURECLASS = 1;
        const int IDX_KEY = 2;
        const int IDX_DATA = 3;
        const int IDX_GEOMETRY = 4;
        const int IDX_OBJECT = 5;
        const int IDX_ASSOCIATION = 6;
        const int IDX_RASTER = 7;

        private PartialSchemaSaveDialog()
        {
            InitializeComponent();
        }

        private FeatureSchema _schema;

        public PartialSchemaSaveDialog(FeatureSchema schema)
            : this()
        {
            _schema = FdoSchemaUtil.CloneSchema(schema); //Operate on a clone
            InitTree();
        }

        protected override void OnLoad(EventArgs e) 
        {
            btnSave.Enabled = CanSave();
            base.OnLoad(e);
        }

        private void InitTree()
        {
            //Pre-sort
            SortedList<string, ClassDefinition> sorted = new SortedList<string, ClassDefinition>();
            foreach (ClassDefinition cls in _schema.Classes)
                sorted.Add(cls.Name, cls);

            foreach (ClassDefinition cls in sorted.Values)
            {
                TreeNode clsNode = new TreeNode
                {
                    Name = cls.Name,
                    Text = cls.Name,
                    Checked = true
                };

                if (cls.ClassType == ClassType.ClassType_FeatureClass)
                    clsNode.ImageIndex = clsNode.SelectedImageIndex = IDX_FEATURECLASS;
                else
                    clsNode.ImageIndex = clsNode.SelectedImageIndex = IDX_CLASS;

                //Pre-sort
                SortedList<string, PropertyDefinition> psorted = new SortedList<string, PropertyDefinition>();
                foreach (PropertyDefinition pd in cls.Properties)
                    psorted.Add(pd.Name, pd);

                foreach (PropertyDefinition pd in psorted.Values)
                {
                    TreeNode propNode = new TreeNode
                    {
                        Name = pd.Name,
                        Text = pd.Name,
                        Checked = true
                    };

                    switch (pd.PropertyType)
                    {
                        case PropertyType.PropertyType_AssociationProperty:
                            propNode.ImageIndex = propNode.SelectedImageIndex = IDX_ASSOCIATION;
                            break;
                        case PropertyType.PropertyType_DataProperty:
                            if (cls.IdentityProperties.Contains(pd.Name))
                                propNode.ImageIndex = propNode.SelectedImageIndex = IDX_KEY;
                            else
                                propNode.ImageIndex = propNode.SelectedImageIndex = IDX_DATA;
                            break;
                        case PropertyType.PropertyType_GeometricProperty:
                            propNode.ImageIndex = propNode.SelectedImageIndex = IDX_GEOMETRY;
                            break;
                        case PropertyType.PropertyType_ObjectProperty:
                            propNode.ImageIndex = propNode.SelectedImageIndex = IDX_OBJECT;
                            break;
                        case PropertyType.PropertyType_RasterProperty:
                            propNode.ImageIndex = propNode.SelectedImageIndex = IDX_RASTER;
                            break;
                    }

                    clsNode.Nodes.Add(propNode);
                }

                treeSchema.Nodes.Add(clsNode);
            }
        }

        private void btnCheckAll_Click(object sender, EventArgs e)
        {
            CheckAll(treeSchema.Nodes, true);
        }

        private static void CheckAll(TreeNodeCollection nodes, bool state)
        {
            foreach (TreeNode node in nodes)
            {
                node.Checked = state;
                if (node.Nodes.Count > 0)
                {
                    CheckAll(node.Nodes, state);
                }
            }
        }

        private void btnCheckNone_Click(object sender, EventArgs e)
        {
            CheckAll(treeSchema.Nodes, false);
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            var schema = _schema;
            //Remove elements that have been unchecked. 
            foreach (TreeNode clsNode in treeSchema.Nodes)
            {
                string className = clsNode.Name;
                int index = schema.Classes.IndexOf(className);
                if (!clsNode.Checked)
                {
                    if (index >= 0)
                        schema.Classes.RemoveAt(index);
                }
                else
                {
                    if (index >= 0)
                    {
                        ClassDefinition clsDef = schema.Classes[index];
                        foreach (TreeNode propNode in clsNode.Nodes)
                        {
                            if (!propNode.Checked)
                            {
                                string propName = propNode.Text;
                                int pidx = clsDef.Properties.IndexOf(propName);
                                if (pidx >= 0)
                                {
                                    clsDef.Properties.RemoveAt(pidx);
                                    if (clsDef.IdentityProperties.Contains(propName))
                                    {
                                        int idpdx = clsDef.IdentityProperties.IndexOf(propName);
                                        clsDef.IdentityProperties.RemoveAt(idpdx);
                                    }
                                    if (clsDef.ClassType == ClassType.ClassType_FeatureClass)
                                    {
                                        FeatureClass fc = (FeatureClass)clsDef;
                                        if (fc.GeometryProperty.Name == propName)
                                        {
                                            fc.GeometryProperty = null;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }

            if (rdXml.Checked)
            {
                using (var ios = new IoFileStream(txtXml.Text, "w"))
                {
                    using (var writer = new XmlWriter(ios, false, XmlWriter.LineFormat.LineFormat_Indent))
                    {
                        schema.WriteXml(writer);
                        writer.Close();
                    }
                    ios.Close();
                }
                
                MessageService.ShowMessage("Schema saved to: " + txtXml.Text);
                this.DialogResult = DialogResult.OK;
            }
            else if (rdFile.Checked)
            {
                string fileName = txtFile.Text;
                if (ExpressUtility.CreateFlatFileDataSource(fileName))
                {
                    FdoConnection conn = ExpressUtility.CreateFlatFileConnection(fileName);
                    bool disposeConn = true;
                    using (FdoFeatureService svc = conn.CreateFeatureService())
                    {
                        svc.ApplySchema(schema);
                        if (MessageService.AskQuestion("Schema saved to: " + txtFile.Text + " connect to it?", "Saved"))
                        {
                            FdoConnectionManager mgr = ServiceManager.Instance.GetService<FdoConnectionManager>();
                            string name = MessageService.ShowInputBox(ResourceService.GetString("TITLE_CONNECTION_NAME"), ResourceService.GetString("PROMPT_ENTER_CONNECTION"), "");
                            if (name == null)
                                return;

                            while (name == string.Empty || mgr.NameExists(name))
                            {
                                MessageService.ShowError(ResourceService.GetString("ERR_CONNECTION_NAME_EMPTY_OR_EXISTS"));
                                name = MessageService.ShowInputBox(ResourceService.GetString("TITLE_CONNECTION_NAME"), ResourceService.GetString("PROMPT_ENTER_CONNECTION"), name);

                                if (name == null)
                                    return;
                            }
                            disposeConn = false;
                            mgr.AddConnection(name, conn);
                        }
                    }
                    if (disposeConn)
                    {
                        conn.Close();
                        conn.Dispose();
                    }
                    this.DialogResult = DialogResult.OK;
                }
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
        }

        private bool CanSave()
        {
            if (rdXml.Checked)
            {
                return !string.IsNullOrEmpty(txtXml.Text);
            }
            else if (rdFile.Checked)
            {
                return !string.IsNullOrEmpty(txtFile.Text);
            }
            return false;
        }

        private void treeSchema_AfterCheck(object sender, TreeViewEventArgs e)
        {
            //Uncheck all children if unchecking class node
            if (!e.Node.Checked)
            {
                if (e.Node.Level == 0) //Class
                {
                    foreach (TreeNode pNode in e.Node.Nodes)
                    {
                        pNode.Checked = false;
                    }
                }
            }
            else 
            {
                if (e.Node.Level == 1 && !e.Node.Parent.Checked) //Property
                {
                    e.Node.Parent.Checked = true;   
                }
            }

            btnSave.Enabled = CanSave();
        }

        const string MSG_ORPHANED_ASSOCIATIONS = "You are excluding a class or property that other association properties depend on. Excluding this element will remove these association properties (re-checking will not restore them!). Do you want to continue?";
        const string MSG_ORPHANED_CONSTRAINTS = "You are excluding a property that other constraints depend on. Excluding this property will remove these affected constraints (re-checking will not restore them!). Do you want to continue?";
        
        private void treeSchema_BeforeCheck(object sender, TreeViewCancelEventArgs e)
        {
            if (e.Node.Checked)
            {
                if (e.Node.Level == 0) //Class
                {
                    //Check for associations
                    string className = e.Node.Name;
                    var affectedProperties = GetAffectedAssociationProperties(className);
                    var affectedClasses = GetAffectedClasses(className);

                    if (affectedProperties.Length > 0 || affectedClasses.Length > 0)
                    {
                        string msg = "You are un-checking a class that {0} association properties and {1} class definitions depend on. Un-checking this class will remove these association properties and make the affected class definition not derived from this class (re-checking will not restore these changes). Do you want to continue?";
                        if (!MessageService.AskQuestion(MSG_ORPHANED_ASSOCIATIONS))
                        {
                            e.Cancel = true;
                            return;
                        }
                        else
                        {
                            int removedProperties = 0;
                            int alteredClasses = 0;

                            if (affectedProperties.Length > 0)
                                removedProperties = PurgeAffectedAssocationProperties(affectedProperties);
                            if (affectedClasses.Length > 0)
                                alteredClasses = DetachAffectedClasses(affectedClasses);
                            MessageService.ShowMessage(removedProperties + " affected association properties removed and " + alteredClasses + " class definitions altered");
                        }
                    }
                }
                else if (e.Node.Level == 1) //Property
                {
                    string propName = e.Node.Name;
                    string className = e.Node.Parent.Name;

                    var affectedProperties = GetAffectedAssociationProperties(className);
                    var affectedConstraints = GetAffectedConstraints(className, propName);
                    
                    if (affectedProperties.Length > 0 || affectedConstraints.Length > 0)
                    {
                        string msg = "You are un-checking a property that {0} association properties and {1} unique constraints depend on. By un-checking this property, these affected elements will be removed or modified (re-checking will not restore these changes. Do you want to continue?";
                        msg = string.Format(msg, affectedProperties.Length, affectedConstraints.Length);
                        if (!MessageService.AskQuestion(msg))
                        {
                            e.Cancel = true;
                            return;
                        }
                        else
                        {
                            int removedProperties = 0;
                            int removedConstraints = 0;

                            if (affectedProperties.Length > 0)
                                removedProperties = PurgeAffectedAssocationProperties(affectedProperties);

                            if (affectedConstraints.Length > 0)
                                removedConstraints = PurgeAffectedConstraints(className, affectedConstraints);
                            MessageService.ShowMessage(removedProperties + " association properties and " + removedConstraints + " unique constraints removed");
                        }
                    }
                    
                }
            }
        }

        private void RemoveNode(string className, string propertyName)
        {
            treeSchema.Nodes[className].Nodes.RemoveByKey(propertyName);
        }

        private int PurgeAffectedAssocationProperties(AssociationPropertyDefinition[] properties)
        {
            int removed = 0;
            for (int i = 0; i < properties.Length; i++)
            {
                var ap = properties[i];
                System.Diagnostics.Debug.Assert(ap.Parent != null);

                var cls = (ClassDefinition)ap.Parent;
                cls.Properties.Remove(ap);
                RemoveNode(cls.Name, ap.Name); //Sync UI

                removed++;
            }
            return removed;
        }

        private int PurgeAffectedConstraints(string className, UniqueConstraint[] constraints)
        {
            if (constraints == null || constraints.Count() == 0)
                return 0;

            int removed = 0;
            var cls = _schema.Classes.Cast<ClassDefinition>().Where(c => c.Name == className).FirstOrDefault();
            if (cls != null)
            {
                foreach (var uc in constraints)
                {
                    cls.UniqueConstraints.Remove(uc);
                    removed++;
                }
            }
            return removed;
        }

        private int DetachAffectedClasses(ClassDefinition[] classes)
        {
            int detached = 0;
            //Make these class definitions the topmost classes in whatever inheritance hierarchy
            for (int i = 0; i < classes.Length; i++)
            {
                var cls = classes[i];
                if (cls.BaseClass == null)
                    continue;

                cls.BaseClass = null;
                //cls.SetBaseProperties(null);
                //You're now top dog
                cls.IsAbstract = false;
                detached++;
            }
            return detached;
        }

        private UniqueConstraint[] GetAffectedConstraints(string className, string propName)
        {
            //Find all unique constraints that contain said property
            var uniq = new List<UniqueConstraint>();
            var cls = _schema.Classes.Cast<ClassDefinition>().Where(c => c.Name == className).FirstOrDefault();
            if (cls != null)
            {
                var constraints = cls.UniqueConstraints.Cast<UniqueConstraint>().Where(uc =>
                        uc.Properties.IndexOf(propName) >= 0);
                uniq.AddRange(constraints);
            }
            return uniq.ToArray();
        }

        private ClassDefinition[] GetAffectedClasses(string className)
        {
            //If specified class is a base class, find affected derived classes
            return _schema.Classes.Cast<ClassDefinition>().Where(c =>
                c.BaseClass != null && c.BaseClass.Name == className).ToArray();
        }

        private AssociationPropertyDefinition[] GetAffectedAssociationProperties(string className)
        {
            // The miracle that is LINQ has allowed me to express in 10 lines of code what
            // would've probably taken at least 100 lines in C# 2.0
            //
            // Basically we are accumulating all the association properties that contain the
            // affected class definition using the Aggregate() extension method and returning
            // the accumulated result.

            var ap = new List<AssociationPropertyDefinition>();
            var cls = _schema.Classes.Cast<ClassDefinition>().Aggregate((a, b) =>
            {
                ap.AddRange(b.Properties.Cast<PropertyDefinition>()
                    .Where(p => p.PropertyType == PropertyType.PropertyType_AssociationProperty &&
                            ((AssociationPropertyDefinition)p).AssociatedClass.Name == className)
                    .Cast<AssociationPropertyDefinition>());
                return b;
            });
            return ap.ToArray();
        }

        private void btnBrowseXml_Click(object sender, EventArgs e)
        {
            SaveFileDialog diag = new SaveFileDialog
            {
                Filter = "FDO Feature Schemas (*.schema)|*.schema",
                FileName = _schema.Name + ".schema"
            };
            if (diag.ShowDialog() == DialogResult.OK)
            {
                txtXml.Text = diag.FileName;
            }
        }

        private void btnBrowseFile_Click(object sender, EventArgs e)
        {
            SaveFileDialog diag = new SaveFileDialog
            {
                Filter = "SDF Files (*.sdf)|*.sdf|SQLite files (*.db;*.sqlite;*.slt)|*.db;*.sqlite;*.slt",
                FileName = _schema.Name
            };
            if (diag.ShowDialog() == DialogResult.OK)
            {
                txtFile.Text = diag.FileName;
            }
        }

        private void txtXml_TextChanged(object sender, EventArgs e)
        {
            btnSave.Enabled = CanSave();
        }

        private void txtFile_TextChanged(object sender, EventArgs e)
        {
            btnSave.Enabled = CanSave();
        }

        private void rdXml_CheckedChanged(object sender, EventArgs e)
        {
            btnSave.Enabled = CanSave();
        }

        private void rdFile_CheckedChanged(object sender, EventArgs e)
        {
            btnSave.Enabled = CanSave();
        }
    }
}