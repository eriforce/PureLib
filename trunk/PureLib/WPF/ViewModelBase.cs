using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Windows;
using System.Windows.Threading;
using PureLib.Properties;

namespace PureLib.WPF {
    public abstract class ViewModelBase : INotifyPropertyChanged {
        private readonly Dispatcher uiDispatcher;

        public Window View { get; set; }
        public event PropertyChangedEventHandler PropertyChanged;

        public ViewModelBase()
            : this(Dispatcher.CurrentDispatcher) {
        }

        public ViewModelBase(Dispatcher uiDispatcher) {
            this.uiDispatcher = uiDispatcher;
        }

        protected void SetProperty<T>(ref T storage, T val, params string[] fieldsToNotify) {
            storage = val;
            foreach (var field in fieldsToNotify)
                RaiseChange(field);
        }

        protected void RaiseChange<T>(Expression<Func<T>> propertyExpression) {
            if (propertyExpression == null)
                throw new ArgumentNullException("propertyExpression");

            MemberExpression memberExpression = propertyExpression.Body as MemberExpression;
            if (memberExpression == null)
                throw new ArgumentException(Resources.ViewModelBase_NotMemberAccessExpression_Exception, "propertyExpression");

            PropertyInfo property = memberExpression.Member as PropertyInfo;
            if (property == null)
                throw new ArgumentException(Resources.ViewModelBase_ExpressionNotProperty_Exception, "propertyExpression");

            MethodInfo getMethod = property.GetGetMethod(true);
            if (getMethod.IsStatic)
                throw new ArgumentException(Resources.ViewModelBase_StaticExpression_Exception, "propertyExpression");

            RaiseChange(memberExpression.Member.Name);
        }

        protected void RaiseChange(string fieldName) {
            if (PropertyChanged == null)
                return;

            Action<string> action = (t) => {
                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs(t));
            };

            uiDispatcher.BeginInvoke(action, fieldName);
        }

        protected void WorkInBackground(DoWorkEventHandler work, RunWorkerCompletedEventHandler onCompleted = null) {
            if (uiDispatcher == null)
                work.Invoke(null, new DoWorkEventArgs(null));
            else {
                using (BackgroundWorker worker = new BackgroundWorker()) {
                    worker.DoWork += (s, e) => {
                        work(s, e);
                    };
                    if (onCompleted != null)
                        worker.RunWorkerCompleted += onCompleted;
                    worker.RunWorkerAsync();
                }
            }
        }

        public void RunOnUIThread(Action code) {
            uiDispatcher.BeginInvoke(code);
        }
    }
}
