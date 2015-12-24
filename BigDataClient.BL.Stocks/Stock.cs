﻿using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BigDataClient.BL.Stocks
{
    [Export(typeof(IStock))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class Stock : IStock
    {
        #region Properties

        public string Symbol { get; set; }
        public string Name { get; set; }
        public string MarketCategory { get; set; }
        public string TestIssue { get; set; }
        public string FinancialStatus { get; set; }
        public string RoundLotSize { get; set; }

        #endregion

        #region Methods

        public override bool Equals(object obj)
        {
            var other = obj as IStock;
            return other == null ? base.Equals(obj) : Symbol.Equals(other.Symbol);
        }

        public override int GetHashCode()
        {
            return Symbol?.GetHashCode() ?? base.GetHashCode();
        }

        #endregion
    }
}
