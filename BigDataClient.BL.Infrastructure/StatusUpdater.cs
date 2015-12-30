using System;
using System.ComponentModel.Composition;

namespace BigDataClient.BL.Infrastructure
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
