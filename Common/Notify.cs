/* 
    This file is part of crANNy. Copyright (C) 2017 Christian Rauch.
    Distributed under terms of the GPL3 license.
*/

namespace CRAI.Common
{
    using System;
    using System.Linq;
    using System.ComponentModel;
    using System.Linq.Expressions;

    public static class Notify
    {
        public static void NotifyPropertyChanged2<TObject>(
            this TObject sender,
            Expression<Func<TObject, Object>> selector)
            where TObject : NotifyBase
        {
            if (sender == null)
            {
                return;
            }

            sender.NotifyPropertyChanged(sender, selector);
        }

        public static void NotifyPropertyChangedAll<TObject>(this TObject sender)
            where TObject : NotifyBase
        {
            if (sender == null)
            {
                return;
            }

            var properties
                = sender
                .GetType()
                .GetProperties()
                .Select(p => p.Name);

            foreach (var property in properties)
            {
                sender.NotifyPropertyChanged(property);
            }
        }

        public static void NotifyPropertyChanged<TObject>(
            this TObject sender,
            PropertyChangedEventHandler propertyChangedEventHandler,
            Expression<Func<TObject, Object>> selector)
            where TObject : INotifyPropertyChanged
        {
            if (propertyChangedEventHandler == null) return;
            var propertyName = Notify.GetPropertyName(selector);
            propertyChangedEventHandler.Invoke(sender, new PropertyChangedEventArgs(propertyName));
        }

        public static String GetPropertyName<TObject>(
            this TObject obj,
            Expression<Func<TObject, Object>> selector)
        {
            return GetPropertyName(selector);
        }

        public static String GetPropertyName<TObject>(
            Expression<Func<TObject, Object>> selector)
        {
            var propertyString = selector.ToString();
            var delimiterPosition = propertyString.IndexOf('.');

            if (delimiterPosition >= 0)
            {
                propertyString
                    = propertyString
                    .Substring(delimiterPosition + 1)
                    .TrimEnd(')');
            }

            return propertyString;
        }

        public static String GetMemberName<TObject, TMember>(
            this TObject obj,
            Expression<Func<TObject, TMember>> expression)
        {
            var memberExpression = expression.Body as MemberExpression;
            if (memberExpression != null)
            {
                return memberExpression.Member.Name;
            }

            var methodCallExpression = expression.Body as MethodCallExpression;
            if (methodCallExpression != null)
            {
                return methodCallExpression.Method.Name;
            }

            throw new ArgumentException(@"Argument 'expression' needs to be a property or method.", "expression");
        }
    }
}
