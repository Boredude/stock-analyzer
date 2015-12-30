using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Windows;
using BigData.UI.Client.Infrastructure;
using BigDataClient.BL.Infrastructure;
using BigDataClient.BL.Stocks;
using Prism.Mef;
using Prism.Regions;

namespace BigData.UI.Client
{
    public class AppBootstrapper : MefBootstrapper
    {
        protected override DependencyObject CreateShell()
        {
            var shell = this.Container.GetExportedValue<ShellView>();
            Application.Current.MainWindow = shell;
            return shell;
        }

        protected override void InitializeShell()
        {
            Application.Current.MainWindow.Show();
        }

        protected override void ConfigureContainer()
        {
            base.ConfigureContainer();
            Composition.Initialize(Container);
            Container.ComposeExportedValue(Container);
        }


        protected override void ConfigureAggregateCatalog()
        {
            base.ConfigureAggregateCatalog();
            AggregateCatalog.Catalogs.Add(new AssemblyCatalog(typeof(AppBootstrapper).Assembly));
            AggregateCatalog.Catalogs.Add(new AssemblyCatalog(typeof(IStatusUpdater).Assembly));
            AggregateCatalog.Catalogs.Add(new AssemblyCatalog(typeof(IStocksDataManager).Assembly));
        }

        protected override IRegionBehaviorFactory ConfigureDefaultRegionBehaviors()
        {
            var factory = base.ConfigureDefaultRegionBehaviors();
            factory.AddIfMissing(nameof(AutoPopulateExportedViewsBehavior), typeof(AutoPopulateExportedViewsBehavior));
            return factory;
        }
    }
}
