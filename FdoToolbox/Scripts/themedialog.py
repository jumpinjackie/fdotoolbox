"""
Copyright (C) 2009, Jackie Ng
https://github.com/jumpinjackie/fdotoolbox, jumpinjackie@gmail.com

This library is free software; you can redistribute it and/or
modify it under the terms of the GNU Lesser General Public
License as published by the Free Software Foundation; either
version 2.1 of the License, or (at your option) any later version.

This library is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public
License along with this library; if not, write to the Free Software
Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301  USA

See license.txt for more/additional licensing information
"""

import clr

clr.AddReference("System.Drawing")

import System
import System.Drawing
import System.Windows.Forms
import System.Collections.Generic

from System import String
from System.Drawing import *
from System.Windows.Forms import *
from System.Collections.Generic import List, SortedList
from OSGeo.FDO.Schema import PropertyType
from OSGeo.FDO.Commands import CommandType
from FdoToolbox.Core.Feature import FeatureQueryOptions, FeatureAggregateOptions, CapabilityType

class ThemeOptionsDialog(Form):
	def __init__(self, connMgr):
		self.InitializeComponent()
		self._connMgr = connMgr
	
	def InitializeComponent(self):
		self._label1 = System.Windows.Forms.Label()
		self._cmbConnection = System.Windows.Forms.ComboBox()
		self._groupBox1 = System.Windows.Forms.GroupBox()
		self._groupBox2 = System.Windows.Forms.GroupBox()
		self._label3 = System.Windows.Forms.Label()
		self._cmbSchema = System.Windows.Forms.ComboBox()
		self._label4 = System.Windows.Forms.Label()
		self._cmbClass = System.Windows.Forms.ComboBox()
		self._groupBox3 = System.Windows.Forms.GroupBox()
		self._label2 = System.Windows.Forms.Label()
		self._txtSdf = System.Windows.Forms.TextBox()
		self._btnBrowse = System.Windows.Forms.Button()
		self._label5 = System.Windows.Forms.Label()
		self._cmbProperty = System.Windows.Forms.ComboBox()
		self._label6 = System.Windows.Forms.Label()
		self._label7 = System.Windows.Forms.Label()
		self._txtClassPrefix = System.Windows.Forms.TextBox()
		self._txtClassSuffix = System.Windows.Forms.TextBox()
		self._btnPreview = System.Windows.Forms.Button()
		self._grdRules = System.Windows.Forms.DataGridView()
		self._btnOK = System.Windows.Forms.Button()
		self._btnCancel = System.Windows.Forms.Button()
		self._COL_VALUE = System.Windows.Forms.DataGridViewTextBoxColumn()
		self._COL_CLASS = System.Windows.Forms.DataGridViewTextBoxColumn()
		self._COL_FILTER = System.Windows.Forms.DataGridViewTextBoxColumn()
		self._groupBox1.SuspendLayout()
		self._groupBox2.SuspendLayout()
		self._groupBox3.SuspendLayout()
		self._grdRules.BeginInit()
		self.SuspendLayout()
		# 
		# label1
		# 
		self._label1.Location = System.Drawing.Point(18, 27)
		self._label1.Name = "label1"
		self._label1.Size = System.Drawing.Size(100, 14)
		self._label1.TabIndex = 0
		self._label1.Text = "Connection"
		# 
		# cmbConnection
		# 
		self._cmbConnection.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
		self._cmbConnection.FormattingEnabled = True
		self._cmbConnection.Location = System.Drawing.Point(18, 44)
		self._cmbConnection.Name = "cmbConnection"
		self._cmbConnection.Size = System.Drawing.Size(212, 21)
		self._cmbConnection.TabIndex = 1
		self._cmbConnection.SelectedIndexChanged += self.CmbConnectionSelectedIndexChanged
		# 
		# groupBox1
		# 
		self._groupBox1.Controls.Add(self._label4)
		self._groupBox1.Controls.Add(self._cmbClass)
		self._groupBox1.Controls.Add(self._label3)
		self._groupBox1.Controls.Add(self._cmbSchema)
		self._groupBox1.Controls.Add(self._label1)
		self._groupBox1.Controls.Add(self._cmbConnection)
		self._groupBox1.Location = System.Drawing.Point(12, 13)
		self._groupBox1.Name = "groupBox1"
		self._groupBox1.Size = System.Drawing.Size(255, 159)
		self._groupBox1.TabIndex = 2
		self._groupBox1.TabStop = False
		self._groupBox1.Text = "Source"
		# 
		# groupBox2
		# 
		self._groupBox2.Controls.Add(self._btnBrowse)
		self._groupBox2.Controls.Add(self._txtSdf)
		self._groupBox2.Controls.Add(self._label2)
		self._groupBox2.Location = System.Drawing.Point(273, 13)
		self._groupBox2.Name = "groupBox2"
		self._groupBox2.Size = System.Drawing.Size(268, 159)
		self._groupBox2.TabIndex = 3
		self._groupBox2.TabStop = False
		self._groupBox2.Text = "Target"
		# 
		# label3
		# 
		self._label3.Location = System.Drawing.Point(18, 68)
		self._label3.Name = "label3"
		self._label3.Size = System.Drawing.Size(100, 14)
		self._label3.TabIndex = 2
		self._label3.Text = "Schema"
		# 
		# cmbSchema
		# 
		self._cmbSchema.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
		self._cmbSchema.FormattingEnabled = True
		self._cmbSchema.Location = System.Drawing.Point(18, 85)
		self._cmbSchema.Name = "cmbSchema"
		self._cmbSchema.Size = System.Drawing.Size(212, 21)
		self._cmbSchema.TabIndex = 3
		self._cmbSchema.SelectedIndexChanged += self.CmbSchemaSelectedIndexChanged
		# 
		# label4
		# 
		self._label4.Location = System.Drawing.Point(18, 109)
		self._label4.Name = "label4"
		self._label4.Size = System.Drawing.Size(100, 14)
		self._label4.TabIndex = 4
		self._label4.Text = "Class"
		# 
		# cmbClass
		# 
		self._cmbClass.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
		self._cmbClass.FormattingEnabled = True
		self._cmbClass.Location = System.Drawing.Point(18, 126)
		self._cmbClass.Name = "cmbClass"
		self._cmbClass.Size = System.Drawing.Size(212, 21)
		self._cmbClass.TabIndex = 5
		self._cmbClass.SelectedIndexChanged += self.CmbClassSelectedIndexChanged
		# 
		# groupBox3
		# 
		self._groupBox3.Controls.Add(self._grdRules)
		self._groupBox3.Controls.Add(self._btnPreview)
		self._groupBox3.Controls.Add(self._txtClassSuffix)
		self._groupBox3.Controls.Add(self._txtClassPrefix)
		self._groupBox3.Controls.Add(self._label7)
		self._groupBox3.Controls.Add(self._label6)
		self._groupBox3.Controls.Add(self._cmbProperty)
		self._groupBox3.Controls.Add(self._label5)
		self._groupBox3.Location = System.Drawing.Point(12, 179)
		self._groupBox3.Name = "groupBox3"
		self._groupBox3.Size = System.Drawing.Size(529, 173)
		self._groupBox3.TabIndex = 4
		self._groupBox3.TabStop = False
		self._groupBox3.Text = "Theme Settings"
		# 
		# label2
		# 
		self._label2.Location = System.Drawing.Point(17, 27)
		self._label2.Name = "label2"
		self._label2.Size = System.Drawing.Size(103, 14)
		self._label2.TabIndex = 0
		self._label2.Text = "SDF File"
		# 
		# txtSdf
		# 
		self._txtSdf.Location = System.Drawing.Point(17, 44)
		self._txtSdf.Name = "txtSdf"
		self._txtSdf.Size = System.Drawing.Size(211, 20)
		self._txtSdf.TabIndex = 1
		# 
		# btnBrowse
		# 
		self._btnBrowse.Location = System.Drawing.Point(234, 42)
		self._btnBrowse.Name = "btnBrowse"
		self._btnBrowse.Size = System.Drawing.Size(25, 23)
		self._btnBrowse.TabIndex = 2
		self._btnBrowse.Text = "..."
		self._btnBrowse.UseVisualStyleBackColor = True
		self._btnBrowse.Click += self.BtnBrowseClick
		# 
		# label5
		# 
		self._label5.Location = System.Drawing.Point(18, 22)
		self._label5.Name = "label5"
		self._label5.Size = System.Drawing.Size(100, 17)
		self._label5.TabIndex = 0
		self._label5.Text = "Theme Property"
		# 
		# cmbProperty
		# 
		self._cmbProperty.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
		self._cmbProperty.FormattingEnabled = True
		self._cmbProperty.Location = System.Drawing.Point(109, 19)
		self._cmbProperty.Name = "cmbProperty"
		self._cmbProperty.Size = System.Drawing.Size(411, 21)
		self._cmbProperty.TabIndex = 1
		# 
		# label6
		# 
		self._label6.Location = System.Drawing.Point(18, 51)
		self._label6.Name = "label6"
		self._label6.Size = System.Drawing.Size(120, 15)
		self._label6.TabIndex = 2
		self._label6.Text = "Generated Class Name"
		# 
		# label7
		# 
		self._label7.Location = System.Drawing.Point(227, 51)
		self._label7.Name = "label7"
		self._label7.Size = System.Drawing.Size(110, 15)
		self._label7.TabIndex = 3
		self._label7.Text = "[PROPERTY VALUE]"
		# 
		# txtClassPrefix
		# 
		self._txtClassPrefix.Location = System.Drawing.Point(155, 48)
		self._txtClassPrefix.Name = "txtClassPrefix"
		self._txtClassPrefix.Size = System.Drawing.Size(66, 20)
		self._txtClassPrefix.TabIndex = 4
		# 
		# txtClassSuffix
		# 
		self._txtClassSuffix.Location = System.Drawing.Point(343, 48)
		self._txtClassSuffix.Name = "txtClassSuffix"
		self._txtClassSuffix.Size = System.Drawing.Size(66, 20)
		self._txtClassSuffix.TabIndex = 5
		# 
		# btnPreview
		# 
		self._btnPreview.Location = System.Drawing.Point(445, 46)
		self._btnPreview.Name = "btnPreview"
		self._btnPreview.Size = System.Drawing.Size(75, 23)
		self._btnPreview.TabIndex = 6
		self._btnPreview.Text = "Preview"
		self._btnPreview.UseVisualStyleBackColor = True
		self._btnPreview.Click += self.BtnPreviewClick
		# 
		# grdRules
		# 
		self._grdRules.AllowUserToAddRows = False
		self._grdRules.AllowUserToDeleteRows = False
		self._grdRules.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
		self._grdRules.Columns.AddRange(System.Array[System.Windows.Forms.DataGridViewColumn](
			[self._COL_VALUE,
			self._COL_CLASS,
			self._COL_FILTER]))
		self._grdRules.Location = System.Drawing.Point(18, 75)
		self._grdRules.Name = "grdRules"
		self._grdRules.ReadOnly = True
		self._grdRules.RowHeadersVisible = False
		self._grdRules.Size = System.Drawing.Size(502, 92)
		self._grdRules.TabIndex = 7
		# 
		# btnOK
		# 
		self._btnOK.Location = System.Drawing.Point(385, 362)
		self._btnOK.Name = "btnOK"
		self._btnOK.Size = System.Drawing.Size(75, 23)
		self._btnOK.TabIndex = 5
		self._btnOK.Text = "OK"
		self._btnOK.UseVisualStyleBackColor = True
		self._btnOK.Click += self.BtnOKClick
		# 
		# btnCancel
		# 
		self._btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel
		self._btnCancel.Location = System.Drawing.Point(466, 362)
		self._btnCancel.Name = "btnCancel"
		self._btnCancel.Size = System.Drawing.Size(75, 23)
		self._btnCancel.TabIndex = 6
		self._btnCancel.Text = "Cancel"
		self._btnCancel.UseVisualStyleBackColor = True
		self._btnCancel.Click += self.BtnCancelClick
		# 
		# COL_VALUE
		# 
		self._COL_VALUE.DataPropertyName = "Value"
		self._COL_VALUE.HeaderText = "Property Value"
		self._COL_VALUE.Name = "COL_VALUE"
		self._COL_VALUE.ReadOnly = True
		# 
		# COL_CLASS
		# 
		self._COL_CLASS.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill
		self._COL_CLASS.DataPropertyName = "ClassName"
		self._COL_CLASS.HeaderText = "Generated Class Name"
		self._COL_CLASS.Name = "COL_CLASS"
		self._COL_CLASS.ReadOnly = True
		# 
		# COL_FILTER
		# 
		self._COL_FILTER.DataPropertyName = "Filter"
		self._COL_FILTER.HeaderText = "Source Filter"
		self._COL_FILTER.Name = "COL_FILTER"
		self._COL_FILTER.ReadOnly = True
		# 
		# Form1
		# 
		self.AcceptButton = self._btnOK
		self.CancelButton = self._btnCancel
		self.ClientSize = System.Drawing.Size(553, 397)
		self.ControlBox = False
		self.Controls.Add(self._btnCancel)
		self.Controls.Add(self._btnOK)
		self.Controls.Add(self._groupBox3)
		self.Controls.Add(self._groupBox2)
		self.Controls.Add(self._groupBox1)
		self.Name = "Form1"
		self.Text = "Theme Options"
		self.Load += self.ThemeOptionsDialogLoad
		self._groupBox1.ResumeLayout(False)
		self._groupBox2.ResumeLayout(False)
		self._groupBox2.PerformLayout()
		self._groupBox3.ResumeLayout(False)
		self._groupBox3.PerformLayout()
		self._grdRules.EndInit()
		self.ResumeLayout(False)

	def BtnCancelClick(self, sender, e):
		self.DialogResult = DialogResult.Cancel
		pass

	def BtnOKClick(self, sender, e):
		self.DialogResult = DialogResult.OK
		pass

	def ThemeOptionsDialogLoad(self, sender, e):
		sourceConns = List[str](self._connMgr.GetConnectionNames())
		if sourceConns.Count > 0:
			self._cmbConnection.DataSource = sourceConns
			self._cmbConnection.SelectedIndex = 0
		pass
		
	def GetConnection(self):
		if self._cmbConnection.SelectedItem is None:
			return None
		connName = self.GetConnectionName()
		conn = self._connMgr.GetConnection(connName)
		return conn
	
	def GetConnectionName(self):
		return self._cmbConnection.SelectedItem.ToString()
	
	def GetSchemaName(self):
		return self._cmbSchema.SelectedItem.ToString()
	
	def GetClassName(self):
		return self._cmbClass.SelectedItem.ToString()
		
	def GetPropertyName(self):
		return self._cmbProperty.SelectedItem.ToString()
		
	def GetThemeRules(self):
		return self._grdRules.DataSource
		
	def GetSdfFile(self):
		return self._txtSdf.Text

	def CmbConnectionSelectedIndexChanged(self, sender, e):
		conn = self.GetConnection()
		if conn is None:
			return
		svc = conn.CreateFeatureService()
		try:
			schemaNames = svc.GetSchemaNames()
			if schemaNames.Count > 0:
				self._cmbSchema.DataSource = schemaNames
				self._cmbSchema.SelectedIndex = 0
		finally:
			svc.Dispose()
		pass

	def CmbSchemaSelectedIndexChanged(self, sender, e):
		conn = self.GetConnection()
		if conn is None:
			return
		schemaName = self.GetSchemaName()
		if String.IsNullOrEmpty(schemaName):
			return
		svc = conn.CreateFeatureService()
		try:
			classNames = svc.GetClassNames(schemaName)
			if classNames.Count > 0:
				self._cmbClass.DataSource = classNames
				self._cmbClass.SelectedIndex = 0
		finally:
			svc.Dispose()
		pass

	def CmbClassSelectedIndexChanged(self, sender, e):
		conn = self.GetConnection()
		if conn is None:
			return
		schemaName = self.GetSchemaName()
		if String.IsNullOrEmpty(schemaName):
			return
		className = self.GetClassName()
		if String.IsNullOrEmpty(className):
			return
		svc = conn.CreateFeatureService()
		try:
			cls = svc.GetClassByName(schemaName, className)
			if not cls is None:
				properties = List[str]()
				for prop in cls.Properties:
					if prop.PropertyType == PropertyType.PropertyType_DataProperty:
						properties.Add(prop.Name)
				if properties.Count > 0:
					self._cmbProperty.DataSource = properties
					self._cmbProperty.SelectedIndex = 0
		finally:
			svc.Dispose()
		pass

	def BtnPreviewClick(self, sender, e):
		conn = self.GetConnection()
		if conn is None:
			return
		className = self.GetClassName()
		if String.IsNullOrEmpty(className):
			return
		propName = self.GetPropertyName()
		if String.IsNullOrEmpty(propName):
			return
		svc = conn.CreateFeatureService()
		propertyValues = List[str]()
		try:
		 	canDistinct = conn.Capability.GetBooleanCapability(CapabilityType.FdoCapabilityType_SupportsSelectDistinct)
		 	if canDistinct:
		 		#SortedList not only allows us to hackishly get set-like qualities, but we get sorting for free.
		 		values = SortedList[str, str]()
				query = FeatureAggregateOptions(className)
				query.AddFeatureProperty(propName)
				query.Distinct = True
				reader = svc.SelectAggregates(query)
				try:
					while reader.ReadNext():
						if not reader.IsNull(propName):
							values.Add(reader[propName].ToString(), String.Empty)
				finally:
					reader.Dispose()
				propertyValues.AddRange(values.Keys)
			elif svc.SupportsCommand(CommandType.CommandType_SQLCommand):
				sql = "SELECT DISTINCT " + propName + " FROM " + className + " ORDER BY " + propName
				values = List[str]()
				reader = svc.ExecuteSQLQuery(sql)
				try:
					while reader.ReadNext():
						if not reader.IsNull(propName):
							values.Add(reader[propName].ToString())
				finally:
					reader.Dispose()
				propertyValues.AddRange(values)
			else:
				if App.Ask("Get Values", "About to fetch distinct values by brute force. Continue?"):
					#SortedList not only allows us to hackishly get set-like qualities, but we get sorting for free.
		 			values = SortedList[str, str]()
		 			query = FeatureQueryOptions(className)
		 			query.AddFeatureProperty(propName)
		 			reader = svc.SelectFeatures(query)
		 			try:
		 				while reader.ReadNext():
		 					if not reader.IsNull(propName):
								values.Add(reader[propName].ToString(), String.Empty)
		 			finally:
		 				reader.Dispose()
		 			propertyValues.AddRange(values.Keys)
		finally:
			svc.Dispose()
		
		if propertyValues.Count > 0:
			self._grdRules.Rows.Clear()
			for pval in propertyValues:
				className = self._txtClassPrefix.Text + pval + self._txtClassSuffix.Text
				filter = propName + " = '" + pval + "'"
				self._grdRules.Rows.Add(pval, className, filter)
			
		pass

	def BtnBrowseClick(self, sender, e):
		save = SaveFileDialog()
		save.Filter = "SDF files (*.sdf)|*.sdf"
		if save.ShowDialog() == DialogResult.OK:
			self._txtSdf.Text = save.FileName
		pass