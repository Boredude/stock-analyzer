using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BigData.UI.Client.Infrastructure
{
    public interface IStatusUpdater
    {
        void UpdateStatus(string status);
        event Action<string> StatusChanged;
    }

    [Export(typeof (IStatusUpdater))]
    [PartCreationPolicy(CreationPolicy.Shared)]
    public class StatusUpdater : IStatusUpdater
    {
        public void UpdateStatus(string status)
        {
            var handler = StatusChanged;
            handler?.Invoke(status);
        }

        public event Action<string> StatusChanged;
    }
}
