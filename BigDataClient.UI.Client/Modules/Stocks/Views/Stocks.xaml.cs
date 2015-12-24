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

namespace BigData.UI.Client.Modules.Stocks.Views
{
    /// <summary>
    /// Interaction logic for Stocks.xaml
    /// </summary>
    [ViewExport(RegionName = RegionNames.MainRegion)]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public partial class Stocks : UserControl
    {
        public Stocks()
        {
            InitializeComponent();
        }
    }
}
