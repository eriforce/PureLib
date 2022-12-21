using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace PureLib.WPF {
    public static class WpfExtensions {
        public static void AddRange<T>(this ObservableCollection<T> list, IEnumerable<T> items) {
            if (list == null)
                throw new ArgumentNullException(nameof(list));
            if (items == null)
                throw new ArgumentNullException(nameof(items));

            foreach (T item in items)
                list.Add(item);
        }
    }
}
