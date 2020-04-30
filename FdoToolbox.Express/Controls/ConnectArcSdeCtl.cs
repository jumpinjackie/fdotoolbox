using System;
using System.ComponentModel;
using System.Windows.Forms;
using FdoToolbox.Base.Controls;
using ICSharpCode.Core;
using FdoToolbox.Base.Services;

namespace FdoToolbox.Express.Controls
{
    [ToolboxItem(false)]
    public partial class ConnectArcSdeCtl : ViewContent, IConnectArcSdeView
    {
        private ConnectArcSdePresenter _presenter;

        public ConnectArcSdeCtl()
        {
            InitializeComponent();
            _presenter = new ConnectArcSdePresenter(this, ServiceManager.Instance.GetService<IFdoConnectionManager>());
        }

        public override string Title => ResourceService.GetString("TITLE_CONNECT_ARCSDE");

        private void btnConnect_Click(object sender, EventArgs e)
        {
            using (new TempCursor(Cursors.WaitCursor))
            {
                _presenter.PendingConnect();
            }
        }

        public string Server => txtServer.Text;

        public string Instance => txtInstance.Text;

        public string Username => txtUsername.Text;

        public string Password => txtPassword.Text;

        public bool DataStoreEnabled
        {
            set { cmbDataStore.Enabled = value; }
        }

        public bool SubmitEnabled
        {
            set { btnOK.Enabled = value; }
        }

        public string[] DataStores
        {
            set { cmbDataStore.DataSource = value; }
        }

        public string SelectedDataStore => cmbDataStore.SelectedItem != null ? cmbDataStore.SelectedItem.ToString() : string.Empty;

        public string ConnectionName => txtConnectionName.Text;

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            if (_presenter.Connect())
                this.Close();
        }
    }
}
