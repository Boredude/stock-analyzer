using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BigData.UI.Client.Infrastructure;
using BigData.UI.Client.Modules.Settings.Models;
using Prism.Mvvm;

namespace BigData.UI.Client.Modules.Settings.ViewModels
{
    [Export(typeof(ISettingsViewModel))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class SettingsViewModel : BindableBase, ISettingsViewModel, IPartImportsSatisfiedNotification
    {
        #region Data Members

        [Import]
        private ISettingsModel _settingsModel;
        [Import]
        private IStatusUpdater _statusUpdater;

        private bool _isOpenFeatureSelected;
        private bool _isHighFeatureSelected;
        private bool _isLowFeatureSelected;
        private bool _isCloseFeatureSelected;

        #endregion

        #region Ctor

        public SettingsViewModel()
        {
            
        }

        #endregion

        #region Properties

        public int NumOfStocks
        {
            get
            {
                return _settingsModel.NumOfStocks;
            }
            set
            {
                _settingsModel.NumOfStocks = value;
                OnPropertyChanged(() => NumOfStocks);
                UpdateStatus();
            }
        }

        public int DaysToAnalyze
        {
            get { return _settingsModel.DaysToAnalyze; }
            set
            {
                _settingsModel.DaysToAnalyze = value;
                OnPropertyChanged(() => DaysToAnalyze);
                UpdateStatus();

            }
        }

        public bool IsOpenFeatureSelected
        {
            get { return _isOpenFeatureSelected; }
            set
            {
                _isOpenFeatureSelected = value;
                UpdateFeaturesToAnalyze();
                OnPropertyChanged(() => IsOpenFeatureSelected);
            }
        }

        public bool IsHighFeatureSelected
        {
            get { return _isHighFeatureSelected; }
            set
            {
                _isHighFeatureSelected = value;
                UpdateFeaturesToAnalyze();
                OnPropertyChanged(() => IsHighFeatureSelected);
            }
        }

        public bool IsLowFeatureSelected
        {
            get { return _isLowFeatureSelected; }
            set
            {
                _isLowFeatureSelected = value;
                UpdateFeaturesToAnalyze();
                OnPropertyChanged(() => IsLowFeatureSelected);
            }
        }

        public bool IsCloseFeatureSelected
        {
            get { return _isCloseFeatureSelected; }
            set
            {
                _isCloseFeatureSelected = value;
                UpdateFeaturesToAnalyze();
                OnPropertyChanged(() => IsCloseFeatureSelected);
            }
        }

        public int NumOfClusters
        {
            get { return _settingsModel.NumOfClusters; }
            set
            {
                _settingsModel.NumOfClusters = value;
                OnPropertyChanged(() => NumOfClusters);
                UpdateStatus();
            }
        }

        #endregion

        #region Methods

        private void UpdateFeaturesToAnalyze()
        {
            var featureToAnalyze = IsOpenFeatureSelected
                                    ? StockPriceType.Open :
                                    0;
            featureToAnalyze |= IsHighFeatureSelected
                                ? StockPriceType.High
                                : 0;
            featureToAnalyze |= IsLowFeatureSelected
                                ? StockPriceType.Low
                                : 0;
            featureToAnalyze |= IsCloseFeatureSelected
                                ? StockPriceType.Close
                                : 0;
            // update settings model
            _settingsModel.FeaturesToAnalyze = featureToAnalyze;
            // update status
            UpdateStatus();
        }

        private void UpdateStatus()
        {
            _statusUpdater.UpdateStatus(_settingsModel.ToString());
        }

        public void OnImportsSatisfied()
        {
            // read features to analyze directly to data members to avoid unessecary write
            _isOpenFeatureSelected = _settingsModel.FeaturesToAnalyze.HasFlag(StockPriceType.Open);
            _isHighFeatureSelected = _settingsModel.FeaturesToAnalyze.HasFlag(StockPriceType.High);
            _isLowFeatureSelected = _settingsModel.FeaturesToAnalyze.HasFlag(StockPriceType.Low);
            _isCloseFeatureSelected = _settingsModel.FeaturesToAnalyze.HasFlag(StockPriceType.Close);

            // update status intially
            UpdateStatus();
        }

        #endregion


    }
}
