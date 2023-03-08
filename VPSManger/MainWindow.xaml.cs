using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using VPSManger.DTO;
using VPSManger.Biz;
using MaterialDesignThemes.Wpf;
using System.DirectoryServices.ActiveDirectory;
using System.Linq;

namespace VPSManger
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private VpsRestAPI api;
        private List<ServerDTO> _servers;
        private bool _isInitialized = false;
        private string sSortInfo = string.Empty;
        private ServerDTO selectedServer;

        public MainWindow()
        {
            InitializeComponent();

            api = new VpsRestAPI();

            RefreshServerList();
        }


        #region Main Toolbar Events

        private void GridBarraTitulo_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (e.ClickCount == 2)
            {
                if (WindowState == WindowState.Normal)
                {
                    this.WindowState = WindowState.Maximized;
                    MaxIcon.Kind = PackIconKind.CheckboxMultipleBlankOutline;
                }
                else
                {
                    this.WindowState = WindowState.Normal;
                    MaxIcon.Kind = PackIconKind.Maximize;
                }
            }
            else
            {
                DragMove();
            }
        }

        private void btnPower_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void btnMinimize_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void btnMaximize_Click(object sender, RoutedEventArgs e)
        {
            if (WindowState == WindowState.Normal)
            {
                this.WindowState = WindowState.Maximized;
            }
            else
            {
                this.WindowState = WindowState.Normal;
            }
        }

        #endregion

        #region Condition Events

        private void cmbRegion_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_isInitialized == false) return;

            _servers.Clear();

            List<ServerDTO> serverList = api.GetServerList();

            string sSelectedItem = cmbRegion.SelectedItem.ToString();

            if (sSelectedItem == "ALL")
            {
                _servers = serverList;
            }
            else
            {
                foreach (ServerDTO server in serverList)
                {
                    if (server.RegionName.Equals(sSelectedItem))
                    {
                        _servers.Add(server);
                    }
                }
            }

            BindGrid(_servers);

            int iServerCnt = _servers.Count;
            int iUserCnt = _servers.Sum(x => Convert.ToInt32(x.UserCount));

            lblServerCount.Content = iServerCnt.ToString();
            lblUserCount.Content = iUserCnt.ToString();
        }

        private void btnNew_Click(object sender, RoutedEventArgs e)
        {
            ServerDTO server = ServerDTO.NewDTO();
            if (PopupShowDialog(server))
            {
                RefreshServerList();
            }
        }

        private void btnRefresh_Click(object sender, RoutedEventArgs e)
        {
            RefreshServerList();
        }

        #endregion

        #region ListView Event Handler

        /// <summary>
        /// GridView Row Click Event 처리
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void lvServerList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count > 0)
            {
                if (e.AddedItems[0].GetType() == typeof(ServerDTO))
                {
                    selectedServer = (ServerDTO)e.AddedItems[0];
                }
                else
                {
                    selectedServer = null;
                }
            }
        }

        /// <summary>
        /// GridView Column Header Click Event 처리
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void lvServerList_Click(object sender, RoutedEventArgs e)
        {
            string sSelectedColumnHeader = ((GridViewColumnHeader)e.OriginalSource).Column.Header.ToString();
            //ID일 경우 제외 Ascending 부터 시작...

            string[] arrSortInfo = sSortInfo.Split("|");

            if (arrSortInfo.Length > 0)
            {
                if (arrSortInfo[0].Equals(sSelectedColumnHeader))
                {
                    if (arrSortInfo[1].Equals("A"))
                    {
                        arrSortInfo[1] = "D";
                    }
                    else
                    {
                        arrSortInfo[1] = "A";
                    }
                }
                else
                {
                    arrSortInfo[0] = sSelectedColumnHeader;
                    arrSortInfo[1] = "A";
                }
            }

            sSortInfo = arrSortInfo[0] + "|" + arrSortInfo[1];

            SortGridData(arrSortInfo);
        }

        /// <summary>
        /// Context Menu - Detail Click Event 처리
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mnuViewJson_Click(object sender, RoutedEventArgs e)
        {
            PopupShowDialog(selectedServer);
        }

        /// <summary>
        /// Context Menu - Modify Click Event 처리
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mnuMonify_Click(object sender, RoutedEventArgs e)
        {
            selectedServer.ChangeMode(Common.Status.Modify);
            if (PopupShowDialog(selectedServer))
            {
                RefreshServerList();
            }
        }

        /// <summary>
        /// Context Menu - Delete Click Event 처리
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mnuDelete_Click(object sender, RoutedEventArgs e)
        {
            if(MessageBox.Show("삭제 하시겠습니까?", "Delete Server", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
            {
                selectedServer.ChangeMode(Common.Status.Delete);
                if (api.SaveServer(selectedServer))
                {
                    MessageBox.Show("삭제 되었습니다.");
                    RefreshServerList();
                }
            }
        }

        #endregion

        #region Private Methods

        private void BindGrid(List<ServerDTO> serverList)
        {
            lvServerList.ItemsSource = null;
            lvServerList.ItemsSource = serverList;
        }

        private void BindRegionComboBoxList(List<ServerDTO> serverList)
        {
            cmbRegion.Items.Clear();
            cmbRegion.Items.Add("ALL");

            foreach (ServerDTO server in serverList)
            {
                cmbRegion.Items.Add(server.RegionName);
            }

            cmbRegion.SelectedIndex = 0;
        }

        private void SortGridData(string[] arrSortInfo)
        {
            if (arrSortInfo[0] == "Id")
            {
                if (arrSortInfo[1] == "A")
                {
                    _servers.Sort((x, y) => int.Parse(x.Id).CompareTo(int.Parse(y.Id)));
                }
                else
                {
                    _servers.Sort((x, y) => int.Parse(y.Id).CompareTo(int.Parse(x.Id)));
                }
            }
            else
            {
                switch (arrSortInfo[0])
                {
                    case "State":
                        if (arrSortInfo[1].Equals("A")) _servers.Sort((x, y) => x.State.CompareTo(y.State));
                        else _servers.Sort((x, y) => y.State.CompareTo(x.State));
                        break;
                    case "UserCount":
                        if (arrSortInfo[1].Equals("A")) _servers.Sort((x, y) => x.UserCount.CompareTo(y.UserCount));
                        else _servers.Sort((x, y) => y.UserCount.CompareTo(x.UserCount));
                        break;
                    case "RegionName":
                        if (arrSortInfo[1].Equals("A")) _servers.Sort((x, y) => x.RegionName.CompareTo(y.RegionName));
                        else _servers.Sort((x, y) => y.RegionName.CompareTo(x.RegionName));
                        break;
                    case "ServerScript":
                        if (arrSortInfo[1].Equals("A")) _servers.Sort((x, y) => x.ServerScript.CompareTo(y.ServerScript));
                        else _servers.Sort((x, y) => y.ServerScript.CompareTo(x.ServerScript));
                        break;
                }
            }

            BindGrid(_servers);
        }

        private void RefreshServerList()
        {
            _servers = api.GetServerList();
            BindGrid(_servers);

            _isInitialized = false;
            BindRegionComboBoxList(_servers);
            cmbRegion.SelectedIndex = 0;
            _isInitialized = true;

            sSortInfo = "Id|A";

            int iServerCnt = _servers.Count;
            int iUserCnt = _servers.Sum(x => Convert.ToInt32(x.UserCount));

            lblServerCount.Content = iServerCnt.ToString();
            lblUserCount.Content = iUserCnt.ToString();
        }

        private bool PopupShowDialog(ServerDTO server)
        {
            PopupServerInfo popup = new PopupServerInfo();
            popup.Server = server;
            bool? result = popup.ShowDialog();
            if(result == null) result = false;
            return result.Value;
        }

        #endregion

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (WindowState == WindowState.Normal)
            {
                MaxIcon.Kind = PackIconKind.Maximize; 
            }
            else
            {
                MaxIcon.Kind = PackIconKind.CheckboxMultipleBlankOutline;
            }
        }
    }
}
