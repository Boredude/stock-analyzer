﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BigData.UI.Client.Modules.Settings.ViewModels
{
    public interface ISettingsViewModel
    {
        int NumOfStocks { get; set; }
        int DaysToAnalyze { get; set; }
        bool IsOpenFeatureSelected { get; set; }
        bool IsHighFeatureSelected { get; set; }
        bool IsLowFeatureSelected { get; set; }
        bool IsCloseFeatureSelected { get; set; }
        int NumOfClusters { get; set; }
    }
}
