using System.ComponentModel;
using System.ComponentModel.Composition;
using MahApps.Metro.Controls;

namespace BigData.UI.Client
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    [Export]
    public partial class ShellView : MetroWindow
    {
        public ShellView()
        {
            InitializeComponent();
        }

        [Import]
        public IShellViewModel ViewModel
        {
            get { return DataContext as IShellViewModel; }
            set { DataContext = value; }
        }
    }
}
