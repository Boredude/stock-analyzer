using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
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
using BigData.UI.Client.Infrastructure;
using BigData.UI.Client.Modules.Results.ViewModels;

namespace BigData.UI.Client.Modules.Results.Views
{
    /// <summary>
    /// Interaction logic for ResultsView.xaml
    /// </summary>
    [ViewExport(RegionName = RegionNames.ResultsRegion)]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public partial class ResultsView : UserControl
    {
        public ResultsView()
        {
            InitializeComponent();
        }

        [Import]
        public IResultsViewModel ViewModel
        {
            get { return DataContext as IResultsViewModel; }
            set { DataContext = value; }
        }
    }
}
