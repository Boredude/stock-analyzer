using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using Microsoft.Practices.EnterpriseLibrary.Common.Utility;

namespace BigData.UI.Client.Infrastructure
{
    public class EnhancedObservableCollection<T> : ObservableCollection<T>
    {
        #region Ctor

        public EnhancedObservableCollection() : base()
        {
        }

        public EnhancedObservableCollection(IEnumerable<T> enumerable) : base(enumerable) 
        {
        }

        public EnhancedObservableCollection(IList<T> list) : base(list)
        {
        }

        #endregion

        #region Data Members

        private bool _suppressNotification;

        #endregion

        #region Methods

        protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            if (!_suppressNotification)
                Application.Current
                           .Dispatcher
                           .Invoke(() => base.OnCollectionChanged(e));
        }

        public void AddRange(IEnumerable<T> enumerable)
        {
            if (enumerable == null)
                throw new ArgumentNullException(nameof(enumerable));

            _suppressNotification = true;

            foreach (T item in enumerable)
            {
                Add(item);
            }
            _suppressNotification = false;
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }

        public void AlignWith(IEnumerable<T> enumerable)
        {
            _suppressNotification = true;

            var list = enumerable as IList<T> ?? enumerable.ToList();

            // enumerate items in the current list and remove items
            // that doesn't exist in the list to we want to align with
            this.Where(item => !list.Contains(item))
                .ToList()
                .ForEach(item => Remove(item));

            // add the items that exists in the list we want to align with
            // and doesnt exist in the current list
            list.Except(this).ForEach(Add);

            _suppressNotification = false;
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }


        #endregion
    }
}
