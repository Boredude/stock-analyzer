using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BigData.UI.Client.Modules.Results.Views;

namespace BigData.UI.Client.Modules.Results.ViewModels
{
    [Export(typeof(IResultsViewModel))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class ResultsViewModel : IResultsViewModel
    {
    }
}
