using System;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Reflection;
using PureLib.Properties;

namespace PureLib.Common {
    [Serializable]
    public class NotifyObject : INotifyPropertyChanged {
        public event PropertyChangedEventHandler PropertyChanged;

        protected void RaiseChange<T>(Expression<Func<T>> propertyExpression) {
            if (propertyExpression == null)
                throw new ArgumentNullException(nameof(propertyExpression));

            MemberExpression memberExpression = propertyExpression.Body as MemberExpression;
            if (memberExpression == null)
                throw new ArgumentException(Resources.NotifyObject_NotMemberAccessExpression_Exception, nameof(propertyExpression));

            PropertyInfo property = memberExpression.Member as PropertyInfo;
            if (property == null)
                throw new ArgumentException(Resources.NotifyObject_ExpressionNotProperty_Exception, nameof(propertyExpression));

            MethodInfo getMethod = property.GetGetMethod(true);
            if (getMethod.IsStatic)
                throw new ArgumentException(Resources.NotifyObject_StaticExpression_Exception, nameof(propertyExpression));

            RaiseChange(memberExpression.Member.Name);
        }

        protected void RaiseChange(string fieldName) {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(fieldName));
        }
    }
}
