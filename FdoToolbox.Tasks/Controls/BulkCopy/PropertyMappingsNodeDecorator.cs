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
using OSGeo.FDO.Schema;
using System.Diagnostics;
using System.Drawing;
using ICSharpCode.Core;
using FdoToolbox.Core.Feature;
using System.Collections.Specialized;
using System.Collections;

namespace FdoToolbox.Tasks.Controls.BulkCopy
{
    /// <summary>
    /// Helper class to perform manipulation of the tree nodes
    /// </summary>
    internal class PropertyMappingsNodeDecorator
    {
        private TreeView _treeView;

        private TreeNode _node;

        internal readonly CopyTaskNodeDecorator Parent;

        private Dictionary<string, PropertyConversionNodeDecorator> _conversionOptions;

        private ContextMenuStrip _selectedPropertyMenu;

        internal PropertyMappingsNodeDecorator(CopyTaskNodeDecorator parent, TreeNode mappingsNode)
        {
            Parent = parent;
            _node = mappingsNode;
            _treeView = _node.TreeView;
            _selectedPropertyMenu = BuildSelectedPropertyContextMenu(Parent.TargetClass, OnMapProperty, OnRemovePropertyMapping, OnMapAutoCreate);
            _conversionOptions = new Dictionary<string, PropertyConversionNodeDecorator>();
            SortedList<string, PropertyDefinition> props = new SortedList<string, PropertyDefinition>();
            foreach (PropertyDefinition pd in Parent.SourceClass.Properties)
            {
                if (pd.PropertyType == PropertyType.PropertyType_DataProperty || pd.PropertyType == PropertyType.PropertyType_GeometricProperty)
                {
                    props.Add(pd.Name, pd);
                }
            }
            foreach (PropertyDefinition prop in props.Values)
            {
                this.AddProperty(prop);
            }

            _node.ContextMenuStrip = new ContextMenuStrip();
            var autoMapItem = _node.ContextMenuStrip.Items.Add("Auto-Map", null, OnAutoMap);
            autoMapItem.ToolTipText = "Maps each un-mapped property to a property of the same name (will be created if doesn't exist)";

            var unMapAllItem = _node.ContextMenuStrip.Items.Add("Un-map all", null, OnUnmapAll);
            unMapAllItem.ToolTipText = "Removes all property mappings";
        }

        private void OnUnmapAll(object sender, EventArgs e)
        {
            foreach (TreeNode node in _node.Nodes)
            {
                try
                {
                    this.RemoveMapping(node.Name);
                }
                catch
                {

                }
            }
        }

        internal void OnAutoMap(object sender, EventArgs e)
        {
            foreach (TreeNode node in _node.Nodes)
            {
                if (node.Tag == null)
                {
                    try
                    {
                        this.MapProperty(node.Name, node.Name, true);
                    }
                    catch (MappingException ex) //Target may not exist, so let it be
                    {
                        LoggingService.Info("Skipping mapping " + node.Name + " -> " + node.Name + ": " + ex.Message);
                    } 
                }
            }
        }

        public NameValueCollection GetPropertyMappings()
        {
            NameValueCollection nvc = new NameValueCollection();
            foreach (TreeNode node in _node.Nodes)
            {
                if (node.Tag != null)
                    nvc[node.Name] = node.Tag.ToString();
            }
            return nvc;
        }

        private void OnMapAutoCreate(object sender, EventArgs e)
        {
            ToolStripItem itm = sender as ToolStripItem;
            Debug.Assert(itm != null);
            TreeNode propertyNode = _treeView.SelectedNode;
            try
            {
                this.MapProperty(propertyNode.Name, propertyNode.Name, true);
            }
            catch(MappingException ex)
            {
                MessageService.ShowMessage(ex.Message);
            }
        }

        private void OnRemovePropertyMapping(object sender, EventArgs e)
        {
            ToolStripItem itm = sender as ToolStripItem;
            Debug.Assert(itm != null);

            TreeNode propertyNode = _treeView.SelectedNode;
            TreeNode taskNode = propertyNode.Parent.Parent;
            this.RemoveMapping(propertyNode.Name);
        }

        private void OnMapProperty(object sender, EventArgs e)
        {
            ToolStripItem itm = sender as ToolStripItem;
            Debug.Assert(itm != null);
            Debug.Assert(itm.Tag != null);

            TreeNode propertyNode = _treeView.SelectedNode;
            TreeNode taskNode = propertyNode.Parent.Parent;
            try
            {
                this.MapProperty(propertyNode.Name, itm.Tag.ToString(), false);
            }
            catch (MappingException ex)
            {
                MessageService.ShowMessage(ex.Message);
            }
        }

        private static ContextMenuStrip BuildSelectedPropertyContextMenu(ClassDefinition cd, EventHandler mapHandler, EventHandler removeMappingHandler, EventHandler autoCreateHandler)
        {
            ContextMenuStrip ctxSelectedProperty = new ContextMenuStrip();
            ctxSelectedProperty.Items.Add("Remove Mapping", ResourceService.GetBitmap("cross"), removeMappingHandler);
            ctxSelectedProperty.Items.Add(new ToolStripSeparator());
            ctxSelectedProperty.Items.Add("Map to property of same name (create if necessary)", null, autoCreateHandler);
            ctxSelectedProperty.Items.Add(new ToolStripSeparator());

            SortedList<string, ToolStripMenuItem> items = new SortedList<string, ToolStripMenuItem>();

            if (cd != null)
            {
                foreach (PropertyDefinition p in cd.Properties)
                {
                    if (p.PropertyType == PropertyType.PropertyType_DataProperty || p.PropertyType == PropertyType.PropertyType_GeometricProperty)
                    {
                        DataPropertyDefinition d = p as DataPropertyDefinition;
                        GeometricPropertyDefinition g = p as GeometricPropertyDefinition;
                        string name = p.Name;
                        string text = "Map to property: " + name;
                        Image icon = null;
                        if (d != null)
                        {
                            if (d.IsAutoGenerated || d.ReadOnly)
                                continue;

                            icon = ResourceService.GetBitmap("table");
                        }
                        else if (g != null)
                        {
                            if (g.ReadOnly)
                                continue;

                            icon = ResourceService.GetBitmap("shape_handles");
                        }
                        ToolStripMenuItem itm1 = new ToolStripMenuItem(text, icon, mapHandler)
                        {
                            Tag = name
                        };
                        items.Add(name, itm1);
                    }
                }
            }
            foreach (ToolStripMenuItem item in items.Values)
            {
                ctxSelectedProperty.Items.Add(item);
            }
            return ctxSelectedProperty;
        }

        public void AddProperty(PropertyDefinition prop)
        {
            TreeNode p = new TreeNode(prop.Name + " (Unmapped)")
            {
                Name = prop.Name,
                ContextMenuStrip = _selectedPropertyMenu,
                ToolTipText = prop.Name + " (" + prop.PropertyType + ")" + Environment.NewLine + prop.Description
            };
            _node.Nodes.Add(p);
            _conversionOptions[p.Name] = new PropertyConversionNodeDecorator(p);
        }

        static (PropertyDefinition prop, string name) FindProperty(ClassDefinition cls, string name, bool caseSensitive)
        {
            var clsProps = cls.Properties;
            if (caseSensitive)
            {
                var pidx = clsProps.IndexOf(name);
                return (clsProps[pidx], name);
            }
            else
            {
                foreach (PropertyDefinition p in clsProps)
                {
                    if (string.Equals(p.Name, name, StringComparison.OrdinalIgnoreCase))
                    {
                        return (p, p.Name);
                    }
                }
                return (null, null);
            }
        }

        public void MapProperty(string propertyName, string destProperty, bool createIfNotExists)
        {
            var srcCls = Parent.SourceClass;
            var dstCls = Parent.TargetClass;
            string targetProperty = destProperty;

            LoggingService.Info(string.Format("Mapping {0} to {1} (create if not exists: {2})", propertyName, destProperty, createIfNotExists));

            //PropertyDefinition src = srcCls.Properties[propertyName];
            var (src, _) = FindProperty(srcCls, propertyName, true);
            if (dstCls != null)
            {
                //if (!dstCls.Properties.Contains(destProperty))
                //    throw new MappingException("Target property " + destProperty + " not found");

                //PropertyDefinition dst = dstCls.Properties[destProperty];
                var (dst, tp) = FindProperty(dstCls, destProperty, false);
                if (dst == null)
                {
                    throw new MappingException("Target property " + destProperty + " not found");
                }
                targetProperty = tp;
                if (src.PropertyType == dst.PropertyType)
                {
                    if (src.PropertyType == PropertyType.PropertyType_AssociationProperty ||
                        src.PropertyType == PropertyType.PropertyType_ObjectProperty ||
                        src.PropertyType == PropertyType.PropertyType_RasterProperty)
                    {
                        throw new MappingException("Association, Object and Raster properties are not mappable");
                    }

                    if (src.PropertyType == PropertyType.PropertyType_DataProperty)
                    {
                        DataType sdt = ((DataPropertyDefinition)src).DataType;
                        DataType ddt = ((DataPropertyDefinition)dst).DataType;
                        if (!ValueConverter.IsConvertible(sdt, ddt))
                        {
                            throw new MappingException("Unable to map source property to " + dst.Name + ". The source data type " + sdt + " is not convertible to " + ddt);
                        }

                        if (((DataPropertyDefinition)dst).IsAutoGenerated)
                        {
                            throw new MappingException("Unable to map source property to " + dst.Name + ". The target property auto-generates values on insert");
                        }
                    }

                    if (src.PropertyType == PropertyType.PropertyType_GeometricProperty)
                    {
                        //Geometry checks?
                    }
                }
                else
                {
                    throw new MappingException("Cannot map source property " + src.Name + " to " + dst.Name + ". They are different property types");
                }
            }
            else
            {
                if (!createIfNotExists)
                    throw new MappingException("Cannot map source property " + src.Name + " to " + destProperty + ". The target property doesn't exist and the \"create if necessary\" option was not specified");
            }

            TreeNode propNode = _node.Nodes[propertyName];
            propNode.Text = propertyName + " ( => " + targetProperty + " )";
            propNode.Tag = targetProperty;

            GetConversionRule(propertyName).CreateIfNotExists = createIfNotExists;
        }

        public void RemoveMapping(string propertyName)
        {
            TreeNode propNode = _node.Nodes[propertyName];
            propNode.Text = propertyName + " (Unmapped)";
            propNode.Tag = null;
        }

        internal PropertyConversionNodeDecorator GetConversionRule(string p)
        {
            return _conversionOptions[p];
        }
    }
}
