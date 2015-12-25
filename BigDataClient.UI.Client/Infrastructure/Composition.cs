using System;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Linq;

namespace BigData.UI.Client.Infrastructure
{
    [Export]
    public static class Composition
    {
        #region Data Members

        private static CompositionContainer _container;

        #endregion

        #region Methods

        public static void Initialize(CompositionContainer container)
        {
            _container = container;
        }

        public static TContract Compose<TContract>()
        {
            return _container.GetExportedValue<TContract>();
        }

        public static TContract Compose<TContract, TMetadata>(Predicate<TMetadata> predicate)
        {
            try
            {
                // Get the exports
                var exports = _container.GetExports<TContract, TMetadata>();
                // Get export that meets the predicate
                var export = exports.SingleOrDefault(exp => predicate(exp.Metadata));
                // return the exported value
                return export == null ? default(TContract) : export.Value;
            }
            catch (Exception ex)
            {
                return default (TContract);

            }
        }

        public static bool HasExport<TContract, TMetadata>(Predicate<TMetadata> predicate)
        {
            try
            {
                return _container.GetExports<TContract, TMetadata>()
                                 .Any(export => predicate(export.Metadata));
            }
            catch
            {
                return false;
            }

        }

        #endregion
    }
}
