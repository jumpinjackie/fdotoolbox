﻿using FdoToolbox.Core.ETL.Specialized;
using FdoToolbox.Core.Feature;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FdoToolbox.Tasks.Controls.BulkCopy
{
    public partial class SpatialContextOverridesDialog : Form
    {
        readonly BindingList<SCOverrideItemModel> _overrides = new BindingList<SCOverrideItemModel>();

        public SpatialContextOverridesDialog(IEnumerable<SpatialContextInfo> allSpatialContexts, Dictionary<string, SCOverrideItem> overrides)
        {
            InitializeComponent();
            foreach (var sci in allSpatialContexts)
            {
                var scim = new SCOverrideItemModel(sci.Name)
                {
                    Override = overrides.ContainsKey(sci.Name),
                    CsName = overrides.ContainsKey(sci.Name) ? overrides[sci.Name].CsName : sci.CoordinateSystem,
                    WKT = overrides.ContainsKey(sci.Name) ? overrides[sci.Name].CsWkt : sci.CoordinateSystemWkt
                };
                _overrides.Add(scim);
            }
            dgvSpatialContextOverrides.DataSource = _overrides;
        }

        public Dictionary<string, SCOverrideItem> GetOverrides()
        {
            return _overrides
                .Where(ov => ov.Override)
                .ToDictionary(ov => ov.Name, ov => new SCOverrideItem { CsName = ov.CsName, CsWkt = ov.WKT });
        }

        private void btnApply_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
        }
    }

    public class SCOverrideItemModel : INotifyPropertyChanged
    {
        private bool _override;
        private string _wkt;
        private string _csName;

        public SCOverrideItemModel(string name)
        {
            this.Name = name;
        }

        public bool Override
        {
            get { return _override; }
            set
            {
                if (_override != value)
                {
                    _override = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Override)));
                }
            }
        }

        public string Name { get; }

        public string CsName
        {
            get { return _csName; }
            set
            {
                if (_csName != value)
                {
                    _csName = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(CsName)));
                }
            }
        }

        public string WKT
        {
            get { return _wkt; }
            set
            {
                if (_wkt != value)
                {
                    _wkt = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(WKT)));
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}