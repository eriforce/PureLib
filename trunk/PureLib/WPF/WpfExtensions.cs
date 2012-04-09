using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace PureLib.WPF {
    public static class WpfExtensions {
        public static void AddRange<T>(this ObservableCollection<T> list, IEnumerable<T> items) {
            if ((list == null) || items == null)
                throw new ArgumentNullException();
            foreach (T item in items)
                list.Add(item);
        }
    }
}
