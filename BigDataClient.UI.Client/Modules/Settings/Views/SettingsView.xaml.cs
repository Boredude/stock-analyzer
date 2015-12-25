using System.ComponentModel.Composition;
using System.Windows.Controls;
using BigData.UI.Client.Infrastructure;
using BigData.UI.Client.Modules.Settings.ViewModels;

namespace BigData.UI.Client.Modules.Settings.Views
{
    /// <summary>
    /// Interaction logic for SettingsView.xaml
    /// </summary>
    [ViewExport(RegionName = RegionNames.FlyoutRegion)]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public partial class SettingsView : UserControl
    {
        public SettingsView()
        {
            InitializeComponent();
        }

        [Import]
        public ISettingsViewModel ViewModel
        {
            get { return DataContext as ISettingsViewModel; }
            set { DataContext = value; }
        }
    }
}
