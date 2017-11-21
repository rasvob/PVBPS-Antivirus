using System.Windows.Input;
using PVBPS_Antivirus.Config;
using PVBPS_Antivirus.ViewModels;

namespace PVBPS_Antivirus
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        private ConfigGateway _config = new ConfigGateway();
        private readonly MainWindowViewModel _viewModel = new MainWindowViewModel();

        public MainWindow()
        {
            InitializeComponent();
            DataContext = _viewModel;
        }

        private void UIElement_OnPreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (e.Delta < 0)
            {
                ScanScrollViewer.LineDown();
            }
            else
            {
                ScanScrollViewer.LineUp();
            }

            e.Handled = true;
        }
    }
}
