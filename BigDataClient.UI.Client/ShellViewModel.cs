using System.ComponentModel.Composition;
using System.Windows;
using BigData.UI.Client.Infrastructure;
using BigDataClient.BL.Infrastructure;
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
        [Import]
        private ITabManager _tabManager;

        private bool _isFlyOutOpen;
        private string _flyoutTitle;
        private string _statusBarText;
        private int _selectedTab;

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

        public int SelectedTab
        {
            get { return _selectedTab; }
            set
            {
                if (_selectedTab != value)
                {
                    _selectedTab = value;
                    OnPropertyChanged(() => SelectedTab);
                }
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
            Application.Current.Dispatcher.InvokeAsync(() => StatusBarText = status);
        }

        public void OnImportsSatisfied()
        {
            // register to status update notifications
            _statusUpdater.StatusChanged += OnStatusChanged;
            // register to tab change event
            _tabManager.SelectedTabChanged += tab => SelectedTab = (int) tab;
        }

        #endregion
    }
}
