using System.ComponentModel.Composition;
using System.Windows;
using BigData.UI.Client.Infrastructure;
using Prism.Commands;
using Prism.Mvvm;

namespace BigData.UI.Client
{
    [Export(typeof(IShellViewModel))]
    [PartCreationPolicy(CreationPolicy.Shared)]
    public class ShellViewModel : BindableBase, IShellViewModel, IPartImportsSatisfiedNotification
    {
        #region Data Members

        [Import]
        private IStatusUpdater _statusUpdater;

        private bool _isFlyOutOpen;
        private string _flyoutTitle;
        private string _statusBarText;

        #endregion

        #region Ctor

        public ShellViewModel()
        {
            // Init commands
            SettingsCommand = new DelegateCommand(OnOpenSettings);
        }

        #endregion

        #region Properties

        public bool IsFlyoutOpen
        {
            get { return _isFlyOutOpen; }
            set
            {
                _isFlyOutOpen = value;
                OnPropertyChanged(() => IsFlyoutOpen);
            }
        }

        public string FlyoutTitle
        {
            get { return _flyoutTitle; }
            set
            {
                _flyoutTitle = value;
                OnPropertyChanged(() => FlyoutTitle);
            }
        }

        public string StatusBarText
        {
            get { return _statusBarText; }
            set
            {
                _statusBarText = value;
                OnPropertyChanged(() => StatusBarText);
            }
        }


        #endregion

        #region Commands

        public DelegateCommand SettingsCommand { get; set; }

        #endregion

        #region Methods

        private void OnOpenSettings()
        {
            // set title
            FlyoutTitle = "Settings";
            // Toggle flyout open
            IsFlyoutOpen = !IsFlyoutOpen;
        }

        private void OnStatusChanged(string status)
        {
            // update text
            StatusBarText = status;
        }

        public void OnImportsSatisfied()
        {
            // register to status update notifications
            _statusUpdater.StatusChanged += OnStatusChanged;
        }

        #endregion
    }
}
