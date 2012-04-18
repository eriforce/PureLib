using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using PureLib.Properties;

namespace PureLib.WPF {
    public class NotifyObject : INotifyPropertyChanged {
        public event PropertyChangedEventHandler PropertyChanged;

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
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(fieldName));
        }
    }
}
