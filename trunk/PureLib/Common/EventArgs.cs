using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PureLib.Common {
    public class EventArgs<T> : EventArgs {
        public T Data { get; private set; }

        public EventArgs(T data) {
            Data = data;
        }
    }
}
