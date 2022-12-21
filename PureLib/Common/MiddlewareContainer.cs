using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PureLib.Common {
    public class MiddlewareContainer<T> where T : class {
        public delegate Task NextDelegate(T context);

        private readonly IList<Func<NextDelegate, NextDelegate>> _middlewares =
            new List<Func<NextDelegate, NextDelegate>>();

        public NextDelegate Compose() {
            NextDelegate next = context => Task.CompletedTask;

            foreach (var middleware in _middlewares.Reverse()) {
                next = middleware(next);
            }

            return next;
        }

        public MiddlewareContainer<T> Use(Func<T, Func<Task>, Task> middleware) {
            return Use(next => {
                return context => {
                    Func<Task> simpleNext = () => next(context);
                    return middleware(context, simpleNext);
                };
            });
        }

        private MiddlewareContainer<T> Use(Func<NextDelegate, NextDelegate> middleware) {
            _middlewares.Add(middleware);
            return this;
        }
    }
}
