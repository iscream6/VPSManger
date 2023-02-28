using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using VPSManger.Biz;
using VPSManger.Common;
using VPSManger.DTO;

namespace VPSManger
{
    /// <summary>
    /// PopupServerInfo.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class PopupServerInfo : Window
    {
        public ServerDTO Server { get; set; }
        private ServerDTO CopyServer;

        private VpsRestAPI api;

        public PopupServerInfo()
        {
            InitializeComponent();
            api = new VpsRestAPI();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (Server == null) Close();

            InitializeControl();

            CopyServer = Server.Copy();
        }

        private void GridHead_MouseDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if(Save())
            {
                MessageBox.Show("Save is success~!");
                DialogResult = true;
            }
            else
            {
                MessageBox.Show("Save is fail~!");
                DialogResult = false;
            }
        }

        private void InitializeControl()
        {
            cmbRegion.ItemsSource = Enum.GetValues(typeof(Region));

            txtId.Text = Server.Id;
            txtState.Text = Server.State;
            txtServerScript.Text = Server.ServerScript;
            cmbRegion.SelectedIndex = int.Parse(Server.RegionCode) - 1;

            if (Server.IsNew || Server.IsModify)
            {
                btnSave.IsEnabled = true;
                if (Server.IsModify)
                {
                    txtId.IsReadOnly = true;
                }
            }
            else
            {
                txtId.IsReadOnly= true;
                txtState.IsReadOnly= true;
                txtServerScript.IsReadOnly= true;
                btnSave.IsEnabled = false;
            }
        }

        private bool Save()
        {
            Server.Id = txtId.Text;
            Server.State = txtState.Text;
            int iRegionCode = cmbRegion.SelectedIndex + 1;
            Server.RegionCode = iRegionCode.ToString();
            Server.RegionName = cmbRegion.Text;
            Server.ServerScript = txtServerScript.Text;

            if (Server.Id != CopyServer.Id ||
                Server.State != CopyServer.State ||
                Server.RegionCode != CopyServer.RegionCode ||
                Server.ServerScript != CopyServer.ServerScript)
            {
                return api.SaveServer(Server);
            }

            return true;
        }
    }
}
