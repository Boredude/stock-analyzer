using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Windows;
using BigData.UI.Client.Infrastructure;
using Prism.Mef;
using Prism.Regions;

namespace BigData.UI.Client
{
    public class AppBootstrapper : MefBootstrapper
    {
        protected override DependencyObject CreateShell()
        {
            return this.Container.GetExportedValue<ShellView>();
        }

        protected override void InitializeShell()
        {
            Application.Current.MainWindow.Show();
        }

        protected override void ConfigureContainer()
        {
            base.ConfigureContainer();
            Container.ComposeExportedValue(Container);
        }


        protected override void ConfigureAggregateCatalog()
        {
            base.ConfigureAggregateCatalog();
            AggregateCatalog.Catalogs.Add(new AssemblyCatalog(typeof(AppBootstrapper).Assembly));
        }

        protected override IRegionBehaviorFactory ConfigureDefaultRegionBehaviors()
        {
            var factory = base.ConfigureDefaultRegionBehaviors();
            factory.AddIfMissing(nameof(AutoPopulateExportedViewsBehavior), typeof(AutoPopulateExportedViewsBehavior));
            return factory;
        }
    }
}
