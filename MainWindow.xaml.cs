using SeeTec.SDK;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace CaguyaWPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Guid ConnectedInstallationID;

        public MainWindow()
        {
            SDKVideoManagerFactory.SetDispatcherThread();
            SDKVideoManagerFactory.SetMainWindowHandle(
            new System.Windows.Interop.WindowInteropHelper(this).Handle);

            InitializeComponent();
        }

        private void bConnect_Click(object sender, RoutedEventArgs e)
        {
            SDKConnectionInfo connectionInfo = new SDKConnectionInfo();
            connectionInfo.Host = tbServer.Text;
            connectionInfo.Port = 60000;
            connectionInfo.UserName = tbUser.Text;
            connectionInfo.Profile = tbProfile.Text;
            connectionInfo.Password = tbPass.Text;
            SDKVideoManager videoManager = SDKVideoManagerFactory.GetManager();
            SDKMethodResult<Guid> installationIDResult = videoManager.Connect(connectionInfo);
            if (installationIDResult.Result != SDKErrorCode.OK)
            {
                MessageBox.Show(this, installationIDResult.Result.ToString(),
               "Error");
            }
            else
            {
                ConnectedInstallationID = installationIDResult.ReturnValue;
                searchCameras();
            }

        }

        private void searchCameras()
        {
            SDKVideoManager videoManager = SDKVideoManagerFactory.GetManager();
            SDKEntity root = videoManager.GetRootEntity(ConnectedInstallationID);
            Stack<SDKEntity> searchStack = new Stack<SDKEntity>();
            searchStack.Push(root);
            while (searchStack.Count > 0)
            {
                SDKEntity currentEntity = searchStack.Pop();
                if (videoManager.HasRight(ConnectedInstallationID, currentEntity.ID,
               SDKRightType.CameraLive))
                {
                    if (videoManager.IsSubType(SDKEntityType.VideoSource,
                   currentEntity.EntityType))
                    {
                        listView1.Items.Add(currentEntity);
                    }
                }
                foreach (long childID in currentEntity.ChildrenIDs)
                {

                    searchStack.Push(videoManager.GetEntity(currentEntity.InstallationID,
                     childID));
                }
            }
        }
    }
}
